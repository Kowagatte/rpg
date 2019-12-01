Shader "Voxel/Voxel Shader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Tint ("Tint", Color) = (1,1,1,1)
        _SpecularTint ("Specular", Color) = (0.5, 0.5, 0.5)
        [Gamma] _Metallic("Metallic", Range(0,1)) = 0
        _Smoothness("Smoothness", Range(0,1)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            Tags
            {
                "LightMode" = "ForwardBase"
            }

            CGPROGRAM

            #pragma target 5.0

            #pragma multi_compile _ VERTEXLIGHT_ON
            
            #pragma vertex vert
            #pragma fragment frag

            #define FORWARD_BASE_PASS
            
            #include "VoxelLighting.cginc"

            ENDCG
        }

        Pass
        {
            Tags
            {
                "LightMode" = "ForwardAdd"
            }

            Blend One One
            ZWrite Off

            CGPROGRAM

            #pragma target 5.0

            #pragma multi_compile_fwdadd

            #pragma vertex vert
            #pragma fragment frag

            #include "VoxelLighting.cginc"

            ENDCG
        }
    }
}
