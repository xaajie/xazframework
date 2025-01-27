//------------------------------------------------------------
// Xaz Framework
// Feedback: qq515688254
//------------------------------------------------------------
using UnityEngine;
using System.Collections;
using Xaz;

[System.Serializable]
public sealed class StateNode:UIState.IState
{
    [SerializeField]
    private Component _node = null;
    public Component node{
        get{
            return  _node;
        }
        set {
            if(value != _node){
                _node = value;
                CopyComToData();
            }
        }
    }

	public override Component CopyComToData()
    {
		return node;
    }


	public override Component CopyDataToCom()
    {
        if(node){
            node.gameObject.SetActive(enable);
        }
		return node;
    }
	public override Component DefauleHide ()
	{
		if(node != null){
			node.gameObject.SetActive(false);
		}
		return node;
	}
	public override Component GetComponent ()
	{
		return node;
	}
}
