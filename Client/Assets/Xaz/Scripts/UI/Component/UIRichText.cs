//------------------------------------------------------------
// Xaz Framework
// 富文本组件
// Feedback: qq515688254
//------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace Xaz
{
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    public class UIRichText : MonoBehaviour, IControl
    {
        private static string PREFAB_PATH = "RichText/{0}";
        public const string SPEC_CATEGORY = "dl";
        public List<Style> styles = new List<Style>();
        public RectTransform cacheRectTransform;
        public RectTransform contentRectTransform;
        private Dictionary<string, Style> dicStyles = null;
        private Text m_defaultText = null;
        private List<LineValue> lineValues = new List<LineValue>();
        private Vector2 lineStart = Vector3.zero;
        const string BR = "/br";
        const string DEFAULT = "default";
        [SerializeField] private string _text = string.Empty;

        public string text
        {
            get { return _text; }
            set
            {
                if (_text != value)
                {
                    _text = value;
                    Refresh();
                }
            }
        }

        [SerializeField] private int _maxWidth = 0;
        public int maxWidth
        {
            get { return _maxWidth; }
            set
            {
                if (_maxWidth != value)
                {
                    _maxWidth = Mathf.Max(0, value);
                    Refresh();
                }
            }
        }

        // 自定义渲染队列，-1表示使用默认的，不修改
        [SerializeField] private int _customRenderQueue = -1;
        public int customRenderQueue
        {
            get { return _customRenderQueue; }
            set
            {
                if (_customRenderQueue != value)
                {
                    _customRenderQueue = Mathf.Max(-1, value);
                    Refresh();
                }
            }
        }


        private RectTransform _rectTransform;
        public RectTransform rectTransform
        {
            get
            {
                if (_rectTransform == null)
                    _rectTransform = this.transform as RectTransform;
                return _rectTransform;
            }
        }

        [SerializeField] private TextAnchor _pivot = TextAnchor.UpperLeft;
        public TextAnchor pivot
        {
            get { return _pivot; }
            set
            {
                if (_pivot != value)
                {
                    _pivot = value;
                    Refresh();
                }
            }
        }

        [SerializeField] private TextAnchor _align = TextAnchor.UpperLeft;
        public TextAnchor align
        {
            get { return _align; }
            set
            {
                if (_align != value)
                {
                    _align = value;
                    Refresh();
                }
            }
        }

        [SerializeField] private float _verticalSpacing = 0f;
        public float verticalSpacing
        {
            get { return _verticalSpacing; }
            set
            {
                if (_verticalSpacing != value)
                {
                    _verticalSpacing = value;
                    Refresh();
                }
            }
        }


        [SerializeField] private RectOffset _padding = new RectOffset();
        public RectOffset padding
        {
            get { return _padding; }
            set
            {
                if (_padding.left != value.left || _padding.right != value.right || _padding.top != value.top || _padding.bottom != value.bottom)
                {
                    _padding = value;
                    Refresh();
                }
            }
        }

        /// <summary>
        /// 外部自定义材质，如果指定了这个材质则每次创建的富文本text都是这个材质
        /// </summary>
        [System.NonSerialized]
        private Material _customMaterial = null;
        public Material CustomMaterial
        {
            get { return _customMaterial; }
            set
            {
                if (_customMaterial == value)
                    return;
                _customMaterial = value;
            }
        }

        //监听文字模式点击事件
        private Dictionary<string, System.Action<string, Transform>> dicStyleClick = null;
        public void AddClickListener(string styleName, System.Action<string, Transform> click)
        {
            if (dicStyleClick == null)
            {
                dicStyleClick = new Dictionary<string, System.Action<string, Transform>>();
            }
            dicStyleClick[styleName] = click;
        }

        //是否开始方法模式
        private bool m_IsFunctionMode = false;
        //记录方法模式sytle and param
        private List<FunctionValue> m_FunctionList = null;
        private List<FunctionValue> m_LastFunctionList = null;
        //是否开始方法模式（只能以参数的方式对RichText赋值）
        public void BeginFunctionMode()
        {
            m_FunctionList = new List<FunctionValue>();
            m_IsFunctionMode = true;
        }

        private Dictionary<string, Style> funcationInitStyles = null;

        public void AddFunctionStyle(string param)
        {
            AddFunctionStyle(DEFAULT, param, null);
        }

        public void AddFunctionStyle(string styleName, string param)
        {
            AddFunctionStyle(styleName, param, null);
        }
        //增加方法模式 syle 和参数 
        public void AddFunctionStyle(string styleName, string param, System.Action<string, Transform> click)
        {
            if (styleName != DEFAULT)
            {
                if (funcationInitStyles == null)
                    funcationInitStyles = new Dictionary<string, Style>();
                //RichText没有找到标签， 在Resources下找公共标签 jietodo xiejie
                RectTransform temp = Resources.Load<RectTransform>(string.Format(PREFAB_PATH, styleName));
                if (temp)
                {
                    Style s = new Style();
                    Text t = temp.GetComponent<Text>();
                    if (t)
                    {
                        s.content = t.text;
                        s.type = Type.Text;
                        if (_customRenderQueue != -1 && (t.material == null || t.material.renderQueue != _customRenderQueue))
                        {
                            t.material = Instantiate(t.defaultMaterial);
                            t.material.renderQueue = _customRenderQueue;
                        }
                    }
                    else
                    {
                        //addby xiejie 2020-3-19
                        UIImage img = temp.GetComponent<UIImage>();
                        if (img)
                        {
                            s.type = Type.AtalsImg;
                        }
                        else
                        {
                            s.type = Type.GameObject;
                        }
                    }
                    s.styleName = styleName;
                    s.isCommonStyle = true;
                    s.uiObject = temp;
                    funcationInitStyles[styleName] = s;
                }
            }
            m_FunctionList.Add(new FunctionValue() { styleName = styleName, styleValue = param, styleClick = click });
        }

        //结束方法模式，开始刷新UI
        public void EndFunctionMode()
        {
            bool checkRefresh = true; //如果两次内容完全一致，不在刷新
            if (m_LastFunctionList != null && m_FunctionList != null && m_LastFunctionList.Count == m_FunctionList.Count)
            {
                checkRefresh = false;
                for (int i = 0; i < m_LastFunctionList.Count; i++)
                {
                    FunctionValue a = m_LastFunctionList[i];
                    FunctionValue b = m_FunctionList[i];
                    if (a.styleName != b.styleName || a.styleValue != b.styleValue || a.styleClick != b.styleClick)
                    {
                        checkRefresh = true;
                        break;
                    }
                }
            }
            if (checkRefresh)
            {
                Refresh();
            }
            m_LastFunctionList = m_FunctionList;
            m_IsFunctionMode = false;
        }



        void Refresh()
        {
            //ProfilerUtil.BeginSample ("RichText Refresh");
            this.InitStyle();
            this.InitHierarchy();

            lineStart = Vector3.zero;
            if (!string.IsNullOrEmpty(this.text) || m_IsFunctionMode)
            {
                lineValues.Clear();
                if (!m_IsFunctionMode)
                {
                    //字符串模式
                    string[] textLens = this.text.Split('\n');
                    for (int i = 0; i < textLens.Length; i++)
                    {
                        GetValueLineByText(textLens[i], ref lineValues);
                    }
                }
                else if (m_IsFunctionMode && m_FunctionList != null)
                {
                    //方法模式
                    GetValueLineByFunction(ref lineValues);
                }
                int count = lineValues.Count;
                for (int i = 0; i < count; i++)
                {
                    LineValue l = lineValues[i];
                    RectTransform horizon = new GameObject("horizon").AddComponent<RectTransform>();
                    horizon.anchoredPosition = new Vector2(0f, -lineStart.y);
                    horizon.SetParent(contentRectTransform, false);
                    horizon.anchorMin = horizon.anchorMax = horizon.pivot = AnchorToLine(pivot, align);
                    //优化文本
                    Text lastText = null;
                    float lastTextStart = 0f;
                    float lastRectWidth = 0f;
                    System.Action<string, Transform> lastClick = null;
                    string lastStyleName = string.Empty;

                    //记录区域
                    float lineHeight = 0f;
                    float lineWidth = 0f;
                    RectTransform childRectTransform = null;
                    foreach (Value v in l.values)
                    {
                        Style style;
                        childRectTransform = null;
                        if (dicStyles.TryGetValue(v.styleName, out style))
                        {
                            if (style.type == Type.Text)
                            {
                                Text uiText = lastText;
                                if (v.styleName != lastStyleName || (lastClick != null && lastClick != v.styleClick))
                                {
                                    lastTextStart += lastRectWidth;
                                    uiText = style.Get(horizon).GetComponent<Text>(); // GameObjectUtil.AddChild<Text>(horizon,style.uiObject.gameObject);
                                    uiText.text = string.Empty;
                                    uiText.rectTransform.anchoredPosition = new Vector2(lastTextStart, 0);
                                    uiText.horizontalOverflow = HorizontalWrapMode.Overflow;
                                    uiText.verticalOverflow = VerticalWrapMode.Overflow;
                                    if (_customMaterial) uiText.material = _customMaterial;
                                    lastText = uiText;
                                    lastClick = v.styleClick;
                                }
                                childRectTransform = uiText.rectTransform;
                                uiText.text += v.param!=null?v.param.ToString():string.Empty;
                                Vector2 rect = GetTextRect(uiText);
                                childRectTransform.sizeDelta = rect;
                                lastRectWidth = rect.x;
                                lineWidth = lastTextStart + rect.x;
                                lineHeight = Mathf.Max(lineHeight, rect.y);
                                ListenerClick(v, uiText.gameObject);
                            }
                            else if (style.type == Type.GameObject)
                            {
                                GameObject uiObject = style.Get(horizon);
                                childRectTransform = uiObject.GetComponent<RectTransform>();
                                childRectTransform.anchoredPosition = new Vector2(lineWidth, 0);
                                lastRectWidth += childRectTransform.sizeDelta.x;
                                lineWidth += childRectTransform.sizeDelta.x;
                                lineHeight = Mathf.Max(lineHeight, childRectTransform.sizeDelta.y);
                                ListenerClick(v, uiObject);
                            }
                            else if (style.type == Type.BR)
                            {
                                if (this.m_defaultText)
                                {
                                    lineHeight = this.m_defaultText.fontSize;
                                }
                            }
                            //addby xiejie 2020-3-19
                            else if (style.type == Type.AtalsImg)
                            {
                                UIImage uiObject = style.Get(horizon).GetComponent<UIImage>();
                                childRectTransform = uiObject.GetComponent<RectTransform>();
                                childRectTransform.anchoredPosition = new Vector2(lineWidth, 0);
                                childRectTransform.sizeDelta = new Vector2(this.m_defaultText.fontSize + 7, this.m_defaultText.fontSize + 7);
                                lastRectWidth += childRectTransform.sizeDelta.x;
                                lineWidth += childRectTransform.sizeDelta.x;
                                lineHeight = Mathf.Max(lineHeight, childRectTransform.sizeDelta.y);
                                if (v.styleName == SPEC_CATEGORY){
                                    UserCategoryData vt = ModuleMgr.CategoryMgr.CreateFrom(v.param);
                                    uiObject.SetSprite(vt.GetAtlas(), vt.GetIcon(), null);
                                }
                                else
                                {
                                    uiObject.SetSprite(v.styleName, v.param.ToString(), null);
                                }
                                ListenerClick(v, uiObject.gameObject);
                            }
                            lastStyleName = v.styleName;
                        }

                        if (childRectTransform)
                        {
                            childRectTransform.pivot = childRectTransform.anchorMin = childRectTransform.anchorMax = AnchorToChild(pivot);
                        }
                    }
                    lineStart.x = Mathf.Max(lineStart.x, lineWidth);
                    lineStart.y += lineHeight;
                    if (verticalSpacing != 0 && i != count - 1)
                    {
                        lineStart.y += verticalSpacing;
                    }

                    horizon.sizeDelta = new Vector2(lineWidth, lineHeight);

#if UNITY_EDITOR
                    if (!Application.isPlaying)
                    {
                        foreach (Transform t in horizon.GetComponentsInChildren<Transform>())
                            t.gameObject.hideFlags = HideFlags.NotEditable;
                    }
#endif
                }
            }
            LayoutElement layoutElement = gameObject.GetComponent<LayoutElement>();
            contentRectTransform.sizeDelta = lineStart;
            if (padding.left != 0 || padding.bottom != 0 || padding.right != 0 || padding.top != 0)
            {
                if (layoutElement)
                {
                    layoutElement.preferredHeight = lineStart.y + padding.top + padding.bottom;
                    layoutElement.preferredWidth = lineStart.x + padding.left + padding.right;
                }
                rectTransform.sizeDelta = new Vector2(lineStart.x + padding.left + padding.right, lineStart.y + padding.top + padding.bottom);
                contentRectTransform.anchoredPosition = AnchorToPadding(align);
            }
            else
            {
                if (layoutElement)
                {
                    layoutElement.preferredHeight = lineStart.y;
                    layoutElement.preferredWidth = lineStart.x;
                }
                rectTransform.sizeDelta = lineStart;
                contentRectTransform.anchoredPosition = Vector2.zero;
            }
            //ProfilerUtil.EndSample ();
        }

        void InitStyle()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                dicStyles = null;
            }
#endif

            if (dicStyles == null)
            {
                dicStyles = new Dictionary<string, Style>();
                foreach (Style s in styles)
                {
                    dicStyles[s.styleName] = s;
                    if (s.styleName == DEFAULT)
                    {
                        this.m_defaultText = s.uiObject.GetComponent<Text>();
                    }
                }
                dicStyles[BR] = new Style() { type = Type.BR };
            }
            if (funcationInitStyles != null)
            {
                foreach (KeyValuePair<string, Style> style in funcationInitStyles)
                {
                    dicStyles[style.Key] = style.Value;
                }
            }
        }

        void InitHierarchy()
        {
            if (cacheRectTransform)
            {
                cacheRectTransform.gameObject.layer = LayerMask.NameToLayer(XazConfig.LayerDefine.UIINVISIBLE);
                foreach (Style s in styles)
                {
                    s.Rest(cacheRectTransform);
                }
            }

            bool isWhile = true;
            do
            {
                isWhile = false;
                foreach (Transform t in contentRectTransform)
                {
                    t.SetParent(null);
                    XazHelper.Destroy(t.gameObject);
                    isWhile = true;
                }
            } while (isWhile);

            contentRectTransform.anchorMin = contentRectTransform.anchorMax = contentRectTransform.pivot = rectTransform.pivot = AnchorToPivot(align);
        }

        void ListenerClick(Value v, GameObject ui)
        {
            if (v.styleClick != null)
            {
                MaskableGraphic g = ui.GetComponent<MaskableGraphic>();
                if (g)
                {
                    g.raycastTarget = true;
                }
                //监听点击事件
                ClickListener.Get(ui).onClick = delegate (GameObject go)
                {
                    if (go)
                    {
                        v.styleClick(v.param, ui.transform);
                    }
                };

            }
        }

        void GetValueLineByText(string text, ref List<LineValue> LineValues)
        {
            LineValues.Add(new LineValue());
            if (string.IsNullOrEmpty(text))
            {
                //空字符串视为换行
                LineValues[LineValues.Count - 1].values.Add(new Value() { styleName = BR });
            }
            else
            {

                int index = 0;
                //【符号出现的数量
                int leftMarkCount = 0;
                //】符号出现的数量
                int rightMarkCount = 0;
                //开始提取StyleName
                bool beginCollectStyleName = false;
                //结束提取StyleName
                bool endCollectStyleName = false;
                //开始提取参数
                bool beginCollectParameter = false;
                //解析是否出错
                bool isParserError = false;
                //全部
                string all = string.Empty;
                //提取样式名
                string name = string.Empty;
                //提取样式参数
                string param = string.Empty;
                //提取默认样式参数
                string defaultParam = string.Empty;

                while (index < text.Length)
                {
                    char c = text[index];
                    all += c;
                    if (c == '[')
                    {
                        leftMarkCount++;
                        if (!beginCollectStyleName)
                        {
                            //开始提取
                            beginCollectStyleName = true;
                            endCollectStyleName = false;
                            name = string.Empty;
                            param = string.Empty;

                            if (!string.IsNullOrEmpty(defaultParam))
                            {
                                TryPushStyleLineValueByParma(ref LineValues, DEFAULT, defaultParam);
                                defaultParam = string.Empty;
                            }
                        }
                        else
                        {
                            param += c;
                        }
                    }
                    else if (c == ']')
                    {
                        rightMarkCount++;
                        if (rightMarkCount == leftMarkCount)
                        {
                            //匹配结束
                            beginCollectStyleName = false;
                            endCollectStyleName = true;
                            if (beginCollectParameter)
                            {
                                beginCollectParameter = false;
                            }

                            //RichText没有找到标签， 在Resources下找公共标签
                            if (!dicStyles.ContainsKey(name))
                            {
                                //RectTransform temp = Xaz.Assets.LoadAsset<RectTransform>(string.Format("UI/RichText/{0}", name));
                                //RichText没有找到标签， 在Resources下找公共标签 jietodo xiejie
                                RectTransform temp = Resources.Load<RectTransform>(string.Format(PREFAB_PATH, name));
                                if (temp)
                                {
                                    Style s = new Style();
                                    Text t = temp.GetComponent<Text>();
                                    if (t)
                                    {
                                        s.content = t.text;
                                        s.type = Type.Text;
                                        if (_customRenderQueue != -1 && (t.material == null || t.material.renderQueue != _customRenderQueue))
                                        {
                                            t.material = Instantiate(t.defaultMaterial);
                                            t.material.renderQueue = _customRenderQueue;
                                        }
                                    }
                                    else
                                    {
                                        //addby xiejie 2020-3-19
                                        UIImage img = temp.GetComponent<UIImage>();
                                        if (img)
                                        {
                                            s.type = Type.AtalsImg;
                                        }
                                        else
                                        {
                                            s.type = Type.GameObject;
                                        }
                                    }
                                    s.styleName = name;
                                    s.uiObject = temp;
                                    s.isCommonStyle = true;
                                    dicStyles[name] = s;
                                }
                                else
                                {
                                    //在Resources下没有找公共标签标记解析错误
                                    isParserError = true;
                                    break;
                                }
                            }

                            param = param.Replace("[" + BR + "]", "\n");
                            TryPushStyleLineValueByParma(ref LineValues, name, param);

                        }
                        else
                        {
                            param += c;
                        }
                    }
                    else if (beginCollectStyleName && c == '=')
                    {
                        if (!beginCollectParameter)
                        {
                            beginCollectParameter = true;
                            param = string.Empty;
                        }
                        else
                        {
                            param += c;
                        }
                    }
                    else
                    {
                        if (beginCollectStyleName && !beginCollectParameter && !endCollectStyleName)
                        {
                            name += c;
                        }
                        else if (beginCollectParameter && !endCollectStyleName)
                        {
                            param += c;
                        }
                        else
                        {
                            defaultParam += c.ToString();
                        }
                    }
                    index++;
                }

                if (!string.IsNullOrEmpty(defaultParam))
                {
                    TryPushStyleLineValueByParma(ref LineValues, DEFAULT, defaultParam);
                    defaultParam = string.Empty;
                }

                //处理解析错误，使用默认标签
                if (isParserError || rightMarkCount != leftMarkCount)
                {
                    all = all.Replace("[" + BR + "]", "\n");
                    TryPushStyleLineValueByParma(ref LineValues, DEFAULT, all);
                }
            }
        }

        void GetValueLineByFunction(ref List<LineValue> LineValues)
        {
            LineValues.Add(new LineValue());
            foreach (FunctionValue functionValue in m_FunctionList)
            {
                TryPushStyleLineValueByParma(ref LineValues, functionValue.styleName, functionValue.styleValue, functionValue.styleClick);
            }
        }



        private void TryPushStyleLineValueByParma(ref List<LineValue> LineValues, string styleName, string param, System.Action<string, Transform> clickCb = null)
        {
            LineValue lineValueTemp = LineValues[LineValues.Count - 1];
            bool isClick = ((dicStyleClick != null && dicStyleClick.ContainsKey(styleName) || clickCb != null));
            bool isWrap = this.maxWidth > 0;
            bool isText = !string.IsNullOrEmpty(param);
            if (!isWrap && !isClick)
            {
                if (isText)
                {
                    foreach (var item in param.Split('\n'))
                    {
                        lineValueTemp.values.Add(new Value() { styleName = styleName, param = item });
                    }
                }
                else
                {
                    lineValueTemp.values.Add(new Value() { styleName = styleName, param = param });
                }
            }
            else
            {
                Style s;
                if (dicStyles.TryGetValue(styleName, out s) && s.uiObject)
                {
                    Value valueTemp = null;
                    if (s.type == Type.Text)
                    {
                        Text textTemp = s.uiObject.GetComponent<Text>();
                        string realText = GetStringNoHtml(param);
                        int realIndex = 0;
                        int startIndex;
                        int endIndex;
                        string[] tagList = GetCurrTag(param, 0, out startIndex, out endIndex);
                        for (int i = 0; i < param.Length; i++)
                        {
                            char ch = param[i];
                            char realCh = realIndex < realText.Length ? realText[realIndex] : char.MinValue;

                            if (i == endIndex + 1 && tagList[0] != null)
                            {
                                tagList = GetCurrTag(param, i, out startIndex, out endIndex);
                            }

                            valueTemp = new Value() { styleName = styleName, param = ch.ToString() };
                            if (isClick)
                            {
                                if (clickCb != null)
                                {
                                    valueTemp.styleClick = clickCb;
                                }
                                else
                                {
                                    valueTemp.styleClick = dicStyleClick[styleName];
                                }
                            }

                            if (isWrap && ch == realCh)
                            {
                                float newWidth = GetCharWidthBefore(ch, textTemp.font, textTemp.fontSize);
                                if (lineValueTemp.width + newWidth > this.maxWidth)
                                {
                                    if (i <= endIndex)
                                    {
                                        lineValueTemp.values.Add(new Value() { styleName = styleName, param = tagList[1] });
                                    }
                                    lineValueTemp = new LineValue();
                                    LineValues.Add(lineValueTemp);
                                    if (i + 1 < endIndex)
                                    {
                                        lineValueTemp.values.Add(new Value() { styleName = styleName, param = tagList[0] });
                                    }
                                }
                                realIndex++;
                                lineValueTemp.width += newWidth;
                            }
                            lineValueTemp.values.Add(valueTemp);

                        }
                    }
                    else
                    {
                        valueTemp = new Value() { styleName = styleName, param = param };
                        if (isClick)
                        {
                            if (clickCb != null)
                            {
                                valueTemp.styleClick = clickCb;
                            }
                            else
                            {
                                valueTemp.styleClick = dicStyleClick[styleName];
                            }
                        }
                        if (isWrap)
                        {
                            float newWidth = s.uiObject.sizeDelta.x;
                            if (lineValueTemp.width + newWidth > this.maxWidth)
                            {
                                lineValueTemp = new LineValue();
                                LineValues.Add(lineValueTemp);
                            }
                            lineValueTemp.width += newWidth;
                        }
                        lineValueTemp.values.Add(valueTemp);
                    }
                }
            }
        }

        private float GetCharWidthBefore(char c, Font font, int fontSize)
        {
            int characterInfoWidth = 0;
            CharacterInfo characterInfo;
            try
            {
                font.RequestCharactersInTexture(c.ToString(), fontSize, FontStyle.Normal);
                if (font.GetCharacterInfo(c, out characterInfo, fontSize))
                {
                    characterInfoWidth += characterInfo.advance;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning(e.Message);
            }
            return (float)characterInfoWidth;
        }

        private Vector2 GetTextRect(Text text)
        {
            string content = text.text;
            return new Vector2(text.preferredWidth, text.fontSize);
        }



        private Vector2 AnchorToPivot(TextAnchor anchor)
        {
            switch (anchor)
            {
                case TextAnchor.UpperLeft:
                    return new Vector2(0f, 1f);
                case TextAnchor.UpperCenter:
                    return new Vector2(0.5f, 1f);
                case TextAnchor.UpperRight:
                    return new Vector2(1f, 1f);
                case TextAnchor.MiddleLeft:
                    return new Vector2(0f, 0.5f);
                case TextAnchor.MiddleCenter:
                    return new Vector2(0.5f, 0.5f);
                case TextAnchor.MiddleRight:
                    return new Vector2(1f, 0.5f);
                case TextAnchor.LowerLeft:
                    return new Vector2(0f, 0f);
                case TextAnchor.LowerCenter:
                    return new Vector2(0.5f, 0f);
                case TextAnchor.LowerRight:
                    return new Vector2(1f, 0f);
            };
            return Vector2.one;
        }


        private Vector2 AnchorToLine(TextAnchor anchor, TextAnchor align)
        {
            return new Vector2((float)((int)anchor % 3) / 2f, 1f);
            //return new Vector2 ((float)((int)anchor % 3) / 2f, 1f - ((float)((int)align /3)/2f));
        }

        private Vector2 AnchorToChild(TextAnchor anchor)
        {
            return new Vector2(0f, 1f - ((float)((int)anchor / 3) / 2f));
        }

        private Vector2 AnchorToPadding(TextAnchor anchor)
        {
            Vector2 pos = Vector2.zero;
            int a = (int)anchor;
            if (a % 3 == 0)
            {
                pos.x = _padding.left;
            }
            else if (a % 3 == 2)
            {
                pos.x = -_padding.right;
            }

            if (a / 3 == 0)
            {
                pos.y = -_padding.top;
            }
            else if (a / 3 == 2)
            {
                pos.y = _padding.bottom;
            }
            return pos;
        }


        [System.Serializable]
        public class Style
        {
            //样式名称
            public string styleName = string.Empty;
            //游戏对象
            public RectTransform uiObject = null;
            //风格类型
            public Type type = Type.Text;
            //Text显示内容
            public string content = string.Empty;
            //是否公共标签
            public bool isCommonStyle = false;
            public GameObject Get(Transform parent)
            {
                GameObject go = null;
                go = GameObject.Instantiate<GameObject>(uiObject.gameObject, parent, false);
                return go;
            }
            public void Rest(Transform parent)
            {
                if (uiObject && uiObject.parent != parent)
                {
                    uiObject.SetParent(parent, false);
                    //uiObject.gameObject.SetActive (false);

#if UNITY_EDITOR
                    if (!Application.isPlaying)
                    {
                        if (type == Type.Text)
                        {
                            Text cachetText = uiObject.GetComponent<Text>();
                            if (cachetText)
                            {
                                cachetText.text = string.Empty;
                                cachetText.transform.position = Vector3.one;

                            }
                        }
                    }
#endif
                }

            }
        }

        [System.Serializable]
        public enum Type : byte
        {
            //默认是字体
            Text = 1,
            //是否游戏对象
            GameObject,
            //是否图集图片
            AtalsImg,
            //换行
            BR,
        }
        public class FunctionValue
        {
            public string styleName = string.Empty;
            public string styleValue = string.Empty;
            public System.Action<string,Transform> styleClick = null;
        }
        public class LineValue
        {
            public List<Value> values = new List<Value>();
            public float width = 0f;
        }

        public class Value
        {
            public string styleName = string.Empty;
            public string param = string.Empty;
            public int height;
            public int width;
            public Action<string, Transform> styleClick = null;
        }

        private class ClickListener : MonoBehaviour, IPointerClickHandler, IPointerDownHandler
        {
            public delegate void VoidDelegate(GameObject go);
            public VoidDelegate onClick;
            public VoidDelegate onDown;
            #region IPointerClickHandler implementation

            public void OnPointerClick(PointerEventData eventData)
            {
                if (onClick != null)
                {
                    onClick(gameObject);
                }
            }
            public void OnPointerDown(PointerEventData eventData)
            {
                if (onDown != null)
                {
                    onDown(gameObject);
                }
            }
            #endregion
            static public ClickListener Get(GameObject go)
            {
                ClickListener listener = go.GetComponent<ClickListener>();
                if (listener == null)
                    listener = go.AddComponent<ClickListener>();
                return listener;
            }

            void OnDestroy()
            {
                onClick = null;
                onDown = null;
            }

        }

#if UNITY_EDITOR
        void Reset()
        {
            Text defaultText = XazHelper.AddChild<Text>(transform.gameObject);
            if (_customMaterial) defaultText.material = _customMaterial;
            //jietodo
            defaultText.font = AssetDatabase.LoadAssetAtPath<Font>(XazConfig.DEFAULT_Font);
            defaultText.name = "default";
            defaultText.color = Color.white;
            defaultText.fontSize = 20;
            defaultText.raycastTarget = false;
            defaultText.rectTransform.sizeDelta = Vector2.zero;
            Style style = new Style();
            style.styleName = "default";
            style.uiObject = defaultText.rectTransform;
            style.type = Type.Text;
            styles.Add(style);
            name = "UIRichText";
            text = "";
            RestCache();
        }

        void RestCache()
        {
            if (cacheRectTransform == null)
            {
                Canvas canvas = XazHelper.AddChild<Canvas>(transform.gameObject);
                cacheRectTransform = (canvas.transform as RectTransform);
            }
            cacheRectTransform.anchoredPosition = Vector2.zero;
            cacheRectTransform.sizeDelta = Vector2.zero;
            cacheRectTransform.name = "cache";
            cacheRectTransform.gameObject.layer = LayerMask.NameToLayer(XazConfig.LayerDefine.UIINVISIBLE);
            cacheRectTransform.SetAsFirstSibling();
            foreach (var item in styles)
            {
                if (item.uiObject)
                {
                    Text text = item.uiObject.GetComponent<Text>();
                    if (text)
                    {
                        text.text = string.Empty;
                        text.rectTransform.sizeDelta = Vector2.zero;
                    }
                    item.uiObject.anchoredPosition = Vector2.zero;
                    item.uiObject.gameObject.SetActive(true);
                    item.uiObject.SetParent(cacheRectTransform, false);
                }
            }

            if (contentRectTransform == null)
            {
                contentRectTransform = XazHelper.AddChild<RectTransform>(transform.gameObject);
            }
            contentRectTransform.name = "content";
            contentRectTransform.anchoredPosition = Vector2.zero;
            contentRectTransform.sizeDelta = Vector2.zero;
        }

        public void EditorRest()
        {
            text = string.Empty;
            contentRectTransform.sizeDelta = Vector2.zero;
            RestCache();
        }
#endif

        /// <summary>
        /// 通过富文本标签，取到clone的预制列表 by xiao~
        /// </summary>
        /// <param name="_con"></param>
        public List<UnityEngine.Object> GetCloneGosByContent(string _con)
        {
            List<UnityEngine.Object> list = new List<UnityEngine.Object>();
            foreach (var item in dicStyles)
                if (item.Value.type == Type.GameObject && item.Value.styleName == _con)
                    list.Add(item.Value.uiObject);
            return list;
        }

        // checkIndex 表示开始的位置。值返回当前的标签组
        private string[] GetCurrTag(string param, int checkIndex, out int startIndex, out int endIndex)
        {
            string[] tagList = new string[2];
            startIndex = 0;
            endIndex = 0;
            string realParam = param.Substring(checkIndex);
            string realText = GetStringNoHtml(realParam);
            int realIndex = 0;
            int lastOffset = 0;
            string tagStr = "";
            for (int i = 0; i < realParam.Length; i++)
            {
                char ch = realParam[i];
                if (realIndex >= realText.Length || realParam[i] != realText[realIndex])
                {
                    startIndex = i + checkIndex;
                    tagStr = tagStr + realParam[i];
                }
                else
                {
                    if (i != (realIndex + lastOffset))
                    {
                        if (tagList[0] == null)
                        {
                            tagList[0] = tagStr;
                            tagStr = "";
                            lastOffset = i - realIndex;
                        }
                        else
                        {
                            tagList[1] = tagStr;
                            endIndex = i + checkIndex;
                            break;
                        }

                    }
                    realIndex++;
                }
            }
            return tagList;
        }

        private string GetStringNoHtml(string strHtml)
        {
            if (String.IsNullOrEmpty(strHtml))
            {
                return strHtml;
            }
            else
            {
                string[] aryReg ={
@"<script[^>]*?>.*?</script>",
@"<!--.*\n(-->)?",
@"<(\/\s*)?(.|\n)*?(\/\s*)?>",
@"<(\w|\s|""|'| |=|\\|\.|\/|#)*",
@"([\r\n|\s])*",
@"&(quot|#34);",
@"&(amp|#38);",
@"&(lt|#60);",
@"&(gt|#62);",
@"&(nbsp|#160);",
@"&(iexcl|#161);",
@"&(cent|#162);",
@"&(pound|#163);",
@"&(copy|#169);",
@"&#(\d+);"};

                string newReg = aryReg[0];
                string strOutput = strHtml.Replace("&nbsp;", " ");
                for (int i = 0; i < aryReg.Length; i++)
                {
                    Regex regex = new Regex(aryReg[i], RegexOptions.IgnoreCase);
                    strOutput = regex.Replace(strOutput, "");
                }
                strOutput.Replace("<", "&gt;");
                strOutput.Replace(">", "&lt;");
                return strOutput.Replace(" ", "&nbsp;");
            }
        }
    }
}
