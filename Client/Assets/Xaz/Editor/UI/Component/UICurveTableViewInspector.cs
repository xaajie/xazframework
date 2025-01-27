//------------------------------------------------------------
// Xaz Framework
// Feedback: qq515688254
// Xaz: A Framework For Unity
//  @author xiejie
//----------------------------------------------------


using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Xaz;

namespace XazEditor
{
    [CustomEditor(typeof(Xaz.UICurveTableView), true)]
    public class UICurveTableViewInspector : UIScrollerInspector
    {
        protected override void DrawCustomProperties()
        {
            XazEditorTools.SetLabelWidth(120f);
            GUILayout.Space(3f);
            XazEditorTools.DrawProperty("Alignment", serializedObject, "m_Alignment");
            XazEditorTools.DrawProperty("Spacing", serializedObject, "m_Spacing");
            //有需要再加吧
            //XazEditorTools.DrawProperty("LongPress Delay", serializedObject, "m_LongPressDelay");
            //XazEditorTools.DrawProperty("LongPress Interval", serializedObject, "m_LongPressInterval");
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
                base.DrawCustomProperties();
                XazEditorTools.EndContents();
            }
            DrawEditorProperties();
        }

        #region 编辑模式下加载cell (用于调坐标)
        UICurveTableView tableView = null;
        int editorShowCellCount = 0;
        List<string> identifiers = new List<string>();
        Dictionary<int, string> ideDic = new Dictionary<int, string>();

        void OnEnable()
        {
            if (target)
            {
                tableView = target as UICurveTableView;
                Transform container = tableView.transform.Find("Container");
                if (container)
                {
                    editorShowCellCount = container.childCount;
                }
                identifiers = tableView.EditorGetCellIdes();
                ideDic.Clear();
            }
        }

        void DrawEditorProperties()
        {
            if (EditorApplication.isPlaying) return;
            Action refresh = delegate ()
            {
                if (editorShowCellCount > 0)
                {
                    tableView.EditorAwake();
                    tableView.onCellInit = delegate (UITableView table, UITableViewCell cell, object data)
                    {
                        tableView.EditorCalculateCellOffset(table, cell, data);
                        //避免 once的cell计算不出来
                    };
                    tableView.Clear(false);
                    for (int i = 0; i < editorShowCellCount; i++)
                    {
                        string iden;
                        if (ideDic.TryGetValue(i, out iden) && !string.IsNullOrEmpty(iden))
                        {
                            tableView.AddData(i, iden);
                        }
                        else
                        {
                            tableView.AddData(0);
                        }
                    }
                    tableView.EditorUpdate();
                    tableView.EditorCurveShow();
                }
            };

            if (XazEditorTools.DrawHeader("Editor"))
            {
                GUILayout.BeginHorizontal();
                int newCount = EditorGUILayout.IntField("Count:", editorShowCellCount);
                if (newCount != editorShowCellCount)
                {
                    editorShowCellCount = 0;
                    tableView.EditorRest(false);
                    editorShowCellCount = newCount;
                    refresh();
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
            }
        }
        #endregion
        #region Editor by xiejie 自动初始化构建
        [MenuItem("GameObject/UI/UICurveTableView", false, 0)]
        static private void UIStateCreate()
        {
            if (Selection.activeTransform)
            {
                GameObject go = new GameObject("curvelist", typeof(UICurveTableView));
                go.transform.tag = UIViewExporter.UIPropertyTagName;
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
                UICurveTableView tv = go.GetComponent<UICurveTableView>();
                Selection.activeGameObject = go;
            }
        }
        #endregion
    }
}