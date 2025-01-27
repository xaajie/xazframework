//----------------------------------------------------------------------------
//-- UserSlotData数据封装
//-- @author xiejie
//----------------------------------------------------------------------------
using System.Collections.Generic;
using Table;

public class UserCustomerOrderData : Data

{
    public int uid;
    private int id;
    public Dictionary<int,int> needProductNum = new Dictionary<int, int>() { };
    public UserCustomerOrderData(int sid,int suid) : base()
    {
        id = sid;
        uid = suid;
        List<int> ratios;
        List<int> products = ModuleMgr.FightMgr.GetOrderProductIdAndRatio(GetInfo().needIem, out ratios);
        UserChallengeShowData curChallengeInfo = ModuleMgr.ChallengeMgr.GetCurChallege();
        for (int i = 0; i < GetInfo().neednum; i++)
        {
            int Inx = MathUtil.GetRandomByWeight(ratios);
            int productId = products[Inx];
            if (needProductNum.ContainsKey(productId))
            {
                needProductNum[productId]++;
            }
            else
            {
                needProductNum[productId] = 1;
            }
        }
    }

    public int GetID()
    {
        return GetInfo().id;
    }

    public int GetOrderCashPrice()
    {
        int sellprice = 0;
        foreach (int productId in needProductNum.Keys)
        {
            sellprice = sellprice + ModuleMgr.FightMgr.GetPricebyProductId(productId) * needProductNum[productId];
        }
        return sellprice;
    }

    private Dictionary<int,UserCategoryData> cashTar = new Dictionary<int,UserCategoryData>();
    public UserCategoryData GetCategory(int productId)
    {
        if (!cashTar.ContainsKey(productId))
        {
            UserCategoryData product = ModuleMgr.CategoryMgr.CreateFromParm((int)Const.Category.PRODUCT, productId, GetNeedProductNum(productId));
            cashTar.Add(productId, product);
        }
        return cashTar[productId];
    }
    public int GetNeedProductNum(int productId)
    {
        int num=0;
        needProductNum.TryGetValue(productId,out num);
        return num;
    }
    public order GetInfo()
    {
        return StaticDataMgr.Instance.orderInfo[id];
    }

    public actor GetGuestInfo()
    {
        if (StaticDataMgr.Instance.actorInfo.ContainsKey(GetInfo().guestId))
        {
            return StaticDataMgr.Instance.actorInfo[GetInfo().guestId];
        }
        else
        {
            Logger.Error(string.Format("订单id{0}客人id错误{1}", id,GetInfo().guestId));
        }
        return null;
    }

    public float GetSpeedVal()
    {
        return (float)GetGuestInfo().walkspeed/100;
    }

    public int GetGuestModelId()
    {
        return GetGuestInfo().modelId;
    }

    public string GetModelPath()
    {
        return StaticDataMgr.Instance.GetModelPath(GetGuestModelId());
    }
   
    //public void AddOrderAward()
    //{
    //    List<UserCategoryData> awa = ModuleMgr.CategoryMgr.CreateFromList(GetInfo().award);
    //    //int txtId = GetNumZoneId(GetCurPutNum());
    //    //float addratio = Constant.NUMARR_RATIO[txtId];
    //    //int addnum = GetCurPutNum() > GetNeedNum() ? GetCurPutNum() - GetNeedNum() : 0;
    //    for (int i = 0; i < awa.Count; i++)
    //    {
    //        //float ratio = float.Parse(GetInfo().addratio[i]);
    //        //awa[i].itemNum = (int)Math.Ceiling(awa[i].itemNum * addratio) + (int)Math.Ceiling(addnum * ratio);
    //        ModuleMgr.AwardMgr.ChangeItem(awa[i], true);
    //    }
    //}
}
