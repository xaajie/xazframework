//------------------------------------------------------------
// Xaz Framework
// Feedback: qq515688254
//------------------------------------------------------------
using UnityEngine;
using UnityEngine.UI;
using Xaz;
[System.Serializable]
public sealed class StateShadow : UIState.IState {
    [SerializeField]
    private Shadow _shadow = null;
    public Shadow shadow{
        get{
            return  _shadow;
        }
        set {
            if(value != _shadow){
                _shadow = value;
                CopyComToData();
            }
        }
    }

	public override Component CopyDataToCom()
    {
        if(shadow != null){
            shadow.enabled = enable;
            shadow.effectColor= this.effectColor;
            shadow.effectDistance = this.effectDistance;
        }
		return shadow;
    }

	public override Component CopyComToData()
    {
        if(shadow){
            this.effectColor = shadow.effectColor;
            this.effectDistance = shadow.effectDistance;
        }
		return shadow;
    }
	public override Component DefauleHide ()
	{
		if(shadow != null){
			shadow.enabled = false;
		}
		return shadow;
	}
	public override Component GetComponent ()
	{
		return shadow;
	}
    public Color effectColor = new Color(0,0,0,0.5f);
    public Vector2 effectDistance = new Vector2(1f,-1f);


}
