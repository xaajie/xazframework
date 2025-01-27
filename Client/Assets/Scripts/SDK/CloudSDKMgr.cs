using System;
using System.Collections;
using UnityEngine;
#if USE_WX
using WeChatWASM;
using Xaz;
using LitJson;
#endif

public class CloudSDKMgr : Singleton<CloudSDKMgr>
{
#if USE_CLOUD
    private CloudUserData cloudUserData;
    private bool isInitFinish = false;
    private readonly string envId = "flymo-cloud-server-0daj1af371ac7";
    public IEnumerator InitSDK(Action callback = null)
    {
#if UNITY_EDITOR
        Profile.Instance.sdkUserName = "1000001";
        isInitFinish = true;
#elif USE_WX
        WX.cloud.Init();
        WX.cloud.CallFunction(new CallFunctionParam()
        {
            name = "GetUserInfo",
            data = { },
            success = (res) =>
            {
                Debug.Log("dddddddddddddddddddddd-----"+ res.result);
                var data = JsonMapper.ToObject(res.result);
                if (data.ContainsKey("userInfo"))
                {
                    cloudUserData = new CloudUserData(data["userInfo"]);
                    Profile.Instance.sdkUserName = cloudUserData.uid.ToString() ;
                    Logger.Print("CallGetUserData success", Profile.Instance.sdkUserName);
                    UserRecordData _userRecord = ClientServerCenter.Instance.GetCurUserData(cloudUserData.uid.ToString(),false);
                    if (cloudUserData.fileid != string.Empty)
                    {
                        try
                        {
                            
                            if(_userRecord.timestamp < cloudUserData.timestamp || _userRecord.user.level < cloudUserData.level)
                            {
                                DownloadCloudRecord();
                                
                            }else
                            {
                                Logger.Print("CallGetUserData success 111111but no data");
                                isInitFinish = true;
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError(ex.Message);
                        }
                    }
                    else
                    {
                        Logger.Print("CallGetUserData success 2222222but no data");
                        isInitFinish = true;
                    }
                }
                else
                {
                    Logger.Print("CallGetUserData success 33333333but no data");
                    isInitFinish = true;
                }
                
            },
            fail = (res) =>
            {
                Debug.Log("eeeeeeeeeeeeeeeeee---"+  res.errMsg);
                isInitFinish = true;
            }
        });
#endif
        yield return new WaitUntil(IsFinish);
        if (callback != null)
        {
            callback();
        }
    }
    public void UpLoadUserInfo()
    {
#if USE_WX
        Logger.Print("---UpLoadRecord start-----");
        WX.cloud.CallFunction(new CallFunctionParam()
        {
            name = "UpdateUserInfo",
            data = cloudUserData,
            success = (res) =>
            {
                Logger.Print("UpLoadRecord success", res.result);
                var resData = JsonMapper.ToObject(res.result);
                if (resData.ContainsKey("code"))
                {
                    if ((int)resData["code"] == 1)
                    {
                        Logger.Print("----UpLoadUserInfo----changeID-----");
                        UIMgr.ShowFlyTipKey("cloud_data_error");
                        Scheduler.Timeout(delegate ()
                        {
                            WX.RestartMiniProgram(new RestartMiniProgramOption(){
                                success=(res)=>{
                                    Logger.Print("----RestartMiniProgram----success-----");
                                },
                                fail=(res)=>{
                                    Logger.Print("----RestartMiniProgram----fail-----");
                                    UIMgr.ShowFlyTipKey("cloud_data_error");
                                },
                            });
                        }, 1);
                        
                    }
                }
            }
        });
#endif
    }
    public bool IsFinish()
    {
        return isInitFinish;
    }

    public void UploadCloudRecord(int timestamp)
    {
#if USE_WX
        Logger.Print("UploadCloudRecord----",timestamp);
        string uid = cloudUserData != null ? cloudUserData.uid.ToString() : "111";
        string path = string.Format("{0}{1}.record", RecordUtil.RecordRootPath, Profile.Instance.GetRecordKey(uid));
        string cloudPath = string.Format("{0}{1}.record", "PlayerInfo/", Profile.Instance.GetRecordKey(uid));
        WX.cloud.UploadFile(new UploadFileParam()
        {
            cloudPath = cloudPath,
            filePath = path,
            success = (res) =>
            {
                Logger.Print("UploadFile success", res.fileID);
                cloudUserData.fileid = res.fileID;
                cloudUserData.level = Profile.Instance.user.level;
                cloudUserData.timestamp = timestamp;
                UpLoadUserInfo();
            },
            fail = (res) =>
            {
                Debug.Log("UploadFile--fail-" +  res.errMsg);
                isInitFinish = true;
            }
        }) ;
#endif
    }
    public void DownloadCloudRecord()
    {
        string userkey = Profile.Instance.GetRecordKey( cloudUserData.uid.ToString() );
        RecordUtil.DeleteFile(userkey);
#if USE_WX
        Logger.Print("DownloadCloudRecord----");
        WX.cloud.DownloadFile(new DownloadFileParam()
        {
            cloudPath = "PlayerInfo",
            fileID = cloudUserData.fileid,
            success = (res) =>
            {
                Logger.Print("DownloadFile success", res.tempFilePath, cloudUserData.fileid);
                RecordUtil.CreatPath();
                WX.GetFileSystemManager().SaveFile(new SaveFileOption()
                {
                    tempFilePath = res.tempFilePath,
                    filePath = string.Format("{0}{1}.record", RecordUtil.RecordRootPath, Profile.Instance.GetRecordKey(cloudUserData.uid.ToString())),
                    success = (res) =>
                    {
                        Logger.Print("----save success----",res.savedFilePath);
                        RecordUtil.ReadInitInfo();
                        isInitFinish = true;
                    },
                    fail = (res) =>
                    {
                        Debug.Log("rrrrrrrrrrrrrr---" + res.errMsg);
                    }
                });
               // isInitFinish = true;
            },
            fail = (res) =>
            {
                Debug.Log("DownloadFile--fail--" + res.errMsg);
            }
        });
#endif
    }
#endif
}
public class CloudUserData
{
    public int uid;
    public string openid;
    public string fileid;
    public int timestamp;
    public int level;
#if USE_WX
    public CloudUserData(JsonData data)
    {
        if (data.ContainsKey("uid"))
        {
            uid = ((int)data["uid"]);
            timestamp = ((int)data["timestamp"]);
            level = ((int)data["level"]);
            openid = ((string)data["openid"]);
            fileid = ((string)data["fileid"]);
        }
    }
#endif
}