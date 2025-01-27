using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System;
public class BuyLandController : Interactable
{
    private float payingInterval = 0.01f;
    private float payingTime = 1.2f;
    private float scaleTime = 0.15f;
    [SerializeField] private RectTransform scaleContent;
    [SerializeField] private Image progressFill;
    [SerializeField] private TMP_Text priceLabel;
    private int playerMoney => Profile.Instance.user.GetGold();
    public UserSceneUnlockData curUnlockDat;
    bool isPlayerInTrigger;

    public void SetCtrlData(UserSceneUnlockData info)
    {
        curUnlockDat = info;
        ShowPayment();
    }
    void UpdatePayment(int amount)
    {
        curUnlockDat.pay += amount;
        ShowPayment();
    }

    void ShowPayment()
    {
        progressFill.fillAmount = (float)curUnlockDat.pay / curUnlockDat.GetUnlockPrice();
        priceLabel.text = (curUnlockDat.GetUnlockPrice() - curUnlockDat.pay).ToString();
    }
    protected override void OnPlayerEnter()
    {
        isPlayerInTrigger = true;
        scaleContent.transform.DOScale(1.2f, scaleTime);
        StartCoroutine(DelayedCheckAddGold());
    }
    private IEnumerator DelayedCheckAddGold()
    {
        yield return new WaitForSeconds(0.4f); 

        if (isPlayerInTrigger)
        {
            CheckAddGold();
        }
    }
    protected override void OnPlayerExit()
    {
        isPlayerInTrigger = false;
        scaleContent.transform.DOScale(1.0f, scaleTime); // 恢复 progressFill 原始大小
    }

    int paymentRate;
    private void CheckAddGold()
    {
        paymentRate = Mathf.CeilToInt((float)curUnlockDat.GetUnlockPrice() * payingInterval / payingTime);
        StartCoroutine(Pay());
    }
    IEnumerator Pay()
    {
        while (owner!=null && curUnlockDat.pay < curUnlockDat.GetUnlockPrice() && playerMoney > 0)
        {
            int payment = Mathf.Min(playerMoney, paymentRate, curUnlockDat.GetUnlockPrice()- curUnlockDat.pay);

            UpdatePayment(payment);
            //Logger.Print("ssssssssssss", payment,paymentRate, payingInterval, payingTime);
            RushManager.Instance.AdjustMoney(-payment);
            PlayMoneyAnimation();
 
            if (curUnlockDat.pay >= curUnlockDat.GetUnlockPrice())
            {
               // ModuleMgr.AwardMgr.ChangeCurrency((int)Const.CurrencyType.GOLD, curUnlockDat.GetUnlockPrice(), false);
                RushManager.Instance.BuyUnlockable(this);
            }
            yield return new WaitForSeconds(payingInterval);
        }
        if(owner != null && curUnlockDat.pay < curUnlockDat.GetUnlockPrice())
        {
            UIMgr.Open<UIAdGold>(uiview => uiview.SetData((curUnlockDat.GetUnlockPrice() - curUnlockDat.pay), () =>
            {
                CheckAddGold();
            }));
        }
    }

    void PlayMoneyAnimation()
    {
        var moneyObj = PoolManager.Instance.SpawnObject(PoolManager.PoolEnum.Money,-1);
        moneyObj.transform.position = owner.transform.position + Vector3.up * 2;
        moneyObj.transform.DOJump(transform.position, 3f, 1, 0.3f)
            .OnComplete(() => PoolManager.Instance.ReturnObject(moneyObj));

        AudioMgr.Instance.Play(AudioEnum.Money);
        AudioMgr.Instance.Shake();
    }
}

