//------------------------------------------------------------
// Xaz Framework
// Feedback: qq515688254
//------------------------------------------------------------
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Xaz;

[System.Serializable]
public sealed class StateRawImage :UIState.IState{

    /// <summary>
    /// 代码赋值图片，state仅变色  modifyby IanCao ,learned from StateImage.cs by xiejie
    /// </summary>
    public bool codeImg = false;
    [SerializeField]
	private RawImage _rawImage = null;
	public RawImage rawImage{
		get{
			return  _rawImage;
		}
		set {
			if(value != _rawImage){
				_rawImage = value;
				CopyComToData();
			}
		}
	}

	#region implemented abstract members of IState
	public override Component CopyDataToCom ()
	{
		if(rawImage != null){
			rawImage.gameObject.SetActive(enable);
			if (enable) {
				if (!codeImg)
				{
                    rawImage.texture = this.texture;
                }
				rawImage.color = this.color;
			}
		}
		return rawImage;
	}
	public override Component CopyComToData ()
	{
		if(rawImage){
			if (!codeImg) 
			{
                this.texture = rawImage.texture;
            }
			this.color = rawImage.color;
		}
		return rawImage;
	}
	public override Component DefauleHide ()
	{
		if(rawImage != null){
			rawImage.gameObject.SetActive(false);
		}
		return rawImage;
	}
	public override Component GetComponent ()
	{
		return rawImage;
	}

	#endregion

	public Texture texture = null;
	public Color color = Color.white;
}
