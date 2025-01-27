//------------------------------------------------------------
// Xaz Framework
// Feedback: qq515688254
//------------------------------------------------------------

using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Xaz;

namespace XazEditor
{
    [CustomEditor(typeof(Xaz.UIFixTableView), true)]
	public class UIFixTableViewInspector : UIScrollerInspector
	{
		protected override void DrawCustomProperties()
		{
			XazEditorTools.SetLabelWidth(120f);
			GUILayout.Space(3f);
            GUILayout.BeginHorizontal();
            GUILayout.Space(15f);
            XazEditorTools.SetLabelWidth(105f);
            XazEditorTools.SetLabelWidth(120f);
            GUILayout.EndHorizontal();
            if (!EditorApplication.isPlaying) {
				SerializedProperty sp = serializedObject.FindProperty("m_CellList");
				if (sp != null && sp.isArray) {
					if (XazEditorTools.DrawHeader("Cells")) {
						XazEditorTools.BeginContents();
						for (int i = 0; i < sp.arraySize; i++) {
							SerializedProperty item = sp.GetArrayElementAtIndex(i);
							UnityEngine.Object obj = item.objectReferenceValue;
							if (obj == null) {
								sp.DeleteArrayElementAtIndex(i--);
							} else {
								if (i == 0) {
									EditorGUILayout.BeginHorizontal();
								}
								UnityEngine.Object ret = EditorGUILayout.ObjectField(obj, typeof(Xaz.UIFixTableViewCell), true);
								if (i == 0) {
									EditorGUILayout.LabelField("Default", GUILayout.Width(50f));
									EditorGUILayout.EndHorizontal();
								}
								if (ret != obj) {
									if (ret == null) {
										sp.DeleteArrayElementAtIndex(i--);
									} else {
										item.objectReferenceValue = ret;
									}
								}
							}
						}
						UnityEngine.Object result = null;
						result = EditorGUILayout.ObjectField(result, typeof(Xaz.UIFixTableViewCell), true);
						if (result != null) {
							sp.InsertArrayElementAtIndex(sp.arraySize);
							sp.GetArrayElementAtIndex(sp.arraySize - 1).objectReferenceValue = result;
						}
						XazEditorTools.EndContents();
					}
				}
			}
			EditorGUILayout.Space();
		}

        #region Editor by xiejie 自动初始化构建
        [MenuItem("GameObject/UI/UIFixTableView", false, 0)]
        static private void UIFixListStateCreate()
        {
            if (Selection.activeTransform)
            {
                GameObject go = new GameObject("fixlist", typeof(UIFixTableView), typeof(RectTransform));
                go.transform.tag = UIViewExporter.UIPropertyTagName;
                go.transform.SetParent(Selection.activeTransform, false);
                (go.transform as RectTransform).sizeDelta = new Vector2(500, 400);
                go.AddComponent<RectMask2D>();
                GameObject cell = new GameObject("cell", typeof(UIFixTableViewCell), typeof(RectTransform));
                cell.transform.SetParent(go.transform, false);
                UIFixTableViewCell cellcom =  cell.GetComponent<UIFixTableViewCell>();
                cellcom.identifier = "";
                GameObject img = new GameObject("img", typeof(Image), typeof(RectTransform));
                img.transform.SetParent(cell.transform, false);
                (img.transform as RectTransform).sizeDelta = new Vector2(90, 90);
                //jietodo 默认挂cell
                //UITableView tv = go.GetComponent<UITableView>();
                //tv.m_CellList.
                Selection.activeGameObject = go;
            }
        }
        #endregion
    }
}