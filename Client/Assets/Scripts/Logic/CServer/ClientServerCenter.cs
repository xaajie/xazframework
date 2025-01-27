//------------------------------------------------------------
// Xaz Framework
// 前端协议模拟处理中心
// Feedback: qq515688254
//------------------------------------------------------------
using CatJson;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace Xaz
{
    public class ClientServerCenter : Singleton<ClientServerCenter>
    {
        private UserRecordData _userRecord;
        private string userkey;
        public void Begin()
        {

        }

        public void ClearRecord()
        {
            RecordUtil.Delete(userkey);
            RecordUtil.ReadInitInfo();
            _userRecord = null;
        }
        public UserRecordData GetCurUserData(string loginname, string loginkey, out bool isNew)
        {
            isNew = true;
            if (_userRecord == null)
            {
                userkey = Profile.Instance.GetRecordKey( loginname, loginkey);
                string pt = RecordUtil.Get(userkey);
                if (pt != string.Empty)
                {
                    // _userRecord = JsonCodeUtil.FromJsonCode<UserRecordData>(pt);
                    try
                    {
                        _userRecord = JsonParser.Default.ParseJson<UserRecordData>(pt);
                        isNew = false;
                    }
                    catch (Exception)
                    {
                       
                    }
                }
                if(isNew)
                {
                    _userRecord = new UserRecordData();
                    _userRecord.SetUserData(loginname, loginkey);
                    string json = JsonParser.Default.ToJson(_userRecord); 
                    RecordUtil.Set(userkey, json);
                    isNew = true;
                }
            }
            return _userRecord;
        }

        private void SaveGameRecord()
        {
            string json = JsonParser.Default.ToJson(_userRecord);
            RecordUtil.Set(userkey, json);
        }

        private UserChallengeDataBase GetChallengeInfo(int cid)
        {
            for (int i = 0; i < _userRecord.challengeInfo.Count; i++)
            {
                if (_userRecord.challengeInfo[i].id == cid)
                {
                    return _userRecord.challengeInfo[i];
                }
            }
            return null;
        }
        public void SendProtocolHandler(int vt, INetData data)
        {
            ProtocolEnum protocol = (ProtocolEnum)vt;
            int backProtocol = vt + 1;
            switch (protocol)
            {
                case ProtocolEnum.GM:
                    GMHander(backProtocol,data);
                    break;
                case ProtocolEnum.LOGIN:
                    LoginSend senddat = data as LoginSend;
                    LoginBack res = new LoginBack();
                    bool isnew = false;
                    UserRecordData userRecord = GetCurUserData(senddat.loginId, senddat.loginkey, out isnew);
                    res.record = userRecord;
                    res.isnew = isnew;
                    NetCenter.Instance.OnProtocolReceived(backProtocol, res);
                    break;
                case ProtocolEnum.SYNUSER:
                    SYNUserSend senddatv = data as SYNUserSend;
                    SYNUserSend atb = new SYNUserSend();
                    UserDataBase userdat = new UserDataBase();
                    userdat.SetData(Profile.Instance.user);
                    _userRecord.user = userdat;
                    List<UserChallengeDataBase> challrecord= new List<UserChallengeDataBase>();
                    foreach (UserChallengeDataBase cha in Profile.Instance.challenges.Values)
                    {
                        challrecord.Add(cha);
                    }
                    _userRecord.challengeInfo = challrecord;
                    _userRecord.attrup = Profile.Instance.attrIdLv;
                    List<UserBuffDataBase> buffs = new List<UserBuffDataBase>();
                    Dictionary<int,UserBuffData> runbufss = ModuleMgr.BuffMgr.GetBuffs();
                    foreach (UserBuffData cha in runbufss.Values)
                    {
                        UserBuffDataBase chadv = new UserBuffDataBase();
                        chadv.SetData(cha);
                        buffs.Add(chadv);
                    }
                    _userRecord.buffs = buffs;
                    //成就
                    List<UserAchivementChallengeDataBase> achs = new List<UserAchivementChallengeDataBase>();
                    foreach (int chaId in ModuleMgr.AchivementMgr.userAchives.Keys)
                    {
                        UserAchivementChallengeDataBase dv = new UserAchivementChallengeDataBase();
                        dv.challengeId = chaId;
                        List<UserAchivementData> datas = ModuleMgr.AchivementMgr.userAchives[chaId];
                        dv.datas = new List<UserAchivementDataBase>();
                        for (int i = 0; i < datas.Count; i++)
                        {
                            UserAchivementDataBase vn = new UserAchivementDataBase();
                            vn.SetData(datas[i]);
                            dv.datas.Add(vn);
                        }
                        achs.Add(dv);
                    }
                    _userRecord.achivements = achs;
                    //商店购买记录
                    List<UserShopBuyDataBase> shopbuys = new List<UserShopBuyDataBase>();
                    foreach (UserShopBuyDataBase cha in ModuleMgr.ShopMgr.userShopBuyInfos.Values)
                    {
                        shopbuys.Add(Data.DeepClone<UserShopBuyDataBase>(cha));
                    }
                    _userRecord.shopbuyInfos = shopbuys;
                    //图鉴
                    List<UserBookDataBase> books = new List<UserBookDataBase>();
                    foreach (UserBookDataBase cha in ModuleMgr.BookMgr.books.Values)
                    {
                        UserBookDataBase dat = new UserBookDataBase();
                        dat.SetData(cha);
                        books.Add(Data.DeepClone<UserBookDataBase>(dat));
                    }
                    _userRecord.books = books;
                    SaveGameRecord();
                    NetCenter.Instance.OnProtocolReceived(backProtocol, atb);
                    break;
                default:
                    Debug.Log("no back");
                    break;
            }
        }


        public UserChangeItems AwardAction(UserChangeItems changeItems)
        {
            AwardHandler(changeItems.delItems, false);
            AwardHandler(changeItems.addItems, true);
            return changeItems;
        }

        public void AwardHandler(List<UserChangeItem> target, bool isAdd)
        {
            if(target != null)
            {
                for (int i = 0; i < target.Count; i++)
                {
                    ChangeItem(target[i], isAdd);
                }
            }
        }

        public void ChangeItem(UserChangeItem info, bool isAdd)
        {
            int numcount = isAdd ? info.num : -info.num;
            switch (info.itemType)
            {
                case (int)Const.Category.CURRENCY:
                    if (info.itemId == (int)Const.CurrencyType.GOLD)
                    {
                        _userRecord.user.gold = _userRecord.user.gold + numcount;
                        _userRecord.user.gold = Mathf.Max(0, _userRecord.user.gold);
                    }
                    break;
                case (int)Const.Category.ITEM:
                    AddBagInfo(info, isAdd);
                    break;
                default:

                    break;
            }
        }

        private UserBagDataBase FindBagInfo(UserChangeItem info)
        {
            List<UserBagDataBase> bagInfo = _userRecord.bagInfo;
            for (int i = 0; i < bagInfo.Count; i++)
            {
                if (bagInfo[i].itemType == (int)info.itemType && bagInfo[i].itemId == info.itemId && bagInfo[i].num>0)
                {
                    return bagInfo[i];
                }
            }
            return null;
        }

        public void AddBagInfo(UserChangeItem info,bool isAdd)
        {
            int numcount = isAdd ? info.num : -info.num;
            UserBagDataBase tar = FindBagInfo(info);
            if (tar != null)
            {
                tar.num = tar.num + numcount;
            }
            else
            {
                if (isAdd)
                {
                    UserBagDataBase vt = new UserBagDataBase();
                    vt.itemType = (int)info.itemType;
                    vt.itemId = info.itemId;
                    vt.num = numcount;
                    _userRecord.bagInfo.Add(vt);
                }
            }

            List<UserBagDataBase> bagInfonew = new List<UserBagDataBase>();
            for (int i = 0; i < _userRecord.bagInfo.Count; i++)
            {
                if (_userRecord.bagInfo[i].num > 0)
                {
                    bagInfonew.Add(_userRecord.bagInfo[i]);
                }
            }
            _userRecord.bagInfo = bagInfonew;
        }
        public void GenerAwardServerList(List<UserChangeItem> target, List<string> infostr)
        {
            for (int i = 0; i < infostr.Count; i++)
            {
                if (!string.IsNullOrEmpty(infostr[i]))
                {
                    string[] dat = infostr[i].Split('_');
                    UserChangeItem additem = new UserChangeItem(int.Parse(dat[0]), int.Parse(dat[1]), int.Parse(dat[2]));
                    target.Add(additem);
                }
            }
        }

        public void GMHander(int backProtocol, INetData data)
        {
            GMSend senddat = data as GMSend;
            GMBack res = new GMBack();
            res.changeItems = new UserChangeItems();
            string[] arr = senddat.order.Split(',');
            switch (arr[0])
            {
                case "gold":
                    int val = int.Parse(arr[1]);
                    UserChangeItem restt = new UserChangeItem((int)Const.Category.CURRENCY, (int)Const.CurrencyType.GOLD, Mathf.Abs(val));
                    if (val > 0)
                    {
                        res.changeItems.addItems.Add(restt);
                    }
                    else
                    {
                        res.changeItems.delItems.Add(restt);
                    }
                    break;
                case "add":
                    int itemType = int.Parse(arr[1]);
                    int itemId = int.Parse(arr[2]);
                    int itemNum = int.Parse(arr[3]);
                    UserChangeItem awa = new UserChangeItem(itemType, itemId, Mathf.Abs(itemNum));
                    if (itemNum > 0)
                    {
                        res.changeItems.addItems.Add(awa);
                    }
                    else
                    {
                        res.changeItems.delItems.Add(awa);
                    }
                    break;
                default:
                    break;
            }
            res.user = Data.DeepClone<UserDataBase>(_userRecord.user);
            res.changeItems = AwardAction(res.changeItems);
            NetCenter.Instance.OnProtocolReceived(backProtocol, res);
            SaveGameRecord();
        }

    }
}
