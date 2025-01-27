//------------------------------------------------------------
// Xaz Framework
// Feedback: qq515688254
//------------------------------------------------------------
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;
using Xaz;

[CustomEditor(typeof(UIButton), true)]
public class UIButtonInspector : ButtonEditor
{

    private static string path = "Assets/AssetsPackage/UI/Atlas/Common/Common_button_01.png";
    private static UIButton CreateButton()
    {
        UIButton uibtn = null;
        if (Selection.activeTransform)
        {
            if (Selection.activeTransform.GetComponentInParent<Canvas>())
            {
                GameObject go = new GameObject("UIButton", typeof(UIButton));
                go.transform.SetParent(Selection.activeTransform);
                //go.AddComponent<CanvasRenderer>();
                uibtn = go.GetComponent<UIButton>();
                uibtn.enabled = true;
                uibtn.transform.localScale = Vector3.one;
                uibtn.transform.localPosition = Vector3.zero;
                ColorBlock cb = uibtn.colors;
                cb.pressedColor = Color.white;
                uibtn.colors = cb;
                GameObject go2 = new GameObject("image", typeof(Image));
                Image img = go2.GetComponent<Image>();
                img.sprite = (Sprite)XazEditorTools.LoadAssetAtPath(path, typeof(Sprite));
                img.SetNativeSize();
                go2.transform.SetParent(go.transform, false);
                GameObject txt2 = new GameObject("text", typeof(Text));
                Text txt = txt2.GetComponent<Text>();
                txt.raycastTarget = false;
                //根据项目设置默认字体
                txt.font = (Font)XazEditorTools.LoadAssetAtPath(XazConfig.DEFAULT_Font, typeof(Font));
                txt.fontSize = 40;
                txt.color = new Color(255, 255, 236);
                txt.text = "按钮";
                txt.rectTransform.sizeDelta = new Vector2(120, 60);
                txt.alignment = TextAnchor.MiddleCenter;
                txt.transform.SetParent(go.transform, false);
                uibtn.targetGraphic = img;
                Selection.activeObject = go;
                EditorGUIUtility.PingObject(go);
            }
        }
        return uibtn;
    }

    [MenuItem("GameObject/UI/UIButton")]
    static void CreatUIVButton()
    {
        CreateButton();
    }

    [MenuItem("GameObject/UI/UIButton+UIRect")]
    static void CreatUIRectButton()
    {
        UIButton ubtn = CreateButton();
        ubtn.gameObject.AddComponent<UIRect>();
        ubtn.targetGraphic.raycastTarget = false;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GUILayout.Space(3f);
        serializedObject.Update();
        UIButton tabButton = target as UIButton;
        XazEditorTools.DrawProperty("长按响应间隔", serializedObject, "longPressInterval");
        tabButton.isScaleAnimation = EditorGUILayout.Toggle("是否开启点击放缩", tabButton.isScaleAnimation);
        XazEditorTools.DrawProperty("scaleDuration", serializedObject, "scaleDuration");
        XazEditorTools.DrawProperty("scaleOffset", serializedObject, "scaleOffset");
        tabButton.isPressHighlight = EditorGUILayout.Toggle("是否按下时高亮", tabButton.isPressHighlight);
        tabButton.isDefaultCachelightImg = EditorGUILayout.Toggle("是否缓存高亮图", tabButton.isDefaultCachelightImg);
        GUILayout.Space(3f);
        serializedObject.ApplyModifiedProperties();
    }
}
