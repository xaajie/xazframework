//------------------------------------------------------------
// Xaz Framework
// Feedback: qq515688254
//------------------------------------------------------------
using UnityEngine;
using Xaz;

[System.Serializable]
public sealed class StateGray : UIState.IState
{
    [SerializeField]
    private UIGray _grayctrl = null;
    public UIGray grayctrl
    {
        get
        {
            return _grayctrl;
        }
        set
        {
            if (value != _grayctrl)
            {
                _grayctrl = value;
                CopyComToData();
            }
        }
    }

    public override Component CopyDataToCom()
    {
        if (grayctrl != null)
        {
            grayctrl.IsGray = this.isGray;
            if (isCanSetRayCast)
            {
                grayctrl.disableSelect = this.disableSelect;
            }
        }
        return grayctrl;
    }
    public override Component CopyComToData()
    {
        if (grayctrl)
        {
            this.isGray = grayctrl.IsGray;
            this.disableSelect = grayctrl.disableSelect;
        }
        return grayctrl;
    }
    public override Component DefauleHide()
    {
        if (grayctrl != null)
        {
            grayctrl.enabled = false;
            grayctrl.IsGray = false;
            if (isCanSetRayCast)
            {
                grayctrl.disableSelect = false;
            }
        }
        return grayctrl;
    }

    public override Component GetComponent()
    {
        return grayctrl;
    }
    public bool isGray = false;
    public bool isCanSetRayCast = false;
    public bool disableSelect = false;
}
