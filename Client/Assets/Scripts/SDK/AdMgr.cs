//----------------------------------------------------------------------------
//-- 广告模块管理
//-- @author xiangzheng
//----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using Xaz;
#if USE_WX
using WeChatWASM;
#endif

public class AdMgr
{
#if USE_WX
    private WXRewardedVideoAd rewardAd;
    private WXCustomAd bannerAd;
    private WXInterstitialAd fullScreenAd;
#endif
    public delegate void AdDelegate(AdEnum.AdType adType);
    private Dictionary<int, int[]> rewardADRecord = new Dictionary<int, int[]>();
    public AdMgr()
    {
        
    }

    public void ClickAd(AdEnum.AdType adType, AdDelegate callbck)
    {
        UIMgr.Open<UIAdBg>();
#if USE_AD
#if USE_WX
        PlayRewardAD(adType, callbck);
        Scheduler.Timeout(delegate ()
        {
            UIMgr.Close<UIAdBg>();
        }, 3f);
#endif
#else
        Scheduler.Timeout(delegate ()
        {
            UIMgr.Close<UIAdBg>();
            if (callbck != null)
            {
                callbck(adType);
            }
        }, 1);
#endif
    }

    private bool showSettingBanner = false;
    public void PlayBannerAD()
    {
#if USE_WX
        showSettingBanner = true;
        if (bannerAd == null)
        {
            var windowWidth = WX.GetSystemInfoSync().windowWidth;
            var windowHeight = WX.GetSystemInfoSync().windowHeight;
            var targetBannerAdWidth = Math.Min(windowWidth, 350);
            bannerAd = WX.CreateCustomAd(new WXCreateCustomAdParam()
            {
                adUnitId = AdEnum.BannerId,
                adIntervals = 30,
                style = new CustomStyle()
                {
                    width = (int)windowWidth,
                    top = (int)(windowHeight - 150),
                    left = 0
                }
            });
        }
        bannerAd.Show((res) =>
        {
            if (!showSettingBanner)
            {
                bannerAd.Hide();
            }

        }, (res) =>
        {
            UIMgr.ShowFlyTipKey("ad_error");
            showSettingBanner = false;
        });
        bannerAd.OnError(err => {
            UIMgr.ShowFlyTipKey("ad_error");
            showSettingBanner = false;
        });
#endif
    }
    public void CloseBannerAD()
    {
#if USE_WX
        showSettingBanner = false;
        if (bannerAd != null)
        {
            bannerAd.Hide();
        }
#endif
    }
    public void PlayFullScreenAd()
    {
#if USE_WX
        if (fullScreenAd == null)
        {
            fullScreenAd = WX.CreateInterstitialAd(new WXCreateInterstitialAdParam()
            {
                adUnitId = AdEnum.InterstitialId,
            });
        }
        fullScreenAd.Show();
        fullScreenAd.OnError(err => {
            UIMgr.ShowFlyTipKey("ad_error");
        });
#endif
    }
    
    private void PlayRewardAD(AdEnum.AdType adType, AdDelegate callbck)
    {
#if USE_WX
        if (rewardAd == null)
        {
            rewardAd = WX.CreateRewardedVideoAd(new WXCreateRewardedVideoAdParam()
            {
                adUnitId = AdEnum.RewardId,
                multiton = false
            }) ;
        }
        rewardAd.Show(
            (res) =>
            {
                ChangeRewardADRecord((int)(adType), false);
            },
            (res) =>
            {
                rewardAd.Load(
                    (res) =>
                    {
                        ChangeRewardADRecord((int)(adType), false);
                        rewardAd.Show(
                            null, (res) =>
                            {
                                UIMgr.ShowFlyTipKey("ad_reward_error");
                            });
                    },
                    (res) =>
                    {
                        UIMgr.ShowFlyTipKey("ad_reward_error");
                    }
               );
            });
        rewardAd.onCloseAction = (res) =>
        {
            rewardAd.OffClose(rewardAd.onCloseAction);
            if (res != null && res.isEnded)
            {
                if (callbck != null)
                {
                    ChangeRewardADRecord((int)(adType), true);
                    callbck(adType);
                }
            }
        };
#endif
    }
    public void Release()
    {
        
    }
    private void ChangeRewardADRecord(int adtype, bool playEnd)
    {
        if (!rewardADRecord.ContainsKey(adtype))
        {
            rewardADRecord.Add(adtype, new int[2]);
        }
        if (playEnd)
        {
            rewardADRecord[adtype][1] += 1;
        }
        else
        {
            rewardADRecord[adtype][0] += 1;
        }
    }
}

