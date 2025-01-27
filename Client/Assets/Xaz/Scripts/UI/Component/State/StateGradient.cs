//------------------------------------------------------------
// Xaz Framework
// Feedback: qq515688254
//------------------------------------------------------------
﻿using UnityEngine;
using Xaz;
[System.Serializable]
public sealed class StateGradient :UIState.IState {
    [SerializeField]
    private UIGradient _gradient = null;
    public UIGradient gradient{
        get{
            return  _gradient;
        }
        set {
            if(value != _gradient){
                _gradient = value;
                CopyComToData();
            }
        }
    }
	public override Component CopyDataToCom()
    {
        if(gradient != null){
            gradient.enabled = enable;
            gradient.topColor= this.topColor;
            gradient.bottomColor = this.bottomColor;
            gradient.Refresh();
        }
		return gradient;
    }
	public override Component CopyComToData()
    {
        if(gradient){
            this.topColor = gradient.topColor;
            this.bottomColor = gradient.bottomColor;
        }
		return gradient;
    }
	public override Component DefauleHide ()
	{
		if(gradient != null){
			gradient.enabled = false;
		}
		return gradient;
	}

	public override Component GetComponent ()
	{
		return gradient;
	}

    public Color topColor = new Color(0,0,0,1f);
    public Color bottomColor = new Color(1f,1f,1f,1f);
}
