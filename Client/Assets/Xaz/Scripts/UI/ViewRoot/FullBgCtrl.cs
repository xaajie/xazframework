//------------------------------------------------------------
// 全屏背景处理，需要适应有刘海屏拉伸的情况
//------------------------------------------------------------
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class FullBgCtrl : MonoBehaviour
{
    private RectTransform fullRect;
    private Vector2 initSize;
    private RectTransform rectTransform;


    public float rawSetWidth = -1f;
    public float rawSetHeight = -1f;
    //图片原始尺寸
    private float picRawWidth = 750f;
    private float picRawHeight = 1334f;
    //是否自动设置
    //如果是动态加载图，关闭自动设置，异步加载完背景图代码调用SetSizeRefresh
    public bool autoSet = true;
    // Start is called before the first frame update
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }
    void Start()
    {
        //rectTransform = GetComponent<RectTransform>();
        initSize = new Vector2(rectTransform.sizeDelta.x, rectTransform.sizeDelta.y);
        if (autoSet)
        {
            ///自定义分辨率的情况下  走这个流程   比如    rawSetWidth = 1920   rawSetHeight = 1080   如果没有特殊诉求不填参数  走默认 2460fX1080f
            if (rawSetWidth > 0)
            {
                picRawWidth = rawSetWidth;
            }
            if (rawSetHeight > 0)
            {
                picRawHeight = rawSetHeight;
            }
            RefreshSize();
        }
    }

    void OnDestroy()
    {
        if (rectTransform != null)
        {
            rectTransform.sizeDelta = initSize;
        }
    }

    /// <summary>
    /// 动态赋值的背景图拉出安全区外的处理
    /// 地图界面所需
    /// </summary>
    /// <param name="width"></param>
    /// <param name=""></param>
    /// <param name=""></param>
    public void SetSizeRefresh(float width, float height)
    {
        picRawWidth = width;
        picRawHeight = height;
        RefreshSize();
    }
    public void RefreshSize()
    {
        Canvas targetCanvas = GameMain.GetCanvasObj().GetComponent<Canvas>();
        if (targetCanvas == null)
        {
            return;
        }
        fullRect = targetCanvas.GetComponent<RectTransform>();
        Vector2 canvasSize = fullRect.sizeDelta;
        float aspectRatio = (float)Screen.width / (float)Screen.height;
        float targetAspectRatio = picRawWidth / picRawHeight;
        if (aspectRatio >= targetAspectRatio)
        {
            float width = canvasSize.y * aspectRatio;
            float height = width / targetAspectRatio;
            rectTransform.sizeDelta = new Vector2(width, height);
        }
        else
        {
            float height = canvasSize.x / aspectRatio;
            float width = height * targetAspectRatio;
            rectTransform.sizeDelta = new Vector2(width, height);
        }
        transform.position = new Vector3(0, 0, transform.position.z);

    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (!Application.isPlaying)
        {
            GameObject cavObj = GameObject.Find("Canvas");
            if (cavObj != null)
            {
                Canvas targetCanvas = cavObj.GetComponent<Canvas>();
                if (targetCanvas != null)
                {
                    rectTransform = GetComponent<RectTransform>();
                    rectTransform.sizeDelta = targetCanvas.GetComponent<RectTransform>().sizeDelta;
                }
            }
        }
    }
#endif
}
