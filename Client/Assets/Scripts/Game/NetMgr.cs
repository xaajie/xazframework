//------------------------------------------------------------
// Xaz Framework
// 模块网络协议管理
//------------------------------------------------------------
public static class NetMgr
{
    public static NetLogin NetLogin;
    public static NetChallenge NetChallenge;
    public static NetShop NetShop;

    public static void Init()
    {
        NetLogin = new NetLogin();
        NetChallenge = new NetChallenge();
        NetShop = new NetShop();
    }

    // Update is called once per frame
    public static void Release()
    {
        NetLogin.Release();
        NetChallenge.Release();
        NetShop.Release();
    }
}
