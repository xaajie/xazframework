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
    [MenuItem("��Դ����/������ʧ���õ���Դ")]
    static void ShowWindow()
    {
        GetWindow<MyFindMissingWindow>("����Missing��Դ").Show();
        Find();
    }

    static Dictionary<Object, List<Object>> artPrefabs = new Dictionary<Object, List<Object>>();
    static Dictionary<Object, List<string>> artRefPaths = new Dictionary<Object, List<string>>();
    static Dictionary<Object, List<Object>> codePrefabs = new Dictionary<Object, List<Object>>();
    static Dictionary<Object, List<string>> codeRefPaths = new Dictionary<Object, List<string>>();
    //Ѱ��missing����Դ
    static void Find()
    {
        //��ȡ������Դ��·��
        string[] paths = AssetDatabase.GetAllAssetPaths();
        //ɸѡ��prefab��Դ��·���б�
        var prefabPath = paths.Where(path => path.EndsWith("prefab"));
        //���ڻ�����Ҫ��ӡ������
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
                if (cp == null)//���Ϊ��
                {
                    //��ʧ�ű������������б�
                    if (!codePrefabs.ContainsKey(go)) codePrefabs.Add(go, new List<Object>());
                    codePrefabs[go].Add(cp);
                    codeContent.Append(path + "->\t" + "��ʧ�ű�" + "\t");
                }
                else
                {
                    SerializedObject so = new SerializedObject(cp);
                    SerializedProperty iterator = so.GetIterator();
                    //����Ƿ�Ϊtimeline��� ���Ϊtimeline���������������б�
                    bool isCodePrefabs = false;
                    PlayableDirector timelineCp = cp as PlayableDirector;
                    if (timelineCp != null)
                    {
                        isCodePrefabs = true;
                    }
                    //��ȡ��������
                    while (iterator.NextVisible(true))
                    {
                        if (iterator.propertyType == SerializedPropertyType.ObjectReference)
                        {
                            //���ö�����null ���� ����ID����0 ˵����ʧ������
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
        EditorTool.WriteString(Path.Combine(repotUrl, string.Format("��ʧ��Դ�ܽ�(����).txt")), artContent.ToString());
        EditorTool.WriteString(Path.Combine(repotUrl, string.Format("��ʧ��Դ�ܽ�(����).txt")), codeContent.ToString());
        ////��������prefab��Դ
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
        //        if (cp == null)  //���Ϊ��
        //        {
        //            if (!prefabs.ContainsKey(go)) prefabs.Add(go, new List<Object>());
        //            prefabs[go].Add(cp);
        //        }
        //        else   //�����Ϊ��
        //        {
        //            SerializedObject so = new SerializedObject(cp);
        //            SerializedProperty iterator = so.GetIterator();
        //            //��ȡ��������
        //            while (iterator.NextVisible(true))
        //            {
        //                if (iterator.propertyType == SerializedPropertyType.ObjectReference)
        //                {
        //                    //���ö�����null ���� ����ID����0 ˵����ʧ������
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
        m_ShowCodeRes = new AnimBool(true);//����һ��AnimBool����true��Ĭ����ʾ��
        m_ShowCodeRes.valueChanged.AddListener(Repaint);//�����ػ�
        m_ShowArtRes = new AnimBool(true);//����һ��AnimBool����true��Ĭ����ʾ��
        m_ShowArtRes.valueChanged.AddListener(Repaint);//�����ػ�
    }

    //����ֻ�ǽ����ҽ����ʾ
    private Vector3 scroll = Vector3.zero;
    private void OnGUI()
    {
        m_ShowCodeRes.target = EditorGUILayout.ToggleLeft("��ʾ������ĵ���Դ", m_ShowCodeRes.target);
        m_ShowArtRes.target = EditorGUILayout.ToggleLeft("��ʾ�������ĵ���Դ", m_ShowArtRes.target);
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
                        GUILayout.Label("��ʧ�ű���");
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
                        GUILayout.Label("��ʧ�ű���");
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

