//----------------------------------------------------------------------------
//-- UserAttrupDataBase
//-- @author xiejie
//----------------------------------------------------------------------------
using System;
using System.Collections.Generic;

[Serializable]
public class UserAchivementChallengeDataBase : Data
{
    public int challengeId;
    public List<UserAchivementDataBase> datas;
}
