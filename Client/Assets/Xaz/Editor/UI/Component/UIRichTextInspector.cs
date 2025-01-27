//------------------------------------------------------------
// Xaz Framework
// Feedback: qq515688254
//------------------------------------------------------------
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using XazEditor;
using Style = Xaz.UIRichText.Style;
using Type = Xaz.UIRichText.Type;


namespace Xaz
{
    [CustomEditor(typeof(UIRichText), true)]
    public class UIRichTextInspector : ComponentEditor
    {


        [MenuItem("GameObject/UI/UIRichText")]
        static void CreatToUIRichText()
        {
            RectTransform richTxt = XazHelper.AddChild<RectTransform>(Selection.activeTransform.gameObject);
            richTxt.tag = XazConfig.UIPropertyTagName;
            richTxt.transform.tag = UIViewExporter.UIPropertyTagName;
            richTxt.transform.SetParent(Selection.activeTransform);
            richTxt.gameObject.AddComponent<UIRichText>();
            //Selection.activeGameObject = XazHelper.AddChild<UIRichText>(Selection.activeTransform.gameObject).gameObject;
        }

        private UIRichText mRichText = null;
        private bool isPaddingFoldout = false;

        public void OnEnable()
        {
            if (target)
            {
                mRichText = target as UIRichText;
            }
        }


        public override void OnInspectorGUI()
        {
            string text = string.IsNullOrEmpty(mRichText.text) ? "" : mRichText.text;
            text = EditorGUILayout.TextArea(text, GUI.skin.textArea, GUILayout.Height(100f));
            if (!text.Equals(mRichText.text))
            {
                mRichText.text = text;
            }

            isPaddingFoldout = EditorGUILayout.Foldout(isPaddingFoldout, "Padding");
            if (isPaddingFoldout)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(8);
                GUILayout.BeginVertical();
                int left = EditorGUILayout.IntField("Left", mRichText.padding.left);
                int right = EditorGUILayout.IntField("Right", mRichText.padding.right);
                int top = EditorGUILayout.IntField("Top", mRichText.padding.top);
                int bottom = EditorGUILayout.IntField("Bottom", mRichText.padding.bottom);
                mRichText.padding = new RectOffset(left, right, top, bottom);
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
            }
            mRichText.maxWidth = EditorGUILayout.IntField("MaxWidth", mRichText.maxWidth);

            mRichText.customRenderQueue = EditorGUILayout.IntField("CustomRenderQueue", mRichText.customRenderQueue);
            mRichText.verticalSpacing = EditorGUILayout.FloatField("VerticalSpacing", mRichText.verticalSpacing);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Align");
            isAlignLeft = GUILayout.Toggle(isAlignLeft, "◄", "ButtonLeft");
            isAlignCenter = GUILayout.Toggle(isAlignCenter, "▬", "ButtonMid");
            isAlignRight = GUILayout.Toggle(isAlignRight, "►", "ButtonRight");
            isAlignUp = GUILayout.Toggle(isAlignUp, "▲", "ButtonLeft");
            isAlignMiddle = GUILayout.Toggle(isAlignMiddle, "▌", "ButtonMid");
            isAlignLower = GUILayout.Toggle(isAlignLower, "▼", "ButtonRight");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Pivot");
            isPivotLeft = GUILayout.Toggle(isPivotLeft, "◄", "ButtonLeft");
            isPivotCenter = GUILayout.Toggle(isPivotCenter, "▬", "ButtonMid");
            isPivotRight = GUILayout.Toggle(isPivotRight, "►", "ButtonRight");
            isPivotUp = GUILayout.Toggle(isPivotUp, "▲", "ButtonLeft");
            isPivotMiddle = GUILayout.Toggle(isPivotMiddle, "▌", "ButtonMid");
            isPivotLower = GUILayout.Toggle(isPivotLower, "▼", "ButtonRight");
            GUILayout.EndHorizontal();

            GUILayout.Space(5);

            EditorGUIUtility.labelWidth = EditorGUIUtility.fieldWidth = 100;

            GUI.enabled = false;
            EditorGUILayout.ObjectField("cache", mRichText.cacheRectTransform, typeof(RectTransform), true);
            EditorGUILayout.ObjectField("content", mRichText.contentRectTransform, typeof(RectTransform), true);
            GUI.enabled = true;

            foreach (Style style in mRichText.styles)
            {
                GUILayout.BeginHorizontal();
                if (style.styleName != "default")
                {
                    style.styleName = EditorGUILayout.TextField("style", style.styleName);
                    style.uiObject = EditorGUILayout.ObjectField(style.uiObject, typeof(RectTransform), true) as RectTransform;
                    if (style.uiObject)
                    {
                        style.type = style.uiObject.GetComponent<Text>() ? Type.Text : Type.GameObject;
                    }

                    if (GUILayout.Button("remove"))
                    {
                        mRichText.styles.Remove(style);
                        EditorGUIUtility.ExitGUI();
                    }

                }
                else
                {
                    style.uiObject = (RectTransform)EditorGUILayout.ObjectField("style", style.uiObject, typeof(RectTransform));
                    if (GUILayout.Button("add"))
                    {
                        mRichText.styles.Add(new Style() { styleName = "s" + mRichText.styles.Count.ToString() });
                        EditorGUIUtility.ExitGUI();
                    }
                }

                GUILayout.EndHorizontal();
            }
            enableLayoutProperties = EditorGUILayout.Toggle("EnableLayoutProperties", enableLayoutProperties);
        }

        bool enableLayoutProperties
        {
            get { return mRichText.gameObject.GetComponent<LayoutElement>() != null; }
            set
            {
                LayoutElement layoutElement = mRichText.gameObject.GetComponent<LayoutElement>();
                if (value)
                {
                    if (!layoutElement)
                    {
                        mRichText.gameObject.AddComponent<LayoutElement>();
                    }
                }
                else
                {
                    if (layoutElement)
                    {
                        DestroyImmediate(layoutElement);
                    }
                }
            }
        }

        bool isPivotLeft
        {
            get { return mRichText.pivot == TextAnchor.LowerLeft || mRichText.pivot == TextAnchor.MiddleLeft || mRichText.pivot == TextAnchor.UpperLeft; }
            set
            {
                if (value)
                {
                    if (isPivotUp) mRichText.pivot = TextAnchor.UpperLeft;
                    if (isPivotLower) mRichText.pivot = TextAnchor.LowerLeft;
                    if (isPivotMiddle) mRichText.pivot = TextAnchor.MiddleLeft;
                }
            }
        }
        bool isPivotCenter
        {
            get { return mRichText.pivot == TextAnchor.LowerCenter || mRichText.pivot == TextAnchor.MiddleCenter || mRichText.pivot == TextAnchor.UpperCenter; }
            set
            {
                if (value)
                {
                    if (isPivotUp) mRichText.pivot = TextAnchor.UpperCenter;
                    if (isPivotLower) mRichText.pivot = TextAnchor.LowerCenter;
                    if (isPivotMiddle) mRichText.pivot = TextAnchor.MiddleCenter;
                }
            }
        }

        bool isPivotRight
        {
            get { return mRichText.pivot == TextAnchor.LowerRight || mRichText.pivot == TextAnchor.MiddleRight || mRichText.pivot == TextAnchor.UpperRight; }
            set
            {
                if (value)
                {
                    if (isPivotUp) mRichText.pivot = TextAnchor.UpperRight;
                    if (isPivotLower) mRichText.pivot = TextAnchor.LowerRight;
                    if (isPivotMiddle) mRichText.pivot = TextAnchor.MiddleRight;
                }
            }
        }

        bool isPivotMiddle
        {
            get { return mRichText.pivot == TextAnchor.MiddleLeft || mRichText.pivot == TextAnchor.MiddleCenter || mRichText.pivot == TextAnchor.MiddleRight; }
            set
            {
                if (value)
                {
                    if (isPivotLeft) mRichText.pivot = TextAnchor.MiddleLeft;
                    if (isPivotRight) mRichText.pivot = TextAnchor.MiddleRight;
                    if (isPivotCenter) mRichText.pivot = TextAnchor.MiddleCenter;
                }
            }
        }

        bool isPivotUp
        {
            get { return mRichText.pivot == TextAnchor.UpperCenter || mRichText.pivot == TextAnchor.UpperLeft || mRichText.pivot == TextAnchor.UpperRight; }
            set
            {
                if (value)
                {
                    if (isPivotLeft) mRichText.pivot = TextAnchor.UpperLeft;
                    if (isPivotRight) mRichText.pivot = TextAnchor.UpperRight;
                    if (isPivotCenter) mRichText.pivot = TextAnchor.UpperCenter;
                }
            }
        }
        bool isPivotLower
        {
            get { return mRichText.pivot == TextAnchor.LowerCenter || mRichText.pivot == TextAnchor.LowerLeft || mRichText.pivot == TextAnchor.LowerRight; }
            set
            {
                if (value)
                {
                    if (isPivotLeft) mRichText.pivot = TextAnchor.LowerLeft;
                    if (isPivotRight) mRichText.pivot = TextAnchor.LowerRight;
                    if (isPivotCenter) mRichText.pivot = TextAnchor.LowerCenter;
                }
            }
        }
        bool isAlignLeft
        {
            get { return mRichText.align == TextAnchor.LowerLeft || mRichText.align == TextAnchor.MiddleLeft || mRichText.align == TextAnchor.UpperLeft; }
            set
            {
                if (value)
                {
                    if (isAlignUp) mRichText.align = TextAnchor.UpperLeft;
                    if (isAlignLower) mRichText.align = TextAnchor.LowerLeft;
                    if (isAlignMiddle) mRichText.align = TextAnchor.MiddleLeft;
                }
            }
        }
        bool isAlignCenter
        {
            get { return mRichText.align == TextAnchor.LowerCenter || mRichText.align == TextAnchor.MiddleCenter || mRichText.align == TextAnchor.UpperCenter; }
            set
            {
                if (value)
                {
                    if (isAlignUp) mRichText.align = TextAnchor.UpperCenter;
                    if (isAlignLower) mRichText.align = TextAnchor.LowerCenter;
                    if (isAlignMiddle) mRichText.align = TextAnchor.MiddleCenter;
                }
            }
        }

        bool isAlignRight
        {
            get { return mRichText.align == TextAnchor.LowerRight || mRichText.align == TextAnchor.MiddleRight || mRichText.align == TextAnchor.UpperRight; }
            set
            {
                if (value)
                {
                    if (isAlignUp) mRichText.align = TextAnchor.UpperRight;
                    if (isAlignLower) mRichText.align = TextAnchor.LowerRight;
                    if (isAlignMiddle) mRichText.align = TextAnchor.MiddleRight;
                }
            }
        }

        bool isAlignMiddle
        {
            get { return mRichText.align == TextAnchor.MiddleLeft || mRichText.align == TextAnchor.MiddleCenter || mRichText.align == TextAnchor.MiddleRight; }
            set
            {
                if (value)
                {
                    if (isAlignLeft) mRichText.align = TextAnchor.MiddleLeft;
                    if (isAlignRight) mRichText.align = TextAnchor.MiddleRight;
                    if (isAlignCenter) mRichText.align = TextAnchor.MiddleCenter;
                }
            }
        }

        bool isAlignUp
        {
            get { return mRichText.align == TextAnchor.UpperCenter || mRichText.align == TextAnchor.UpperLeft || mRichText.align == TextAnchor.UpperRight; }
            set
            {
                if (value)
                {
                    if (isAlignLeft) mRichText.align = TextAnchor.UpperLeft;
                    if (isAlignRight) mRichText.align = TextAnchor.UpperRight;
                    if (isAlignCenter) mRichText.align = TextAnchor.UpperCenter;
                }
            }
        }
        bool isAlignLower
        {
            get { return mRichText.align == TextAnchor.LowerCenter || mRichText.align == TextAnchor.LowerLeft || mRichText.align == TextAnchor.LowerRight; }
            set
            {
                if (value)
                {
                    if (isAlignLeft) mRichText.align = TextAnchor.LowerLeft;
                    if (isAlignRight) mRichText.align = TextAnchor.LowerRight;
                    if (isAlignCenter) mRichText.align = TextAnchor.LowerCenter;
                }
            }
        }

    }
}