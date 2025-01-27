//------------------------------------------------------------
// Xaz Framework
// Feedback: qq515688254
//------------------------------------------------------------
﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Xaz;
[System.Serializable]
public sealed class StateText :UIState.IState {

    [SerializeField]
    private Text _text = null;
    public Text text{
        get{
            return  _text;
        }
        set {
            if(value != _text){
                _text = value;
                CopyComToData();
            }
        }
    }
	public override Component CopyDataToCom()
    {
        if(text != null){
            text.gameObject.SetActive(enable);
			if (enable) {
				text.font = this.font;
				text.fontSize = this.fontSize;
				text.color = this.color;
			}
        }
		return text;
    }
	public override Component CopyComToData()
    {
        if(text){
            this.font = text.font;
            this.fontSize = text.fontSize;
            this.color = text.color;
        }
		return text;
    }
	public override Component DefauleHide ()
	{
		if(text != null){
			text.gameObject.SetActive(false);
		}
		return text;
	}
	public override Component GetComponent ()
	{
		return text;
	}
    public Font font = null;
    public string content = string.Empty;
    public int fontSize = 0;
    public Color color = Color.white;
}
