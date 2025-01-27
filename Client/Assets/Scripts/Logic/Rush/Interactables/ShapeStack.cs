using DG.Tweening;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static Const;

public class ShapeStack : Interactable
{
    [SerializeField] public StackEnum stype = Const.StackEnum.None;
    private List<Transform> points;
    private float stackTimer;
    private float stackInterval = 0.05f;
    public int Count => objects.Count;
    public int MaxStack { get; set; }
    public Stack<GameObject> objects = new Stack<GameObject>();
    public int productId { get; set; }
    protected virtual void Start()
    {
        CheckPoints();
    }

    private void CheckPoints()
    {
        points = new List<Transform>();
        for (int i = 0; i < transform.childCount; i++)
        {
            points.Add(transform.GetChild(i));
        }
    }
    protected virtual void OnDestroy()
    {
        foreach (GameObject obj in objects)
        {
            Destroy(obj);
        }
    }

    public void AddToStack(GameObject obj, bool needfly)
    {
        obj.transform.DOKill();
        objects.Push(obj);
        ArrangeAddedObject(needfly);
        obj.transform.SetParent(this.transform);
    }

    public Transform RemoveFromStack()
    {
        Transform removed = objects.Pop().transform;
        DOTween.Kill(removed);

        return removed;
    }


    public void ArrangeAddedObject(bool needfly)
    {
        int lastIndex = objects.Count - 1;
        Vector3 post = points[lastIndex].transform.position;
        var latestObjectPushed = objects.Peek();
        if (needfly)
        {
            latestObjectPushed.transform.DOJump(post, 3f, 1, Const.InvFlyTime);
        }
        else
        {
            latestObjectPushed.transform.position = post;
        }
    }


    void Update()
    {
        if (stype == Const.StackEnum.None) return;

        stackTimer += Time.deltaTime;

        if (stackTimer >= stackInterval)
        {
            stackTimer = 0f;

            if (owner == null) return;
            if (stype == Const.StackEnum.Put)
            {
                if (RushManager.Instance.mainplayer.Stack.productIds.IndexOf(productId) == -1) return;
                if (RushManager.Instance.mainplayer.Stack.Count == 0) return;

                if (objects.Count >= MaxStack) return;

                var objToStack = RushManager.Instance.mainplayer.Stack.RemoveFromStack(productId);
                if (objToStack == null) return;
                AddToStack(objToStack.gameObject, true);
                PlayObjectSound();
            }
            else if (stype == Const.StackEnum.Get)
            {
                if (objects.Count > 0)
                {
                    Drop();
                }
            }
        }
    }

    protected virtual void Drop()
    {
        if (RushManager.Instance.mainplayer.Stack.Count < RushManager.Instance.mainplayer.ctrldata.capacity)
        {
            var removedObj = objects.Pop();
            RushManager.Instance.mainplayer.AddHandStack(removedObj.transform, productId);

            PlayObjectSound();
        }
    }

    private void PlayObjectSound()
    {
        AudioMgr.Instance.Play(AudioEnum.Popproduct);
        AudioMgr.Instance.Shake();
        //AudioMgr.Instance.Play(AudioEnum.Trash);
    }
#if UNITY_EDITOR

    private GameObject outcube;

    void OnDrawGizmosSelected()
    {
        CheckPoints();
        Gizmos.color = Color.yellow;
        for (int i = 0; i < points.Count; i++)
        {
            Gizmos.DrawSphere(points[i].transform.position, 0.2f);
        }
        //if (Utils.IsEditor() && transform.childCount <= 1)
        //{
        //    while (objects.Count > 0)
        //    {
        //        Destroy(objects.Pop());
        //    }
        //    for (int j = 0; j < 15; j++)
        //    {
        //        GameObject ct = Instantiate(GetOutCube());
        //        AddToStack(ct, false);
        //    }
        //}
    }

    private GameObject GetOutCube()
    {
        if (outcube == null)
        {
            GameObject loadPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Resources/Prefabs/Product/apple_juice.prefab");
            outcube = Instantiate(loadPrefab);
            outcube.transform.localScale = Vector3.one;
        }
        return outcube;
    }
#endif
}

