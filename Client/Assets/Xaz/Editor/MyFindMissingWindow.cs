using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;
using System.Text;
using UnityEngine.Playables;
using UnityEditor.AnimatedValues;

public class MyFindMissingWindow : EditorWindow
{
    [MenuItem("资源工具/搜索丢失引用的资源")]
    static void ShowWindow()
    {
        GetWindow<MyFindMissingWindow>("查找Missing资源").Show();
        Find();
    }

    static Dictionary<Object, List<Object>> artPrefabs = new Dictionary<Object, List<Object>>();
    static Dictionary<Object, List<string>> artRefPaths = new Dictionary<Object, List<string>>();
    static Dictionary<Object, List<Object>> codePrefabs = new Dictionary<Object, List<Object>>();
    static Dictionary<Object, List<string>> codeRefPaths = new Dictionary<Object, List<string>>();
    //寻找missing的资源
    static void Find()
    {
        //获取所有资源的路径
        string[] paths = AssetDatabase.GetAllAssetPaths();
        //筛选出prefab资源的路径列表
        var prefabPath = paths.Where(path => path.EndsWith("prefab"));
        //用于缓存需要打印的内容
        StringBuilder artContent = new StringBuilder();
        StringBuilder codeContent = new StringBuilder();
        foreach (string path in prefabPath)
        {
            var go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (go == null)
            {
                continue;
            }
            Component[] cps = go.GetComponentsInChildren<Component>(true);
            foreach (var cp in cps)
            {
                if (cp == null)//组件为空
                {
                    //丢失脚本加入程序关心列表
                    if (!codePrefabs.ContainsKey(go)) codePrefabs.Add(go, new List<Object>());
                    codePrefabs[go].Add(cp);
                    codeContent.Append(path + "->\t" + "丢失脚本" + "\t");
                }
                else
                {
                    SerializedObject so = new SerializedObject(cp);
                    SerializedProperty iterator = so.GetIterator();
                    //检测是否为timeline组件 如果为timeline组件则加入程序关心列表
                    bool isCodePrefabs = false;
                    PlayableDirector timelineCp = cp as PlayableDirector;
                    if (timelineCp != null)
                    {
                        isCodePrefabs = true;
                    }
                    //获取所有属性
                    while (iterator.NextVisible(true))
                    {
                        if (iterator.propertyType == SerializedPropertyType.ObjectReference)
                        {
                            //引用对象是null 并且 引用ID不是0 说明丢失了引用
                            if (iterator.objectReferenceValue == null && iterator.objectReferenceInstanceIDValue != 0)
                            {
                                if (isCodePrefabs)
                                {
                                    if (!codeRefPaths.ContainsKey(cp))
                                    {
                                        codeRefPaths.Add(cp, new List<string>());
                                    }
                                    codeRefPaths[cp].Add(iterator.propertyPath);

                                    if (!codePrefabs.ContainsKey(go))
                                    {
                                        codePrefabs.Add(go, new List<Object>());
                                    }
                                    codePrefabs[go].Add(cp);
                                    codeContent.Append(path + "->\t" + cp.gameObject.name + "\t" + cp.GetType().ToString() + "\t res:" + iterator.propertyPath + "\n");
                                }
                                else
                                {
                                    if (!artRefPaths.ContainsKey(cp))
                                    {
                                        artRefPaths.Add(cp, new List<string>());
                                    }
                                    artRefPaths[cp].Add(iterator.propertyPath);

                                    if (!artPrefabs.ContainsKey(go))
                                    {
                                        artPrefabs.Add(go, new List<Object>());
                                    }
                                    artPrefabs[go].Add(cp);
                                    artContent.Append(path + "->\t" + cp.gameObject.name + "\t" + cp.GetType().ToString() + "\t res:" + iterator.propertyPath + "\n");
                                }

                            }

                        }
                    }
                }
            }
        }
        string repotUrl = Application.dataPath + "/../Report";
        EditorTool.Report(repotUrl);
        EditorTool.WriteString(Path.Combine(repotUrl, string.Format("丢失资源总结(美术).txt")), artContent.ToString());
        EditorTool.WriteString(Path.Combine(repotUrl, string.Format("丢失资源总结(程序).txt")), codeContent.ToString());
        ////加载所有prefab资源
        //var gos = paths.Where(path => path.EndsWith("prefab")).Select(path => AssetDatabase.LoadAssetAtPath<GameObject>(path));

        //foreach (var item in gos)
        //{
        //    GameObject go = item as GameObject;
        //    if (go == null)
        //    {
        //        continue;
        //    }
        //    Component[] cps = go.GetComponentsInChildren<Component>(true);
        //    foreach (var cp in cps)
        //    {
        //        if (cp == null)  //组件为空
        //        {
        //            if (!prefabs.ContainsKey(go)) prefabs.Add(go, new List<Object>());
        //            prefabs[go].Add(cp);
        //        }
        //        else   //组件不为空
        //        {
        //            SerializedObject so = new SerializedObject(cp);
        //            SerializedProperty iterator = so.GetIterator();
        //            //获取所有属性
        //            while (iterator.NextVisible(true))
        //            {
        //                if (iterator.propertyType == SerializedPropertyType.ObjectReference)
        //                {
        //                    //引用对象是null 并且 引用ID不是0 说明丢失了引用
        //                    if (iterator.objectReferenceValue == null && iterator.objectReferenceInstanceIDValue != 0)
        //                    {
        //                        if (!refPaths.ContainsKey(cp)) refPaths.Add(cp, new List<string>());
        //                        refPaths[cp].Add(iterator.propertyPath);

        //                        if (!prefabs.ContainsKey(go)) prefabs.Add(go, new List<Object>());
        //                        prefabs[go].Add(cp);
        //                    }

        //                }
        //            }
        //        }
        //    }

        //}
    }
    AnimBool m_ShowCodeRes;
    AnimBool m_ShowArtRes;
    private void OnEnable()
    {
        m_ShowCodeRes = new AnimBool(true);//创建一个AnimBool对象，true是默认显示。
        m_ShowCodeRes.valueChanged.AddListener(Repaint);//监听重绘
        m_ShowArtRes = new AnimBool(true);//创建一个AnimBool对象，true是默认显示。
        m_ShowArtRes.valueChanged.AddListener(Repaint);//监听重绘
    }

    //以下只是将查找结果显示
    private Vector3 scroll = Vector3.zero;
    private void OnGUI()
    {
        m_ShowCodeRes.target = EditorGUILayout.ToggleLeft("显示程序关心的资源", m_ShowCodeRes.target);
        m_ShowArtRes.target = EditorGUILayout.ToggleLeft("显示美术关心的资源", m_ShowArtRes.target);
        scroll = EditorGUILayout.BeginScrollView(scroll);
        EditorGUILayout.BeginVertical();
        if (EditorGUILayout.BeginFadeGroup(m_ShowCodeRes.faded))
        {
            foreach (var item in codePrefabs)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.ObjectField(item.Key, typeof(GameObject), true, GUILayout.Width(200));
                EditorGUILayout.BeginVertical();
                foreach (var cp in item.Value)
                {
                    EditorGUILayout.BeginHorizontal();
                    if (cp)
                    {
                        EditorGUILayout.ObjectField(cp, cp.GetType(), true, GUILayout.Width(200));
                        if (artRefPaths.ContainsKey(cp))
                        {
                            string missingPath = null;
                            foreach (var path in artRefPaths[cp])
                            {
                                missingPath += path + "|";
                            }
                            if (missingPath != null)
                                missingPath = missingPath.Substring(0, missingPath.Length - 1);
                            GUILayout.Label(missingPath);
                        }
                    }
                    else
                    {
                        GUILayout.Label("丢失脚本！");
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
            }
        }
        EditorGUILayout.EndFadeGroup();
        if (EditorGUILayout.BeginFadeGroup(m_ShowArtRes.faded))
        {
            foreach (var item in artPrefabs)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.ObjectField(item.Key, typeof(GameObject), true, GUILayout.Width(200));
                EditorGUILayout.BeginVertical();
                foreach (var cp in item.Value)
                {
                    EditorGUILayout.BeginHorizontal();
                    if (cp)
                    {
                        EditorGUILayout.ObjectField(cp, cp.GetType(), true, GUILayout.Width(200));
                        if (artRefPaths.ContainsKey(cp))
                        {
                            string missingPath = null;
                            foreach (var path in artRefPaths[cp])
                            {
                                missingPath += path + "|";
                            }
                            if (missingPath != null)
                                missingPath = missingPath.Substring(0, missingPath.Length - 1);
                            GUILayout.Label(missingPath);
                        }
                    }
                    else
                    {
                        GUILayout.Label("丢失脚本！");
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
            }
        }
        EditorGUILayout.EndFadeGroup();
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndScrollView();
    }
}

