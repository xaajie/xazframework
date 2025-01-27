using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class OutlineEffectModifier : MonoBehaviour
{
    [MenuItem("Tools/Modify Outline Effect Distance")]
    public static void ModifyOutlineEffectDistance()
    {
        // ��ȡѡ�еĶ���
        GameObject[] selectedObjects = Selection.gameObjects;

        foreach (GameObject obj in selectedObjects)
        {
            // ��ȡ�����Ӷ����й���Outline����Ķ���
            Outline[] outlines = obj.GetComponentsInChildren<Outline>(true);

            foreach (Outline outline in outlines)
            {
                // �޸�Effect Distance����
                outline.effectDistance = new Vector2(1, -1);
                EditorUtility.SetDirty(outline); // ��Ƕ���Ϊ�Ѹ���
            }
        }

        // ���������޸�
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Effect Distance parameters updated for selected objects.");
    }
}
