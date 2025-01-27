using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xaz;

public class GoldTargetMove : MonoBehaviour
{
    [SerializeField]
    public List<UIImage> arrPar;
    private List<Vector3> orignPos;

    private int currencyId;
    void Awake()
    {
        Initvt();
    }

    private void Initvt()
    {
        orignPos = new List<Vector3>() { };
        for (var i = 0; i < arrPar.Count; i++)
        {
            orignPos.Add(arrPar[i].transform.localPosition);
        }
    }

    //新加了一个play，可以根据不同需求去设置并且播放。
    public void Play(UserCategoryData flydata)
    {
        for (int i = 0; i < arrPar.Count; i++)
        {
            arrPar[i].SetSprite(flydata.GetAtlas(), flydata.GetIcon());
        }
        currencyId = flydata.GetID();
        Vector3 pos = ModuleMgr.MainMgr.GetCurrencyEndTarget(currencyId);
        if (orignPos == null)
        {
            Initvt();
        }
        this.gameObject.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(BeginFlyGold(pos));
    }

    IEnumerator BeginFlyGold(Vector3 pos)
    {
        float dur = 0.4f;
        float endfly = 0.01f;
        for (var i = 0; i < arrPar.Count; i++)
        {
            FadeInOut.StopFade(arrPar[i].gameObject);
            arrPar[i].transform.DOKill();
            arrPar[i].transform.localPosition = Vector3.zero;
        }
        yield return new WaitForEndOfFrame();
        for (var i = 0; i < arrPar.Count; i++)
        {
            arrPar[i].transform.DOLocalMove(orignPos[i], dur);
            FadeInOut.FadeFrom(arrPar[i].gameObject, 0.6f, 1, dur, null);
            yield return new WaitForSeconds(endfly);
        }
        yield return new WaitForSeconds((dur));
        for (var i = 0; i < arrPar.Count; i++)
        {
            arrPar[i].transform.DOMove(pos, dur);
          // FadeInOut.FadeFrom(arrPar[i].gameObject, 0.6f, 1, dur, null);
            yield return new WaitForSeconds(endfly);
        }
        UIMgr.Get<UIMainBottom>().ShowCurrencyIconEffect(currencyId);
        yield return new WaitForSeconds((dur+endfly*arrPar.Count));
        this.gameObject.SetActive(false);
    }
}