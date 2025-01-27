//------------------------------------------------------------
// Xaz Framework
// Feedback: qq515688254
//  界面导出
//  @author xiejie 
//----------------------------------------------
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Xaz;
namespace XazEditor
{
    public abstract class ScriptGenerator
    {
        public string extension
        {
            get;
            protected set;
        }

        public abstract void Generate(string file, List<KeyValuePair<Component, string>> properties, string prefabPath, List<UIViewExporter.UISubViewCollection> tables, List<UIViewExporter.UISubViewCollection> subgrops);
    }
    static public class UIViewExporter
    {
        public class UISubViewCollection
        {
            public class View
            {
                public string name;
                public string identifier;
                public List<KeyValuePair<Component, string>> properties = new List<KeyValuePair<Component, string>>();
            }
            public string name;
            public List<View> views = new List<View>();
        }

        static public string UIViewTagName = "UIView";
        static public string UIIgnoreTagName = "UIIgnore";
        static public string UIPropertyTagName = "UIProperty";
        static private List<Transform> m_ViewStack = new List<Transform>();
        static private List<Transform> m_TransformStack = new List<Transform>();
        static private Dictionary<Transform, List<string>> m_ViewPropertyNames = new Dictionary<Transform, List<string>>();
        static private Dictionary<Transform, List<KeyValuePair<Component, string>>> m_ViewComponents = new Dictionary<Transform, List<KeyValuePair<Component, string>>>();
        static private Dictionary<Transform, List<UISubViewCollection>> m_ViewTableCollections = new Dictionary<Transform, List<UISubViewCollection>>();
        static private Dictionary<Transform, List<UISubViewCollection>> m_SubGroupCollections = new Dictionary<Transform, List<UISubViewCollection>>();

        static private List<Type> m_ComponentTypes = new List<Type>() {
            typeof(IControl),//自定义组件基类
            typeof(Button), typeof(Dropdown), typeof(InputField), typeof(Scrollbar), typeof(ScrollRect), typeof(Slider), typeof(Toggle)
        };

        static private List<Type> m_SpecificPropertyTypes = new List<Type>() {
            typeof(Image), typeof(TMP_Text),typeof(RawImage), typeof(Text), typeof(LayoutGroup), typeof(RectTransform), typeof(Transform)
        };
        static List<string> noSafeAreaArr = new List<string> {  };
        static bool IsNoSafeAreaView(string viewName)
        {
            return noSafeAreaArr.IndexOf(viewName) != -1;
        }
        static public void Export(string prefabPatht, string scriptPath, ScriptGenerator generator, bool isComponent = false)
        {
            Debug.Log("检查Tag=UIView,开始导出");
            ClearAll();
            //AssetDatabase.StartAssetEditing();
            GameObject[] list = GameObject.FindGameObjectsWithTag(UIViewTagName);
            bool onlyOne = list.Length <= 1;
            foreach (var child in list)
            {
                string viewName = child.name.ToUpperFirst();

                UISafeAreaCtrl uISafeAreaCtrl = child.GetComponent<UISafeAreaCtrl>();
                UIViewSettings viewset = child.GetComponent<UIViewSettings>();
                if (IsNoSafeAreaView(viewName) || !viewset.IsNeedCountSafe())
                {
                    if (uISafeAreaCtrl != null)
                    {
                        UnityEngine.Object.DestroyImmediate(uISafeAreaCtrl);
                    }
                }
                else
                {
                    if (uISafeAreaCtrl == null)
                    {
                        child.AddComponent<UISafeAreaCtrl>();
                    }
                }

                m_ViewStack.Add(child.transform);
                m_TransformStack.Add(child.transform);
                List<KeyValuePair<Component, string>> properties = ExtractProperties(child.transform, child.transform);
                string sPath = string.Format("{0}/Base{1}.{2}", scriptPath, viewName, generator.extension);
                List<UISubViewCollection> tablechild = m_ViewTableCollections.ContainsKey(child.transform) ? m_ViewTableCollections[child.transform] : null;
                List<UISubViewCollection> subgrop = m_SubGroupCollections.ContainsKey(child.transform) ? m_SubGroupCollections[child.transform] : null;

                Debug.Log("导出lua:" + sPath);
                string prefabPath = prefabPatht + viewName.ToString() + ".prefab";
                generator.Generate(sPath, properties, viewName.ToString(), tablechild, subgrop);
                //注意，组件清理处理
                PreResExportUI(child.gameObject);
                XazEditorTools.CreateOrReplacePrefab(child.gameObject, prefabPath);
                Debug.Log("导出Prefab:" + prefabPath);
                GameObject.DestroyImmediate(child.gameObject);
            }
            //AssetDatabase.StopAssetEditing();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            generator = null;

        }

        static private List<KeyValuePair<Component, string>> ExtractProperties(Transform view, Transform root)
        {
            // generate script
            ParseViewComponents(view, root, root);
            List<KeyValuePair<Component, string>> components;
            UIComponentCollection componentCollection = root.GetComponent<UIComponentCollection>();
            if (componentCollection == null)
            {
                componentCollection = root.gameObject.AddComponent<UIComponentCollection>();
            }
            FieldInfo fieldInfo = typeof(UIComponentCollection).GetField("components", BindingFlags.NonPublic | BindingFlags.Instance);
            var fieldValue = fieldInfo.GetValue(componentCollection) as List<Component>;
            fieldValue.Clear();
            if (m_ViewComponents.TryGetValue(root, out components))
            {
                foreach (var kv in components)
                {
                    fieldValue.Add(kv.Key);
                }
            }
            return components;
        }

        static private void ParseUIFixTableView(UIFixTableView tableView)
        {
            Transform view = m_ViewStack[m_ViewStack.Count - 1];
            List<UISubViewCollection> collections;
            if (!m_ViewTableCollections.TryGetValue(view, out collections))
            {
                collections = new List<UISubViewCollection>();
                m_ViewTableCollections[view] = collections;
            }
            FieldInfo cellList = typeof(UIFixTableView).GetField("m_CellList", BindingFlags.Instance | BindingFlags.NonPublic);
            List<UIFixTableViewCell> cells = cellList.GetValue(tableView) as List<UIFixTableViewCell>;
            if (cells != null)
            {
                UISubViewCollection collection = new UISubViewCollection() { name = GetPropertyName(tableView, m_TransformStack[m_TransformStack.Count - 1]) }; // .gameObject.name.ToUpperFirst()
                for (int i = 0; i < cells.Count; i++)
                {
                    var cell = cells[i];
                    collection.views.Add(new UISubViewCollection.View()
                    {
                        identifier = cell.identifier,
                        properties = ExtractProperties(cell.transform, cell.transform)
                    });
                    cell.gameObject.SetActive(false);
                }
                collections.Add(collection);
            }
        }

        static private void ParseUITableView(UITableView tableView, Dictionary<Transform, List<UISubViewCollection>> targetCollections)
        {
            Transform view = m_ViewStack[m_ViewStack.Count - 1];
            List<UISubViewCollection> collections;
            if (!targetCollections.TryGetValue(view, out collections))
            {
                collections = new List<UISubViewCollection>();
                targetCollections[view] = collections;
            }
            FieldInfo cellList = typeof(UITableView).GetField("m_CellList", BindingFlags.Instance | BindingFlags.NonPublic);
            List<UITableViewCell> cells = cellList.GetValue(tableView) as List<UITableViewCell>;
            if (cells != null)
            {
                UISubViewCollection collection = new UISubViewCollection() { name = GetPropertyName(tableView, m_TransformStack[m_TransformStack.Count - 1]) }; // .gameObject.name.ToUpperFirst()
                //Debug.Log(tableView.name+"ssssssssssssssssssss"+ collection.name);
                if (collection.name == string.Empty)
                {
                    collection.name = GenerUpperName(tableView.name);
                }
                for (int i = 0; i < cells.Count; i++)
                {
                    var cell = cells[i];
                    collection.views.Add(new UISubViewCollection.View()
                    {
                        identifier = cell.identifier,
                        properties = ExtractProperties(cell.transform, cell.transform)
                    });
                    cell.gameObject.SetActive(false);
                }
                collections.Add(collection);
            }
        }
        static private void ParseViewComponents(Transform view, Transform root, Transform node)
        {
            Component comp = null;
            bool found = false, parseChildren = true;
            do
            {
                if (node.CompareTag(UIIgnoreTagName))
                {
                    found = false;
                    parseChildren = false;
                    break;
                }
                //注意：此处和tableview顺序不能变
                comp = node.GetComponent<UISubGroup>();
                if (comp != null)
                {
                    found = false;
                    parseChildren = false;
                    AddComponent(view, root, comp, GetTransPath(comp.transform, view.transform));
                    ParseUITableView((UITableView)comp, m_SubGroupCollections);
                    break;
                }

                comp = node.GetComponent<UITableView>();
                if (comp != null)
                {
                    found = false;
                    parseChildren = false;
                    AddComponent(view, root, comp, GetTransPath(comp.transform, view.transform));
                    ParseUITableView((UITableView)comp, m_ViewTableCollections);
                    break;
                }

                comp = node.GetComponent<UIFixTableView>();
                if (comp != null)
                {
                    found = false;
                    parseChildren = false;
                    AddComponent(view, root, comp, GetTransPath(comp.transform, view.transform));
                    ParseUIFixTableView((UIFixTableView)comp);
                    break;
                }
                for (int i = 0; i < m_ComponentTypes.Count; i++)
                {
                    comp = node.GetComponent(m_ComponentTypes[i]);
                    if (comp != null)
                    {
                        found = true;
                        break;
                    }
                }

                if (!found && node.CompareTag(UIPropertyTagName))
                {
                    for (int i = 0; i < m_SpecificPropertyTypes.Count; i++)
                    {
                        comp = node.GetComponent(m_SpecificPropertyTypes[i]);
                        if (comp != null)
                        {
                            found = true;
                            break;
                        }
                    }
                }
            } while (false);

            if (found)
            {
                AddComponent(view, root, comp, GetTransPath(comp.transform, view.transform));
            }

            if (parseChildren)
            {
                foreach (Transform child in node)
                {
                    ParseViewComponents(view, root, child);
                }
            }
        }
        static private string GetTransPath(Transform _tra, Transform startname)
        {
            if (_tra == null)
            {
                UnityEngine.Debug.Log("对象为空" + _tra.name);
            }
            StringBuilder tempPath = new StringBuilder(_tra.name);
            Transform tempTra = _tra;
            string g = "/";
            while (tempTra.parent != null)
            {
                tempTra = tempTra.parent;
                if (tempTra.name == startname.name)
                {
                    break;
                }
                tempPath.Insert(0, tempTra.name + g);
            }
            //Debug.Log("路径: " + tempPath);
            return tempPath.ToString();
        }
        static private void AddComponent(Transform view, Transform root, Component comp, string path)
        {
            List<KeyValuePair<Component, string>> components;
            if (!m_ViewComponents.TryGetValue(root, out components))
            {
                components = new List<KeyValuePair<Component, string>>();
                m_ViewComponents[root] = components;
            }
            components.Add(new KeyValuePair<Component, string>(comp, GetPropertyName(comp.name, view)));
        }

        static private string GetPropertyName(Component comp, Transform root)
        {
            List<KeyValuePair<Component, string>> components;
            if (m_ViewComponents.TryGetValue(root, out components))
            {
                foreach (var kv in components)
                {
                    if (kv.Key == comp)
                    {
                        return kv.Value;
                    }
                }
            }
            return string.Empty;
        }

        static private string GenerUpperName(string name)
        {
            return Regex.Replace(name, "[^_0-9a-zA-Z]", "").ToUpperFirst();
        }
        static private string GetPropertyName(string name, Transform view)
        {
            string propName = GenerUpperName(name);
            List<string> propertyNames;
            if (!m_ViewPropertyNames.TryGetValue(view, out propertyNames))
            {
                propertyNames = new List<string>() { propName };
                m_ViewPropertyNames[view] = propertyNames;
            }
            else
            {
                int i = 1;
                name = propName;
                while (propertyNames.Contains(propName))
                {
                    propName = name + (i++);
                }
                propertyNames.Add(propName);
            }
            return propName;
        }

        static private void ClearAll()
        {
            m_ViewStack.Clear();
            m_TransformStack.Clear();
            m_ViewComponents.Clear();
            m_ViewPropertyNames.Clear();
            m_ViewTableCollections.Clear();
            m_SubGroupCollections.Clear();
        }

        /// <summary>
        /// 生成发布界面Prefab前的预处理
        /// </summary>
        static public void PreResExportUI(GameObject child)
        {
            //layer校正 modifyby xiejie 2020-3-27
            RayUtil.SetLayer(child.gameObject, "UI");

            //设置导出的TableView删除编辑时残留的cell
            foreach (UITableView tableView in child.GetComponentsInChildren<UITableView>(true))
            {
                tableView.EditorRest();
                tableView.EditorRestCell();
            }

            //设置导出的RichText删除编辑时残留的 modifyby xiejie 2020-3-27
            foreach (UIRichText richtext in child.GetComponentsInChildren<UIRichText>(true))
            {
                richtext.EditorRest();
            }

            //UIImage清除默认图片，图集也不能保留 modifyby xiejie 2022-12-12
            foreach (UIImage uiImage in child.GetComponentsInChildren<UIImage>(true))
            {
                uiImage.ResetComp();
            }

            foreach (UIRawImge uiImage in child.GetComponentsInChildren<UIRawImge>(true))
            {
                uiImage.ResetComp();
            }
            foreach (PrefabPathLoad prefabLoad in child.GetComponentsInChildren<PrefabPathLoad>(true))
            {
                prefabLoad.ClearLoadPrefab();
            }
        }

    }
}

