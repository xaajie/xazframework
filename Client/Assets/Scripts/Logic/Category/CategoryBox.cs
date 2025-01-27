using UnityEngine;
using UnityEngine.UI;
using Xaz;

public class CategoryBox:MonoBehaviour, IControl
{
    // Start is called before the first frame update
    public UIImage m_Icon;
    public Text m_name;
    public Text m_desc;
    public Text m_count;
    public Text m_namenum;
    public UIRichText m_richDesc;
    public Text m_xcount;
    //public Text m_quailtyTxt;
    //public UIState qualityState;
    public UIImage qualityImg;
    // Update is called once per frame
    public void SetBoxData(IPackageData data, bool ownnum=false)
    {
        if (data == null)
        {
            return;
        }
        if (m_Icon != null)
        {
            m_Icon.SetSprite(data.GetAtlas(), data.GetIcon());
        }
        //if (m_quailtyTxt != null)
        //{
        //    m_quailtyTxt.text = Utils.GetLang(Const.Quailty_Pre+data.GetQuality());
        //    m_quailtyTxt.color = Utils.HexToColor(Const.COLORS_STR[data.GetQuality() - 1]);
        //}
        //if (qualityState != null)
        //{
        //    qualityState.SetState(data.GetQuality());
        //}
        if(qualityImg!=null)
        {
            qualityImg.SetSprite(Const.cardbgatlas,Const.cardbgpre+data.GetQuality().ToString());
            qualityImg.type = Image.Type.Sliced;
        }
        if (m_namenum!=null)
        {
            m_namenum.text = string.Format("{0}x{1}",data.GetName(),data.GetNum());
        }
        if (m_name != null)
        {
            m_name.text = data.GetName();
        }
        if (m_desc != null)
        {
            m_desc.text = data.GetDesc();
        }
        if (m_count != null)
        {
            if (ownnum)
            {
                m_count.text = data.GetOwnNum().ToString();
            }
            else
            {
                //if (data.GetItemType() == (int)Const.Category.CURRENCY)
                //{
                //    if (data.GetNum() >= 1000)
                //    {
                //        string resultString = ((float)data.GetNum() / 1000.0).ToString("0.#");
                //        m_count.text = string.Format("{0}k", resultString);
                //    }
                //    else
                //    {
                //        m_count.text = data.GetNum().ToString();
                //    }
                //}
                //else
                //{
                    m_count.text = data.GetNum().ToString();
                //}
            }
        }
        if (m_xcount != null)
        {
            if (ownnum)
            {
                m_xcount.text = string.Format("x{0}", data.GetOwnNum());
            }
            else
            {
                m_xcount.text = string.Format("x{0}", data.GetNum());
            }
        }
        //if (data.GetItemType() == (int)Const.Category.ITEM || data.GetItemType() == (int)Const.Category.ITEM)
        //{
        //    ModuleMgr.CategoryMgr.SetTip(data,m_Icon.gameObject);
        //}
    }

    private void ShowTip(IPackageData dat, Transform obj)
    {
        //UIMgr.ShowTip(dat);
    }
    public void SetCountTxt(string res)
    {
        m_count.text = res;
    }
}
