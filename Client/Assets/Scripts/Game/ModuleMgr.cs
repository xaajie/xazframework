//------------------------------------------------------------
// Xaz Framework
// 模块管理
// Feedback: qq515688254
//------------------------------------------------------------
using UnityEngine.UIElements;

public static class ModuleMgr
{
    public static LoginMgr LoginMgr;
    public static ChallengeMgr ChallengeMgr;
    public static CategoryMgr CategoryMgr;
    public static MainMgr MainMgr;
    public static AwardMgr AwardMgr;
    public static BagMgr BagMgr;
    public static FightMgr FightMgr;
    public static AdMgr AdMgr;
    public static BookMgr BookMgr;
    public static NoviceMgr NoviceMgr;
    public static BuildMgr BuildMgr;
    public static AttrMgr AttrMgr;
    public static ShopMgr ShopMgr;
    public static AchivementMgr AchivementMgr;
    public static HappeningMgr HappeningMgr;
    public static BuffMgr BuffMgr;
    public static void Init()
    {
        LoginMgr = new LoginMgr();
        ChallengeMgr = new ChallengeMgr();
        CategoryMgr = new CategoryMgr();
        MainMgr = new MainMgr();
        AwardMgr = new AwardMgr();
        BagMgr = new BagMgr();
        FightMgr = new FightMgr();
        AdMgr = new AdMgr();
        BookMgr = new BookMgr();
        NoviceMgr = new NoviceMgr();
        BuildMgr = new BuildMgr();
        AttrMgr = new AttrMgr();
        ShopMgr = new ShopMgr();
        AchivementMgr = new AchivementMgr();
        HappeningMgr = new HappeningMgr();
        BuffMgr = new BuffMgr();
    }

    // Update is called once per frame
    public static void Release()
    {
        LoginMgr.Release();
        BuffMgr.Release();
        ChallengeMgr.Release();
        CategoryMgr.Release();
        MainMgr.Release();
        AwardMgr.Release();
        BagMgr.Release();
        FightMgr.Release();
        AdMgr.Release();
        BookMgr.Release();
        NoviceMgr.Release();
        BuildMgr.Release();
        AttrMgr.Release();
        ShopMgr.Release();
        AchivementMgr.Release();
        HappeningMgr.Release();
    }
}
