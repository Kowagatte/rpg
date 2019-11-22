using System.Runtime.InteropServices;
using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
 
[Serializable]
[PostProcess(typeof(AnamorphicSharpenRenderer), PostProcessEvent.AfterStack, "Custom/AnamorphicSharpen")]
public sealed class AnamorphicSharpen : PostProcessEffectSettings
{
    [Range(0.0f, 100.0f), Tooltip("Strength of the sharpen effect")]
    public FloatParameter Strength = new FloatParameter { value = 60.0f };

    [Range(0.0f, 2.0f), Tooltip("High-pass cross offset in pixels")]
    public FloatParameter Offset = new FloatParameter { value = 0.1f };

    [Range(0.5f, 1.0f)]
    public FloatParameter Clamp = new FloatParameter { value = 0.65f };

    [Tooltip("Sharpen only in the center of the image")]
    public BoolParameter UseMask = new BoolParameter { value = false };

    [Tooltip("Depth high-pass mask switch")]
    public BoolParameter DepthMask = new BoolParameter { value = false };

    [Range(0, 2000), Tooltip("Depth high-pass mask amount")]
    public IntParameter DepthMaskContrast = new IntParameter { value = 128 };

    //public IntParameter Coefficient = new IntParameter { value = 0 };

    public BoolParameter Preview = new BoolParameter { value = false };

    //static int cameraDepthTextureId = Shader.PropertyToID("_CameraDepthTexture");
}
 
public sealed class AnamorphicSharpenRenderer : PostProcessEffectRenderer<AnamorphicSharpen>
{
    public override DepthTextureMode GetCameraFlags()
    {
        return DepthTextureMode.Depth;
    }

    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(Shader.Find("Hidden/Custom/AnamorphicSharpen"));
        sheet.properties.SetFloat("_Strength", settings.Strength);
        sheet.properties.SetFloat("_Offset", settings.Offset);
        sheet.properties.SetFloat("_Clamp", settings.Clamp);
        if( settings.UseMask )
            sheet.properties.SetInt("_UseMask", 1);
        else
            sheet.properties.SetInt("_UseMask", 0);
        if( settings.DepthMask )
            sheet.properties.SetInt("_DepthMask", 1);
        else
            sheet.properties.SetInt("_DepthMask", 0);
        sheet.properties.SetInt("_DepthMaskContrast", settings.DepthMaskContrast);
        //sheet.properties.SetInt("_Coefficient", settings.Coefficient);
        if( settings.Preview )
            sheet.properties.SetInt("_Preview", 1);
        else
            sheet.properties.SetInt("_Preview", 0);
        
        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}
