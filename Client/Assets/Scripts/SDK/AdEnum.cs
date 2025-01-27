//----------------------------------------------------------------------------
//-- 广告类型
//-- @author xiejie
//----------------------------------------------------------------------------
public static class AdEnum
{
    public const string RewardId = "adunit-374d2187327ee547"; 
    public const string BannerId = "adunit-5e748c0826ff026e"; 
    public const string InterstitialId = "adunit-6c11d24268aed741";

    //激励广告的类型
    public enum AdType
    {
        Banner = 0,
        Interstitial = 1,
        Reward_Shop = 2,
        Reward_Achivement = 3,
        Reward_Lvup = 4,
        Reward_Attrup = 5,
        Reward_Buildingup = 6,
        Reward_Wakeup = 7,
        Reward_Adgold = 8,
        Reward_Adhappen=9,
        Reward_Offline = 10,
    }
}

