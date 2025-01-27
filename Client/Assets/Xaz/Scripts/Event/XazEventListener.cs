//------------------------------------------------------------
// Xaz Framework
// Feedback: qq515688254
//------------------------------------------------------------
﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Xaz
{
    #if USE_LUA
    [SLua.CustomLuaClass]
    #endif
    [AddComponentMenu("Xaz/Event Listener")]
    public class XazEventListener : EventTrigger
    {
        public delegate void VoidDelegate(GameObject go);
        public delegate void BoolDelegate(GameObject go, bool state);
        public delegate void BoolEventDelegate(bool state, PointerEventData eve);
        public delegate void FloatDelegate(GameObject go, float delta);
        public delegate void VectorDelegate(GameObject go, Vector2 delta);
        public delegate void ObjectDelegate(GameObject go, GameObject draggedObject);
        public delegate void BaseEventListener(GameObject go, BaseEventData eventData);

        public VoidDelegate onClick;
        public VoidDelegate onDoubleClick;
        public BoolDelegate onPress;
        public BoolEventDelegate onPressEve;
        public BoolDelegate onSelect;
        public FloatDelegate onScroll;
        public VectorDelegate onDrag;
        public VoidDelegate onDragOver;
        public VoidDelegate onDragOut;
        public ObjectDelegate onDrop;
        public BaseEventListener onClickBaseEvent;
		public VoidDelegate onBeginDrag;
		public VoidDelegate onEndDrag;

        private bool m_Dragging;
        public override void OnBeginDrag(PointerEventData eventData)
        {
            m_Dragging = true;
			if (onBeginDrag != null)
				onBeginDrag(gameObject);
        }
        public override void OnEndDrag(PointerEventData eventData)
        {
            m_Dragging = false;
			if (onEndDrag != null)
				onEndDrag(gameObject);
        }
        public override void OnPointerClick(PointerEventData eventData)
        {
            if (!m_Dragging && onClick != null)
                onClick(gameObject);
            if (onClickBaseEvent != null) { onClickBaseEvent(gameObject, eventData); }
        }
        public override void OnPointerDown(PointerEventData eventData)
        {
            if (onPress != null)
                onPress(gameObject, true);
            if(onPressEve!=null)
                onPressEve(true, eventData);
        }
        public override void OnPointerUp(PointerEventData eventData)
        {
            if (onPress != null)
                onPress(gameObject, false);
            if (onPressEve != null)
                onPressEve(false,eventData);
        }
        public override void OnSelect(BaseEventData eventData)
        {
            if (onSelect != null)
                onSelect(gameObject, true);
        }
        public override void OnDeselect(BaseEventData eventData)
        {
            if (onSelect != null)
                onSelect(gameObject, false);
        }
        public override void OnDrag(PointerEventData eventData)
        {
            if (onDrag != null)
                onDrag(gameObject, eventData.delta);
        }
        public override void OnDrop(PointerEventData eventData)
        {
            if (onDrop != null)
                onDrop(gameObject, eventData.selectedObject);
        }
        public override void OnPointerEnter(PointerEventData eventData)
        {
            if (onDragOver != null)
                onDragOver(gameObject);
        }
        public override void OnPointerExit(PointerEventData eventData)
        {
            if (onDragOut != null)
                onDragOut(gameObject);
        }
        public override void OnScroll(PointerEventData eventData)
        {
            if (onScroll != null)
                onScroll(gameObject, eventData.scrollDelta.y);
        }
        /// <summary>
        /// Get or add an event listener to the specified game object.
        /// </summary>

        static public XazEventListener Get(GameObject go)
        {
            XazEventListener listener = go.GetComponent<XazEventListener>();
            if (listener == null)
                listener = go.AddComponent<XazEventListener>();
            return listener;
        }
    }
}