//------------------------------------------------------------
// Xaz Framework
// Feedback: qq515688254
//------------------------------------------------------------
﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Xaz
{
    [AddComponentMenu("UI/UIState")]
    public class UIState : MonoBehaviour, IControl
    {
        public List<State> states = new List<State>();
        public int index = 0;

        private State state
        {
            get {
                return (states.Count > index) ? states[index] : null;
            }
        }

        //默认组件
        [System.NonSerialized]
        public List<IState> defaults = new List<IState>();



        //获取当前状态名
        public string currentStateName
        {
            get {
                if (state != null) {
                    return state.name;
                }
                return string.Empty;
            }
        }

        //获取当前状态索引
        public int currentStateIndex
        {
            get {
                if (state != null) {
                    return states.FindIndex(obj => obj == state);
                }
                return 0;
            }
        }

        //获取当前状态索引（前提只有2个状态,索引0表示false，索引1表示true）
        public bool currentStateBool
        {
            get {
                return System.Convert.ToBoolean(currentStateIndex);
            }
        }

        //通过顺序索引设置状态
        public void SetStateAtIndex(int i)
        {
            if (states.Count > i) {
                SetState(states[i].name);
            } else {
                Debug.LogError("UIState : 当前没有第 " + i + " 个状态");
            }
        }

        //通过bool值设置（前提只有2个状态,索引0表示false，索引1表示true）
        public void SetStateAtBool(bool b)
        {
            SetStateAtIndex(System.Convert.ToInt32(b));
        }

        //这个方法已经过期，推荐使用 SetStateAtIndex 
        public void SetState(int intName) {
            SetState(intName.ToString());
        }

        //通过状态名索引
        public void SetState(string name)
        {
            //if (currentStateName != name) {

            //ProfilerUtil.BeginSample ("UIState SetState");

            if (m_DefaultHash == null) {
                ResetDefaultValue();
            }

            State state = null;
            for (int i = 0; i < states.Count; i++) {
                state = states[i];
                if (states[i].name == name) {
                    index = i;
                    break;
                }
            }
            if (state != null)
                CopyDataToCom(state);
            //ProfilerUtil.EndSample ();
            //}
        }

        //判断索引是否存在（前提只有2个状态,索引0表示false，索引1表示true）
        public bool HasStateAtBool(bool b)
        {
            return HasStateAtIndex(System.Convert.ToInt32(b));
        }

        //判断索引是否存在
        public bool HasStateAtIndex(int i)
        {
            if (states.Count > i) {
                return HasState(states[i].name);
            }
            return false;
        }

        //判断状态是否存在
        public bool HasState(string name)
        {
            for (int i = 0; i < states.Count; i++)
            {
                if (states[i].name == name)
                    return true;
            }
            return false;
        }

        public Component GetNode(string name)
        {
            if (state != null) {
                for (int i = 0; i < state.nodes.Count; i++) {
                    if (state.nodes[i].node.name == name) {
                        return state.nodes[i].node;
                    }
                }
            }
            return null;
        }

        public RectTransform GetRectTransform(string name)
        {
            if (state != null) {
                for (int i = 0; i < state.rectTransforms.Count; i++) {
                    if (state.rectTransforms[i].rectTransform.name == name) {
                        return state.rectTransforms[i].rectTransform;
                    }
                }
            }
            return null;
        }

        public Image GetImage(string name)
        {
            if (state != null) {
                for (int i = 0; i < state.uiImages.Count; i++) {
                    if (state.uiImages[i].image.name == name) {
                        return state.uiImages[i].image;
                    }
                }
            }
            Debug.LogWarningFormat("UIState 没有找到 ：{0}", name);
            return null;
        }

        public RawImage GetRawImage(string name)
        {
            if (state != null) {
                for (int i = 0; i < state.uiRawImages.Count; i++) {
                    if (state.uiRawImages[i].rawImage.name == name) {
                        return state.uiRawImages[i].rawImage;
                    }
                }
            }
            Debug.LogWarningFormat("UIState 没有找到 ：{0}", name);
            return null;
        }

        public Text GetText(string name)
        {
            if (state != null) {
                for (int i = 0; i < state.uiTexts.Count; i++) {
                    if (state.uiTexts[i].text.name == name) {
                        return state.uiTexts[i].text;
                    }
                }
            }
            Debug.LogWarningFormat("UIState 没有找到 ：{0}", name);
            return null;
        }

        public Outline GetuiOutLine(string name)
        {
            if (state != null) {
                for (int i = 0; i < state.uiOutLine.Count; i++) {
                    if (state.uiOutLine[i].outLine.name == name) {
                        return state.uiOutLine[i].outLine;
                    }
                }
            }
            Debug.LogWarningFormat("UIState 没有找到 ：{0}", name);
            return null;
        }

        public UIGradient GetuiGradient(string name)
        {
            if (state != null) {
                for (int i = 0; i < state.uiGradient.Count; i++) {
                    if (state.uiGradient[i].gradient.name == name) {
                        return state.uiGradient[i].gradient;
                    }
                }
            }
            Debug.LogWarningFormat("UIState 没有找到 ：{0}", name);
            return null;
        }

        public UIGray GetuiGray(string name)
        {
            if (state != null)
            {
                for (int i = 0; i < state.uiGray.Count; i++)
                {
                    if (state.uiGray[i].grayctrl.name == name)
                    {
                        return state.uiGray[i].grayctrl;
                    }
                }
            }
            Debug.LogWarningFormat("UIState 没有找到 ：{0}", name);
            return null;
        }
        public Shadow GetuiShadow(string name)
        {
            if (state != null) {
                for (int i = 0; i < state.uiShadow.Count; i++) {
                    if (state.uiShadow[i].shadow.name == name) {
                        return state.uiShadow[i].shadow;
                    }
                }
            }
            Debug.LogWarningFormat("UIState 没有找到 ：{0}", name);
            return null;
        }

        private void CopyDataToCom(State state)
        {
            int i = 0, count = 0;
            HashSet<Component> seted = new HashSet<Component>();
            if (state != null)
            {
                count = state.nodes.Count;
                for (i = 0; i < count; i++)
                    seted.Add(state.nodes[i].CopyDataToCom());
                count = state.rectTransforms.Count;
                for (i = 0; i < count; i++)
                    seted.Add(state.rectTransforms[i].CopyDataToCom());
                count = state.uiImages.Count;
                for (i = 0; i < count; i++)
                    seted.Add(state.uiImages[i].CopyDataToCom());
                count = state.uiRawImages.Count;
                for (i = 0; i < count; i++)
                    seted.Add(state.uiRawImages[i].CopyDataToCom());
                count = state.uiTexts.Count;
                for (i = 0; i < count; i++)
                    seted.Add(state.uiTexts[i].CopyDataToCom());
                count = state.uiOutLine.Count;
                for (i = 0; i < count; i++)
                    seted.Add(state.uiOutLine[i].CopyDataToCom());
                count = state.uiGradient.Count;
                for (i = 0; i < count; i++)
                    seted.Add(state.uiGradient[i].CopyDataToCom());
                count = state.uiShadow.Count;
                for (i = 0; i < count; i++)
                    seted.Add(state.uiShadow[i].CopyDataToCom());
                count = state.uiGray.Count;
                for (i = 0; i < count; i++)
                    seted.Add(state.uiGray[i].CopyDataToCom());
            }

            count = this.defaults.Count;
            for (i = 0; i < count; i++) {
                Component c = this.defaults[i].GetComponent();
                if (!seted.Contains(c)) {
                    this.defaults[i].DefauleHide();
                }
            }
        }



        HashSet<Component> m_DefaultHash = null;
        void AddDefaultValue(List<UIState.IState> defaults, UIState.IState state, Component c)
        {
            if (!m_DefaultHash.Contains(c)) {
                m_DefaultHash.Add(c);
                defaults.Add(state);
            }
        }

        public void ResetDefaultValue()
        {
            m_DefaultHash = new HashSet<Component>();
            defaults.Clear();
            foreach (UIState.State current in states) {
                foreach (StateNode node in current.nodes)
                    AddDefaultValue(defaults, node, node.node);
                foreach (StateRectTransform rectTransform in current.rectTransforms)
                    AddDefaultValue(defaults, rectTransform, rectTransform.rectTransform);
                foreach (StateImage image in current.uiImages)
                    AddDefaultValue(defaults, image, image.image);
                foreach (StateRawImage rawImage in current.uiRawImages)
                    AddDefaultValue(defaults, rawImage, rawImage.rawImage);
                foreach (StateText text in current.uiTexts)
                    AddDefaultValue(defaults, text, text.text);
                foreach (StateOutline outLine in current.uiOutLine)
                    AddDefaultValue(defaults, outLine, outLine.outLine);
                foreach (StateGradient gradient in current.uiGradient)
                    AddDefaultValue(defaults, gradient, gradient.gradient);
                foreach (StateShadow shadow in current.uiShadow)
                    AddDefaultValue(defaults, shadow, shadow.shadow);
                foreach (StateGray grayt in current.uiGray)
                    AddDefaultValue(defaults, grayt, grayt.grayctrl);
            }
        }

        [System.Serializable]
        public class State
        {
            //UI状态唯一名子
            public string name = string.Empty;
            // component组件(仅用于显示隐藏)
            public List<StateNode> nodes = new List<StateNode>();
            //坐标组件
            public List<StateRectTransform> rectTransforms = new List<StateRectTransform>();
            //图片组件
            public List<StateImage> uiImages = new List<StateImage>();
            //rawImage组件
            public List<StateRawImage> uiRawImages = new List<StateRawImage>();
            //文字组件
            public List<StateText> uiTexts = new List<StateText>();
            //文字描边组件
            public List<StateOutline> uiOutLine = new List<StateOutline>();
            //文字渐变组件
            public List<StateGradient> uiGradient = new List<StateGradient>();
            //文字阴影组件
            public List<StateShadow> uiShadow = new List<StateShadow>();
            // 置灰组件
            public  List<StateGray> uiGray = new List<StateGray>();
        }

        [System.Serializable]
        public abstract class IState
        {
            public bool enable = true;
            public abstract Component GetComponent();
            public abstract Component CopyDataToCom();
            public abstract Component CopyComToData();
            public abstract Component DefauleHide();
        }


    }
}
