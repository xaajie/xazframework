//----------------------------------------------------------------------------
//-- NetChallenge模块协议
//-- @author xiejie
//----------------------------------------------------------------------------
using Xaz;
public class NetChallenge : NetModuleBase
{
    public NetChallenge()
    {
        //this.Register(ProtocolEnum.CHALLENGE_FINISH_BACK, OnChallenge);
        //this.Register(ProtocolEnum.CHALLENGE_TURNINFO_BACK, OnTurnInfo);
        //this.Register(ProtocolEnum.CHALLENGE_GETAWARD_BACK, OnGetAward);
    }

  
    override public void Release()
    {
        base.Release();
    }
}

