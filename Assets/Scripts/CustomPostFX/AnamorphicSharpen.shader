Shader "Hidden/Custom/AnamorphicSharpen"
{
    HLSLINCLUDE

        #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

        TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
        TEXTURE2D_SAMPLER2D(_CameraDepthTexture, sampler_CameraDepthTexture);
        float _Strength;
        float _Offset;
        bool _UseMask;
        bool _DepthMask;
        float _Clamp;
        int _DepthMaskContrast;
        bool _Preview;
        static const float3 Luma709 = float3(0.2126729, 0.7151522, 0.0721750);
        static const float3 Luma601 = float3(0.299, 0.587, 0.114);

        float Overlay(float LayerA, float LayerB)
        {
            float MinA = min(LayerA, 0.5);
            float MinB = min(LayerB, 0.5);
            float MaxA = max(LayerA, 0.5);
            float MaxB = max(LayerB, 0.5);
            return 2.0 * (MinA * MinB + MaxA + MaxB - MaxA * MaxB) - 1.5;
        }

        float Overlay(float LayerAB)
        {
            float MinAB = min(LayerAB, 0.5);
            float MaxAB = max(LayerAB, 0.5);
            return 2.0 * (MinAB * MinAB + MaxAB + MaxAB - MaxAB * MaxAB) - 1.5;
        }

        float4 Frag(VaryingsDefault i) : SV_Target
        {
            float4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord);

            float Mask;
            if( _UseMask )
            {
                // Generate radial mask
                Mask = 1.0-length(i.texcoord*2.0-1.0);
                Mask = Overlay(Mask) * _Strength;
                // Bypass
                if (Mask <= 0) return color;
            }
            else Mask = _Strength;

            // Pixel size
            float2 Pixel = 1.0 / _ScreenParams.xy;

            float luminance = dot(color.rgb, Luma709);

            if( _DepthMask )
            {
                float2 DepthPixel = Pixel*_Offset + Pixel;
                Pixel *= _Offset;
                // Sample deisplay depth image
                //float SourceDepth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, i.texcoordStereo));
                float SourceDepth = SAMPLE_TEXTURE2D(_CameraDepthTexture, sampler_CameraDepthTexture, i.texcoord);
                float2 NorSouWesEst[4] = {
                    float2(i.texcoord.x, i.texcoord.y + Pixel.y),
                    float2(i.texcoord.x, i.texcoord.y - Pixel.y),
                    float2(i.texcoord.x + Pixel.x, i.texcoord.y),
                    float2(i.texcoord.x - Pixel.x, i.texcoord.y)
                };

                float2 DepthNorSouWesEst[4] = {
                    float2(i.texcoord.x, i.texcoord.y + DepthPixel.y),
                    float2(i.texcoord.x, i.texcoord.y - DepthPixel.y),
                    float2(i.texcoord.x + DepthPixel.x, i.texcoord.y),
                    float2(i.texcoord.x - DepthPixel.x, i.texcoord.y)
                };

                float HighPassColor = 0.0, DepthMask = 0.0;

                [unroll]for(int s = 0; s < 4; s++)
                {
                    HighPassColor += dot(SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, NorSouWesEst[s]).rgb, Luma709);
                    DepthMask += SAMPLE_TEXTURE2D(_CameraDepthTexture, sampler_CameraDepthTexture, float4(NorSouWesEst[s], 0, 0)).x
                                    + SAMPLE_TEXTURE2D(_CameraDepthTexture, sampler_CameraDepthTexture, float4(DepthNorSouWesEst[s], 0, 0)).x;
                }

                HighPassColor = 0.5 - 0.5 * (HighPassColor * 0.25 - dot(color, Luma709));

                DepthMask = 1.0 - DepthMask * 0.125 + SourceDepth;
                DepthMask = min(1.0, DepthMask) + 1.0 - max(1.0, DepthMask);
                DepthMask = saturate(_DepthMaskContrast * DepthMask + 1.0 - _DepthMaskContrast);

                // Sharpen Strength
                HighPassColor = lerp(0.5, HighPassColor, Mask * DepthMask);

                // Clamping sharpen
                HighPassColor = (_Clamp != 1.0) ? max(min(HighPassColor, _Clamp), 1.0 - _Clamp) : HighPassColor;

                float4 Sharpen = float4(
                    Overlay(color.r, HighPassColor),
                    Overlay(color.g, HighPassColor),
                    Overlay(color.b, HighPassColor),
                    1
                );

                if( _Preview )
                {
                    float PreviewChannel = lerp(HighPassColor, HighPassColor * DepthMask, 0.5);
                    return float4(
                        1.0 - DepthMask * ( 1.0 - HighPassColor ),
                        PreviewChannel,
                        PreviewChannel,
                        1
                    );
                }

                return Sharpen;
            }
            else
            {
                Pixel *= _Offset;

                float2 NorSouWesEst[4] = {
                    float2(i.texcoord.x, i.texcoord.y + Pixel.y),
                    float2(i.texcoord.x, i.texcoord.y - Pixel.y),
                    float2(i.texcoord.x + Pixel.x, i.texcoord.y),
                    float2(i.texcoord.x - Pixel.x, i.texcoord.y)
                };

                float HighPassColor = 0.0;
                [unroll]
                for(int s = 0; s < 4; s++) HighPassColor += dot(SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, NorSouWesEst[s]).rgb, Luma709);
                HighPassColor = 0.5 - 0.5 * (HighPassColor * 0.25 - dot(color, Luma709));

                HighPassColor = lerp(0.5, HighPassColor, Mask);

                HighPassColor = (_Clamp != 1.0) ? max(min(HighPassColor, _Clamp), 1.0 - _Clamp) : HighPassColor;

                float4 Sharpen = float4(
                    Overlay(color.r, HighPassColor),
                    Overlay(color.g, HighPassColor),
                    Overlay(color.b, HighPassColor),
                    1
                );

                return _Preview ? HighPassColor : Sharpen;
            }
        }

    ENDHLSL

    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            HLSLPROGRAM

                #pragma vertex VertDefault
                #pragma fragment Frag

            ENDHLSL
        }
    }
}