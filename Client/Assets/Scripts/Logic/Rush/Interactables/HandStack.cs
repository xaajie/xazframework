using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class HandStack : MonoBehaviour
{
    [SerializeField] private Vector2 rateRange = new Vector2(0.8f, 0.4f);
    [SerializeField] private float bendFactor = 0.1f;
    private ActorController owner;

    public List<int> productIds= new List<int>() { };
    private List<Transform> stack = new List<Transform>();
    public int Count => stack.Count;
    public int MaxStack { get; set; }
    public int Height => height;

    private int height;
    private float stackOffset = 0.35f;
    void OnDestroy()
    {
        owner = null;
        foreach (Transform obj in stack)
        {
            Destroy(obj);
        }
        stack.Clear();
    }

    public void SetOwner(ActorController vt)
    {
        owner = vt;
    }

    private bool CanShakeMove()
    {
        return owner.GetActorType() == Const.ActorType.Player;
    }

    void Update()
    {
        if (stack.Count == 0) return;

        if (owner == null || !CanShakeMove()) return;
        stack[0].transform.position = transform.position;
        stack[0].transform.rotation = transform.rotation;

        for (int i = 1; i < stack.Count; i++)
        {
            float rate = Mathf.Lerp(rateRange.x, rateRange.y, i / (float)stack.Count);
            stack[i].position = Vector3.Lerp(stack[i].position, stack[i - 1].position + (stack[i - 1].up * stackOffset), rate);
            stack[i].rotation = Quaternion.Lerp(stack[i].rotation, stack[i - 1].rotation, rate);
            stack[i].rotation *= Quaternion.Euler(-i * bendFactor * rate, 0, 0);
        }
    }

    public void AddToHandStack(Transform child, int pId,bool showJump=true)
    {
        child.DOKill();
        Vector3 peakPoint = transform.position + Vector3.up * height * stackOffset;
        height++;
        productIds.Add(pId);
        stack.Add(child);
        if (!CanShakeMove())
        {
            child.SetParent(this.transform);
        }
        if (showJump)
        {
            child.DOJump(peakPoint, 3f, 1, Const.InvFlyTime).OnComplete(() =>
                child.transform.localPosition = new Vector3(0, child.transform.localPosition.y, 0)
            );
        }
        else
        {
            child.transform.position = peakPoint;
            child.transform.localPosition = new Vector3(0, child.transform.localPosition.y, 0);
        }
    }

    public bool IsHandHasPackage()
    {
        return productIds.IndexOf(PoolManager.PoolEnumUid[(int)PoolManager.PoolEnum.Package])!=-1;
    }
    public int GetNumById(int pId)
    {
        int num = 0;
        for (int i = 0; i < productIds.Count; i++)
        {
            if (productIds[i] == pId)
            {
                num++;
            }
        }
        return num;
    }
    public Transform RemoveFromStack(int pId)
    {
        if (height == 0) return null;
        int Inx = pId<0? (productIds.Count-1):productIds.IndexOf(pId);
        if (Inx < stack.Count && Inx>=0)
        {
            Transform chooseChild = stack[Inx];
            chooseChild.rotation = Quaternion.identity;
            stack.Remove(chooseChild);
            productIds.RemoveAt(Inx);
            height--;
            for (int i = Inx; i < stack.Count; i++)
            {
                stack[i].transform.position = transform.position + Vector3.up * (Inx + 1) * stackOffset;
            }
            chooseChild.transform.DOKill();
            return chooseChild;
        }
        else
        {
            return null;
        }
    }
}
