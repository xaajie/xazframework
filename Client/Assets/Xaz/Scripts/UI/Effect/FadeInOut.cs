//------------------------------------------------------------
// Xaz Framework
// Feedback: qq515688254
//-- 淡入淡出效果
//-- @author xiejie
//----------------------------------------------------------------------------
using UnityEngine;
using Xaz;
[RequireComponent(typeof(CanvasGroup))]
public class FadeInOut : MonoBehaviour
{

    public delegate void FadeCallback();

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (IsFadeIn)
        {
            ShowFadeIn();
        }
        if (IsFadeOut)
        {
            ShowFadeOut();
        }
    }
    private bool IsFadeIn = false;
    private bool IsFadeOut = false;

    private float mChangeValuePerSec = 0;
    private float mEndValue = 0;
    private GameObject mFade = null;

    private CanvasGroup _CG = null;

    private FadeCallback callback;

    public CanvasGroup canvasGroup
    {
        get
        {
            if (_CG == null && mFade)
            {
                _CG = mFade.GetComponent<CanvasGroup>();
                if (_CG == null)
                {
                    _CG = mFade.AddComponent<CanvasGroup>();
                }
            }
            return _CG;
        }
    }
    static private CanvasGroup CheckAddCanvasGroup(GameObject o)
    {
        CanvasGroup CG = o.GetComponent<CanvasGroup>();
        if (CG == null)
        {
            CG = o.AddComponent<CanvasGroup>();
        }
        return CG;
    }

    static public void FadeInFromDelay(GameObject o, float fromValue, float endValue, float duration, float delay,FadeCallback CallBack)
    {
        CanvasGroup CG = CheckAddCanvasGroup(o);
        CG.alpha = fromValue;
        Scheduler.Timeout(delegate ()
        {
            FadeFrom(o, fromValue,endValue, duration, CallBack);
        }, delay);
    }

    static public void FadeFrom(GameObject o, float fromValue, float endValue, float duration, FadeCallback CallBack)
    {
        CanvasGroup CG = CheckAddCanvasGroup(o);
        CG.alpha = fromValue;
        if(fromValue< endValue)
        {
            FadeIn(o, endValue, duration, CallBack);
        }
        else if (fromValue > endValue)
        {
            FadeOut(o, endValue, duration, CallBack);
        }
    }
    //淡入
    static public void FadeIn(GameObject o, float endValue, float duration, FadeCallback CallBack)
    {
        CanvasGroup CG = CheckAddCanvasGroup(o);
        FadeInOut fi = o.GetComponent<FadeInOut>();
        if (endValue <= CG.alpha)
        {
            CallBack?.Invoke();
            if (fi != null)
            {
                fi.callback = null;
            }
            return;
        }
        if (duration <= 0)
        {
            CallBack?.Invoke();
            if (fi != null)
            {
                fi.callback = null;
            }
            CG.alpha = endValue;
            return;
        }
        if (fi == null)
        {
            fi = o.AddComponent<FadeInOut>();
        }
        fi.mFade = o;
        fi.IsFadeIn = true;
        float mInValue = endValue - CG.alpha;             //透明度变化值
        float n = duration / Time.deltaTime;
        fi.mChangeValuePerSec = mInValue / duration;        //每秒透明度减少的值
        fi.mEndValue = endValue;
        if (CallBack != null)
        {
            fi.callback = CallBack;
        }
    }

    private void ShowFadeIn()
    {
        if (canvasGroup.alpha < mEndValue)
        {
            canvasGroup.alpha += (mChangeValuePerSec * Time.deltaTime);
        }
        else
        {
            canvasGroup.alpha = mEndValue;
            callback?.Invoke();
            callback = null;
            IsFadeIn = false;
        }

    }

    //淡出
    static public void FadeOut(GameObject o, float endValue, float duration, FadeCallback CallBack)
    {
        if (o == null)
        {
            return;
        }
        CanvasGroup CG = CheckAddCanvasGroup(o);
        FadeInOut fo = o.GetComponent<FadeInOut>();
        if (endValue >= CG.alpha)
        {
            CallBack?.Invoke();
            if(fo != null)
            {
                fo.callback = null;
            }
            return;
        }

        if (duration <= 0)
        {
            CG.alpha = endValue;
            CallBack?.Invoke();
            if (fo != null)
            {
                fo.callback = null;
            }
            return;
        }
        if (fo == null)
        {
            fo = o.AddComponent<FadeInOut>();
        }

        fo.mFade = o;
        float mInValue = CG.alpha - endValue;             //透明度变化值
        float n = duration / Time.deltaTime;
        fo.mChangeValuePerSec = mInValue / duration;
        fo.mEndValue = endValue;
        fo.IsFadeOut = true;
        fo.callback = CallBack;
    }

    private void ShowFadeOut()
    {

        if (canvasGroup.alpha > mEndValue)
        {
            canvasGroup.alpha -= (mChangeValuePerSec * Time.deltaTime);
        }
        else
        {
            canvasGroup.alpha = mEndValue;
            callback?.Invoke();
            callback = null;
            IsFadeOut = false;
        }

    }

    static public void StopFadeIn(GameObject o)
    {
        FadeInOut fo = o.GetComponent<FadeInOut>();
        if (fo != null)
        {
            fo.IsFadeIn = false;
            fo.callback = null;
        }
    }

    static public void StopFade(GameObject o)
    {
        FadeInOut fo = o.GetComponent<FadeInOut>();
        if (fo != null)
        {
            fo.IsFadeOut = false;
            fo.IsFadeIn = false;
            CanvasGroup comp = o.GetComponent<CanvasGroup>();
            if (comp)
            {
                comp.alpha = 1;
            }
            fo.callback = null;
        }
    }

    static public void StopFadeOut(GameObject o)
    {
        FadeInOut fo = o.GetComponent<FadeInOut>();
        if (fo != null)
        {
            fo.IsFadeOut = false;
            fo.callback = null;
        }
    }
}
