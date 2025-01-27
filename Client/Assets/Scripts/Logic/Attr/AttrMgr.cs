using System;
using System.Collections.Generic;
using Table;

public class AttrMgr
{
    private List<UserAttrupShowData> attrList = new List<UserAttrupShowData>();

    public AttrMgr()
    {
        List<int> groups = new List<int>();
        foreach (attrup cha in StaticDataMgr.Instance.attrupInfo.Values)
        {
            if (groups.IndexOf(cha.groupId) == -1)
            {
                UserAttrupShowData dat = new UserAttrupShowData(cha.groupId);
                attrList.Add(dat);
                groups.Add(cha.groupId);
            }
        }
    }

    public List<UserAttrupShowData> CheckGetShowAttrupList()
    {
        foreach (UserAttrupShowData cha in attrList)
        {
            cha.RefreshLevelData();
        }
        attrList.Sort((a, b) => a.GetSort().CompareTo(b.GetSort()));
        return attrList;
    }

    public int GetAttrGroupId(int attrId, int attrTar)
    {
        List<UserAttrupShowData> info = CheckGetShowAttrupList();
        for (int i = 0; i < info.Count; i++)
        {
            if (info[i].GetAttrId() == attrId && info[i].GetActionTarget() == attrTar)
            {
                return info[i].GetAttrGroupId();
            }
        }
        return -1;
    }
    public float GetCountAddVal(float baseval, int attrId, int attrTar)
    {
        attr info = StaticDataMgr.Instance.attrInfo[attrId];
        float val = ModuleMgr.BuffMgr.CheckGetBuffAttrVal(attrId, attrTar);
        if (val > 0)
        {
            if (info.countType == (int)Const.AttrCountType.Add)
            {
                return val;
            }
            else
            {
                return baseval * (float)val / 100;
            }
        }
        return 0;
    }
    public string GetShowAttrval(int val, int countType)
    {
        if (countType == (int)Const.AttrCountType.Add)
        {
            if (val > 0)
            {
                return string.Format("+{0}", val.ToString());
            }
            else if (val > 0)
            {
                return string.Format("-{0}", val.ToString());
            }
            else
            {
                return "";
            }
        }
        else
        {
            if (val > 0)
            {
                return string.Format("+{0}%", val.ToString());
            }
            else if (val > 0)
            {
                return string.Format("-{0}%", val.ToString());
            }
            else
            {
                return "";
            }
        }
    }
    public void Release()
    {

    }
}
