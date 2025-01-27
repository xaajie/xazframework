//----------------------------------------------------------------------------
//-- 协议号定义
//协议内容定义
//-- @author xiejie
//----------------------------------------------------------------------------
using System.Collections.Generic;
using Xaz;

public enum ProtocolEnum
{
    //登录LoginSend
    LOGIN = 0,
    LOGIN_BACK = 1,

    GM,
    GM_BACK,

    SYNUSER,
    SYNUSER_BACK,
    //商城-购买BoxBuySend
    SHOP_BUY,
    SHOP_BUY_BACK,
}


public enum ErrorCode
{
    OK = 0,
    FAIL = -1,
}

public class GMSend : INetData
{
    public GMSend(string info)
    {
        order = info;
    }
    public string order;
}

public class GMBack : INetData
{
    //大类变更内容
    public UserChangeItems changeItems;
    public UserDataBase user;
}
public class ChallengGetAwardSend : INetData
{
    public int roundId;
}
public class ChallengGetAwardBack : INetData
{
    public UserChangeItems changeItems;
    public int roundId;
}

public class ChallengSend : INetData
{
    public ChallengSend(UserChallengeShowData frominfo)
    {
        info = new UserChallengeDataBase();
        info.id = frominfo.id;
    }

    public UserChallengeDataBase info;
}

public class ChallengBack : INetData
{
    public string loginkey;
    //public UserDataBase user;
    public UserChallengeDataBase userchallenge;

    public UserChangeItems changeItems;
}

public class SYNUserSend : INetData
{
    public SYNUserSend()
    {
    
    }
}
public class LoginSend : INetData
{
    public LoginSend(string id, string key, string avatarUrl)
    {
        loginId = id;
        loginkey = key;
        loginAvatarUrl = avatarUrl;
    }
    public string loginId;
    public string loginkey;
    public string loginAvatarUrl;
}

public class LoginBack : INetData
{
    public string loginkey;
    public bool isnew;
    public UserRecordData record;
}

public class moniSYNBack : INetData
{
    public UserDataBase user;
    public List<UserChallengeDataBase> userchallenge;
}