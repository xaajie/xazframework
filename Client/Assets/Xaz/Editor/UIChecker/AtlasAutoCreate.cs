using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.U2D;
/// <summary>
/// 自动创建spriteAtlas图集工具
///  xiejie
/// </summary>
public class AtlasAutoCreate : Editor
{
    private static string outPath = XazConfig.SpritPath;
    private static string extname = ".spriteatlas";
    [MenuItem("程序工具/Auto Create Atlas")]
    public static void AtlasAutoCreates()
    {
        string rpath = UICheckWindow.atlasPreUrl;
        DirectoryInfo direction = new DirectoryInfo(rpath);
        DirectoryInfo[] directs = direction.GetDirectories();//文件夹
        DirectoryInfo dir;
        Dictionary<string, int> spriteatlasName = new Dictionary<string, int>();
        int i;
        for (i = 0; i < directs.Length; i++)
        {
            dir = directs[i];
            string dataPath = dir.FullName;

            //创建图集
            string atlas = outPath + dir.Name + extname;
            spriteatlasName.Add(dir.Name, 1);
            if (File.Exists(atlas))
            {
                Debug.Log("图集找到" + atlas);
            }
            else
            {
                SpriteAtlas sa = new SpriteAtlas();

                SpriteAtlasPackingSettings packSet = new SpriteAtlasPackingSettings()
                {
                    blockOffset = 1,
                    enableRotation = false,
                    enableTightPacking = false,
                    padding = 4,
                };
                sa.SetPackingSettings(packSet);


                SpriteAtlasTextureSettings textureSet = new SpriteAtlasTextureSettings()
                {
                    readable = true,
                    generateMipMaps = false,
                    sRGB = true,
                    filterMode = FilterMode.Bilinear,
                };
                sa.SetTextureSettings(textureSet);
                AssetDatabase.CreateAsset(sa, atlas);
                //图片的文件夹加入图集。
                Object texture = AssetDatabase.LoadMainAssetAtPath(rpath + dir.Name);
                SpriteAtlasExtensions.Add(sa, new Object[] { texture });
                AssetDatabase.SaveAssets();
            }
        }

        //删除已不匹配的
        //删除已不匹配的
        string[] files = Directory.GetFiles(outPath, "*"+extname, SearchOption.AllDirectories);
        foreach (string file in files)
        {
            string filename = Path.GetFileNameWithoutExtension(file);
            if (!spriteatlasName.ContainsKey(filename))
            {
                File.Delete(file);
            }
        }
        AssetDatabase.SaveAssets();
    }
}