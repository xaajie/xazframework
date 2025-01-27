//----------------------------------------------------------------------------
//-- 游戏内收益系统
//-- @author xiejie
//----------------------------------------------------------------------------
using System.Collections.Generic;
using UnityEngine;

public class AwardMgr
{
    public AwardMgr()
    {
        
    }

    public List<UserCategoryData> GetCategorys(List<UserChangeItem> addItems)
    {
        List<UserCategoryData> list = new List<UserCategoryData>();
        if (addItems != null && addItems.Count > 0)
        {
            for (int i = 0; i < addItems.Count; i++)
            {
                list.Add(ModuleMgr.CategoryMgr.CreateFromParm(addItems[i].itemType, addItems[i].itemId, addItems[i].num));
            }
        }
        return list;
    }

    //处理游戏内物品变更
    public void ChangeItems(UserChangeItems info)
    {
        if (info.delItems!=null &&info.delItems.Count > 0)
        {
            AwardList(GetCategorys(info.delItems), false);
        }

        if (info.addItems != null && info.addItems.Count > 0)
        {
            AwardList(GetCategorys(info.addItems), true);
        }
        EventMgr.DispatchEvent(EventEnum.CHANGE_CATEGORY);
    }

    public void ChangeCurrency(int itemId, int itemNum, bool isAdd)
    {
        ChangeItem(Const.Category.CURRENCY,itemId, itemNum,isAdd);
        EventMgr.DispatchEvent(EventEnum.CHANGE_CATEGORY);
    }

    public void AwardListone(UserCategoryData awa, bool isAdd)
    {
        ChangeItem(awa, isAdd);
        EventMgr.DispatchEvent(EventEnum.CHANGE_CATEGORY);
    }

    public void AwardList(List<UserCategoryData> list, bool isAdd)
    {
        for (int i = 0; i < list.Count; i++)
        {
            ChangeItem(list[i], isAdd);
        }
        EventMgr.DispatchEvent(EventEnum.CHANGE_CATEGORY);
    }

    public void ChangeItem(UserCategoryData info, bool isAdd)
    {
        ChangeItem(info.itemType, info.itemId, info.itemNum, isAdd);
    }
   private void ChangeItem(Const.Category itemType, int itemId,int itemNum,bool isAdd)
    {
        int numcount = isAdd ? itemNum : -itemNum;
        switch (itemType)
        {
            case Const.Category.CURRENCY:
                if (itemId == (int)Const.CurrencyType.GOLD)
                {
                    Profile.Instance.user.gold = Profile.Instance.user.gold + numcount;
                    Profile.Instance.user.gold = Mathf.Max(0, Profile.Instance.user.gold);
                    if(numcount>0)
                    {
                        ModuleMgr.AchivementMgr.UpdateAchivement(Const.AchivementType.CountGold);
                    }
                }
                else if (itemId == (int)Const.CurrencyType.SKIPAD)
                {
                    Profile.Instance.user.skipad = Profile.Instance.user.skipad + numcount;
                    Profile.Instance.user.skipad = Mathf.Max(0, Profile.Instance.user.skipad);
                }
                else if (itemId == (int)Const.CurrencyType.FISH)
                {
                    Profile.Instance.user.fish = Profile.Instance.user.fish + numcount;
                    Profile.Instance.user.fish = Mathf.Max(0, Profile.Instance.user.fish);
                }
                else if(itemId  == (int)Const.CurrencyType.LVEXP)
                {
                    int prelv = Profile.Instance.user.GetLevel();
                    Profile.Instance.user.Addlvexp(numcount);
                    if (Profile.Instance.user.GetLevel() != prelv)
                    {
                        ModuleMgr.ChallengeMgr.SetCurChallege();
                        UIMgr.Open<UILvup>(uiview => uiview.SetData(Profile.Instance.user.GetLevel()));
                    }
                }
                break;
            case Const.Category.ITEM:
                List<UserBagData> bagInfo = ModuleMgr.BagMgr.GetBagList();
                UserBagData tar = FindBagInfo(itemType,itemId);
                if (tar != null)
                {
                    tar.num = tar.num + numcount;
                }
                else
                {
                    if (isAdd)
                    {
                        UserBagData vt = new UserBagData();
                        vt.itemType = (int)itemType;
                        vt.itemId = itemId;
                        vt.num = numcount;
                        bagInfo.Add(vt);
                    }
                }

                List<UserBagData> bagInfonew = new List<UserBagData>();
                bool isNeedSet = false;
                for (int i = 0; i < bagInfo.Count; i++)
                {
                    if (bagInfo[i].num > 0)
                    {
                        bagInfonew.Add(bagInfo[i]);
                    }
                    else
                    {
                        isNeedSet = true;
                    }
                }
                if (isNeedSet)
                {
                    ModuleMgr.BagMgr.SetBagList(bagInfonew);
                }
                break;
            default:
                
                break;
        }
    }

    private UserBagData FindBagInfo(Const.Category itemType, int itemId)
    {
        List<UserBagData> bagInfo = ModuleMgr.BagMgr.GetBagList();
        for (int i = 0; i < bagInfo.Count; i++)
        {
            if (bagInfo[i].itemType == (int)itemType && bagInfo[i].itemId == itemId)
            {
                return bagInfo[i];
            }
        }
        return null; 
    }

    public void Release()
    {
        
    }
}
