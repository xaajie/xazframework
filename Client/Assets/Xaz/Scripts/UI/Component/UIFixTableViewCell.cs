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
    public class UIFixTableViewCell : UIFixTableViewCellTrigger
    {
        public enum Mode
        {
            Fixed, CalcOnce, CalcEvery
        }


        [SerializeField]
        internal Mode mode;

        [NonSerialized]
        internal int dataIndex;

        [NonSerialized]
        internal int cellIndex;

        [NonSerialized]
        internal bool active;

        [NonSerialized]
        public int runIndex = -1;

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
                var trigger = comp.gameObject.GetComponent<UIFixTableViewCellTrigger>();
                if (trigger == null)
                {
                    trigger = comp.gameObject.AddComponent<UIFixTableViewCellTrigger>();
                }
                trigger.tableView = tableView;
                trigger.tableViewCell = this;
            }
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

    }

    
    public class UIFixTableViewCellTrigger : BaseTableCell, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
    {
        [System.NonSerialized]
        internal UIFixTableView tableView;

        [System.NonSerialized]
        internal UIFixTableViewCell tableViewCell;

        [System.NonSerialized]
        internal UIAnimComp animCtrl;

        float startPressTime = 0f;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (tableViewCell != null)
            {
                if (startPressTime == 0 || (Time.time - startPressTime > tableView.LongPressDelay))
                {
                    return;
                }
                startPressTime = 0;
                tableViewCell.SetSelectAnim(true);
                tableView.HandleClick(tableViewCell, gameObject, eventData);
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
