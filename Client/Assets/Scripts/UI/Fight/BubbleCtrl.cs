using UnityEngine;
using UnityEngine.EventSystems;
using Xaz;

public class BubbleCtrl : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private float floatingSpeed = 1f; // 漂浮速度
    private float floatingAmplitude = 25f; // 漂浮振幅

    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private RectTransform parentRect;
    private Vector3 originalPosition;

    [SerializeField]
    public RectTransform eye;
    private bool IsDraging=false;

    private Vector3 m_offset;
    private bool isShow = false;
    //持续几秒消失
    private int duration = 10;
    private int endTime = -1; 
    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();
        parentRect = transform.parent.GetComponent<RectTransform>();
    }

    public void SetShow(bool enbale)
    {
        if(isShow!= enbale)
        {
            isShow = enbale;
            gameObject.SetActive(isShow);
            if (isShow)
            {
                originalPosition = rectTransform.anchoredPosition;
                endTime = TimeUtil.GetNowInt() + duration;
                rectTransform.anchoredPosition = originalPosition + Vector3.up * Random.Range(0, 30) + Vector3.right * Random.Range(0, 10);
            }
            else
            {
                endTime = -1;
            }
        }
    }
    Vector3 mousePos;
    void Update()
    {
        if (isShow)
        {
            if (!IsDraging)
            {
                float offset = Mathf.Sin(Time.time * floatingSpeed) * floatingAmplitude;
                rectTransform.anchoredPosition = originalPosition + Vector3.up * offset;
                var alpha = Mathf.PingPong(Time.time / 5f, 1);
                canvasGroup.alpha = alpha + 0.3f; // 渐变透明效果
                if (endTime > 0 && TimeUtil.GetNowInt() > endTime)
                {
                    SetShow(false);
                }

                if (Input.GetMouseButtonDown(0))
                {
                    RectTransformUtility.ScreenPointToWorldPointInRectangle(parentRect, new Vector2(Input.mousePosition.x, Input.mousePosition.y), CameraMgr.Instance.GetUICam(), out mousePos);
                    float z;
                    if (mousePos.x > eye.transform.position.x)
                    {
                        z = -Vector3.Angle(Vector3.up, mousePos - eye.transform.position);
                    }
                    else
                    {
                        z = Vector3.Angle(Vector3.up, (mousePos - eye.transform.position));
                    }
                    eye.transform.rotation = Quaternion.Euler(0, 0, z + 180);
                }
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        SetDraggedPosition(eventData);
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        IsDraging = true;
        // 存储点击时的鼠标坐标
        Vector3 tWorldPos;
        //UI屏幕坐标转换为世界坐标
        RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out tWorldPos);
        //计算偏移量   
        m_offset = transform.position - tWorldPos;

        SetDraggedPosition(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        IsDraging = false;
        originalPosition = rectTransform.anchoredPosition;
        // SetDraggedPosition(eventData);
    }

    private void SetDraggedPosition(PointerEventData eventData)
    {
        //存储当前鼠标所在位置
        Vector3 globalMousePos;
        //UI屏幕坐标转换为世界坐标
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out globalMousePos))
        {
            //设置位置及偏移量
            rectTransform.position = globalMousePos + m_offset;
        }
    }
}