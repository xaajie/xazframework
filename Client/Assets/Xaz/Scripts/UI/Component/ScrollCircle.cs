//------------------------------------------------------------
// Xaz Framework
// Feedback: qq515688254
//------------------------------------------------------------
﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScrollCircle :ScrollRect 
{
	public float m_thumbResetSpeed = 90f;
	protected float mRadius;
	bool m_isDrag = false;
	const float m_minOffset = 0.01f;

    protected override void Start()
    {
        base.Start();
        mRadius = (transform as RectTransform).sizeDelta.x * 0.5f;
    }

	public override void OnDrag (UnityEngine.EventSystems.PointerEventData eventData)
	{
		base.OnDrag (eventData);
		var contentPostion = this.content.anchoredPosition;
		if (contentPostion.magnitude > mRadius){
			contentPostion = contentPostion.normalized * mRadius ;
			SetContentAnchoredPosition(contentPostion);
		}
	}

	public override void OnBeginDrag(UnityEngine.EventSystems.PointerEventData eventData)
	{
		base.OnBeginDrag(eventData);
		m_isDrag = true;
	}

	public override void OnEndDrag(UnityEngine.EventSystems.PointerEventData eventData)
	{
		base.OnEndDrag(eventData);
		m_isDrag = false;
	}

	void Update()
	{
		if(!m_isDrag)
		{
			//复位
			Vector2 offset = content.anchoredPosition;
			if(offset.magnitude > m_minOffset)
			{
				content.anchoredPosition = Vector2.Lerp(content.anchoredPosition,Vector2.zero,Time.deltaTime*m_thumbResetSpeed);
			}
		}

	}

    public  void ResetComp()
	{
		//base.Reset();
		content.anchoredPosition = Vector2.zero;
		m_isDrag = false;
	}

	public float GetRadius()
	{
		return mRadius;
	}
}
