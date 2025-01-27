//----------------------------------------------------------------------------
//-- 
//-- @author xiejie
//----------------------------------------------------------------------------
using System;
using System.Collections.Generic;

[Serializable]
public class UserChallengeDataBase : Data
{
    public int id;
    public int leaveTime;
    public int level;
    public bool clickOpen;
    public List<UserSceneBuildDataBase> builds;
    public List<UserSceneWorkerDataBase> workers;
    public UserScenePlayerDataBase player;
    public List<UserSceneUnlockDataBase> unlocks;
    public List<UserHappeningDataBase> happenings;
}
