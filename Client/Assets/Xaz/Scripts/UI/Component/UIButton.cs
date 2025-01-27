//------------------------------------------------------------
// Xaz Framework
// Feedback: qq515688254
//------------------------------------------------------------
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Xaz;

public class UIButton : Button, IControl
{
    public bool isScaleAnimation = true;

    //运行时动态改，目前无效
    public bool isPressHighlight = true;

    public bool isDefaultCachelightImg = true;

    public float scaleDuration = 0.1f;

    public float scaleOffset = 0.03f;

    private bool isShowAnim = false;

    private Vector3 initScale = Vector3.one;

    private Image hightImg = null;
    private Material m_lightMaterial;
    private UIGray grayTarget;
    private bool isPressing = false;
    //长按
    public delegate void OnButtonTouchEvent();
    public float longPressInterval = 0.3f;
    private int m_LongPressHandle;
    public OnButtonTouchEvent onButtonLongPress;

    protected override void Awake()
    {
        base.Awake();
        if (this.targetGraphic)
        {
            if (isPressHighlight)
            {
                if (isDefaultCachelightImg)
                {
                    UIImage uirawt = this.targetGraphic.GetComponent<UIImage>();
                    isDefaultCachelightImg = uirawt == null;
                    if (isDefaultCachelightImg)
                    {
                        UIImage uiraw = this.targetGraphic.GetComponentInChildren<UIImage>();
                        isDefaultCachelightImg = uiraw == null;
                    }
                }
                m_lightMaterial = Resources.Load<Material>("Materials/m_ui_srcalpha_one");
            }
            initScale = this.transform.localScale;
        }
    }

    private void PlayHighLight(bool show)
    {
        if (show)
        {
            //置灰不用高亮
            if (grayTarget == null)
            {
                grayTarget = this.transform.GetComponent<UIGray>();
            }
            if (grayTarget != null)
            {
                show = !grayTarget.IsGray;
            }
        }
        if (isPressHighlight && m_lightMaterial)
        {
            ClearHightImg();
            if (hightImg == null && show)
            {
                Image raw = this.targetGraphic.GetComponent<Image>();
                if (raw)
                {
                    hightImg = Instantiate<Image>(raw);
                    Destroy(hightImg.GetComponent<UIButton>());
                    hightImg.rectTransform.position = this.targetGraphic.rectTransform.position;
                    hightImg.transform.SetParent(this.transform, true);
                    hightImg.rectTransform.sizeDelta = this.targetGraphic.rectTransform.sizeDelta;
                    hightImg.rectTransform.localScale = this.targetGraphic.rectTransform.localScale;
                    hightImg.raycastTarget = false;
                    hightImg.material = m_lightMaterial;
                }
            }
            if (hightImg)
            {
                //hightImg.gameObject.SetActive(show);
                hightImg.DOFade(show ? 1 : 0, this.colors.fadeDuration);
            }
        }
    }

    public void CheckChangeImg()
    {
        if (this.isPressing)
        {
            ClearHightImg();
            PlayHighLight(true);
        }
    }

    private void ClearHightImg()
    {
        if (!isDefaultCachelightImg && hightImg != null)
        {
            DestroyImmediate(hightImg.gameObject);
            hightImg = null;
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        ClearHightImg();
        this.transform.localScale = initScale;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        ClearHightImg();
        this.transform.localScale = initScale;
    }
    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        isPressing = true;
        PlayHighLight(true);
        HandlePress(true, eventData);
        if (isScaleAnimation && !isShowAnim)
        {
            isShowAnim = true;
            this.transform.DOScale(initScale.x - scaleOffset, scaleDuration / 2).OnComplete(null);
        }
    }


    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        isPressing = false;
        PlayHighLight(false);
        HandlePress(false, eventData);
        if (isScaleAnimation && isShowAnim)
        {
            this.transform.DOScale(initScale.x, scaleDuration / 2).OnComplete(delegate ()
            {
                isShowAnim = false;
            });
        }
    }

    internal void HandlePress(bool pressed, PointerEventData eventData)
    {
        Scheduler.Remove(ref m_LongPressHandle);
        if (pressed)
        {
            if (onButtonLongPress != null)
            {
                //onButtonLongPress();
                m_LongPressHandle = Scheduler.Interval(delegate ()
                {
                    if (onButtonLongPress != null)
                    {
                        onButtonLongPress();
                    }
                    else
                    {
                        Scheduler.Remove(ref m_LongPressHandle);
                    }
                }, longPressInterval);
            }
        }
    }

    protected override void OnDestroy()
    {
        ClearHightImg();
        grayTarget = null;
        onButtonLongPress = null;
        base.OnDestroy();

    }

    //private void OnClickAnim()
    //{
    //    if (!isClicking)
    //    {
    //        isClicking = true;
    //        if (isScaleAnimation)
    //        {
    //            this.transform.DOScale(this.transform.localScale.x - scaleOffset, duration/2).OnComplete(delegate ()
    //            {
    //                this.transform.DOScale(this.transform.localScale.x + scaleOffset, duration/2).OnComplete(delegate ()
    //                {
    //                    isClicking = false;
    //                });
    //            });
    //        }
    //        else
    //        {
    //            isClicking = false;
    //        }
    //    }
    //}

}
