using Xaz;

public class LoginMgr
{
    public LoginMgr()
    {
        
    }


    public void EnterLogin()
    {
        NetMgr.NetLogin.SendLogin(Profile.Instance.sdkUserName, Profile.Instance.password, Profile.Instance.avatarUrl);
    }

    public void OutLogin()
    {
        GameWorld.Instance.ReloadGame();
    }
    // Update is called once per frame
    public void Release()
    {
        
    }
}
