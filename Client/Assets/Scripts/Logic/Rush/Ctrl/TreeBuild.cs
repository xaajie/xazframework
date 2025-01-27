using UnityEngine;
using System.Collections.Generic;

public class TreeBuild : BuildController
{
    private float productionTimer=0f;
    private float productionInv = 0f;
    private List<int> pickingActors = new List<int>() { };
    protected override void UpdateAttr()
    {
        base.UpdateAttr();
        productionInv = GetCtrlData().GetProductCd() / 1000;
        productionTimer =  Random.Range(0, productionInv);
    }

    public void SetIsPickingActor(int actorid,bool ispick)
    {
        if (ispick)
        {
            if (pickingActors.IndexOf(actorid) == -1)
            {
                pickingActors.Add(actorid);
            }
        }
        else
        {
            pickingActors.Remove(actorid);
        }
    }

    public bool IsFullPick()
    {
        return pickingActors.Count >= 1;
    }
    void Update()
    {
        if (productStack.Count >= productStack.MaxStack) return;

        productionTimer += Time.deltaTime;

        if (productionTimer >= productionInv)
        {
            productionTimer = 0f;

            var food = PoolManager.Instance.SpawnObject(PoolManager.PoolEnum.Product,GetCtrlData().GetProductId());
            productStack.AddToStack(food,false);
        }
    }
}

