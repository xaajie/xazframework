//------------------------------------------------------------
// Xaz Framework
// Feedback: qq515688254
//------------------------------------------------------------
using UnityEditor;
using UnityEngine;
using XazEditor;

namespace Xaz
{
    [CustomEditor(typeof(UITabButton), true)]
    public class UITabButtonInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            XazEditorTools.SetLabelWidth(120f);
            GUILayout.Space(3f);
            serializedObject.Update();

            XazEditorTools.DrawProperty("Interactable", serializedObject, "m_Interactable");
            GUILayout.Space(3f);
            XazEditorTools.DrawProperty("Is On", serializedObject, "m_IsOn");
            XazEditorTools.DrawProperty("On m_State", serializedObject, "m_State");
            XazEditorTools.DrawProperty("On StateName", serializedObject, "m_OnStateName");
            XazEditorTools.DrawProperty("Off StateName", serializedObject, "m_OffStateName");
            XazEditorTools.DrawProperty("Group", serializedObject, "m_Group");
            GUILayout.Space(5f);
            XazEditorTools.DrawProperty(serializedObject, "onValueChanged");

            serializedObject.ApplyModifiedProperties();
        }

        #region Editor by xiejie 自动初始化构建
        [MenuItem("GameObject/UI/UITabGroup", false, 0)]
        static private void UITabGroupsCreate()
        {
            if (Selection.activeTransform)
            {
                GameObject go = new GameObject("tabGroup", typeof(UITabGroup), typeof(RectTransform));
                go.transform.tag = UIViewExporter.UIPropertyTagName;
                UITabGroup tabGroup = go.GetComponent<UITabGroup>();
                go.transform.SetParent(Selection.activeTransform, false);
                (go.transform as RectTransform).sizeDelta = new Vector2(500, 400);
                for (int i = 0; i < 2; i++)
                {
                    GameObject tab = new GameObject("tab" + i, typeof(UITabButton), typeof(RectTransform));
                    tab.transform.localPosition = (Vector3.zero + Vector3.right * i * 100);
                    UITabButton tabComp = tab.GetComponent<UITabButton>();
                    tabComp.group = go.GetComponent<UITabGroup>();
                    tab.transform.SetParent(go.transform, false);
                    GameObject uistate = UIStateTools.CreateExample(tab.transform);
                    uistate.transform.SetParent(tab.transform, false);
                    uistate.name = "uistate" + i;
                    UIState sta = uistate.GetComponent<UIState>();
                    sta.tag = UIViewExporter.UIIgnoreTagName;
                    tabComp.SetStateTarget(sta);
                    tabComp.isOn = i == 0;
                }
                Selection.activeGameObject = go;
                EditorUtility.SetDirty(go);
            }
        }
        #endregion
    }

}
