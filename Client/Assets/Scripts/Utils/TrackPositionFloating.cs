using UnityEngine;
/// <summary>
/// 头顶悬浮标专用
/// author :xiejie
/// </summary>
public class TrackPositionFloating : MonoBehaviour
{
    public Transform displayer;
    bool canSync;
    private Vector3 displayOffset = new Vector3(0f, 2f, 0f);
    private Vector3 contOffset = new Vector3(1f, 1f, 0f);
    private Camera worldCamera;
    private Camera uiCamera;
    private RectTransform par;
    void Start()
    {
        canSync = true;
        worldCamera = CameraMgr.Instance.GetMainCam();
        uiCamera = CameraMgr.Instance.GetUICam();
    }
    public void SetTrackTarget(Transform tar)
    {
        par = transform.parent.GetComponent<RectTransform>();
        _syncPosition();
        displayer = tar;
    }
    public void SetTrackTarget(Transform tar, Vector3 Offset)
    {
        displayOffset = Offset;
        SetTrackTarget(tar);
    }

    Vector3 screenPosition;
    Vector2 uiPosition;
    void _syncPosition()
    {
        if (canSync)
        {
            if (displayer == null) return;

            // 将目标的位置转换为屏幕坐标
            screenPosition = worldCamera.WorldToScreenPoint(displayer.position+ displayOffset);
            if (screenPosition.z > 0)
            {
                transform.gameObject.SetActive(true); // 激活UI元素

                // 将屏幕坐标转换为UI相机的坐标
                RectTransformUtility.ScreenPointToLocalPointInRectangle(par, screenPosition, uiCamera, out uiPosition);

                // 更新UI元素的位置
                transform.localPosition = uiPosition;
            }
            else
            {
                transform.gameObject.SetActive(false); // 隐藏UI元素
            }
        }
    }

    public Vector2 GetRectPos(Transform displayer, RectTransform part)
    {
        if (displayer == null || part == null) return Vector2.zero;
        Vector3 screenPosition = CameraMgr.Instance.GetMainCam().WorldToScreenPoint(displayer.position + contOffset);
        // 将屏幕坐标转换为UI相机的坐标
        Vector2 uiPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(part, screenPosition, uiCamera, out uiPosition);

        return  uiPosition;
    }

    void LateUpdate()
    {
        _syncPosition();
    }
}