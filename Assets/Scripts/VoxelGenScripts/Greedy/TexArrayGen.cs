using System;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;

public class TexArrayGen : MonoBehaviour
{
    private Texture2D[] ordinaryTextures;
    private GameObject objectToAddTextureTo;

    private void CreateTextureArray()
    {
        ordinaryTextures = new Texture2D[2];
        ordinaryTextures[0] = Resources.Load("Textures/Blocks/000_Grass") as Texture2D;
        ordinaryTextures[1] = Resources.Load("Textures/Blocks/001_GrassColor2") as Texture2D;
        // var temptex = TextureScaler.scaled(ordinaryTextures[0], ordinaryTextures[0].width*8, ordinaryTextures[0].height*8, FilterMode.Point);
        // ordinaryTextures[0] = temptex;

        // temptex = TextureScaler.scaled(ordinaryTextures[1], ordinaryTextures[1].width*8, ordinaryTextures[1].height*8, FilterMode.Point);
        // ordinaryTextures[1] = temptex;

        // for (int i = 0; i < ordinaryTextures.Length; i++)
        // {
        //     Texture2D item = ordinaryTextures[i];
        //     item = TextureScaler.scaled(item, 4, 4, FilterMode.Point);
        // }
        // Create Texture2DArray
        Texture2DArray texture2DArray = new Texture2DArray(ordinaryTextures[0].width/3,
                                                           ordinaryTextures[0].height, ordinaryTextures.Length*3,
                                                           TextureFormat.RGB24, false, false);    // Apply settings
        var tWidth = ordinaryTextures[0].width/3;
        var tHeight = ordinaryTextures[0].height;
        texture2DArray.filterMode = FilterMode.Point;
        texture2DArray.wrapMode = TextureWrapMode.Repeat;
        //texture2DArray.mipMapBias = -1.0f;
                           // Loop through ordinary textures and copy pixels to the
                                                             // Texture2DArray
        for (int i = 0; i < ordinaryTextures.Length*3; i+=3)
        {
            texture2DArray.SetPixels(ordinaryTextures[i/3].GetPixels(0, 0, tWidth, tHeight, 0), i, 0);
            texture2DArray.SetPixels(ordinaryTextures[i/3].GetPixels(tWidth, 0, tWidth, tHeight, 0), i+1, 0);
            texture2DArray.SetPixels(ordinaryTextures[i/3].GetPixels(tWidth * 2, 0, tWidth, tHeight, 0), i+2, 0);

        }
        // Apply our changes
        texture2DArray.Apply();
        // Set the texture to a material
        objectToAddTextureTo.GetComponent<Renderer>()
            .sharedMaterial.SetTexture("_Block2DArray", texture2DArray);
    }

    private void Start()
    {
        objectToAddTextureTo = this.gameObject;
        CreateTextureArray();
    }
}
