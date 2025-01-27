//----------------------------------------------------------------------------
//-- 自定义ui元素从A飞到B效果
//-- @create 2023-8-1
//-- @author xiangzheng
//----------------------------------------------------------------------------
using DG.Tweening;
using System;
using UnityEngine;
using Xaz;

public class FlyBox : MonoBehaviour,IControl
{
    [SerializeField]
    public Transform flyObj;

    [SerializeField]
    public float duration = 1;

    [SerializeField]
    public float curveHeight = 3f; // 曲线的高度

    [SerializeField]
    public float initialUpwardDistance = 0.2f; // 初始向上移动的距离


    public Action OnPlayEnd;

    private Vector3 endPos;
    private Transform flyCloneObj = null;
    private CanvasGroup canvasGroup;
    void Start()
    {

    }

    void OnDestroy()
    {
        OnPlayEnd = null;
        Clear();
    }

    void Clear()
    {
        if (flyCloneObj)
        {
            DoTweenUtil.DOKill(flyCloneObj);
            Destroy(flyCloneObj);
            flyCloneObj = null;
        }
        else
        {
            DoTweenUtil.DOKill(flyObj);
        }
    }

    private float delyTime;
    public void PlaySetTarget(float delyTimev, Vector3 endObjtt, float dur, Action callBack)
    {
        endPos = endObjtt;
        duration = dur;
        delyTime = delyTimev;
        Play(callBack);
        //StartCoroutine(Fly(delyTime, callBack));
    }

    public void Play(Action callBack)
    {
        if (null == flyObj) return;

        if (callBack != null) OnPlayEnd = callBack;
        Transform realflyObj = flyObj;

        // 获取或添加 CanvasGroup 组件
        //canvasGroup = realflyObj.GetComponent<CanvasGroup>();
        //if (canvasGroup == null)
        //{
        //    canvasGroup = realflyObj.gameObject.AddComponent<CanvasGroup>();
        //}
        // 计算初始向上移动的位置
        Vector3 startPos = realflyObj.position;
        Vector3 upwardPos = startPos + Vector3.up * initialUpwardDistance;

        // 计算两个曲线控制点
        Vector3 controlPoint1 = upwardPos + (endPos - startPos) / 4 + Vector3.up * curveHeight;
        Vector3 controlPoint2 = upwardPos + 3 * (endPos - startPos) / 4 + Vector3.up * curveHeight / 2;

        // 定义路径点
        Vector3[] path = new Vector3[] { controlPoint1, controlPoint2, endPos};

        // 创建序列动画
        Sequence sequence = DOTween.Sequence();
        sequence.AppendInterval(delyTime)
                .Append(realflyObj.DOPath(path, duration * 0.8f, PathType.CatmullRom) // 沿曲线飞行，持续时间为总时间的60%
                .SetEase(Ease.Linear))
                .OnComplete(() => OnPlayEnd?.Invoke());
    }
}