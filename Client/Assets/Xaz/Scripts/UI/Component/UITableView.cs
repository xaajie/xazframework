//------------------------------------------------------------
// Xaz Framework
// Feedback: qq515688254
//  @author xiejie
//----------------------------------------------------

using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Xaz
{
    public class UITableView : UIScroller, IControl, IPointerClickHandler
    {
        struct Data
        {
            public bool dirty;
            public Rect rect;
            public string ident;
            public object userData;
            public UITableViewCell.State state;
        }

        #region events

        public delegate void OnCellEvent(UITableView table, UITableViewCell cell, object data);

        public delegate void OnCellTouchEvent(UITableView table, UITableViewCell cell, GameObject target, object data);
        public delegate void OnDataEvent(UITableView tableView, object data);
        public delegate void OnSnapCompleted(UITableView table, int lineIndex, int dataIndex);

        public delegate void OnSnapSoundEvent(UITableView table);

        public OnCellEvent onCellInterval;
        public OnCellEvent onCellInit;
        public OnCellEvent onCellStateChanged;
        public OnCellTouchEvent onCellClick;
        public OnCellTouchEvent onCellPress;
        public OnCellTouchEvent onCellRelease;
        public OnCellTouchEvent onCellDelayPress;
        public OnCellTouchEvent onCellLongPress;

        //m_Snapping需要勾选才有效
        public OnSnapCompleted onSnapCompleted;
        public OnSnapSoundEvent onSnapSoundEvent;
        public OnSnapCompleted onLineIndexChange;

        #endregion

        #region serialized fields

        [SerializeField] private List<UITableViewCell> m_CellList = new List<UITableViewCell>();

        [SerializeField] private TextAnchor m_Alignment = TextAnchor.UpperLeft;

        [SerializeField] private TextAnchor m_ChildAlignment = TextAnchor.UpperLeft;

        [SerializeField] private RectOffset m_Padding = new RectOffset();

        [SerializeField] private Vector2 m_Spacing = new Vector2();

        [SerializeField] protected int m_MaxPerLine = 1;

        [SerializeField] private float offSetY_Angle = 0f;

        // 自动吸附
        [SerializeField] private bool m_Snapping = false;

        //// 触发翻页时的滑动长度，范围0~1，默认为1，滑动至50%时翻页
        //[SerializeField] private double m_snapStrength = 1.0;

        [SerializeField] private bool m_SnapCellCenter = false; // Only used when m_Snapping is enabled

        [SerializeField] private float m_SnapVelocityThreshold = 5f; // Only used when m_Snapping is enabled

        // 循环滚动
        [SerializeField] private bool m_Loop = false;

        [SerializeField] private int m_DelayFrames = 1;

        #endregion

        #region private and protected fields

        private Dictionary<string, UITableViewCell> m_CellPrefabs = new Dictionary<string, UITableViewCell>();
        private Dictionary<string, Bounds> m_CellPrefabBounds = new Dictionary<string, Bounds>();

        private Vector2 m_SnapOffset = Vector2.zero;

        // 偏移，用于对齐
        private Vector2 m_LayoutOffset = Vector2.zero;

        //// 记录snaping时的目标位置
        //private float m_LastPosition = 0;

        private RectTransform m_Container;
        private RectTransform m_RecycledContainer;

        private List<Data> m_Datas = new List<Data>();
        protected List<UITableViewCell> m_ActiveCells = new List<UITableViewCell>();
        private List<UITableViewCell> m_RecycledCells = new List<UITableViewCell>();

        private int m_ActiveCellsStartIndex = -1;
        private int m_ActiveCellsEndIndex = -1;

        private float m_ScrollPosition = 0;

        protected float scrollPosition
        {
            get { return m_ScrollPosition; }
            set
            {
                m_ScrollPosition = Mathf.Clamp(value, 0, m_StoredMaxScrollPosition);
                if (m_Direction == Direction.Vertical)
                {
                    normalizedPosition = 1 - (m_ScrollPosition / m_StoredMaxScrollPosition);
                }
                else
                {
                    normalizedPosition = (m_ScrollPosition / m_StoredMaxScrollPosition);
                }
            }
        }

        private float scrollRectSize
        {
            get { return m_RectTransform.rect.size[(int)m_Direction]; }
        }

        #endregion

        #region properties

        public int dataCount
        {
            get { return m_Datas.Count; }
        }


        private int m_LineIndex;

        public int LineIndex
        {
            get { return m_LineIndex; }
        }

        #endregion

        #region public functions

        private bool m_DataDirty;

        public void AddDataList<T>(List<T> data)
        {
            data.ForEach((dat) =>
            {
                AddData(dat);
            });
        }
        public void AddDataList(List<object> data)
        {
            data.ForEach((dat) =>
            {
                AddData(dat);
            });
        }

        public void AddData(object data)
        {
            AddData(data, m_CellList[0].identifier);
        }

        public void AddData(object data, string identifier)
        {
            m_Datas.Add(new Data() { userData = data, ident = identifier, dirty = true });
            m_DataDirty = true;
        }

        public void AddDataAt(int index, object data)
        {
            AddDataAt(index, data, m_CellList[0].identifier);
        }

        public void AddDataAt(int index, object data, string identifier)
        {
            if (index < 0 || index > m_Datas.Count)
                return;

            m_Datas.Insert(index, new Data() { userData = data, ident = identifier, dirty = true });
            m_DataDirty = true;

            if (index < m_Datas.Count - 1)
            {
                m_ActiveCells.ForEach((cell) =>
                {
                    if (cell.dataIndex >= index)
                    {
                        cell.dataIndex++;
                    }
                });
            }
        }

        public void SetData(object oldData, object newData)
        {
            SetData(oldData, newData, m_CellList[0].identifier);
        }

        public void SetData(object oldData, object newData, string identifier)
        {
            SetDataAt(m_Datas.FindIndex((v) => object.Equals(v.userData, oldData)), newData, identifier);
        }

        public void SetDataAt(int index, object newData)
        {
            SetDataAt(index, newData, m_CellList[0].identifier);
        }

        public void SetDataAt(int index, object newData, string identifier)
        {
            if (index < 0 || index >= m_Datas.Count)
                return;

            var data = m_Datas[index];
            data.userData = newData;
            data.dirty = true;
            data.ident = identifier;
            m_Datas[index] = data;

            m_DataDirty = true;
        }

        public void RemoveData(object data)
        {
            RemoveDataAt(m_Datas.FindIndex((v) => object.Equals(v.userData, data)));
        }

        public void RemoveDataAt(int dataIndex)
        {
            if (dataIndex < 0 || dataIndex >= m_Datas.Count)
                return;

            m_Datas.RemoveAt(dataIndex);
            m_DataDirty = true;

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
        }

        public object GetData(UITableViewCell cell)
        {
            return GetDataAt(cell.dataIndex);
        }

        public object GetDataAt(int dataIndex)
        {
            if (dataIndex < 0 || dataIndex >= m_Datas.Count)
                return null;
            return m_Datas[dataIndex].userData;
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

        public int GetDataIndex(UITableViewCell cell)
        {
            return cell != null ? cell.dataIndex : -1;
        }

        public int GetDataIndex(object data)
        {
            UITableViewCell cell = GetCell(data);
            if (cell != null)
                return cell.dataIndex;

            return m_Datas.FindIndex((v) => { return object.Equals(v.userData, data); });
        }

        //xlua getcell的object匹配不上原因未知，补充方法
        public List<UITableViewCell> GetShowCellList()
        {
            return m_ActiveCells;
        }

        //xlua用不了
        public UITableViewCell GetCell(object data)
        {
            return m_ActiveCells.Find((cell) => object.Equals(m_Datas[cell.dataIndex].userData, data));
        }

        public UITableViewCell GetCellAt(int dataIndex)
        {
            return m_ActiveCells.Find((cell) => cell.dataIndex == dataIndex);
        }

        public void RefreshCell(object data)
        {
            onCellInit(this, GetCell(data), data);
        }

        public virtual void Clear(bool keepPosition)
        {
            DOTween.Kill(this);
            TryStopSnap();
            m_Datas.Clear();
            while (m_ActiveCells.Count > 0)
                RecycleCell(m_ActiveCells[0]);
            m_DataDirty = true;
            if (!keepPosition)
            {
                m_ScrollPosition = 0;
            }
        }

        public void RefreshInterval()
        {
            if (m_DataDirty)
                return;

            if (onCellInit != null)
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
            if (m_DataDirty)
                return;

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
        }

        private int m_JumpToIndex = -1;
        private float m_JumpToInterval = 0f;

        public void JumpTo(int dataIndex)
        {
            JumpTo(dataIndex, 0f);
        }

        public void JumpTo(int dataIndex, float interval)
        {
            m_JumpToIndex = dataIndex;
            m_JumpToInterval = interval;
        }

        #endregion

        #region selection and state

        public bool IsSelected(int dataIndex)
        {
            return CheckDataState(dataIndex, UITableViewCell.State.Selected);
        }

        public bool IsDisabled(int dataIndex)
        {
            return CheckDataState(dataIndex, UITableViewCell.State.Disabled);
        }

        public void SetNormal(int dataIndex, bool fireEvent)
        {
            SetState(dataIndex, UITableViewCell.State.Normal, fireEvent);
        }

        public void SetSelected(int dataIndex, bool fireEvent)
        {
            SetState(dataIndex, UITableViewCell.State.Selected, fireEvent);
        }

        public void SetDisabled(int dataIndex, bool fireEvent)
        {
            SetState(dataIndex, UITableViewCell.State.Disabled, fireEvent);
        }

        private bool CheckDataState(int dataIndex, UITableViewCell.State state)
        {
            if (dataIndex >= 0 && dataIndex < m_Datas.Count)
            {
                return m_Datas[dataIndex].state == state;
            }

            return false;
        }

        private void SetState(UITableViewCell cell, UITableViewCell.State state, bool fireEvent)
        {
            if (cell != null)
            {
                if (cell.state != state)
                {
                    cell.state = state;
                    if (fireEvent && onCellStateChanged != null)
                    {
                        onCellStateChanged(this, cell, m_Datas[cell.dataIndex].userData);
                    }
                }
            }
        }

        private void SetState(int dataIndex, UITableViewCell.State state, bool fireEvent)
        {
            Data data = m_Datas[dataIndex];
            data.state = state;
            m_Datas[dataIndex] = data;

            SetState(GetCellAt(dataIndex), state, fireEvent);
        }

        #endregion

        #region event handlers

        public override void OnInitializePotentialDrag(PointerEventData eventData)
        {
            base.OnInitializePotentialDrag(eventData);
            if (onSnapSoundEvent != null)
            {
                onSnapSoundEvent(this);
            }

            DOTween.Kill(this);
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            base.OnEndDrag(eventData);
            if (onSnapSoundEvent != null)
            {
                onSnapSoundEvent(this);
            }

            TryStartSnap();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;
            TryStartSnap();
        }

        private int m_LongPressHandle;
        [SerializeField] private float m_LongPressDelay = 0.5f;
        [SerializeField] private float m_LongPressInterval = 0.3f;

        public float LongPressDelay
        {
            get { return m_LongPressDelay; }
            set
            {
                if (value < 0 || m_LongPressDelay == value)
                    return;
                m_LongPressDelay = value;
            }
        }

        internal void HandlePress(bool pressed, UITableViewCell cell, GameObject target, PointerEventData eventData)
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

        internal void HandleClick(UITableViewCell cell, GameObject target, PointerEventData eventData)
        {
            TryStartSnap();
            if (onCellClick != null)
            {
                onCellClick(this, cell, target, m_Datas[cell.dataIndex].userData);
            }
        }

        internal void HandlerRelease(UITableViewCell cell, GameObject target, PointerEventData eventData)
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

        protected override void Awake()
        {
            base.Awake();

            GameObject go;

            go = new GameObject("Container", typeof(RectTransform));
            m_Container = go.GetComponent<RectTransform>();
            m_Container.SetParent(m_RectTransform, false);
            if (m_Direction == Direction.Vertical)
            {
                m_Container.anchorMin = new Vector2(0f, 1f);
                m_Container.anchorMax = Vector2.one;
            }
            else
            {
                m_Container.anchorMin = Vector2.zero;
                m_Container.anchorMax = new Vector2(0f, 1f);
            }

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
            m_StoredScrollRectSize = scrollRectSize;
        }

        private int numt = 1;

        protected override void Start()
        {
            base.Start();
            numt = 1;
            for (int i = 0; i < m_CellList.Count; i++)
            {
                var cell = m_CellList[i];
                if (cell == null)
                    continue;
                m_CellPrefabs[cell.identifier] = cell;
                cell.tableView = this;
                cell.gameObject.SetActive(false);
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            TryStartSnap();
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            DOTween.Kill(this, true);
            Scheduler.Remove(ref m_LongPressHandle);
        }

        private void TryStartSnap()
        {
            if (DOTween.IsTweening(this) || m_SnapCoroutine != null)
                return;
            if (m_Snapping && !m_Dragging && m_ScrollPosition > 0 && m_ScrollPosition < m_StoredMaxScrollPosition)
            {
                if (Mathf.Abs(velocity) <= m_SnapVelocityThreshold)
                {
                    velocity = 0f;
                    float position = m_ScrollPosition - padding;
                    if (m_SnapCellCenter)
                    {
                        position += (m_StoredScrollRectSize / 2 - m_SnapOffset[(int)m_Direction]);
                    }

                    m_LineIndex = GetLineIndexAtPosition(position);
                    Vector2 vec = axisRects[m_LineIndex];
                    if (vec.x + vec.y * (m_SnapCellCenter ? 1f : 0.5f) >= position)
                    {
                        position = CalculateScrollPosition(m_LineIndex);
                    }
                    else
                    {
                        position = CalculateScrollPosition(++m_LineIndex);
                    }

                    DOTween.To((v) => scrollPosition = v, m_ScrollPosition, position, 0.1f).SetTarget(this);
                    m_SnapCoroutine = StartCoroutine(SnapCompletedTime(0.1f, m_LineIndex, m_LineIndex * m_MaxPerLine));
                }
            }
        }

        private float CalculateScrollPosition(int lineIndex)
        {
            Vector2 v;
            if (m_Snapping && m_SnapCellCenter)
            {
                v = axisRects[lineIndex];
                return m_SnapOffset[(int)m_Direction] + padding - (m_StoredScrollRectSize - v.y) / 2f + v.x;
            }

            v = axisRects[lineIndex];
            return v.x;
        }

        private UITableViewCell DequeueOrCreateCell(int dataIndex)
        {
            UITableViewCell cell = null;
            Data data = m_Datas[dataIndex];
            for (int i = 0; i < m_RecycledCells.Count; i++)
            {
                cell = m_RecycledCells[i];
                //modify xiejie 解决闪的问题，优先从回收池取上次数据位置一样的
                if (data.ident == cell.identifier && cell.dataIndex == dataIndex)
                {
                    m_RecycledCells.RemoveAt(i);
                    cell.transform.SetParent(m_Container, false);
                    return cell;
                }
            }

            GameObject go = GameObject.Instantiate<GameObject>(m_CellPrefabs[data.ident].gameObject);
            go.SetActive(true);
            cell = go.GetComponent<UITableViewCell>();
            if (cell.runIndex == -1)
            {
                cell.runIndex = numt;
                numt = numt + 1;
            }

            cell.transform.SetParent(m_Container, false);
            cell.tableView = this;
            return cell;
        }

        private void AddCell(int cellIndex, int dataIndex)
        {
            Data data = m_Datas[dataIndex];
            UITableViewCell cell = DequeueOrCreateCell(dataIndex);
            m_ActiveCells.Add(cell);

            // set the cell's properties
            cell.cellIndex = cellIndex;
            cell.dataIndex = dataIndex;
            cell.active = true;
            SetState(cell, data.state, true);

            cell.transform.SetParent(m_Container, false);
            cell.transform.localScale = Vector3.one;

            Vector2 col, row;
            if (m_Direction == Direction.Vertical)
            {
                col = m_ColRects[cellIndex % m_MaxPerLine];
                row = m_RowRects[cellIndex / m_MaxPerLine];
            }
            else
            {
                row = m_RowRects[cellIndex % m_MaxPerLine];
                col = m_ColRects[cellIndex / m_MaxPerLine];
            }

            Rect rect = data.rect;
            col.x += (col.y - rect.width) * ((int)m_ChildAlignment % 3) * 0.5f - rect.x + m_LayoutOffset.x +
                     m_SnapOffset.x + m_Padding.left;
            row.x += (row.y - rect.height) * ((int)m_ChildAlignment / 3) * 0.5f + rect.y + m_LayoutOffset.y +
                     m_SnapOffset.y + m_Padding.top;

            RectTransform rectTransform = (RectTransform)cell.transform;
            Vector2 position = rectTransform.localPosition;
            if (m_Direction == Direction.Vertical)
            {
                if (rectTransform.anchorMin.x == rectTransform.anchorMax.x)
                {
                    position.x = col.x;
                }

                position.y = -row.x;
            }
            else
            {
                if (rectTransform.anchorMin.y == rectTransform.anchorMax.y)
                {
                    position.y = -row.x;
                }

                position.x = col.x;
            }

            cell.transform.localPosition = new Vector2(
                position.y * Mathf.Tan(offSetY_Angle * Mathf.PI / 180) + position.x, position.y);

            if (onCellInit != null)
                onCellInit(this, cell, data.userData);
        }

        private void RecycleCell(UITableViewCell cell)
        {
            cell.SetSelectAnim(false);
            m_ActiveCells.Remove(cell);
            m_RecycledCells.Add(cell);
            cell.transform.SetParent(m_RecycledContainer, false);
            cell.active = false;
        }

        private int m_CellCount = 0;

        private int m_LineCount = 0;

        // x - 坐标， y - 尺寸
        protected Vector2[] m_RowRects = new Vector2[0];
        protected Vector2[] m_ColRects = new Vector2[0];

        private Vector2[] axisRects
        {
            get { return m_Direction == Direction.Horizontal ? m_ColRects : m_RowRects; }
        }

        private void ResizeArray(int newSize, bool reset)
        {
            m_CellCount = newSize;
            m_LineCount = Mathf.CeilToInt(newSize / (float)m_MaxPerLine);
            if (m_Direction == Direction.Vertical)
            {
                ResizeArray(ref m_ColRects, m_MaxPerLine, reset);
                ResizeArray(ref m_RowRects, m_LineCount, reset);
            }
            else
            {
                ResizeArray(ref m_ColRects, m_LineCount, reset);
                ResizeArray(ref m_RowRects, m_MaxPerLine, reset);
            }
        }

        private void ResizeArray<T>(ref T[] array, int newSize, bool reset)
        {
            if (array.Length < newSize)
            {
                var buffer = new T[newSize];
                if (!reset)
                {
                    Array.Copy(array, buffer, array.Length);
                }

                array = buffer;
            }

            if (reset)
            {
                Array.Clear(array, 0, newSize);
            }
        }

        private void ExpandLineSize(int cellIndex, Rect rect)
        {
            int row, col;
            if (m_Direction == Direction.Vertical)
            {
                col = cellIndex % m_MaxPerLine;
                row = cellIndex / m_MaxPerLine;
            }
            else
            {
                row = cellIndex % m_MaxPerLine;
                col = cellIndex / m_MaxPerLine;
            }

            m_ColRects[col].y = Mathf.Max(m_ColRects[col].y, rect.width);
            m_RowRects[row].y = Mathf.Max(m_RowRects[row].y, rect.height);
        }

        private Vector2 CalculateCellOffsets()
        {
            float offsetX = 0f; // m_Padding.left;
            for (int i = 0, count = (m_Direction == Direction.Vertical ? m_MaxPerLine : m_LineCount); i < count; i++)
            {
                m_ColRects[i].x = offsetX;
                offsetX += m_ColRects[i].y + m_Spacing.x;
            }

            float offsetY = 0f; // m_Padding.top;
            for (int i = 0, count = (m_Direction == Direction.Vertical ? m_LineCount : m_MaxPerLine); i < count; i++)
            {
                m_RowRects[i].x = offsetY;
                offsetY += m_RowRects[i].y + m_Spacing.y;
            }

            return new Vector2(offsetX - (offsetX != 0 ? m_Spacing.x : 0), offsetY - (offsetY != 0 ? m_Spacing.y : 0));
        }

        private void CalculateCellSizes()
        {
            int dataCount = m_Datas.Count;
            ResizeArray(dataCount, true);

            Data data;
            Bounds bounds;
            for (int i = 0; i < dataCount; i++)
            {
                data = m_Datas[i];
                if (data.dirty)
                {
                    if (!m_CellPrefabBounds.TryGetValue(data.ident, out bounds))
                    {
                        var cell = m_CellPrefabs[data.ident];
                        if (cell.mode == UITableViewCell.Mode.Fixed)
                        {
                            if (cell.fixRect)
                            {
                                //modify by xiejie
                                Vector2 vt = cell.GetComponent<RectTransform>().sizeDelta;
                                bounds = new Bounds(Vector3.zero, new Vector3(vt.x, vt.y, 0));
                            }
                            else
                            {
                                bounds = RectTransformUtility.CalculateRelativeRectTransformBounds(cell.transform);
                            }

                            m_CellPrefabBounds[data.ident] = bounds;
                        }
                        else if (onCellInit != null)
                        {
                            cell = DequeueOrCreateCell(i);
                            onCellInit(this, cell, data.userData);
                            //foreach (var layout in GetComponentsInChildren<LayoutGroup>(false)) {
                            //	LayoutRebuilder.ForceRebuildLayoutImmediate(layout.GetComponent<RectTransform>());
                            //}
                            bounds = RectTransformUtility.CalculateRelativeRectTransformBounds(cell.transform);
                            if (cell.mode == UITableViewCell.Mode.CalcOnce)
                            {
                                m_CellPrefabBounds[data.ident] = bounds;
                            }

                            RecycleCell(cell);
                        }
                        else
                        {
                            bounds = default(Bounds);
                        }
                    }

                    data.rect = new Rect(new Vector2(bounds.min.x, bounds.max.y), bounds.size);
                    data.dirty = false;
                    m_Datas[i] = data;
                }

                ExpandLineSize(i, data.rect);
            }

            if (m_Loop)
            {
                if (m_MaxPerLine > 1 && dataCount % m_MaxPerLine > 0)
                {
                    for (int i = 2; i <= m_MaxPerLine; i++)
                    {
                        int len = dataCount * i;
                        if (len % m_MaxPerLine == 0)
                        {
                            //size = len / m_MaxPerLine;
                            ResizeArray(len, false);
                            break;
                        }
                    }

                    for (int i = dataCount, length = dataCount * m_MaxPerLine; i < length; i++)
                    {
                        ExpandLineSize(i, m_Datas[i % dataCount].rect);
                    }
                }

                // copy two more cellsizes
                int rate = 1, size = m_LineCount;
                float offset = CalculateCellOffsets()[(int)m_Direction];
                if (offset < m_StoredScrollRectSize)
                {
                    rate = Mathf.CeilToInt(m_StoredScrollRectSize / offset);
                }

                rate *= 3;
                ResizeArray(m_CellCount * rate, false);
                for (int i = 1; i < rate; i++)
                {
                    Array.Copy(axisRects, 0, axisRects, size * i, size);
                }
            }
        }

        private float m_LoopPosition;
        private float m_LoopFirstJumpTrigger;
        private float m_LoopLastJumpTrigger;

        private void CalculateContainerSize()
        {
            Vector2 size = Vector2.zero;
            Vector2 offset = CalculateCellOffsets();
            size.x = m_Padding.left + offset.x + m_Padding.right;
            size.y = m_Padding.top + offset.y + m_Padding.bottom;

            m_SnapOffset = Vector2.zero;
            if (m_SnapCellCenter && !m_Loop)
            {
                if (m_Direction == Direction.Vertical)
                {
                    m_SnapOffset.y = (m_StoredScrollRectSize - m_RowRects[0].y) * 0.5f - m_Padding.top;
                    if (m_LineCount > 0)
                    {
                        size.y += m_SnapOffset.y + (m_StoredScrollRectSize - m_RowRects[m_LineCount - 1].y) * 0.5f -
                                  m_Padding.bottom;
                    }
                }
                else
                {
                    m_SnapOffset.x = (m_StoredScrollRectSize - m_ColRects[0].y) * 0.5f - m_Padding.left;
                    if (m_LineCount > 0)
                    {
                        size.x += m_SnapOffset.x + (m_StoredScrollRectSize - m_ColRects[m_LineCount - 1].y) * 0.5f -
                                  m_Padding.right;
                    }
                }
            }

            float contentSize = size[(int)m_Direction];
            Vector2 sizeDelta = m_Container.sizeDelta;
            sizeDelta[(int)m_Direction] = contentSize;
            m_Container.sizeDelta = sizeDelta;
            m_StoredMaxScrollPosition = contentSize - scrollRectSize;

            m_LayoutOffset = Vector2.zero;
            Rect rect = m_RectTransform.rect;
            if (m_Direction == Direction.Vertical)
            {
                if (!m_Loop && size.y < rect.height)
                {
                    m_LayoutOffset.y = (rect.height - size.y) * ((int)m_Alignment / 3) * 0.5f;
                }

                m_LayoutOffset.x = (rect.width - size.x) * ((int)m_Alignment % 3) * 0.5f;
            }
            else
            {
                if (!m_Loop && size.x < rect.width)
                {
                    m_LayoutOffset.x = (rect.width - size.x) * ((int)m_Alignment % 3) * 0.5f;
                }

                m_LayoutOffset.y = (rect.height - size.y) * ((int)m_Alignment / 3) * 0.5f;
            }

            if (m_Loop)
            {
                int firstIndex = m_LineCount / 3;
                m_LoopPosition = axisRects[firstIndex].x;
                m_LoopFirstJumpTrigger = m_LoopPosition - m_StoredScrollRectSize;
                m_LoopLastJumpTrigger = axisRects[firstIndex * 2].x;
                if (m_SnapCellCenter)
                {
                    scrollPosition = m_LoopPosition - (m_StoredScrollRectSize - axisRects[firstIndex].y) / 2;
                }
                else
                {
                    scrollPosition = m_LoopPosition;
                }
                //Debug.Log(m_LoopPosition + ", " + m_LoopFirstJumpTrigger + ", " + m_LoopLastJumpTrigger);
            }

            //CalculateActiveCells();
        }

        private void CalculateActiveCells()
        {
            int dataCount = m_Datas.Count;
            if (dataCount <= 0)
                return;

            int startIndex, endIndex;
            CalculateCurrentActiveCellRange(out startIndex, out endIndex);

            //Debug.Log(startIndex + ", " + endIndex + " | " + m_ActiveCellsStartIndex + ", " + m_ActiveCellsEndIndex);

            if (startIndex >= m_ActiveCellsStartIndex && endIndex <= m_ActiveCellsEndIndex)
                return;

            int i = 0;
            while (i < m_ActiveCells.Count)
            {
                UITableViewCell cell = m_ActiveCells[i];
                if (cell.cellIndex < startIndex || cell.cellIndex > endIndex)
                {
                    RecycleCell(cell);
                }
                else
                {
                    i++;
                }
            }

            for (i = startIndex; i <= endIndex; i++)
            {
                if (i >= m_ActiveCellsStartIndex && i <= m_ActiveCellsEndIndex)
                    continue;
                AddCell(i, i % dataCount);
            }

            m_ActiveCellsStartIndex = startIndex;
            m_ActiveCellsEndIndex = endIndex;
        }

        private void CalculateCurrentActiveCellRange(out int startIndex, out int endIndex)
        {
            int count = m_Datas.Count;
            var startPosition = m_ScrollPosition - padding - m_SnapOffset[(int)m_Direction];
            var endPosition = startPosition + m_StoredScrollRectSize;

            //Debug.LogWarning(startPosition + ", " + endPosition);

            endIndex = m_LineCount - 1;
            startIndex = GetLineIndexAtPosition(startPosition);

            for (int i = startIndex + 1; i <= endIndex; i++)
            {
                if (axisRects[i].x >= endPosition)
                {
                    endIndex = i - 1;
                    break;
                }
            }

            startIndex = startIndex * m_MaxPerLine;
            endIndex = Mathf.Min((endIndex + 1) * m_MaxPerLine - 1, m_CellCount - 1);
        }

        private float padding
        {
            get { return m_Direction == Direction.Vertical ? m_Padding.top : m_Padding.left; }
        }

        private int GetLineIndexAtPosition(float position)
        {
            return GetLineIndexAtPosition(axisRects, position, 0, m_LineCount - 1);
        }

        private int GetLineIndexAtPosition(Vector2[] array, float position, int startIndex, int endIndex)
        {
            if (startIndex >= endIndex)
            {
                return startIndex;
            }

            var middleIndex = (startIndex + endIndex) / 2;
            if (array[middleIndex].x + array[middleIndex].y >= position)
            {
                return GetLineIndexAtPosition(array, position, startIndex, middleIndex);
            }
            else
            {
                return GetLineIndexAtPosition(array, position, middleIndex + 1, endIndex);
            }
        }

        private int CalLineIndex()
        {
            float position = m_ScrollPosition - padding;
            return GetLineIndexAtPosition(position);
        }

        protected override void OnPositionChanged(float val)
        {
            if (m_Direction == Direction.Vertical)
                m_ScrollPosition = (1f - val) * m_StoredMaxScrollPosition;
            else
                m_ScrollPosition = val * m_StoredMaxScrollPosition;

            if (m_Loop)
            {
                if (m_ScrollPosition < m_LoopFirstJumpTrigger)
                {
                    float velocity = this.velocity;
                    scrollPosition = m_LoopPosition + m_ScrollPosition;
                    this.velocity = velocity;
                }
                else if (m_ScrollPosition > m_LoopLastJumpTrigger)
                {
                    float velocity = this.velocity;
                    scrollPosition = m_LoopPosition + (m_ScrollPosition - m_LoopLastJumpTrigger);
                    this.velocity = velocity;
                }
            }

            if (onLineIndexChange != null)
            {
                int currentLineIndex = CalLineIndex();
                if (m_LineIndex != currentLineIndex)
                {
                    m_LineIndex = currentLineIndex;
                    onLineIndexChange(this, m_LineIndex, m_LineIndex * m_MaxPerLine);
                }
            }

            TryStartSnap();
            CalculateActiveCells();

            if (onValueChanged != null)
                onValueChanged(val);
        }

        void Update()
        {
            if (m_StoredScrollRectSize != scrollRectSize)
            {
                m_StoredScrollRectSize = scrollRectSize;
                m_DataDirty = true;
                m_CellPrefabBounds.Clear();
            }

            if (m_DelayFrames > 0)
            {
                --m_DelayFrames;
                return;
            }

            if (m_DataDirty)
            {
                m_DataDirty = false;

                DOTween.Kill(this);
                TryStopSnap();
                Scheduler.Remove(ref m_LongPressHandle);

                float cachedPosition = m_ScrollPosition;
                while (m_ActiveCells.Count > 0)
                    RecycleCell(m_ActiveCells[0]);
                m_ActiveCellsStartIndex = m_ActiveCellsEndIndex = -1;
                CalculateCellSizes();
                CalculateContainerSize();
                if (m_JumpToIndex < 0)
                {
                    scrollPosition = cachedPosition;
                    CalculateActiveCells();
                }
            }

            if (m_JumpToIndex >= 0)
            {
                m_JumpToIndex = m_JumpToIndex / m_MaxPerLine;
                float position = CalculateScrollPosition(m_JumpToIndex);
                if (m_JumpToInterval > 0f)
                {
                    DOTween.Kill(this, true);
                    DOTween.To((v) => scrollPosition = v, m_ScrollPosition, position, m_JumpToInterval).SetTarget(this);
                    m_SnapCoroutine = StartCoroutine(SnapCompletedTime(m_JumpToInterval, m_JumpToIndex, m_JumpToIndex));
                }
                else
                {
                    scrollPosition = position;
                }

                m_JumpToIndex = -1;
                CalculateActiveCells();
            }
        }

        #endregion

        /// <summary>
        /// 获取滚动实际长度
        /// </summary>
        /// <returns></returns>
        public float GetContentHeight()
        {
            return m_Container.sizeDelta.y;
        }

        /// <summary>
        /// 获取滚动实际宽度
        /// </summary>
        /// <returns></returns>
        public float GetContentWidth()
        {
            return m_Container.sizeDelta.x;
        }

        public void SetSpacing(Vector2 num)
        {
            this.m_Spacing = num;
        }

        public void SetCellLoop(bool loop)
        {
            this.m_Loop = loop;
        }

        /// <summary>
        /// 拖拽结束事件通知 --xiejie add
        /// </summary>
        /// <param name="time"></param>
        /// <param name="lineIndex"></param>
        /// <returns></returns>
        IEnumerator SnapCompletedTime(float time, int lineIndex, int dataIndex)
        {
            yield return new WaitForSeconds(time);
            m_SnapCoroutine = null;
            if (onSnapCompleted != null)
            {
                onSnapCompleted(this, lineIndex, dataIndex);
            }
        }

        private Coroutine m_SnapCoroutine;

        private void TryStopSnap()
        {
            if (m_SnapCoroutine != null)
            {
                StopCoroutine(m_SnapCoroutine);
                m_SnapCoroutine = null;
            }
        }

        /// <summary>
        /// 销毁--jie add
        /// </summary>
        protected override void OnDestroy()
        {
            base.OnDestroy();
            m_CellList.Clear();
            m_ActiveCells.Clear();
            m_CellPrefabBounds.Clear();
            m_CellPrefabs.Clear();

            onCellInterval = null;
            onCellInit = null;
            onCellStateChanged = null;
            onCellStateChanged = null;
            onCellClick = null;
            onCellPress = null;
            onCellRelease = null;
            onCellDelayPress = null;
            onCellLongPress = null;
            onSnapCompleted = null;
            onSnapSoundEvent = null;
            onLineIndexChange = null;
        }

        #region editor simulate

#if UNITY_EDITOR
        // 编辑模式下加载cell (用于调坐标)
        public void EditorAwake()
        {
            if (!Application.isPlaying)
            {
                EditorRest(false);
                Awake();
                Start();
            }
        }

        public void EditorUpdate()
        {
            if (!Application.isPlaying)
            {
                m_DelayFrames = 0;
                Update();
            }
        }

        public List<string> EditorGetCellIdes()
        {
            List<string> identifiers = new List<string>();
            foreach (UITableViewCell cell in m_CellList)
            {
                if (!identifiers.Contains(cell.identifier))
                {
                    identifiers.Add(cell.identifier);
                }
            }

            return identifiers;
        }

        //重置树结构
        public void EditorRest(bool isExport = true)
        {
            if (m_RecycledContainer)
            {
                DestroyImmediate(m_RecycledContainer.gameObject);
            }

            if (transform.Find("RecycledContainer") != null)
            {
                DestroyImmediate(transform.Find("RecycledContainer").gameObject);
            }

            if (m_Container)
            {
                DestroyImmediate(m_Container.gameObject);
            }

            if (!isExport)
            {
                EditorRestList();
            }
        }

        //编辑状态重置
        public void EditorRestList()
        {
            foreach (UITableViewCell cell in m_CellList)
            {
                cell.gameObject.SetActive(true);
            }

            m_RecycledCells.Clear();
            m_ActiveCells.Clear();
            m_CellPrefabBounds.Clear();
            foreach (Transform t in transform.GetComponentInChildren<Transform>())
            {
                if (!t.GetComponent<UITableViewCell>() && (t.GetComponentsInParent<UITableViewCell>().Length == 0))
                {
                    DestroyImmediate(t.gameObject);
                }
            }
        }

        //导出界面，重置cell
        public void EditorRestCell()
        {
            for (int i = 0; i < m_CellList.Count; i++)
            {
                var cell = m_CellList[i];
                if (cell == null)
                    continue;
                cell.gameObject.SetActive(false);
            }
        }
#endif

        #endregion
    }
}