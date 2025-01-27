//------------------------------------------------------------
// Xaz Framework
// Feedback: qq515688254
//------------------------------------------------------------
//----------------------------------------------------
// Xaz: A Framework For Unity
//----------------------------------------------------

using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Xaz;

namespace XazEditor
{
    [CustomEditor(typeof(Xaz.UISubGroup), true)]
    public class UISubGroupInspector : UITableViewInspector
    {
        protected override void DrawCustomProperties()
        {
            XazEditorTools.SetLabelWidth(120f);
            GUILayout.Space(3f);

            XazEditorTools.DrawProperty("Alignment", serializedObject, "m_Alignment");
            XazEditorTools.DrawProperty("Spacing", serializedObject, "m_Spacing");
            XazEditorTools.DrawProperty("Child Alignment", serializedObject, "m_ChildAlignment");

            SerializedProperty maxPerLine = serializedObject.FindProperty("m_MaxPerLine");
            XazEditorTools.DrawProperty("Max Per Line", maxPerLine);
            if (maxPerLine.intValue <= 0)
            {
                maxPerLine.intValue = 1;
            }
            if (!EditorApplication.isPlaying)
            {
                SerializedProperty sp = serializedObject.FindProperty("m_CellList");
                if (sp != null && sp.isArray)
                {
                    if (XazEditorTools.DrawHeader("Cells"))
                    {
                        XazEditorTools.BeginContents();
                        for (int i = 0; i < sp.arraySize; i++)
                        {
                            SerializedProperty item = sp.GetArrayElementAtIndex(i);
                            UnityEngine.Object obj = item.objectReferenceValue;
                            if (obj == null)
                            {
                                sp.DeleteArrayElementAtIndex(i--);
                            }
                            else
                            {
                                if (i == 0)
                                {
                                    EditorGUILayout.BeginHorizontal();
                                }
                                UnityEngine.Object ret = EditorGUILayout.ObjectField(obj, typeof(Xaz.UITableViewCell), true);
                                if (i == 0)
                                {
                                    EditorGUILayout.LabelField("Default", GUILayout.Width(50f));
                                    EditorGUILayout.EndHorizontal();
                                }
                                if (ret != obj)
                                {
                                    if (ret == null)
                                    {
                                        sp.DeleteArrayElementAtIndex(i--);
                                    }
                                    else
                                    {
                                        item.objectReferenceValue = ret;
                                    }
                                }
                            }
                        }
                        UnityEngine.Object result = null;
                        result = EditorGUILayout.ObjectField(result, typeof(Xaz.UITableViewCell), true);
                        if (result != null)
                        {
                            sp.InsertArrayElementAtIndex(sp.arraySize);
                            sp.GetArrayElementAtIndex(sp.arraySize - 1).objectReferenceValue = result;
                        }
                        XazEditorTools.EndContents();
                    }
                }
            }
            EditorGUILayout.Space();
            if (XazEditorTools.DrawHeader("Scroller"))
            {
                XazEditorTools.BeginContents();
                XazEditorTools.DrawProperty("Direction", serializedObject, "m_Direction");
                XazEditorTools.EndContents();
            }
            base.DrawEditorProperties();
        }

        #region Editor by xiejie 自动初始化构建
        [MenuItem("GameObject/UI/UISubGroup", false, 0)]
        static private void UIStateCreate()
        {
            if (Selection.activeTransform)
            {
                if(Selection.activeTransform.GetComponent<UITableViewCell>()==null &&
                    Selection.activeTransform.GetComponent<UIFixTableViewCell>() == null)
                {
                    EditorUtility.DisplayDialog("Error提示", "此组件仅用于嵌套list", "确定");
                    return;
                }
                GameObject go = new GameObject("subgroups", typeof(UISubGroup));
                go.transform.SetParent(Selection.activeTransform, false);
                (go.transform as RectTransform).sizeDelta = new Vector2(500, 400);
                go.AddComponent<RectMask2D>();
                GameObject cell = new GameObject("cell", typeof(UITableViewCell), typeof(RectTransform));
                cell.transform.SetParent(go.transform, false);
                UITableViewCell cellcom = cell.GetComponent<UITableViewCell>();
                cellcom.identifier = "";
                GameObject img = new GameObject("img", typeof(Image), typeof(RectTransform));
                img.transform.SetParent(cell.transform, false);
                (img.transform as RectTransform).sizeDelta = new Vector2(90, 90);
                Selection.activeGameObject = go;
            }
        }
        #endregion
    }
}