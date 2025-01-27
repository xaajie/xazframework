//----------------------------------------------------------------------------
//-- 新手静态常量定义
//-- @author xz
//----------------------------------------------------------------------------

using UnityEngine;

public static class NoviceConst
{
    public readonly static bool OpenNovice = true;
    public readonly static int Novice_StartId = 10001;

    public enum NoviceType
    {
        UI = 1,
        UnLock = 2,
        Joystick = 3,
        Building = 4,
    }

    public enum NoviceID
    {
        OpenJoystick = 10001,
        OpenCash = 10002,
        OpenCasher = 10003,
        OpenAppleGui = 10004,
        OpenAppleTree = 10005,
        AddAppleTree = 10006,
        AddAppleGui = 10007,
        GetGash = 10008,
        OpenAppleTree2 = 10009,
        UpAppleTree1 = 10010,
        UpAppleTree2 = 10011,
        OpenAppleJiang = 10012,
        OpenAppleJiangGui = 10013,
        PdAppJiang1 = 10014,
        PdAppJiang2 = 10015,
        AddAppJiang1 = 10016,
        AddAppJiang2 = 10017,
        RepairAppJiang = 10018
    }
}

