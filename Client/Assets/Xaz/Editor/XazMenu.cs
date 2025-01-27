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
    [InitializeOnLoad]
    static public class XazMenu
    {
        static XazMenu()
        {
            TagHelper.AddTags(XazConfig.UIViewTagName, XazConfig.UIPropertyTagName, XazConfig.UIIgnoreTagName);
        }

        [MenuItem("Xaz/Create UIView", false, 0)]
        //[MenuItem("GameObject/UI/Xaz/UIView", false, 1969)]
        static private void CreateUIView(MenuCommand command)
        {
            GameObject root = GameObject.Find(XazConfig.viewRootNode);
            GameObject view = new GameObject("UITestView", typeof(RectTransform), typeof(Xaz.UIViewSettings));
            view.layer = LayerMask.NameToLayer("UI");
           // view.tag = XazConfig.UIViewTagName;
            string uniqueName = GameObjectUtility.GetUniqueNameForSibling(root.transform, view.name);
            view.name = uniqueName;
            Undo.RegisterCreatedObjectUndo(view, "Create " + view.name);
            Undo.SetTransformParent(view.transform, root.transform, "Parent " + view.name);
            GameObjectUtility.SetParentAndAlign(view, root.gameObject);

            RectTransform t = view.GetComponent<RectTransform>();
            t.offsetMax = t.offsetMin = t.anchorMin = Vector2.zero;
            t.anchorMax = Vector2.one;
            t.pivot = new Vector2(0.5f, 0.5f);

            Selection.activeGameObject = view;
        }

        static public GameObject GetOrCreateCanvasGameObject()
        {
            GameObject selectedGo = Selection.activeGameObject;
            Canvas canvas = (selectedGo != null) ? selectedGo.GetComponentInParent<Canvas>() : null;
            if (canvas != null && canvas.gameObject.activeInHierarchy)
                return canvas.gameObject;

            Debug.LogError("需要找到UIViewRoot节点");
            return null;
        }

        static public void ExportView()
        {
            ExportUIView();
        }

        [MenuItem("Xaz/Export View (C#)", false, 60)]
        static private void ExportUIView()
        {
           // UIViewExporter.Export(XazConfig.UIPrefabPath, "Assets/Scripts/UI/Base", new UI.CSharpScriptGenerator());
        }

        [MenuItem("策划工具/导表", false, 21)]
        static public void ExportExcel()
        {
            Excel2CsBytesTool.Excel2Xml2Bytes();
        }

        [MenuItem("策划工具/打开表所在目录", false, 21)]
        static public void OpenExcel()
        {
            Excel2CsBytesTool.OpenExcelDirectory();
        }

        ///// <summary>
        ///// 自动取消RatcastTarget
        ///// </summary>
        //[MenuItem("GameObject/UI/Image")]
        //static void CreatImage2()
        //{
        //    if (Selection.activeTransform)
        //    {
        //        if (Selection.activeTransform.GetComponentInParent<Canvas>())
        //        {
        //            GameObject go = new GameObject("image", typeof(Image));
        //            go.GetComponent<Image>().raycastTarget = false;
        //            go.transform.SetParent(Selection.activeTransform);
        //            go.GetComponent<Image>().rectTransform.localScale = Vector3.one;
        //            go.GetComponent<Image>().rectTransform.localPosition = Vector3.zero;
        //        }
        //    }
        //}

        [MenuItem("GameObject/断开prefab",false,0)]
        static void UnPackPrefab()
        {
            if (Selection.activeTransform)
            {
                PrefabUtility.UnpackPrefabInstance(Selection.activeTransform.gameObject,PrefabUnpackMode.Completely,InteractionMode.UserAction);
            }
        }

        [MenuItem("GameObject/Remove Raycast from Selected Objects")]
        static void RemoveRaycastFromSelectedObjects()
        {
            // 获取选中的对象
            GameObject[] selectedObjects = Selection.gameObjects;

            foreach (GameObject obj in selectedObjects)
            {
                // 检查对象是否有Button组件
                if (obj.GetComponent<Button>() == null)
                {
                    // 获取对象的所有子组件
                    Graphic[] graphics = obj.GetComponentsInChildren<Graphic>(true);

                    foreach (Graphic graphic in graphics)
                    {
                        // 禁用raycastTarget属性
                        graphic.raycastTarget = false;
                    }
                }
            }

            Debug.Log("Removed Raycast from selected objects, excluding those with Button components.");
        }

        //重写Create->UI->Text事件
        [MenuItem("GameObject/UI/Text")]
        static void CreatText()
        {
            if (Selection.activeTransform)
            {
                //如果选中的是列表里的Canvas  
                if (Selection.activeTransform.GetComponentInParent<Canvas>())
                {
                    //新建Text对象  
                    GameObject go = new GameObject("text", typeof(Text));
                    //将raycastTarget置为false  
                    Text txt = go.GetComponent<Text>();
                    txt.raycastTarget = false;
                    //根据项目设置默认字体
                    txt.font = (Font)AssetDatabase.LoadAssetAtPath(XazConfig.DEFAULT_Font, typeof(Font));
                    txt.fontSize = 30;
                    txt.alignment = TextAnchor.MiddleCenter;
                    txt.text = "文本显示";
                    txt.color = Color.black;
                    txt.rectTransform.sizeDelta = new Vector2(200, 60);
                    //设置其父物体  
                    go.transform.SetParent(Selection.activeTransform);
                    txt.rectTransform.localScale = Vector3.one;
                    txt.rectTransform.localPosition = Vector3.zero;
                }
            }
        }

        [MenuItem("GameObject/UI/UIRawImage")]
        static void CreatRawImage2()
        {
            if (Selection.activeTransform)
            {
                //如果选中的是列表里的Canvas  
                if (Selection.activeTransform.GetComponentInParent<Canvas>())
                {
                    //新建Text对象  
                    GameObject go = new GameObject("RawImage", typeof(UIRawImge));
                    //将raycastTarget置为false  
                    go.GetComponent<UIRawImge>().raycastTarget = false;
                    //设置其父物体  
                    go.transform.SetParent(Selection.activeTransform);
                    go.GetComponent<UIRawImge>().rectTransform.localScale = Vector3.one;
                    go.GetComponent<UIRawImge>().rectTransform.localPosition = Vector3.zero;
                }
            }
        }
    }
}


namespace XazEditor
{
    static public partial class ExtensionMethod
    {
        static public string ToUpperFirst(this string str)
        {
            if (str == string.Empty)
                return str;
            return str.Substring(0, 1).ToUpper() + str.Substring(1);
        }
        static public string ToLowerFirst(this string str)
        {
            if (str == string.Empty)
                return str;
            return str.Substring(0, 1).ToLower() + str.Substring(1);
        }
    }
}