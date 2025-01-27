//------------------------------------------------------------
// 游戏数据和框架音频管理器的桥接器
//------------------------------------------------------------
using System.Collections.Generic;
using Table;
using UnityEngine;
using Xaz;

public class EffectMgr : Singleton<EffectMgr>
{
    private const string effkey = "Prefabs/Effect/";
    private const int palyInv = 3;

    private Dictionary<string, GameObject> effectLoopDic = new Dictionary<string, GameObject>();
    public void Init()
    {

    }

    public string  PlayEffectPos(string effectName, Vector3 pos, Transform parm,bool autoDel=true)
    {
        string cashKey = TimeUtil.GetNowInt().ToString();
        string key = effkey + effectName;
        GameObjectPool.Instance.CreateObject(key, (ass) =>
        {
            ass.gameObject.SetActive(true);
            ass.transform.SetParent(parm);
            ass.transform.position = pos;
            //ass.transform.localPosition = Vector3.zero + Vector3.up * 60;
            if (autoDel)
            {
                GameObjectPool.Instance.DelayDestory(ass, palyInv);
            }
            else
            {
                effectLoopDic.Add(cashKey, ass);
            }
        });
        return cashKey;
    }

    public void DelEffect(string cashKey)
    {
        if (effectLoopDic.ContainsKey(cashKey))
        {
            GameObjectPool.Instance.DelayDestory(effectLoopDic[cashKey], 0);
            effectLoopDic.Remove(cashKey);
        }
    }

    public string PlayEffect(string effectName, Transform parm, bool autoDel = true)
    {
        string cashKey = TimeUtil.GetNowInt().ToString();
        string key = effkey + effectName;
        GameObjectPool.Instance.CreateObject(key, (ass) =>
        {
            ass.gameObject.SetActive(true);
            ass.transform.SetParent(parm);
            ass.transform.localPosition = Vector3.zero;
            //ass.transform.localPosition = Vector3.zero + Vector3.up * 60;
            if (autoDel)
            {
                GameObjectPool.Instance.DelayDestory(ass, palyInv);
            }
            else
            {
                effectLoopDic.Add(cashKey, ass);
            }

        });
        return cashKey;
    }

    public void PlayFlyGold()
    {

        //GoldTargetMove pmove = cell.Gold.GetComponent<GoldTargetMove>();
        //            if (dat.box.itemType == Const.Category.CURRENCY)
        //            {
        //                pmove.transform.position = tablecell.transform.position;
        //                pmove.Play(dat.box);
        //                AudioMgr.Instance.Play(AudioEnum.awa);
        //            }
    }
}
