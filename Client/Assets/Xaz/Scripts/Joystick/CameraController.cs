using Cinemachine;
using DigitalRubyShared;
using UnityEngine;
/// <summary>
/// 拍照相机控制脚本
/// addby xiejie
/// </summary>
public class CameraController : MonoBehaviour
{
    public enum CameraMode
    {
        None = 0,
        MAIN,
        FOUCUS,
    }

    public bool canMoveCamera = false;
    public bool isTouchControl = true;

    [SerializeField]
    public CinemachineVirtualCamera mainLook;

    [SerializeField]
    public CinemachineVirtualCamera foucusLook;

    //视距配置
    private int[] FovLen = new int[] { 4, 12 };
    private float defautCurFov = 9;
    private float curFov =0;

    /// <summary>
    /// 相机平移速度....
    /// </summary>
#if UNITY_EDITOR
    protected float camPanSpeed = 4f;
#else
    protected float camPanSpeed = 20f;
#endif
    protected Vector3 moveVelocity = Vector3.zero;
    protected float zoomVelocity = 0f;
    protected float camZoomSpeed = 0.3f;
    protected float dampening = 0.75f;
    private CameraMode curmode;

    void Awake()
    {
        AddTouchGestureListener();
        ResetCam();
    }

    private void OnDestroy()
    {
       // RemoveTouchGestureListener();
    }

    public Transform GetFollowTarget()
    {
        return mainLook.Follow.transform;
    }

    public void SetFollowTarget(Transform tar)
    {
        mainLook.Follow = tar;
    }

    public void SetCameraMode(CameraMode vt, Transform tar,bool autoReset)
    {
        switch (vt)
        {
            case CameraMode.MAIN:
                if (curmode != vt)
                {
                    Utils.SetActive(mainLook.gameObject, true);
                    Utils.SetActive(foucusLook.gameObject, false);
                }
                break;
            case CameraMode.FOUCUS:
                if (curmode != vt || foucusLook.LookAt!= tar)
                {
                    foucusLook.Follow = tar;
                    foucusLook.LookAt = tar;
                    Utils.SetActive(foucusLook.gameObject, true);
                    Utils.SetActive(mainLook.gameObject, false);
                }
                break;
            default:
                break;
        }
        IsNeedCheckAutoReset = autoReset;
        if (IsNeedCheckAutoReset)
        {
            resetCamTime = Time.time + Const.CamerFocusTime;
        }
        curmode = vt;
    }

    void LateUpdate()
    {
        if (canMoveCamera)
        {
            GetFollowTarget().Translate(moveVelocity, Space.World);
            moveVelocity *= dampening;
           // ConnerCheck();
        }
        if (IsNeedCheckAutoReset)
        {
            if (resetCamTime>0 && Time.time >= resetCamTime)
            {
                ResetCam();
                IsNeedCheckAutoReset = false;
                resetCamTime = -1;
            }
        }
    }

    private bool IsNeedCheckAutoReset = false;
    private float resetCamTime = 0;
    
    public void ResetCam()
    {
        SetCameraMode(CameraMode.MAIN,null,false);
        isTouchControl = false;
        moveVelocity = Vector3.zero;
        zoomVelocity = 0f;
        mainLook.m_Lens.OrthographicSize = defautCurFov;
        curFov = defautCurFov;
        EventMgr.DispatchEvent(EventEnum.Cam_FINISH);
    }


    private void SetLensFieldView(float angle)
    {
        if (mainLook != null)
        {
            //Logger.Print("111111111111111", Mathf.Clamp(mainLook.m_Lens.FieldOfView + angle, minFov, maxFov));
            SetFovVal(Mathf.Clamp(mainLook.m_Lens.OrthographicSize + angle, FovLen[0], FovLen[1]));
        }
    }

    public float GetInitFovSlider()
    {
        return (defautCurFov - FovLen[0]) / (FovLen[1] - FovLen[0]);
    }
    public void SetFovSlider(float val)
    {
        float vt = val * (FovLen[1] - FovLen[0]) + FovLen[0];
        SetFovVal(vt);
    }
    public void SetFovVal(float val)
    {
        mainLook.m_Lens.OrthographicSize = val;
        curFov = val;
    }

    public void OnPanMove(float DeltaX, float DeltaY)
    {
        if (canMoveCamera)
        {
            Quaternion quaternion = transform.rotation;
            quaternion = Quaternion.Euler(0.0f, quaternion.eulerAngles.y, 0.0f);
            moveVelocity += (quaternion * Vector3.left * DeltaX) * Time.deltaTime * camPanSpeed;
            moveVelocity += (quaternion * Vector3.back * DeltaY) * Time.deltaTime * camPanSpeed;
        }
    }

    private ScaleGestureRecognizer m_Recognizer;
    /// <summary>
    /// 添加手势监听
    /// </summary>
    public void AddTouchGestureListener()
    {
        m_Recognizer = new ScaleGestureRecognizer();
        m_Recognizer.StateUpdated += OnScaleGestureUpdated;
        FingersScript.Instance.AddGesture(m_Recognizer);
        isTouchControl = true;
    }

    /// <summary>
    /// 移除手势监听
    /// </summary>
    public void RemoveTouchGestureListener()
    {
        FingersScript.Instance.RemoveGesture(m_Recognizer);
    }

    private void OnScaleGestureUpdated(GestureRecognizer gesture)
    {
        if (isTouchControl && gesture.State == GestureRecognizerState.Executing)
        {
            SetLensFieldView((1 - (gesture as ScaleGestureRecognizer).ScaleMultiplierRange) * camZoomSpeed);
        }
    }

    //private Rect bounds = new Rect(0, 0, Screen.width / 2, Screen.height);

    //public void SetTerrainInfo(float x, float z, float width, float height, float roation)
    //{
    //    bounds.Set(x, z, width, height);
    //}

    /// <summary>
    /// 相机边缘检查
    /// </summary>
    /// 
    //Vector3 pos;
    //private void ConnerCheck()
    //{
    //    pos = mainLook.Follow.transform.position;
    //    if (pos.x < bounds.min.x)
    //    {
    //        pos.x = bounds.min.x;
    //    }
    //    if (pos.x > bounds.max.x)
    //    {
    //        pos.x = bounds.max.x;
    //    }
    //    if (pos.z < bounds.min.y)
    //    {
    //        pos.z = bounds.min.y;
    //    }
    //    if (pos.z > bounds.max.y)
    //    {
    //        pos.z = bounds.max.y;
    //    }
    //    mainLook.Follow.transform.position = pos;
    //}
}
