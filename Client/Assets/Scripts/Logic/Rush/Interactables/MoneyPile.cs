using DG.Tweening;
using UnityEngine;
public class MoneyPile : BaseStack
{
    private int maxPile =100;
    public int exchangeTableMoney = 12;
    public static int MaxPerMoneyNum = 5;
    private int hiddenMoney;
    private bool isGetNow = false;
    protected override void Drop()
    {
        if (isGetNow) return;
        isGetNow = true;
        //Logger.Print("+gold", hiddenMoney, Count);
        //Logger.Print("+allgold", Profile.Instance.user.gold, hiddenMoney + Count * ExchangeTableMoney);
        if (hiddenMoney >0)
        {
            RushManager.Instance.AdjustMoney(hiddenMoney);
            AudioMgr.Instance.Play(AudioEnum.Money);
            AudioMgr.Instance.Shake();
            // Logger.Print("+gold",hiddenMoney);
            hiddenMoney = 0;
        }
        if (Count > 0)
        {
           // for (int i = 0; i < collectRate; i++)
            {
                var removedMoney = objects.Pop();
                if (removedMoney == null)
                {
                    return;
                }
                int changeMoney = Mathf.Min(Mathf.Abs(hiddenMoney), exchangeTableMoney);
                RushManager.Instance.AdjustMoney(exchangeTableMoney - changeMoney);
                hiddenMoney += changeMoney;
                removedMoney.transform.DOJump(owner.transform.position, 2f, 1, Const.InvFlyTime)
                    .OnComplete(() => {
                        AudioMgr.Instance.Play(AudioEnum.Money);
                        AudioMgr.Instance.Shake();
                        PoolManager.Instance.ReturnObject(removedMoney);
                    });
            }
        }
        isGetNow = false;
        //Logger.Print("+resgold", Profile.Instance.user.gold);
    }

    public void AddChangeShowMoney(int gold)
    {
        if (objects.Count < maxPile)
        {
            int xt = Mathf.CeilToInt((float)gold / (float)exchangeTableMoney);
            xt = Mathf.Min(MaxPerMoneyNum, xt);
            int permoenycard = xt * exchangeTableMoney;
            hiddenMoney = hiddenMoney + (gold - permoenycard);
            //Logger.Print(hiddenMoney,xt);
            for (int i = 0; i < xt; i++)
            {
                var moneyObj = PoolManager.Instance.SpawnObject(PoolManager.PoolEnum.Money, -1);
                AddToStack(moneyObj, false);
            }
        }
        else
        {
            hiddenMoney += gold;
        }
    }

}

