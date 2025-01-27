using System;
using System.Collections;
using System.Collections.Generic;
#if USE_TT
using TTSDK;
using TTSDK.UNBridgeLib.LitJson;
#endif
using UnityEngine;
using UnityEngine.UI;
using Xaz;
#if USE_WX
using WeChatWASM;
#endif

public class SDKMgr : Singleton<SDKMgr>
{
    private bool isInitSDKFinish = false;
    private bool noHasAuth = false;
    private bool canGetBar = false;
    private bool canUseBar = false;
#if USE_WX
    private string shareImgId = "lR3AoJgaSPW/TulP1K2fBQ==";
    private string shareImgUrl = "https://mmocgame.qpic.cn/wechatgame/OQZTnribAuD8696MNmnTnnTy7qPdE4TRKwiaZmrNkI43vEdLZJjcXCv8sSw7tjOmhg/0";
#endif
#if USE_TT
    private string shareImgUrl = "";
#endif
    public void SetFramesPerSecond()
    {
#if UNITY_EDITOR
        Application.targetFrameRate = 60;
#else
        Application.targetFrameRate = 30;
#endif
        Time.fixedDeltaTime = 0.033f; // 30 次/秒
    }
#if USE_TT
    private void CheckScene()
    {
        TT.CheckScene(TTSideBar.SceneEnum.SideBar, (res) =>
        {
            if (res)
            {
                canUseBar = true;
            }
        }, null, null);
    }

    private void OnAppShow(Dictionary<string, object> param)
    {
        Debug.Log("-----OnAppShow------");
        //屏蔽插屏广告
        //ModuleMgr.AdMgr.PlayFullScreenAd();
        if (param.ContainsKey("scene") && (string)param["scene"] == "021036")
        { 
            Debug.Log("------scene == 021036-------");
            canGetBar = true;
        }
    }

    private void OnAppHide()
    {
        Debug.Log("------OnAppHide-------");
        UpdateOpTime();
        //GameMain.CheckHideSaveGame();
    }

    public void AddSideBar()
    {
        JsonData data = new JsonData();
        data["scene"] = "sidebar";
        TT.NavigateToScene(data,()=>
        {
            Debug.Log("NavigateToScene success");
        },null, null);
    }
#endif
    public IEnumerator InitSDK(Action callback)
    {
#if USE_TT
        TT.InitSDK((code, env) =>
        {
            
            Debug.Log("-------TT.InitSDK success-------" + env.m_HostEnum + ",GetLaunchFrom()==" + env.GetLaunchFrom());
            //https://developer.open-douyin.com/docs/resource/zh-CN/mini-game/develop/guide/game-engine/rd-to-SCgame/c-api/game-share/share-module
            //TT.OnShareAppMessage();
            CheckScene();
            //冷启动监听不到（game.js untiy没有，用别的办法）
            TT.GetAppLifeCycle().OnShow += OnAppShow;
            TT.GetAppLifeCycle().OnHide += OnAppHide;
            //侧边栏
            if (env.GetLaunchFrom() != string.Empty && env.GetLaunchFrom() == "homepage")
            {
                canGetBar = true;
            }
        });
#elif USE_WX
        WX.InitSDK((code) =>
        {
            Debug.Log("-------WX.InitSDK success-------");
            WX.OnShareAppMessage(
                new WXShareAppMessageParam(){
                    imageUrlId= shareImgId,
                    imageUrl = shareImgUrl
                    },null
            ); 
            WX.OnShow((res) =>
            {
                Debug.Log("调用 WX.OnShow" );
                //屏蔽插屏广告
                //ModuleMgr.AdMgr.PlayFullScreenAd();
                if (GetCheckState())
                {
                    OnShareGame();
                }
                OnUserCaptureScreen();
            });
            WX.OnHide((res) =>
            {
                UpdateOpTime();
                Debug.Log("调用 WX.OnHide" );
                //GameMain.CheckHideSaveGame();
            });
        });
#endif
#if !USE_CLOUD
        //获取用户信息
        GetUserInfo();
#else
        isInitSDKFinish = true;
#endif
        yield return new WaitUntil(IsSDKFinish);
        callback();
    }

    private void SetStaticUserInfo()
    {
        Profile.Instance.sdkUserName = "1000001";
        Profile.Instance.avatarUrl = string.Empty;
    }
    public void GetUserInfo()
    {
        SetStaticUserInfo();
        isInitSDKFinish = true;
    }

    public bool IsSDKFinish()
    {
        return isInitSDKFinish;
    }
    public void ShareGame(Action callback = null)
    {
#if UNITY_EDITOR
        if (callback != null)
        {
            callback();
        }
#elif USE_WX
        WX.ShareAppMessage(new ShareAppMessageOption(){
            imageUrlId= shareImgId,
            imageUrl = shareImgUrl
        }); 
        if (callback != null)
        {
            needCheckShare = true;
            shareCallback = callback;
        }
#elif USE_TT
        JsonData shareJson = new JsonData();
        shareJson["channel"] = "";
        shareJson["title"] = "小妖叠叠消";
        shareJson["desc"] = "不用安装，点击就能免费玩~";
        shareJson["imageUrl"] = shareImgUrl;
        Debug.Log($"ShareAppMessageBtnClicked jsonData: {shareJson.ToJson()}");
        TT.ShareAppMessage(shareJson, (data) =>
            {
                if (callback != null)
                {
                    callback();
                }
                Debug.Log($"ShareAppMessage success");
            },
            (errMsg) =>
            {
                Debug.Log($"ShareAppMessage failed: {errMsg}");
            },
            () =>
            {
                Debug.Log($"ShareAppMessage cancel");
            });
#endif
    }
    public void OnUserCaptureScreen()
    {
#if USE_WX
        WX.OnUserCaptureScreen((ret)=>
        {
            Debug.Log("=====OnUserCaptureScreen====" + ret.query);
            WX.ShareAppMessage(new ShareAppMessageOption()
            {
                //imageUrlId = shareImgId,
                imageUrl = ret.query
            });
        });
#endif
    }
#if USE_WX
    private Action shareCallback;
    private bool needCheckShare = false;
    private bool GetCheckState()
    {
        return needCheckShare;
    }
    private void OnShareGame()
    {
        if(shareCallback != null)
        {
            shareCallback();
           // Profile.Instance.user.UpdateShareInfo();
            needCheckShare = false;
        }
    }
#endif
    public bool NoHasAuth()
    {
        return noHasAuth;
    }

    public bool GetCanGetBar()
    {
        return canGetBar;
    }
    public bool GetCanUseBar()
    {
        return canUseBar;
    }
    public void UpdateOpTime()
    {
        Profile.Instance.user.onLineTime = TimeUtil.GetNowInt();
        NetMgr.NetLogin.SendSynUser();
    }

    public void OpenService()
    {
#if USE_WX
        WX.OpenCustomerServiceConversation(new OpenCustomerServiceConversationOption() 
        {
            showMessageCard = false,
            sendMessageTitle = "OpenCustomerServiceConversation",
            success = (res) =>
            {
                Debug.Log("调用 WX.OpenCustomerServiceConversation");

            },
            fail = (res) =>
            {
                Debug.Log("调用失败 WX.OpenCustomerServiceConversation" + res.errMsg);

            }
        });

#endif
    }
    public void DoVibrate(bool islong = false)
    {
#if USE_WX
        if (islong)
        {
            WX.VibrateLong(new VibrateLongOption()
            {
                success = (res) =>
                {
                    Logger.Print("----VibrateLong-----success------");
                },
                fail = (res) =>
                {
                    Logger.Print("----VibrateLong-----fail------");
                }
            });
        }
        else
        {
            WX.VibrateShort(new VibrateShortOption()
            {
                success = (res) =>
                {
                    Logger.Print("----VibrateShort-----success------");
                },
                fail = (res) =>
                {
                    Logger.Print("----VibrateShort-----fail------");
                }
            });
        }
#endif
    }
    public void ReLogin()
    {
#if USE_WX
        Scheduler.Timeout(delegate ()
        {
            WX.RestartMiniProgram(new RestartMiniProgramOption()
            {
                success = (res) => {
                    Logger.Print("----RestartMiniProgram----success-----");
                },
                fail = (res) => {
                    Logger.Print("----RestartMiniProgram----fail-----");
                    UIMgr.ShowFlyTipKey("relogin_error");
                },
            });
        }, 1);
#endif
    }
}
