using System;
using System.Collections.Generic;
using Table;
using Xaz;

public class BuffMgr
{
    private Dictionary<int,UserBuffData> buffList = new Dictionary<int, UserBuffData>();
    public BuffMgr()
    {
    }

    public void SetBuffData(List<UserBuffDataBase> info)
    {
        for (int i = 0; i < info.Count; i++)
        {
            UserBuffData vt = new UserBuffData();
            vt.SetData(info[i]);
            buffList.Add(vt.buffId, vt);
        }
    }
    public Dictionary<int,UserBuffData> GetBuffs()
    {
        return buffList;
    }

    public void CheckBuff()
    {
        foreach (UserBuffData buff in buffList.Values)
        {
            if (!buff.IsValid())
            {
                ModuleMgr.FightMgr.LevelUpActor(buff.GetActionTarget());
                buff.DelBuffEffect();
                DelBuff(buff.buffId);
                break;
            }
        }
    }
    public void DelBuff(int id)
    {
        if (buffList.ContainsKey(id))
        {
            buffList.Remove(id);
        }
    }

    public void AddBuff(int id)
    {
        UserBuffData buff;
        buffList.TryGetValue(id, out buff);
        if (buff==null)
        {
            buff = new UserBuffData();
            buff.buffId = id;
            buffList.Add(buff.buffId, buff);
        }
        buff.CreatBuffEffect();
        ModuleMgr.FightMgr.LevelUpActor(buff.GetActionTarget());
    }

    public UserBuffData CheckGetBuff(int id)
    {
        if (buffList.ContainsKey(id))
        {
            if (buffList[id].IsValid())
            {
                return buffList[id];
            }
        }
        return null;
    }
    public float CheckGetBuffAttrVal(int attrId, int attrTar)
    {
        float sumval = 0;
        foreach(UserBuffData info in buffList.Values)
        {
            if (info.IsAttrMatch(attrId, attrTar) && info.IsValid())
            {
                sumval = sumval + info.GetAttrVal();
            }
        }
        return sumval;
    }

    public void Release()
    {
        buffList.Clear();
    }
}
