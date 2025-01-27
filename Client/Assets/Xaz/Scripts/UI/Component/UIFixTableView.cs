//------------------------------------------------------------
// Xaz Framework
// Feedback: qq515688254
//------------------------------------------------------------
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
namespace Xaz
{
    public class UIFixTableView : BaseTable, IControl, IPointerClickHandler
    {
        struct Data
        {
            public bool dirty;
            public Rect rect;
            public string ident;
            public object userData;
        }

        #region events
        
        public delegate void OnCellEvent(UIFixTableView table, UIFixTableViewCell cell, object data);
        
        public delegate void OnCellTouchEvent(UIFixTableView table, UIFixTableViewCell cell, GameObject target, object data);
        
        public delegate void OnDataEvent(UIFixTableView tableView, object data);

        public OnCellEvent onCellInterval;
        public OnCellEvent onCellInit;
        public OnCellEvent onCellStateChanged;
        public OnCellTouchEvent onCellClick;
        public OnCellTouchEvent onCellPress;
        public OnCellTouchEvent onCellRelease;
        public OnCellTouchEvent onCellDelayPress;
        public OnCellTouchEvent onCellLongPress;
        #endregion

        #region serialized fields
        [SerializeField]
        private List<UIFixTableViewCell> m_CellList = new List<UIFixTableViewCell>();

        #endregion

        #region private and protected fields
        private Dictionary<string, UIFixTableViewCell> m_CellPrefabs = new Dictionary<string, UIFixTableViewCell>();

        
        public RectTransform scrollContent;
        private int numt = 1;

        private RectTransform m_Container;
        private RectTransform m_RecycledContainer;

        private List<Data> m_Datas = new List<Data>();
        protected List<UIFixTableViewCell> m_ActiveCells = new List<UIFixTableViewCell>();
        private List<UIFixTableViewCell> m_RecycledCells = new List<UIFixTableViewCell>();

        private bool m_DataDirty;
        #endregion

        #region properties
        public int dataCount
        {
            get
            {
                return m_Datas.Count;
            }
        }

        private int m_LineIndex;
        public int LineIndex
        {
            get
            {
                return m_LineIndex;
            }
        }
        #endregion

        void Update()
        {
            if (m_DataDirty)
            {
                m_DataDirty = false;
                CalculateActiveCells();
            }
        }

        #region public functions
        public void AddData(object data)
        {
            AddData(data, m_CellList[0].identifier);
        }

        public void AddData(object data, string identifier)
        {
            m_Datas.Add(new Data() { userData = data, ident = identifier, dirty = true });
            m_DataDirty = true;
        }

        public void AddDataAt(int index, object data, bool needRefreshAll)
        {
            AddDataAt(index, data, m_CellList[0].identifier, needRefreshAll);
        }

        public void AddDataAt(int index, object data, string identifier, bool needRefreshAll)
        {
            m_Datas.Insert(index, new Data() { userData = data, ident = identifier, dirty = true });
            //m_DataDirty = true;
            AddCell(index, index);
            for (int i = (m_ActiveCells.Count - 1); i >= index ; i--)
            {
                var cell = m_ActiveCells[i];
                cell.dataIndex = i;
                cell.cellIndex = i;
            }
            if (needRefreshAll)
            {
                Refresh();
            }
        }

        public void RemoveData(object data)
        {
            RemoveDataAt(m_Datas.FindIndex((v) => object.Equals(v.userData, data)), false);
        }
        public void RemoveDataAt(int dataIndex, bool needRefreshAll)
        {
            if (dataIndex < 0 || dataIndex >= m_Datas.Count)
                return;

            m_Datas.RemoveAt(dataIndex);
            for (int i = m_ActiveCells.Count - 1; i >= 0; i--)
            {
                var cell = m_ActiveCells[i];
                if (cell.dataIndex == dataIndex)
                {
                    RecycleCell(cell);
                }
                else if (cell.dataIndex > dataIndex)
                {
                    cell.dataIndex--;
                }
            }
            if (needRefreshAll)
            {
                Refresh();
            }
        }

        public object GetData(UIFixTableViewCell cell)
        {
            return GetDataAt(cell.dataIndex);
        }
        public object GetDataAt(int dataIndex)
        {
            if (dataIndex < 0 || dataIndex >= m_Datas.Count)
                return null;
            return m_Datas[dataIndex].userData;
        }

        public int GetDataIndex(UIFixTableViewCell cell)
        {
            return cell != null ? cell.dataIndex : -1;
        }
        public int GetDataIndex(object data)
        {
            UIFixTableViewCell cell = GetCell(data);
            if (cell != null)
                return cell.dataIndex;

            return m_Datas.FindIndex((v) =>
            {
                return object.Equals(v.userData, data);
            });
        }

        //xlua getcell的object匹配不上原因未知，补充方法
        public List<UIFixTableViewCell> GetShowCellList()
        {
            return m_ActiveCells;
        }

        //xlua用不了
        public UIFixTableViewCell GetCell(object data)
        {
            return m_ActiveCells.Find((cell) => object.Equals(m_Datas[cell.dataIndex].userData, data));
        }
        public UIFixTableViewCell GetCellAt(int dataIndex)
        {
            return m_ActiveCells.Find((cell) => cell.dataIndex == dataIndex);
        }

        public void RefreshCell(object data)
        {
            onCellInit(this, GetCell(data), data);
        }
        public List<object> GetAllData()
        {
            List<object> userdatas = new List<object>(m_Datas.Count);
            for (int i = 0; i < m_Datas.Count; i++)
            {
                userdatas.Add(m_Datas[i].userData);
            }
            return userdatas;
        }
        public void Clear(bool keepPosition)
        {
            DOTween.Kill(this);
            Scheduler.Remove(ref m_LongPressHandle);
            m_Datas.Clear();
            while (m_ActiveCells.Count > 0)
                RecycleCell(m_ActiveCells[0]);
        }

        public void RefreshInterval()
        {
            if (m_DataDirty)
                return;

            if (onCellInterval != null)
            {
                using (var it = m_ActiveCells.GetEnumerator())
                {
                    while (it.MoveNext())
                    {
                        onCellInterval(this, it.Current, m_Datas[it.Current.dataIndex].userData);
                    }
                }
            }
        }

        public void Refresh()
        {
            if (onCellInit != null)
            {
                using (var it = m_ActiveCells.GetEnumerator())
                {
                    while (it.MoveNext())
                    {
                        onCellInit(this, it.Current, m_Datas[it.Current.dataIndex].userData);
                    }
                }
            }
            if (scrollContent)
            {
                UIFixTableViewCell vt = m_ActiveCells[m_ActiveCells.Count - 1];
                UIFixTableViewCell fvt = m_ActiveCells[0];
                if (vt && fvt)
                {
                    scrollContent.sizeDelta = new Vector2(Mathf.Abs(vt.transform.localPosition.x - fvt.transform.localPosition.x), Mathf.Abs(vt.transform.localPosition.y - fvt.transform.localPosition.y));
                }
                else
                {
                    scrollContent.sizeDelta = new Vector2(200, 200);
                }
            }
        }

        #endregion

        #region event handlers

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

        }

        private int m_LongPressHandle;
        [SerializeField]
        private float m_LongPressDelay = 0.5f;
        [SerializeField]
        private float m_LongPressInterval = 0.2f;

        public float LongPressDelay
        {
            get
            {
                return m_LongPressDelay;
            }
            set
            {
                if (value < 0 || m_LongPressDelay == value)
                    return;
                m_LongPressDelay = value;
            }
        }

        internal void HandlePress(bool pressed, UIFixTableViewCell cell, GameObject target, PointerEventData eventData)
        {
            Scheduler.Remove(ref m_LongPressHandle);

            if (pressed)
            {
                object data = m_Datas[cell.dataIndex].userData;
                if (onCellPress != null)
                {
                    onCellPress(this, cell, target, data);
                }
                if (onCellDelayPress != null || onCellLongPress != null)
                {
                    m_LongPressHandle = Scheduler.Timeout(delegate ()
                    {
                        if (onCellDelayPress != null)
                        {
                            onCellDelayPress(this, cell, target, data);
                        }
                        if (onCellLongPress != null)
                        {
                            onCellLongPress(this, cell, target, data);
                            m_LongPressHandle = Scheduler.Interval(delegate ()
                            {
                                if (onCellLongPress != null)
                                {
                                    onCellLongPress(this, cell, target, data);
                                }
                                else
                                {
                                    Scheduler.Remove(ref m_LongPressHandle);
                                }
                            }, m_LongPressInterval);
                        }
                    }, m_LongPressDelay);
                }
            }
            else
            {
                HandlerRelease(cell, target, eventData);
            }
        }
        internal void HandleClick(UIFixTableViewCell cell, GameObject target, PointerEventData eventData)
        {
            if (onCellClick != null)
            {
                onCellClick(this, cell, target, m_Datas[cell.dataIndex].userData);
            }
        }

        internal void HandlerRelease(UIFixTableViewCell cell, GameObject target, PointerEventData eventData)
        {
            object data = m_Datas[cell.dataIndex].userData;
            //cell 抬起处理
            if (onCellRelease != null)
            {
                onCellRelease(this, cell, target, data);
            }
        }
        #endregion

        #region private and overrided methods
        private float m_StoredScrollRectSize;
        protected float m_StoredMaxScrollPosition = 0f;
        protected RectTransform m_RectTransform
        {
            get; private set;
        }

        public RectTransform m_Content
        {
            get; set;
        }
        protected override void Awake()
        {
            base.Awake();
            m_RectTransform = (RectTransform)transform;
            GameObject go;

            go = new GameObject("Container", typeof(RectTransform));
            m_Container = go.GetComponent<RectTransform>();
            m_Container.SetParent(m_RectTransform, false);
            m_Container.pivot = new Vector2(0f, 1f);
            m_Container.offsetMax = m_Container.offsetMin = Vector3.zero;
            m_Container.localScale = Vector3.one;

            go = new GameObject("RecycledContainer", typeof(RectTransform));
            m_RecycledContainer = go.GetComponent<RectTransform>();
            m_RecycledContainer.SetParent(m_RectTransform, false);
            m_RecycledContainer.anchorMin = Vector2.zero;
            m_RecycledContainer.anchorMax = Vector2.one;
            m_RecycledContainer.pivot = new Vector2(0f, 1f);
            m_RecycledContainer.offsetMax = m_RecycledContainer.offsetMin = Vector3.zero;
            m_RecycledContainer.localScale = Vector3.one;
            m_RecycledContainer.gameObject.SetActive(false);

            m_Content = m_Container;
            numt = 1;
            for (int i = 0; i < m_CellList.Count; i++)
            {
                var cell = m_CellList[i];
                if (cell == null)
                    continue;
                m_CellPrefabs[cell.identifier] = cell;
                cell.gameObject.SetActive(true);
                cell.tableView = this;
                cell.gameObject.SetActive(false);
            }
        }

        protected override void Start()
        {
            base.Start();
            //numt = 1;
            //for (int i = 0; i < m_CellList.Count; i++)
            //{
            //    var cell = m_CellList[i];
            //    if (cell == null)
            //        continue;
            //    m_CellPrefabs[cell.identifier] = cell;
            //    cell.gameObject.SetActive(true);
            //    cell.tableView = this;
            //    cell.gameObject.SetActive(false);
            //}
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            DOTween.Kill(this, true);
            Scheduler.Remove(ref m_LongPressHandle);
        }

        private UIFixTableViewCell DequeueOrCreateCell(int dataIndex)
        {
            UIFixTableViewCell cell = null;
            Data data = m_Datas[dataIndex];
            for (int i = 0; i < m_RecycledCells.Count; i++)
            {
                cell = m_RecycledCells[i];
                if (data.ident == cell.identifier)
                {
                    m_RecycledCells.RemoveAt(i);
                    cell.transform.SetParent(m_Container, false);
                    return cell;
                }
            }
            if (m_CellPrefabs.ContainsKey(data.ident))
            {
                GameObject go = GameObject.Instantiate<GameObject>(m_CellPrefabs[data.ident].gameObject);
                go.SetActive(true);
                cell = go.GetComponent<UIFixTableViewCell>();
                if (cell.runIndex == -1)
                {
                    cell.runIndex = numt;
                    numt = numt + 1;
                }
                cell.transform.SetParent(m_Container, false);
                cell.tableView = this;
                return cell;
            }
            return null;
        }

        private void AddCell(int cellIndex, int dataIndex)
        {
            Data data = m_Datas[dataIndex];
            UIFixTableViewCell cell = DequeueOrCreateCell(dataIndex);
            m_ActiveCells.Insert(cellIndex, cell);
           // m_ActiveCells.Add(cell);
            cell.cellIndex = cellIndex;
            cell.dataIndex = dataIndex;
            cell.active = true;
            cell.transform.SetParent(m_Container, false);
            cell.transform.localScale = Vector3.one;
            RectTransform rectTransform = (RectTransform)cell.transform;
            cell.transform.localPosition = new Vector2(0, 0);

            if (onCellInit != null)
                onCellInit(this, cell, data.userData);
        }

        private void RecycleCell(UIFixTableViewCell cell)
        {
            m_ActiveCells.Remove(cell);
            m_RecycledCells.Add(cell);
            cell.transform.SetParent(m_RecycledContainer, false);
            cell.active = false;
        }

        private void CalculateActiveCells()
        {
            int dataCount = m_Datas.Count;
            if (dataCount <= 0)
                return;

            int i = 0;
            for (i = m_ActiveCells.Count; i < dataCount; i++)
            {
                AddCell(i, i);
            }
        }

        #endregion

        protected override void  OnDestroy()
        {
            base.OnDestroy();
            m_CellList.Clear();
            m_ActiveCells.Clear();
            m_CellPrefabs.Clear();

            onCellInterval = null;
            onCellInit = null;
            onCellStateChanged = null;
            onCellClick = null;
            onCellPress = null;
            onCellRelease = null;
            onCellDelayPress = null;
            onCellLongPress = null;
        }
    }
}
