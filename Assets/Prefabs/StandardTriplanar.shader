Shader "Standard Triplanar"
{
    Properties
    {
        _Color("", Color) = (1,1,1,1)
        _TopTex ("", 2D) = "white" {}
        _SideTex ("", 2D) = "white" {}
        _BotTex ("", 2D) = "white" {}

       



        _Glossiness("", Range(0,1)) = 0.5
        [Gamma] _Metallic("", Range(0,1)) = 0

        _BumpScale("", Float) = 1
        _BumpMap("", 2D) = "bump" {}

        _OcclusionStrength("", Range(0,1)) = 1
        _OcclusionMap("", 2D) = "white" {}

        _MapScale("", Float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }


            CGPROGRAM
            #pragma surface surf Standard vertex:vert fullforwardshadows addshadow

            #pragma shader_feature _NORMALMAP
            #pragma shader_feature _OCCLUSIONMAP

            #pragma target 3.0
            // make fog work
            #pragma multi_compile_fog

            half4 _Color;
            sampler2D _TopTex;
            sampler2D _SideTex;
            sampler2D _BotTex;


            half _Glossiness;
            half _Metallic;

            half _BumpScale;
            sampler2D _BumpMap;

            half _OcclusionStrength;
            sampler2D _OcclusionMap;

            half _MapScale;

            struct Input
            {
                float3 localCoord;
                float3 localNormal;
            };

            void vert (inout appdata_full v, out Input data)
            {
                UNITY_INITIALIZE_OUTPUT(Input, data);
                data.localCoord = v.vertex.xyz;
                data.localNormal = v.normal.xyz;
            }

            void surf(Input IN, inout SurfaceOutputStandard o)
            {
                // Blending factor of triplanar mapping
                float3 bf = normalize(abs(IN.localNormal));
                bf /= dot(bf, (float3)1.0);

                float3 normal = normalize(IN.localNormal);

                float upVec = dot(fixed3(0,1,0), normal);

                float2 tx = IN.localCoord.zy + 0.5 * _MapScale;
                float2 ty = IN.localCoord.zx + 0.5 * _MapScale;
                float2 ty2 = IN.localCoord.xz + 0.5 * _MapScale;
                float2 tz = IN.localCoord.xy + 0.5 * _MapScale;

                half4 cx = tex2D(_SideTex, tx) * bf.x;
                half4 cy = tex2D(_TopTex, ty) * upVec;
                half4 cy2 = tex2D(_BotTex, ty) * -upVec;
                half4 cz = tex2D(_SideTex, tz) * bf.z;
                half4 cymix = max(cy, cy2);
                half4 color = (cymix + cx + cz) * _Color;

                o.Albedo = color;
                o.Alpha = color.a;

                #ifdef _NORMALMAP
                //Normal map
                half4 nx = tex2D(_BumpMap, tx) * bf.x;
                half4 ny = tex2D(_BumpMap, ty) * bf.y;
                half4 nz = tex2D(_BumpMap, tz) * bf.z;
                o.Normal = UnpackScaleNormal(nx + ny + nz, _BumpScale);
                #endif

                #ifdef _OCCLUSIONMAP
                //Occlusion map
                half ox = tex2D(_OcclusionMap, tx).g * bf.x;
                half oy = tex2D(_OcclusionMap, ty).g * bf.y;
                half oz = tex2D(_OcclusionMap, tz).g * bf.z;
                o.Occlusion = lerp((half4)1, ox + oy + oz, _OcclusionStrength);
                #endif

                //Misc Parameters
                o.Metallic = _Metallic;
                o.Smoothness = _Glossiness;
            }
            ENDCG
        

    }
    Fallback "Diffuse"
    CustomEditor "StandardTriplanarInspector"
}
