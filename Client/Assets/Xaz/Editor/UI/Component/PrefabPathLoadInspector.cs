//------------------------------------------------------------
// Xaz Framework
// 加载固定prefab组件（编辑态可视，运行态才加载）
// 组件编辑视角
// Feedback: qq515688254
//------------------------------------------------------------

using UnityEditor;
using UnityEngine;
using Xaz;
namespace XazEditor
{
    [CustomEditor(typeof(PrefabPathLoad), true)]
    public class PrefabPathLoadInspector :Editor
    {
        private string resetTxt = "reset(clear path&prefab)";
        private string tiptext = "拖入要显示的prefab可视\n注：该组件在编辑模式下加载可视，导出界面时只保留路径会删除prefab，\n运行时动态加载指定路径的prefab";
        PrefabPathLoad pathLoad = null;
        void OnEnable()
        {
            if (target)
            {
                pathLoad = target as PrefabPathLoad;
            }
        }
        public override void OnInspectorGUI()
        {
            XazEditorTools.SetLabelWidth(120f);
            GUILayout.Space(3f);
            GUILayout.Label(tiptext);
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.TextArea(pathLoad._prefabPath, GUILayout.Height(50));
            EditorGUI.EndDisabledGroup();
            serializedObject.ApplyModifiedProperties();
            if (GUILayout.Button(resetTxt))
            {
                pathLoad._prefabPath = string.Empty;
                pathLoad.ClearLoadPrefab();
            }
            if (GUILayout.Button("save"))
            {
                pathLoad.ClearLoadPrefab();
            }
            base.OnInspectorGUI();
        }

        static private PrefabPathLoad CreatePathLoader(string name)
        {
            GameObject go = new GameObject(name);
            PrefabPathLoad comp = go.AddComponent<PrefabPathLoad>();
            go.transform.SetParent(Selection.activeTransform);
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;
            return comp;
        }
        [MenuItem("GameObject/UI/UIFullBg")]
        static private void CreateFull()
        {
            if (Selection.activeTransform)
            {
                GameObject loadPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/AssetsPackage/UI/Prefabs/UtilPrefab/UtilBackGround.prefab");
                loadPrefab = Instantiate(loadPrefab);
                loadPrefab.transform.SetParent(Selection.activeTransform);
                loadPrefab.transform.localPosition = Vector3.zero;
                loadPrefab.transform.localScale = Vector3.one;
                RectTransform targetv = loadPrefab.GetComponent<RectTransform>();
                targetv.anchorMin = Vector2.zero;
                targetv.anchorMax = Vector2.one;
                targetv.offsetMin = Vector2.zero;
                targetv.offsetMax = Vector2.zero;
            }
        }

        [MenuItem("GameObject/UI/UIFixEffect (路径装载器)")]
        static private void CreateEffect()
        {
            if (Selection.activeTransform)
            {
                PrefabPathLoad comp = CreatePathLoader("effectNode");
                GameObject go = comp.gameObject;
                RectTransform targetv = go.AddComponent<RectTransform>();
                comp.SetPrefabPath("Assets/AssetsPackage/UI/Prefabs/FixEffect/testprefab.prefab");
            }
        }

        [MenuItem("GameObject/路径装载器")]
        static private void CreatePrefabLoader()
        {
            if (Selection.activeTransform)
            {
                PrefabPathLoad comp = CreatePathLoader("prefabLoader");
                comp.SetPrefabPath("Assets/AssetsPackage/Prefabs/Item/testCube.prefab");
            }
        }
    }
}
