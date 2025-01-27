using System.Collections.Generic;
using System.Security.Cryptography;
using Xaz;
public class NetLogin : NetModuleBase
{
    public NetLogin()
    {
        this.Register(ProtocolEnum.LOGIN_BACK, OnLoginCallBack);
        this.Register(ProtocolEnum.SYNUSER_BACK, OnSynUser);
        this.Register(ProtocolEnum.GM_BACK, OnGMCallBack);
    }
    public void SendGMCallBack(string order)
    {
        this.Request(ProtocolEnum.GM, new GMSend(order));
    }

    private void OnGMCallBack(INetData vt)
    {
        GMBack res = vt as GMBack;
        ModuleMgr.AwardMgr.ChangeItems(res.changeItems);
    }

    public void SendLogin(string id, string key, string avatarUrl)
    {
        this.Request(ProtocolEnum.LOGIN, new LoginSend(id, key, avatarUrl));
    }
    private void OnLoginCallBack(INetData vt)
    {
        LoginBack res = vt as LoginBack;
        Profile.Instance.user.SetData(res.record.user);
        Profile.Instance.SetChallenge(res.record.challengeInfo);
        Profile.Instance.SetAttrupList(res.record.attrup);
        ModuleMgr.AchivementMgr.SetInfo(res.record.achivements);
        ModuleMgr.BagMgr.SetBagInfo(res.record.bagInfo);
        ModuleMgr.BookMgr.SetDataList(res.record.books);
        ModuleMgr.ShopMgr.SetShopBuyList(res.record.shopbuyInfos);
        ModuleMgr.BuffMgr.SetBuffData(res.record.buffs);
        GameWorld.Instance.StartGame();
    }

    public void SendSynUser()
    {
        this.Request(ProtocolEnum.SYNUSER, new SYNUserSend());
    }

    private void OnSynUser(INetData vt)
    {

    }

    override public void Release()
    {
        base.Release();
    }
}



