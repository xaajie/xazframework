//----------------------------------------------------------------------------
//-- 自定义摇杆
//-- @create 2020-3-10
//-- @author xiejie
//----------------------------------------------------------------------------
using UnityEngine;
using UnityEngine.EventSystems;
using Xaz;
public class GameJoystick : MonoBehaviour, Xaz.IControl, IEndDragHandler, IBeginDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public VoidDelegate OnJoystickBeginDrag;
    public VoidDelegate OnJoystickEndDrag;
    public VoidDragDelegate OnJoystickDrag;

    public delegate void VoidDelegate();
    public delegate void VoidDragDelegate(Vector3 pt, float distance);

    public ScrollCircle scrollTarget;
    public bool isDraging = false;
    private bool isPressed = false;
    public bool enableKeyBoard = true;

    bool isValid = true;
    Vector3 m_contentOrigPos;
    public UIState showState;
    public int showLayer = 5;
    private const string START_STATE = "0";
    private const string DRAGRUN_STATE = "1";
    private const string DRAGWALK_STATE = "2";
    /// <summary>
    /// 慢走响应半径
    /// </summary>
    public float walkRadius = 240;
    /// <summary>
    /// 大圈跑动方向圈
    /// </summary>
    [SerializeField] public GameObject runImg;
    /// <summary>
    /// 小圈慢走方向圈
    /// </summary>
    [SerializeField] public GameObject walkImg;
    [SerializeField] public RectTransform thumbBox;
    //幅度x以上才响应移动，否则仅转向
    private float moveStartOffset = 0;
    public bool IsChangePos;
    private RectTransform rangeRect;

    void Start()
    {
        if (scrollTarget != null)
            m_contentOrigPos = scrollTarget.transform.position;
        rangeRect = scrollTarget.GetComponent<RectTransform>();
        showState.SetState(START_STATE);
    }

    void OnDestroy()
    {
        OnJoystickBeginDrag = null;
        OnJoystickEndDrag = null;
        OnJoystickDrag = null;
    }

    private void SetChangePos(PointerEventData eve)
    {
        if (IsChangePos)
        {
            //存储当前鼠标所在位置
            Vector3 globalMousePos;
            //UI屏幕坐标转换为世界坐标
            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(this.rangeRect, eve.position, eve.pressEventCamera, out globalMousePos))
            {
                //设置位置及偏移量
                thumbBox.position = globalMousePos;
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        SetChangePos(eventData);
        isPressed = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        SetChangePos(eventData);
        BeginDrag();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        EndDrag();
    }

    private void BeginDrag()
    {
        isDraging = true;
        if (OnJoystickBeginDrag != null)
        {
            OnJoystickBeginDrag();
        }
    }

    private void DragIng(Vector3 normalized, float radius = -1f)
    {
        if (OnJoystickDrag != null)
        {
            float rad = 0f;
            if (radius > 0)
            {
                rad = radius;
            }
            else
            {
                rad = Vector3.Distance(scrollTarget.content.localPosition, Vector3.zero);
            }
            OnJoystickDrag(normalized, rad);
            if (rad >= walkRadius)
            {
                showState.SetState(DRAGRUN_STATE);
                RotImgControl(runImg, walkImg, normalized);
            }
            else
            {
                showState.SetState(DRAGWALK_STATE);
                RotImgControl(walkImg, runImg, normalized);
            }
        }
    }

    private void RotImgControl(GameObject rot, GameObject hiderot, Vector3 normalized)
    {
        if (rot != null)
        {
            float angle = 360 - Mathf.Atan2(normalized.x, normalized.y) * Mathf.Rad2Deg;
            rot.transform.eulerAngles = new Vector3(0, 0, angle);
        }
    }
    private void EndDrag()
    {
        isDraging = false;
        showState.SetState(START_STATE);
        ResetUIShow();
        if (OnJoystickEndDrag != null)
        {
            OnJoystickEndDrag();
            
        }
    }

    bool isDragKeyDown()
    {
        return Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D);
    }

    private void ResetUIShow()
    {
        if (scrollTarget != null)
        {
            scrollTarget.ResetComp();
            scrollTarget.transform.position = m_contentOrigPos;
        }
    }
    /// <summary>
    /// 暂时中断摇杆
    /// </summary>
    public void InterruptReset()
    {

    }

    /// <summary>
    /// 重置摇杆
    /// </summary>
    public void StopReset()
    {
        EndDrag();
    }

    public void SetValid(bool valid)
    {
        isValid = valid;
    }

    private void DragAction()
    {
        if (isDraging && scrollTarget)
        {
            // 大到一定程度才设置，否则接近0的时候算出来的方向不对
            //if (content.content.localPosition.sqrMagnitude >= 1e-6)
            //Debug.Log(content.content.localPosition.sqrMagnitude);
            //Debug.Log(Time.time + "ssssssssssss"+startTime);
            if (scrollTarget.content.localPosition.sqrMagnitude >= moveStartOffset)
            {
                DragIng(scrollTarget.content.localPosition.normalized);
            }
        }
    }
    Vector3 postion;
    void Update()
    {
        if (!isValid)
            return;
#if UNITY_EDITOR || UNITY_STANDALONE
        if (isDraging)
        {
            postion = Vector3.zero;
            if (Input.GetKey(KeyCode.W))
            {
                postion.y = 1f;
            }
            if (Input.GetKey(KeyCode.S))
            {
                postion.y = -1f;
            }
            if (Input.GetKey(KeyCode.A))
            {
                postion.x = -1f;
            }
            if (Input.GetKey(KeyCode.D))
            {
                postion.x = 1f;
            }

            if (postion != Vector3.zero)
            {
                DragIng(postion, walkRadius);
            }
            else
            {
                if (!isDragKeyDown() && !isPressed)
                {
                    EndDrag();
                }
                else
                {
                    DragAction();
                }
            }
        }
        else
        {
            if (isDragKeyDown() && enableKeyBoard && this.gameObject.layer == showLayer)
            {
                BeginDrag();
            }
        }
#else
        DragAction();
#endif
    }
}
