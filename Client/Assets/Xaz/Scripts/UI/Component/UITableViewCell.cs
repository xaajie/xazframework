//------------------------------------------------------------
// Xaz Framework
// Feedback: qq515688254
//------------------------------------------------------------
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Xaz
{
    public class UITableViewCell : UITableViewCellTrigger
    {
        internal enum State
        {
            Normal,
            Dimmed,
            Selected,
            Disabled,
        }

        public enum Mode
        {
            Fixed, CalcOnce, CalcEvery
        }

        //指定高宽(当内容需要重叠的时候)
        [SerializeField]
        public bool fixRect = false;

        /// <summary>
        /// 是否不响应长按后点击,默认都响应
        /// </summary>
        [SerializeField]
        public bool noLongClick = false;

        [SerializeField]
        internal Mode mode;

        [NonSerialized]
        internal int dataIndex;

        [NonSerialized]
        public int cellIndex;

        [NonSerialized]
        internal bool active;

        [NonSerialized]
        public int runIndex = -1;

        [SerializeField]
        private GameObject m_NormalState = null;

        [SerializeField]
        private GameObject m_DimmedState = null;

        [SerializeField]
        private GameObject m_SelectedState = null;

        [SerializeField]
        private GameObject m_DisabledState = null;

        private State m_State = State.Normal;
        private GameObject[] m_StateObjects = null;

        internal State state
        {
            get
            {
                return m_State;
            }
            set
            {
                if (m_State == value)
                    return;
                SetStateActive(m_State, false);
                m_State = value;
                SetStateActive(m_State, true);
            }
        }

        void Awake()
        {
            m_StateObjects = new GameObject[4] { m_NormalState, m_DimmedState, m_SelectedState, m_DisabledState };
        }

        void Start()
        {
            tableViewCell = this;

            animCtrl = this.gameObject.GetComponent<UIAnimComp>();
            if (animCtrl)
            {
                animCtrl.SetUIAnimCompState((int)UIAnim.UIAnimType.INIT);
            }

            foreach (Selectable comp in GetComponentsInChildren<Selectable>(true))
            {
                var trigger = comp.gameObject.GetComponent<UITableViewCellTrigger>();
                if (trigger == null)
                {
                    trigger = comp.gameObject.AddComponent<UITableViewCellTrigger>();
                }
                trigger.tableView = tableView;
                trigger.tableViewCell = this;
            }

            for (int i = 0; i < 4; i++)
            {
                GameObject go = m_StateObjects[i];
                if (go == null)
                    continue;
                go.SetActive(false);
            }
            SetStateActive(m_State, true);
        }

        public void SetSelectAnim(bool needplayanim)
        {
            if (animCtrl == null)
            {
                return;
            }
            if (needplayanim)
            {
                //常驻动效触发
                animCtrl.SetUIAnimCompState((int)UIAnim.UIAnimType.CellClick, "select");
            }
            else
            {
                animCtrl.SetUIAnimCompState((int)UIAnim.UIAnimType.INIT);
            }
        }

        private void SetStateActive(State state, bool active)
        {
            GameObject go = m_StateObjects[(int)state];
            if (go != null)
            {
                go.SetActive(active);
            }
        }
    }

    public class UITableViewCellTrigger : BaseTableCell, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
    {
        [System.NonSerialized]
        internal UITableView tableView;

        [System.NonSerialized]
        internal UITableViewCell tableViewCell;

        [System.NonSerialized]
        internal UIAnimComp animCtrl;

        float startPressTime = 0f;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (tableViewCell != null)
            {
                if (tableViewCell.noLongClick && (Time.time - startPressTime > tableView.LongPressDelay))
                {
                    return;
                }
                tableViewCell.SetSelectAnim(true);
                tableView.HandleClick(tableViewCell, gameObject, eventData);
                startPressTime = 0;
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            startPressTime = Time.time;
            if (tableViewCell != null)
            {
                tableView.HandlePress(true, tableViewCell, gameObject, eventData);
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (tableViewCell != null)
            {
                tableView.HandlePress(false, tableViewCell, gameObject, eventData);
            }
        }
    }
}
