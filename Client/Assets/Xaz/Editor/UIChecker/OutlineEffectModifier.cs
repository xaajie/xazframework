using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class OutlineEffectModifier : MonoBehaviour
{
    [MenuItem("Tools/Modify Outline Effect Distance")]
    public static void ModifyOutlineEffectDistance()
    {
        // 获取选中的对象
        GameObject[] selectedObjects = Selection.gameObjects;

        foreach (GameObject obj in selectedObjects)
        {
            // 获取所有子对象中挂有Outline组件的对象
            Outline[] outlines = obj.GetComponentsInChildren<Outline>(true);

            foreach (Outline outline in outlines)
            {
                // 修改Effect Distance参数
                outline.effectDistance = new Vector2(1, -1);
                EditorUtility.SetDirty(outline); // 标记对象为已更改
            }
        }

        // 保存所有修改
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Effect Distance parameters updated for selected objects.");
    }
}
