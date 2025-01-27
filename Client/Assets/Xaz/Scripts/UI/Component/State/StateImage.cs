//------------------------------------------------------------
// Xaz Framework
// Feedback: qq515688254
//------------------------------------------------------------
using UnityEngine;
using UnityEngine.UI;
using Xaz;

[System.Serializable]
public sealed class StateImage :UIState.IState {

    /// <summary>
    /// 代码赋值图片，state仅变色  modifyby xiejie
    /// </summary>
    public bool codeImg = false;
    [SerializeField]
    private Image _image = null;
    public Image image{
        get{
            return  _image;
        }
        set {
            if(value != _image){
                _image = value;
                CopyComToData();
            }
        }
    }
	public override Component CopyDataToCom()
    {
        if(image != null){
            image.gameObject.SetActive(enable);
			if (enable) {
                if (!codeImg)
                {
                    image.sprite = this.sprite;
                }
				image.color = this.color;
			}
        }
		return image;
    }
	public override Component CopyComToData()
    {
        if(image){
            if (!codeImg)
            {
                this.sprite = image.sprite;
            }
            this.color = image.color;
        }
		return image;
    }
	public override Component DefauleHide ()
	{
		if(image != null){
			image.gameObject.SetActive(false);
		}
		return image;
	}
	public override Component GetComponent ()
	{
		return image;
	}

    public Sprite sprite = null;
    public Color color = Color.white;
}
