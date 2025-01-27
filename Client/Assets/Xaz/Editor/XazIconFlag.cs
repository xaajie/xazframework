//------------------------------------------------------------
// Xaz Framework
// Feedback: qq515688254
// 将所有Tag为UIProperty的GameObject标记，在Hierarchy视图上，将所有标记的GameObject右边绘制一个小图标作为标记。
// xiejie
//------------------------------------------------------------
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Xaz;
using XazEditor;
[InitializeOnLoad]
class XazIconFlag
{
    static Texture2D texture;
    static List<int> markedObjects = new List<int>();
    static int canvasID = -1;
    private static int frameCounter = 0;
    private static int maxFrame = 180;
    static XazIconFlag()
    {
        texture = AssetDatabase.LoadAssetAtPath(XazConfig.XazPath + "/Editor/Res/Tex/radioButton.png", typeof(Texture2D)) as Texture2D;
        EditorApplication.update += UpdateCB;
        //它是在Hierarchy视图中绘制每个游戏对象时调用的回调函数
        EditorApplication.hierarchyWindowItemOnGUI += HierarchyItemCB;
        EditorApplication.hierarchyChanged += OnHierarchyChanged;
        EditorApplication.hierarchyChanged += OnHierarchyWindowChanged;
    }
    /// <summary>
    /// 拖入的canvas节点下的图片转成ugui的Image,
    /// SpriteRenderer——>Image
    /// </summary>
    static void OnHierarchyWindowChanged()
    {
        GameObject go = Selection.activeGameObject;
        if (go != null)
        {
            SpriteRenderer renderer = go.GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                Canvas canvas = go.GetComponentInParent<Canvas>();
                if (canvas != null && canvas.gameObject != go)
                {
                    go.AddComponent<RectTransform>();
                    go.layer = canvas.gameObject.layer;
                    UnityEngine.UI.Image image = go.AddComponent<UnityEngine.UI.Image>();
                    Sprite sprite = renderer.sprite;
                    image.sprite = sprite;
                    image.rectTransform.localPosition = Vector3.zero;
                    image.rectTransform.localScale = Vector3.one;
                    if (sprite.border == Vector4.zero)
                    {
                        image.type = UnityEngine.UI.Image.Type.Simple;
                        image.SetNativeSize();
                    }
                    else
                    {
                        image.type = UnityEngine.UI.Image.Type.Sliced;
                        image.fillCenter = true;
                        image.rectTransform.sizeDelta = sprite.rect.max - sprite.rect.min;
                    }
                    UnityEngine.Object.DestroyImmediate(renderer);
                }
            }

            //可视组件特殊处理,仅界面的第一次拖入处理
            PrefabPathLoad tar = Selection.activeGameObject.GetComponent<PrefabPathLoad>();
            if (tar == null || (tar != null && tar.GetComponentInParent<Canvas>() != null))
            {
                PrefabPathLoad[] alls = go.GetComponentsInChildren<PrefabPathLoad>();
                if (alls.Length > 0)
                {
                    Canvas canvas = go.GetComponentInParent<Canvas>();
                    if (canvas != null && canvas.name == XazConfig.viewRootNode)
                    {
                        for (int i = 0; i < alls.Length; i++)
                        {
                            if (alls[i].transform.childCount == 0)
                            {
                                alls[i].SetPrefabPath();
                            }
                        }
                    }
                }
            }
        }
    }

    static void OnHierarchyChanged()
    {
        frameCounter = maxFrame;
    }

    static void UpdateCB()
    {
        if (EditorApplication.isPlaying)
        {
            return;
        }
        frameCounter++;
        if (frameCounter >= maxFrame)
        {
            //Debug.Log("tttttttttttttttttt");
            GameObject[] go = GameObject.FindGameObjectsWithTag(XazConfig.UIPropertyTagName);
            markedObjects = new List<int>();
            foreach (GameObject g in go)
            {
                //if (!string.IsNullOrEmpty(g.tag) && g.tag == "UIProperty")
                markedObjects.Add(g.GetInstanceID());
            }
            if (canvasID <= 0)
            {
                GameObject canvas = GameObject.Find(XazConfig.viewRootNode);
                if (canvas != null)
                {
                    canvasID = canvas.GetInstanceID();
                }
            }
            frameCounter = 0;
        }
    }

    static void HierarchyItemCB(int instanceID, Rect selectionRect)
    {
        if (EditorApplication.isPlaying)
        {
            return;
        }
        Rect r = new Rect(selectionRect);
        r.x = r.x + r.width - 16;
        r.width = 16;
        if (canvasID == instanceID)
        {
            if (!EditorApplication.isPlaying && GUI.Button(r, "E"))
            {
                Object canvasObj = EditorUtility.InstanceIDToObject(instanceID);
                if (canvasObj is GameObject)
                {
                    Transform t = (canvasObj as GameObject).transform;
                    if (t.childCount == 0)
                        return;
                }
                XazMenu.ExportView();
            }
        }
        else if (markedObjects.Contains(instanceID))
        {
            GUI.Label(r, texture);
        }
    }
}