//------------------------------------------------------------
//dotween插件和lua端的中间件
// Xaz Framework
// Feedback: qq515688254
//------------------------------------------------------------
using DG.Tweening;
using DG.Tweening.Core;
using UnityEngine;
using UnityEngine.UI;

public class DoTweenUtil
{
    public delegate float GetFloat();
    public delegate void SetFloat(float s);

    //移动
    static public object DOMove(Transform t, TweenCallback onComplete, Vector3 to, float duration)
    {
        return DOMove(t, onComplete, to, duration, (int) Ease.Linear, false, false);
    }

    static public object DOMove(Transform t, TweenCallback onComplete, Vector3 to, float duration, int eascId)
    {
        return DOMove(t, onComplete, to, duration, eascId, false, false);
    }

    static public object DOMove(Transform t, TweenCallback onComplete, Vector3 to, float duration, int eascId,
        bool snapping)
    {
        return DOMove(t, onComplete, to, duration, eascId, snapping, false);
    }

    static public object DOMove(Transform t, TweenCallback onComplete, Vector3 to, float duration, int eascId,
        bool snapping, bool ignorTimeScale)
    {
        Tweener tweener = t.DOLocalMove(to, duration, snapping).SetUpdate(ignorTimeScale);

        tweener = tweener.SetEase((Ease)eascId);
        if (onComplete != null)
            tweener.OnComplete(onComplete);
        return tweener;
    }

    static public object DOWorldMove(Transform t, TweenCallback onComplete, Vector3 to, float duration)
    {
        Tweener tweener = t.DOMove(to, duration, false).SetUpdate(false);

        tweener = tweener.SetEase(Ease.Linear);
        if (onComplete != null)
            tweener.OnComplete(onComplete);
        return tweener;
    }

    //UI移动
    static public object DOMove(RectTransform t, TweenCallback onComplete, Vector2 to, float duration, int eascId)
    {
        return DOMove(t, onComplete, to, duration, eascId, false);
    }

    static public object DoAnchorMove(RectTransform t, TweenCallback onComplete, Vector2 to, float duration,
        bool snapping)
    {
        Tweener tweener = t.DOAnchorPos(to, duration, snapping);
        tweener = tweener.SetEase(Ease.Linear);
        if (onComplete != null)
            tweener.OnComplete(onComplete);
        return tweener;
    }

    //旋转
    static public object DORotate(Transform t, TweenCallback onComplete, Vector3 to, float duration)
    {
        return DORotate(t, onComplete, to, duration, (int) RotateMode.Fast);
    }

    static public object DORotate(Transform t, TweenCallback onComplete, Vector3 to, float duration, int type)
    {
        Tweener tweener = t.DORotate(to, duration, (RotateMode) type);
        if (onComplete != null)
            tweener.OnComplete(onComplete);
        return tweener;
    }

    //抖动
    static public object DOShakePosition(Transform t, TweenCallback onComplete, float duration, Vector3 strength)
    {
        return DOShakePosition(t, onComplete, duration, strength, 10, 90, false);
    }

    static public object DOShakePosition(Transform t, TweenCallback onComplete, float duration, Vector3 strength,
        int vibrato, float randomness, bool snapping)
    {
        Tweener tweener = t.DOShakePosition(duration, strength, vibrato, randomness, snapping);
        if (onComplete != null)
            tweener.OnComplete(onComplete);
        return tweener;
    }

    //抖动
    static public object DOShakeRotation(Transform t, TweenCallback onComplete, float duration, Vector3 strength)
    {
        return DOShakeRotation(t, onComplete, duration, strength, 10, 90, false);
    }

    static public object DOShakeRotation(Transform t, TweenCallback onComplete, float duration, Vector3 strength,
        int vibrato, float randomness, bool fadeOut)
    {
        Tweener tweener = t.DOShakeRotation(duration, strength, vibrato, randomness, fadeOut);
        if (onComplete != null)
            tweener.OnComplete(onComplete);
        return tweener;
    }

    public static Tweener DOSizeDelta(RectTransform target, TweenCallback onComplete, Vector2 endValue, float duration,
        int eascId)
    {
        Tweener tweener = DOTween.To(() => target.sizeDelta, x => target.sizeDelta = x, endValue, duration)
            .SetTarget(target);
        tweener = tweener.SetEase(eascId >0 ? (Ease) eascId : Ease.Linear);
        if (onComplete != null)
            tweener.OnComplete(onComplete);
        return tweener;
    }


    //缩放
    static public object DOScale(Transform t, TweenCallback onComplete, Vector3 to, float duration)
    {
        Tweener tweener = t.DOScale(to, duration).SetUpdate(true);
        if (onComplete != null)
        {
            tweener.OnComplete(onComplete);
        }

        return tweener;
    }

    static public object DOScale(Transform t, TweenCallback onComplete, Vector3 to, float duration, int eascId)
    {
        Tweener tweener = t.DOScale(to, duration).SetUpdate(true);
        tweener = tweener.SetEase((Ease)eascId);
        if (onComplete != null)
        {
            tweener.OnComplete(onComplete);
        }

        return tweener;
    }

    static public object DOScale(Transform t, TweenCallback onComplete, float to, float duration, int eascId)
    {
        Tweener tweener = t.DOScale(new Vector3(to, to, to), duration).SetUpdate(true);
        tweener = tweener.SetEase((Ease)eascId);
        if (onComplete != null)
        {
            tweener.OnComplete(onComplete);
        }

        return tweener;
    }

    //褪色
    static public object DOFade(CanvasGroup g, TweenCallback onComplete, float endvalue, float duration, int eascId)
    {
        Tweener tweener = g.DOFade(endvalue, duration);
        tweener = tweener.SetEase(eascId >0 ? (Ease) eascId : Ease.Linear);
        if (onComplete != null)
            tweener.OnComplete(onComplete);
        return tweener;
    }

    static public object DOFade(Graphic g, TweenCallback onComplete, float endvalue, float duration)
    {
        Tweener tweener = g.DOFade(endvalue, duration);
        if (onComplete != null)
            tweener.OnComplete(onComplete);
        return tweener;
    }

    static public object DOFade(Material m, TweenCallback onComplete, float endvalue, float duration)
    {
        Tweener tweener = m.DOFade(endvalue, duration);
        if (onComplete != null)
            tweener.OnComplete(onComplete);
        return tweener;
    }

    static public object DOFade(AudioSource a, TweenCallback onComplete, float endvalue, float duration)
    {
        Tweener tweener = a.DOFade(endvalue, duration);
        if (onComplete != null)
            tweener.OnComplete(onComplete);
        return tweener;
    }

    //变色
    static public object DOColor(Graphic g, TweenCallback onComplete, Color endValue, float duration)
    {
        Tweener tweener = g.DOColor(endValue, duration);
        if (onComplete != null)
            tweener.OnComplete(onComplete);
        return tweener;
    }

    static public object DOColor(Material m, TweenCallback onComplete, Color endvalue, float duration)
    {
        Tweener tweener = m.DOColor(endvalue, duration);
        if (onComplete != null)
            tweener.OnComplete(onComplete);
        return tweener;
    }

    //不建议lua测用这个，改用下面那个 addby xiejie 
    static public object DOToFloat(GetFloat gf, SetFloat sf, float to, float duration, TweenCallback onComplete)
    {
        TweenerCore<float, float, DG.Tweening.Plugins.Options.FloatOptions> tweenerCore =
            DOTween.To(() => gf(), x => sf(x), to, duration).OnComplete(onComplete);
        tweenerCore.SetAutoKill(true);
        return tweenerCore;
    }

    //对变量进行动画,注：比上面getfloat性能高，addby xiejie 
    //上层持有返回值xx，管理此对象立即销毁需要调用 xx.Kill
    static public Tweener DOToFloat(float start, float to, float duration, SetFloat sf, TweenCallback onComplete)
    {
        Tweener tweener = DOTween.To(() => start, x => sf(x), to, duration);
        if (onComplete != null)
            tweener.OnComplete(onComplete);
        tweener.SetAutoKill(true);
        return tweener;
    }

    //灯光颜色过渡
    static public object DOLightColor(Light l, TweenCallback onComplete, Color endvalue, float duration)
    {
        Tweener tweener = l.DOColor(endvalue, duration);
        if (onComplete != null)
            tweener.OnComplete(onComplete);
        return tweener;
    }

    //灯光强度过渡
    static public object DOLightIntensity(Light l, TweenCallback onComplete, float endvalue, float duration)
    {
        Tweener tweener = l.DOIntensity(endvalue, duration);
        if (onComplete != null)
            tweener.OnComplete(onComplete);
        return tweener;
    }

    // 文本打字效果
    static public object DOText(Text text, TweenCallback onComplete, string endvalue, float duration)
    {
        Tweener tweener = text.DOText(endvalue, duration);
        if (onComplete != null)
            tweener.OnComplete(onComplete);
        return tweener;
    }

    static public void DOKill(RectTransform t)
    {
        DOKill(t, false);
    }

    static public void DOKill(RectTransform t, bool complete)
    {
        t.DOKill(complete);
    }

    static public void DOKill(Transform t)
    {
        DOKill(t, false);
    }

    static public void DOKill(Transform t, bool complete)
    {
        t.DOKill(complete);
    }

    static public void KillTween(Tweener tween)
    {
        tween.Kill();
    }

    /// <summary>
    /// 弹动物体
    /// </summary>
    /// <param name="t"></param>
    static public void BounceTarget(Transform target, TweenCallback onComplete)
    {
        Vector3 m_originalScale = target.localScale;
        Sequence seq = DOTween.Sequence();
        seq.Append(target.DOScale(
            new Vector3(m_originalScale.x * 1.1f, m_originalScale.y * 1.3f, m_originalScale.z * 0.8f), 0.05f));
        seq.Append(target.DOScale(
            new Vector3(m_originalScale.x * 1.2f, m_originalScale.y * 0.8f, m_originalScale.z * 1.2f), 0.1f));
        seq.Append(target.DOScale(
            new Vector3(m_originalScale.x * 0.75f, m_originalScale.y * 1.2f, m_originalScale.z * 0.95f), 0.12f));
        seq.Append(target.DOScale(
            new Vector3(m_originalScale.x * 1.05f, m_originalScale.y * 0.95f, m_originalScale.z * 1.15f), 0.15f));
        seq.Append(target.DOScale(new Vector3(m_originalScale.x, m_originalScale.y, m_originalScale.z), 0.2f));
        seq.onComplete = onComplete;
    }

    static public void ClickZoomEffect(Transform target, TweenCallback onComplete)
    {
        Vector3 m_originalScale = target.localScale;
        Sequence seq = DOTween.Sequence();
        seq.Append(target.DOScale(m_originalScale * 1.2f, 0.1f).SetEase(Ease.OutCirc));
        seq.Append(target.DOScale(m_originalScale, 1f).SetEase(Ease.OutBounce));
        seq.onComplete = onComplete;
    }

    static public void DOLocalJump(Transform t, TweenCallback onComplete, Vector3 endValue, float jumpPower,
        int numJumps, float duration)
    {
        t.DOLocalJump(endValue, jumpPower, numJumps, duration).OnComplete(() => { onComplete(); });
    }

    /// <summary>
    /// z轴一定幅度抖动
    /// </summary>
    /// <param name="target"></param>
    /// <param name="onComplete"></param>
    /// <param name="durtime"></param>
    /// <param name="offset"></param>
    /// <param name="count"></param>
    static public void DORotationShakeZ(Transform target, TweenCallback onComplete, int offset, int count,
        float duration)
    {
        Sequence seq = DOTween.Sequence();
        Vector3 vc = target.transform.localEulerAngles;
        seq.Append(target.DORotate(new Vector3(vc.x, vc.y, vc.z - offset), duration / 2).SetEase(Ease.Linear));
        seq.Append(target.DORotate(new Vector3(vc.x, vc.y, vc.z + offset), duration).SetEase(Ease.Linear));
        seq.Append(target.DORotate(new Vector3(vc.x, vc.y, vc.z), duration / 2).SetEase(Ease.Linear));
        seq.SetLoops(count);
        seq.onComplete = onComplete;
    }

    /// <summary>
    /// 重新启用挂载的tween
    /// </summary>
    static public void DoReplayComponent(Transform target, bool needRewind = true)
    {
        if (target == null)
        {
            return;
        }

        DoControl(target.gameObject, needRewind);
    }

    static public void DoReplayComponentById(Transform target, string showId, bool needRewind = true)
    {
        if (target == null)
        {
            return;
        }

        DoControlById(target.gameObject, needRewind, showId);
    }

    static public void DoControl(GameObject go, bool needRewind = true)
    {
        DoControlById(go, needRewind, "");
    }

    static public void DoControlById(GameObject go, bool needRewind = true, string showId = "")
    {
        if (go)
        {
            if (needRewind)
            {
                DOTween.Rewind(go);
            }

            if (showId == "")
            {
                DOTween.Restart(go);
            }
            else
            {
                DOTween.Restart(go, showId);
            }
        }
    }

    static public void DoControlBackById(GameObject go, bool needRewind = true, string showId = "")
    {
        if (go)
        {
            if (needRewind)
            {
                DOTween.Rewind(go);
            }

            if (showId == "")
            {
                DOTween.PlayBackwards(go);
            }
            else
            {
                DOTween.PlayBackwards(go, showId);
            }
        }
    }
    
    static public float GetDurationAndHoldTime(Transform target, string showId)
    {
        if (target == null)
        {
            return 0;
        }

        GameObject targetGo = target.gameObject;
        DOTweenAnimation[] doTweenAnimList = targetGo.GetComponents<DOTweenAnimation>();
        for (int i = 0; i < doTweenAnimList.Length; i++)
        {
            if (doTweenAnimList[i].id == showId)
            {
                return doTweenAnimList[i].duration;
            }
        }

        return 0;
    }
}