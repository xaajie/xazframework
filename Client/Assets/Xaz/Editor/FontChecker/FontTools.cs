using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class FontTools
{
    [MenuItem("Tools/批量替换字体", false, 110)]
    public static void ReplaceFont()
    {
        string[] paths = new string[] {
        "Assets/Resources/UI/UIPrefab/"
        };

        List<string> tmp = new List<string>();
        for (int i = 0; i < paths.Length; i++)
        {
            string[] files = Directory.GetFiles(paths[i], "*.prefab", SearchOption.AllDirectories);
            tmp.AddRange(files);
        }
        string[] prefabFiles = tmp.ToArray();


        

        Dictionary<string, Font> dic = new Dictionary<string, Font>();
        dic.Add("system_art", AssetDatabase.LoadAssetAtPath<Font>("Assets/Resources/Fonts/GROBOLD.TTF"));
        dic.Add("mini_new", AssetDatabase.LoadAssetAtPath<Font>("Assets/Resources/Fonts/GROBOLD.TTF"));
        dic.Add("system_Btn", AssetDatabase.LoadAssetAtPath<Font>("Assets/Resources/Fonts/GROBOLD.TTF"));


        foreach (string file in prefabFiles)
        {

            GameObject go = AssetDatabase.LoadAssetAtPath(file, typeof(GameObject)) as GameObject;
            Text[] all = go.GetComponentsInChildren<Text>(true);
            foreach (Text tex in all)
            {
                if (tex.font != null)
                {
                    string name = tex.font.name;

                    if (name.Equals("system") || name.Equals("FZFYSJW")||name.Equals("ysz_0"))
                    {

                    }
                    else
                    {

                        Font newFont;
                        if (dic.TryGetValue(name, out newFont))
                        {
                            Debug.Log("prefabName:"+go.name+"\tnodeName:"+tex.gameObject.name+"\t"+tex.font.name+"-->"+newFont.name);
                            tex.font = newFont;
                            //EditorUtility.SetDirty(GetRootGameObject(tex.gameObject.transform));
                            EditorUtility.SetDirty(tex.gameObject.transform);
                        }
                        else
                        {
                            newFont = AssetDatabase.LoadAssetAtPath<Font>("Assets/Resources/Fonts/GROBOLD.TTF");
                            Debug.Log("prefabName:" + go.name + "\tnodeName:" + tex.gameObject.name + "\t" + tex.font.name + "-->" + newFont.name);
                            tex.font = newFont;
                            //EditorUtility.SetDirty(GetRootGameObject(tex.gameObject.transform));
                            EditorUtility.SetDirty(tex.gameObject.transform);
                        }
                    }
                }
            }
        }
        AssetDatabase.SaveAssets();
    }


    [MenuItem("GameObject/替换预制件中字体",false,10)]
    public static void ReplaceSelectFont()
    {
        if (Selection.activeTransform)
        {
            Font newFont = AssetDatabase.LoadAssetAtPath<Font>("Assets/Resources/Fonts/GROBOLD.TTF");
            Text[] all = Selection.activeTransform.gameObject.GetComponentsInChildren<Text>(true);
            foreach (Text tex in all)
            {
                if (tex.font != null)
                {
                    string name = tex.font.name;
                    tex.font = newFont;
                    //EditorUtility.SetDirty(GetRootGameObject(tex.gameObject.transform));
                    EditorUtility.SetDirty(tex.gameObject.transform);
                }
            }
        }
    }
}
