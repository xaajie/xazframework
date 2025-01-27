//------------------------------------------------------------
// Xaz Framework
// Feedback: qq515688254
// author: xiejie
//------------------------------------------------------------
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class ImportSprite : AssetPostprocessor
{
    //请将sprite放在此目录下
    private static string RAW_ATLAS_PATH = UICheckWindow.atlasPreUrl;
    //大张背景图
    public static string[] RAW_BG_PATH = new string[] {
        "AssetsPackage/UI/Img/MapImg/",
        "AssetsPackage/UI/Img/NpcDrawing/" };
    public static string[] RAW_POWER2_BG_PATH = new string[] {
        "Art/UI/Pic/",
        "AssetsPackage/UI/Load/",
        "AssetsPackage/UI/Img/NoviceImg/",
        "AssetsPackage/UI/Img/Viewing/"
    };
    private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromPath)
    {
#if !UNITY_EDITOR_WIN
        if(true)
        {
			 return;
        }
#endif
        List<string> chage = new List<string>();

        foreach (string import in importedAssets)
        {
            OnChangeAtlasPath(import, ref chage);
        }
        foreach (string delete in deletedAssets)
        {
            OnChangeAtlasPath(delete, ref chage);
        }
        foreach (string move in movedAssets)
        {
            AssetDatabase.ImportAsset(move);
        }
        foreach (string path in movedFromPath)
        {
            OnChangeAtlasPath(path, ref chage);
        }
        if (chage.Count > 0)
        {
            AtlasAutoCreate.AtlasAutoCreates();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }

    void OnPreprocessTexture()
    {
        OnImportTexture(assetPath);
    }

    void OnPostprocessTexure(Texture2D texture)
    {
        //Debug.Log(texture.name);
    }
    static private void OnImportTexture(string assetPath)
    {
        //图集图片转格式
        if (assetPath.Contains(RAW_ATLAS_PATH))
        {
            TextureImporter textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (textureImporter != null)
            {
                string AtlasName = new System.IO.DirectoryInfo(System.IO.Path.GetDirectoryName(assetPath)).Name;
                textureImporter.textureType = TextureImporterType.Sprite;
                textureImporter.spriteImportMode = SpriteImportMode.Single;
                textureImporter.spritePackingTag = AtlasName;
                textureImporter.alphaIsTransparency = true;
                textureImporter.mipmapEnabled = false;
                textureImporter.spritePixelsPerUnit = 100;

                textureImporter.wrapMode = TextureWrapMode.Clamp;
                textureImporter.isReadable = false;
                //textureImporter.maxTextureSize = 256;
                //textureImporter.textureCompression = TextureImporterCompression.CompressedHQ;
                //textureImporter.compressionQuality = 100;
                //textureImporter.crunchedCompression = true;
            }
        }

    }

    static private bool CheckInPath(string[] config, string assetPath)
    {
        foreach (string url in config)
        {
            if (assetPath.Contains(url))
            {
                return true;
            }
        }
        return false;
    }
    static private TextureImporterPlatformSettings getPlatSetting(TextureImporter textureImporter, string name)
    {
        TextureImporterPlatformSettings ps = textureImporter.GetPlatformTextureSettings(name);
        ps.overridden = true;
        if (name == "Android")
        {
            ps.format = TextureImporterFormat.ETC2_RGBA8;

        }
        else if (name == "iPhone")
        {
            ps.format = TextureImporterFormat.RGBA32;
        }
        else
        {
            ps.format = TextureImporterFormat.RGBA32;
        }
        return ps;
    }


    static public void OnChangeAtlasPath(string assetPath, ref List<string> change)
    {
        if (assetPath.Contains(RAW_ATLAS_PATH))
        {
            // string path = System.IO.Path.GetDirectoryName(assetPath);
            if (!change.Contains(assetPath))
            {
                change.Add(assetPath);
            }
        }
    }
}
