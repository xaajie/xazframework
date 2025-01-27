//------------------------------------------------------------
// Xaz Framework
// Feedback: qq515688254
//------------------------------------------------------------
//----------------------------------------------------
// Xaz: A Framework For Unity
//----------------------------------------------------

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Xaz;

namespace XazEditor
{
	[CustomEditor(typeof(Xaz.UITableView), true)]
	public class UITableViewInspector : UIScrollerInspector
	{
		protected override void DrawCustomProperties()
		{
			XazEditorTools.SetLabelWidth(120f);
			GUILayout.Space(3f);

			XazEditorTools.DrawProperty("Alignment", serializedObject, "m_Alignment");

			EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Padding"), new GUIContent("Padding"), true);

			XazEditorTools.DrawProperty("Spacing", serializedObject, "m_Spacing");
			XazEditorTools.DrawProperty("Child Alignment", serializedObject, "m_ChildAlignment");
            XazEditorTools.DrawProperty("noDrag", serializedObject, "m_noDrag");
            SerializedProperty maxPerLine = serializedObject.FindProperty("m_MaxPerLine");
			XazEditorTools.DrawProperty("Max Per Line", maxPerLine);
			if (maxPerLine.intValue <= 0) {
				maxPerLine.intValue = 1;
			}

			//SerializedProperty maxSelection = serializedObject.FindProperty("m_MaxSelection");
			//XazEditorTools.DrawProperty("Max Selection", maxSelection);
			//if (maxSelection.intValue == 1) {
			//	GUILayout.BeginHorizontal();
			//	GUILayout.Space(15f);
			//	XazEditorTools.SetLabelWidth(105f);
			//	XazEditorTools.DrawProperty("Cancelable", serializedObject, "m_Cancelable");
			//	XazEditorTools.SetLabelWidth(120f);
			//	GUILayout.EndHorizontal();
			//} else if (maxSelection.intValue < 0) {
			//	maxSelection.intValue = 0;
			//}

			XazEditorTools.DrawProperty("Loop Cells", serializedObject, "m_Loop");

			SerializedProperty snapping = serializedObject.FindProperty("m_Snapping");
			XazEditorTools.DrawProperty("Snapping", snapping);
			if (snapping.boolValue) {
				GUILayout.BeginHorizontal();
				GUILayout.Space(15f);
				XazEditorTools.SetLabelWidth(105f);
				GUILayout.BeginVertical();
				XazEditorTools.DrawProperty("CellCenter", serializedObject, "m_SnapCellCenter");
				XazEditorTools.DrawProperty("VelocityThreshold", serializedObject, "m_SnapVelocityThreshold");
                XazEditorTools.DrawProperty("snapStrength", serializedObject, "m_snapStrength");
				GUILayout.EndVertical();
				XazEditorTools.SetLabelWidth(120f);
				GUILayout.EndHorizontal();
			}

			XazEditorTools.DrawProperty("LongPress Delay", serializedObject, "m_LongPressDelay");
			XazEditorTools.DrawProperty("LongPress Interval", serializedObject, "m_LongPressInterval");
            XazEditorTools.DrawProperty("OffSet Y Angle", serializedObject, "offSetY_Angle");
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
								UnityEngine.Object ret = EditorGUILayout.ObjectField(obj, typeof(Xaz.UITableViewCell), true);
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
						result = EditorGUILayout.ObjectField(result, typeof(Xaz.UITableViewCell), true);
						if (result != null) {
							sp.InsertArrayElementAtIndex(sp.arraySize);
							sp.GetArrayElementAtIndex(sp.arraySize - 1).objectReferenceValue = result;
						}
						XazEditorTools.EndContents();
					}
				}
			}
			EditorGUILayout.Space();
			if (XazEditorTools.DrawHeader("Scroller")) {
				XazEditorTools.BeginContents();
				base.DrawCustomProperties();
				XazEditorTools.EndContents();
			}
            DrawEditorProperties();
		}
        #region 编辑模式下加载cell (用于调坐标)


        //-------------------编辑模式下加载cell (用于调坐标)-------------------
        UITableView tableView = null;
        int editorShowCellCount = 0;
        List<string> identifiers = new List<string>();
        Dictionary<int, string> ideDic = new Dictionary<int, string>();

        void OnEnable()
        {
            if (target)
            {
                tableView = target as UITableView;
                Transform container = tableView.transform.Find("Container");
                if (container)
                {
                    editorShowCellCount = container.childCount;
                }
                identifiers = tableView.EditorGetCellIdes();
                ideDic.Clear();
            }
        }

        public void DrawEditorProperties()
        {
            if (EditorApplication.isPlaying) return;
            Action refresh = delegate()
            {
                if (editorShowCellCount > 0)
                {
                    tableView.EditorAwake();
                    tableView.onCellInit = delegate(UITableView table, UITableViewCell cell, object data)
                    {
                        //避免 once的cell计算不出来
                    };
                    tableView.Clear(false);
                    for (int i = 0; i < editorShowCellCount; i++)
                    {
                        string iden;
                        if (ideDic.TryGetValue(i, out iden) && !string.IsNullOrEmpty(iden))
                        {
                            tableView.AddData(0, iden);
                        }
                        else
                        {
                            tableView.AddData(0);
                        }
                    }
                    tableView.EditorUpdate();
                }
            };

            Action<int> OnSelectMenu = delegate(int index)
            {
                var options = new GUIContent[identifiers.Count];
                for (int i = 0; i < options.Length; i++)
                {
                    string iden = identifiers[i];
                    options[i] = new GUIContent(string.IsNullOrEmpty(iden) ? "default" : iden);
                }
                var current = Event.current;
                var mousePosition = current.mousePosition;
                var width = options.Length * 10;
                var height = 100;
                var position = new Rect(mousePosition.x, mousePosition.y - height, width, height);
                var selected = -1;
                EditorUtility.DisplayCustomMenu(position, options, selected, delegate (object callBackUserData, string[] callBackOptions, int callBackSelected)
                {
                    UIState.State callBackState = callBackUserData as UIState.State;
                    string iden = callBackOptions[callBackSelected];
                    ideDic[index] = (iden == "default") ? string.Empty : iden;
                    refresh();
                }, index);
            };


            if (XazEditorTools.DrawHeader("Editor"))
            {
                GUILayout.BeginHorizontal();
                int newCount = EditorGUILayout.IntField("Count:", editorShowCellCount);
                if (newCount != editorShowCellCount)
                {
                    editorShowCellCount = newCount;
                    if (newCount > 0)
                        refresh();
                    else
                        tableView.EditorRest(false);
                }
                if (GUILayout.Button("Refresh"))
                {
                    refresh();
                }
                if (GUILayout.Button("Reset"))
                {
                    editorShowCellCount = 0;
                    tableView.EditorRest(false);
                }
                GUILayout.EndHorizontal();

                for (int i = 0; i < editorShowCellCount; i++)
                {
                    GUILayout.BeginHorizontal();
                    string ide;
                    ideDic.TryGetValue(i, out ide);
                    if (GUILayout.Button(i + " : " + (string.IsNullOrEmpty(ide) ? "default" : ide), GUILayout.Width(100)))
                    {
                        OnSelectMenu(i);
                    }
                    GUILayout.EndHorizontal();
                }
            }
        }
        //------------------编辑模式下加载cell (用于调坐标)-------------------
        #endregion



        #region Editor by xiejie 自动初始化构建
        [MenuItem("GameObject/UI/UITableView", false, 0)]
        static private void UIStateCreate()
        {
            if (Selection.activeTransform)
            {
                GameObject go = new GameObject("list", typeof(UITableView));
                go.transform.SetParent(Selection.activeTransform, false);
                go.transform.tag = UIViewExporter.UIPropertyTagName;
                (go.transform as RectTransform).sizeDelta = new Vector2(500, 400);
                go.AddComponent<RectMask2D>();
                GameObject cell = new GameObject("cell", typeof(UITableViewCell), typeof(RectTransform));
                cell.transform.SetParent(go.transform, false);
                UITableViewCell cellcom =  cell.GetComponent<UITableViewCell>();
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