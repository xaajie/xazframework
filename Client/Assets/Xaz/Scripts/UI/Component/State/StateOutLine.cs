//------------------------------------------------------------
// Xaz Framework
// Feedback: qq515688254
//------------------------------------------------------------
﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Xaz;

[System.Serializable]
public sealed class StateOutline :UIState.IState {

    [SerializeField]
    private Outline _outLine = null;
    public Outline outLine{
        get{
            return  _outLine;
        }
        set {
            if(value != _outLine){
                _outLine = value;
                CopyComToData();
            }
        }
    }
	public override Component CopyDataToCom()
    {
        if(outLine != null){
            outLine.enabled = enable;
            outLine.effectColor= this.effectColor;
            outLine.effectDistance = this.effectDistance;
        }
		return outLine;
    }
	public override Component CopyComToData()
    {
        if(outLine){
            this.effectColor = outLine.effectColor;
            this.effectDistance = outLine.effectDistance;
        }
		return outLine;
    }
	public override Component DefauleHide ()
	{
		if(outLine != null){
			outLine.enabled = false;
		}
		return outLine;
	}

	public override Component GetComponent ()
	{
		return outLine;
	}
    public Color effectColor = new Color(0,0,0,0.5f);
    public Vector2 effectDistance = new Vector2(1f,-1f);
}
