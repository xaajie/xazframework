//------------------------------------------------------------
// Xaz Framework
// 相机管理器
// Feedback: qq515688254
//------------------------------------------------------------

using UnityEngine;

public class CameraMgr : MonoSingleton<CameraMgr>
{

    private Camera uiCam;
    private Camera mainCam;

    private CameraController camCtrl;

    public Camera GetUICam()
    {
        if (uiCam == null)
        {
            uiCam = GameObject.Find("UICamera").GetComponent<Camera>();
        }
        return uiCam;
    }

    public void SetFollowCam(CameraController.CameraMode mode,Transform vt,bool autoReset)
    {
        GetCamCtrl().SetCameraMode(mode, vt, autoReset);

    }

    public void ResetCam()
    {
        GetCamCtrl().ResetCam();
    }

    public Camera GetMainCam()
    {
        if (mainCam == null)
        {
            mainCam = Camera.main;
        }
        return mainCam;
    }

    public CameraController GetCamCtrl()
    {
        if (camCtrl == null)
        {
            camCtrl = GetMainCam().GetComponent<CameraController>();
        }
        return camCtrl;
    }

}