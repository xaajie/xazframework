//----------------------------------------------------------------------------
//-- UserDataBase
//-- @author xiejie
//----------------------------------------------------------------------------

using System;
using System.Collections.Generic;


[Serializable]
public class UserDataBase : Data
{
    public string name;
    public string key;

    public int gold;

    //当前用户
    public int level;
    //当前等级经验（每次升级扣除）
    public int lvexp;

    public int skipad;

    public int fish;

    public bool musicOpen;
    public bool soundOpen;

    public int curChallengeId;
    public int noviceId;

    public bool closeshake;

    public int onLineTime;
    public int onLineAwardTime; //已积累的奖励时长
    public UserDataBase():base()
    {
            
    }
}
