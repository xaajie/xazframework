//------------------------------------------------------------
// Xaz Framework
// Feedback: qq515688254
//------------------------------------------------------------
﻿using UnityEngine;
using System.Collections;
using Xaz;
[System.Serializable]
public sealed class StateRectTransform:UIState.IState
{
    [SerializeField]
    private RectTransform _rectTransform = null;
    public RectTransform rectTransform{
        get{
            return  _rectTransform;
        }
        set {
            if(value != _rectTransform){
                _rectTransform = value;
                CopyComToData();
            }
        }
    }

	public override Component CopyComToData()
    {
        if(rectTransform){
            this.postion = rectTransform.anchoredPosition;
            this.widthAndHieght = rectTransform.sizeDelta;
            this.AnchorsMin = rectTransform.anchorMin;
            this.AnchorsMax = rectTransform.anchorMax;
            this.Pivot = rectTransform.pivot;
            this.rotation = rectTransform.localRotation.eulerAngles;
            this.scale = rectTransform.localScale;
        }
		return rectTransform;
    }


	public override Component CopyDataToCom()
    {
        if(rectTransform){
            rectTransform.gameObject.SetActive(enable);
			if (enable) {
				rectTransform.anchoredPosition = this.postion;
				rectTransform.sizeDelta = this.widthAndHieght;
				rectTransform.anchorMin = this.AnchorsMin;
				rectTransform.anchorMax = this.AnchorsMax;
				rectTransform.pivot = this.Pivot;
				rectTransform.localRotation = Quaternion.Euler (this.rotation);
				rectTransform.localScale = this.scale;
			}
        }
		return rectTransform;
    }
	public override Component DefauleHide ()
	{
		if(rectTransform != null){
			rectTransform.gameObject.SetActive(false);
		}
		return rectTransform;
	}
	public override Component GetComponent ()
	{
		return rectTransform;
	}
    public Vector3 postion = Vector3.zero;
    public Vector2 widthAndHieght = Vector2.zero;
    public Vector2 AnchorsMin = Vector2.zero;
    public Vector2 AnchorsMax = Vector2.zero;
    public Vector2 Pivot = Vector2.zero;
    public Vector3 rotation = Vector3.zero;
    public Vector3 scale = Vector3.zero;
}
