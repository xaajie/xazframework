//------------------------------------------------------------
// Xaz Framework
// Feedback: qq515688254
//------------------------------------------------------------
//----------------------------------------------------------------------------
// TabGroup组件
//-- @author xiejie
//----------------------------------------------------------------------------
using UnityEngine;
using UnityEngine.UI;

namespace Xaz
{
    public class UITabGroup : ToggleGroup, IControl
    {
        public delegate void OnTabGroupEvent(UITabGroup table, int index);
        public OnTabGroupEvent onTabGroup;
        private Toggle[] toggles;

        //[SerializeField]
        private int selectTab = 0;
        protected UITabGroup()
        {
           
        }
#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            Awake();
        }

#endif
        protected override void Awake()
        {
            base.Awake();
            toggles = GetComponentsInChildren<Toggle>();
            for (int i = 0; i < toggles.Length; i++)
            {
                toggles[i].onValueChanged.AddListener(ChangeState);
            }
        }

        private void ChangeState(bool isOn)
        {
            if (isOn)
            {
                int index = -1;
                for (int i = 0; i < toggles.Length; i++)
                {
                    if (toggles[i].isOn)
                    {
                        index = i;
                    }
                }
                selectTab = index;
                if (onTabGroup != null)
                {
                    onTabGroup(this, index);
                }
            }
        }

        public void SetChooseByIndex(int v, bool noNotify)
        {
            if (!noNotify)
            {
                for (int i = 0; i < toggles.Length; i++)
                {
                    toggles[i].isOn = i == v;
                }
            }
            else
            {
                for (int i = 0; i < toggles.Length; i++)
                {
                    toggles[i].SetIsOnWithoutNotify(i == v);
                }
            }
        }

        public GameObject GetTabByIndex(int v)
        {
            return toggles[v].gameObject;
        }

        //隐藏指定tab
        public void SetVisibleByIndex(int v, bool visible)
        {
            toggles[v].transform.gameObject.SetActive(visible);
        }

        //置灰指定tab
        public void SetGrayByIndex(int v,bool gray, bool canclick)
        {
            UIGray.SetGray(toggles[v].gameObject, gray, canclick);
        }
        //适用于缓存界面的情况需要调用
        public void ResetGroups()
        {
            SetChooseByIndex(selectTab, true);
        }
        /// </summary>
        protected override void OnDestroy()
        {
            onTabGroup = null;
            toggles = null;
            base.OnDestroy();
        }

    }
}
