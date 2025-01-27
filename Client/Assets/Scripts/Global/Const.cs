//----------------------------------------------------------------------------
//-- 游戏逻辑静态常量定义
//-- @author xiejie
//----------------------------------------------------------------------------

using UnityEngine;

public static class Const
{
    public static string nullStr = "";
    public static string cardbgpre = "img_card_";
    public static string cardbgatlas = "IconSign";
    public readonly static string[] COLORS_STR = new string[] { "FFFFFF", "30E818", "1e90ff", "F969F7", "FF4500" };

    public static int SlotCellSizeX = 152;
    public static int SlotCellSizeY = 240;
    public static Vector3 OffsetRankX = new Vector3(-302, 202, 0);
    public static Vector3 OffsetRankX2 = new Vector3(-215, 202, 0);
    public static float InvFlyTime = 0.3f;
    public static float CamerFocusTime = 2f;
    public readonly static int Weight_Ex = 100;

    public static float FixWakeupDuration = 0.8f;
    public static float GuardBarDuration = 1800f;
    public enum CheckTipType
    {
        No=0,
        Fly=1,
        Alert=2
    }

    public static string AtlasChallenge= "Icon";
    public static string AtlasIcon = "Icon";
    public static string AtlasFace = "Scene";
    public static string AtlasBuild = "Scene";
    public enum Category
    {
        //货币属性表currency
        CURRENCY = 1,
        //功能建筑表build
        BUILD = 3,
        //角色表actor
        ACTOR = 4,
        //产物表
        PRODUCT=2,
        //物品表item
        ITEM = 5,

    }

    public enum BuffState
    {
        Runing,
        Finish,
    }
    public enum BuffType
    {
        Attr=1,
        Woker=2,
    }
    public enum CurrencyType 
    {
        Null = 0,
        LVEXP = 1,
        FISH = 3,
        GOLD = 2,
        SKIPAD=4,
    }

    public enum MainShow 
    {
        Bottom = 1, 
        Left = 2,
        Right = 3, 
    }

    public enum GoldAwaEffect
    {
        flytipgold = 1,
        onlytip = 2,
        onlygold = 3
    }

    public enum MainViewID
    {
        main = 0,
        map= 1,
        Achivement = 2,
        attrup=3,
        buildup=4,
        fashion=5,
        lay=6,
        shop=7,
    }

    public enum BuildType
    {
        All = -1,
        Tree = 1,
        Shelf = 2,
        CashDesk=3,
        Machine=4,
    }

    public enum StackEnum
    {
        Get = 1,
        Put = 2,
        None = 3,
    }

    public enum BuildState
    {
        //任意状态
        Every,
        //产物满
        ProductFull,
        //原料不空
        RawNotFull,
        //产物未满
        ProductNotFull,
        //坏掉
        Broken,
        //产物不空
        ProductNotEmpty,
    }
    public enum ScenePoint
    {
        Born = 1,
        Exit = 2,
        Trash = 3,
        GuardBorn=4
    }
    public enum ActorType
    {
        Player= 101,
        Worker_common=201,
        Worker_machine=202,
        Worker_ranch=203,
        Customer=301,
        Guard = 401,
        CashWorker=501,
    }

    public enum ActorSumType
    {
        Player = 1,
        Worker = 2,
        Customer = 3,
    }

    public enum ActorStep
    {
        None = 0,
        Get = 1,
        Cash = 2,
        CashNow=3,
        Exit = 4,
        Sleep=5,
        Wakeup=6,

    }

    public enum AttrCountType
    {
        Add = 1,
        Percent = 2,
    }

    public enum NoticeType
    {
        None = 0,
        Sleep = 1,
        Broken = 2,
    }

    public enum HappenType
    {
        None = 0,
        Money = 1,
        GetBuff=2,
    }

    public enum AchivementType
    {
        UnlockupBuild = 1,
        FinishOrder = 2,
        UnlockupBuildType = 3,
        Attrup=4,
        CountGold=5,
        CountActor=6,
        OpenChallenge=7,
    }

    public enum GotoType
    {
        None=0,
        OpenUI=1,
        MoveCam=2,
        ReturnMainUI=3,
    }
}

