using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using static Const;
using UnityEditor;

public class BaseStack : Interactable
{
    [SerializeField] public bool IsShape = false;
    [SerializeField] public Transform shapeParent;
    private List<Vector3> shapePoints;
    [SerializeField] public int length = 2;
    [SerializeField] public int width = 2;
    [SerializeField] private Transform maxFlag;
    [SerializeField] private Vector3 spacing = new Vector3(0.5f, 0.1f, 0.5f);
    [SerializeField] public StackEnum stype = Const.StackEnum.None;
    private float stackTimer;
    private float stackInterval = 0.05f;
    public int Count => objects.Count;
    public int MaxStack { get; set; }
    public Stack<GameObject> objects = new Stack<GameObject>();
    private Vector3 pileCenter;
    //public int productId { get; private set; }
    public int productId { get;  set; }
    protected virtual void Start()
    {
        if (!IsShape)
        {
            pileCenter = new Vector3((length - 1) * spacing.x / 2f, 0f, (width - 1) * spacing.z / 2f);
        }
        else
        {
            CheckShapePoints();
        }
        if (maxFlag != null)
        {
            maxFlag.gameObject.SetActive(false);
        }
    }
    protected virtual void OnDestroy()
    {
        foreach (GameObject obj in objects)
        {
            Destroy(obj);
        }
    }

    private void CheckShapePoints()
    {
        shapePoints = new List<Vector3>();
        if (shapeParent != null)
        {
            RayUtil.SetLayer(shapeParent.gameObject, XazConfig.LayerDefine.UIINVISIBLE);
            for (int i = 0; i < shapeParent.transform.childCount; i++)
            {
                shapePoints.Add(shapeParent.transform.GetChild(i).position);
            }
        }
    }

    public void AddToStack(GameObject obj,bool needfly)
    {
        obj.transform.DOKill();
        objects.Push(obj);
        ArrangeAddedObject(needfly);
        obj.transform.SetParent(this.transform);
        RefreshMaxFlag();
    }

    public Transform RemoveFromStack()
    {
        Transform removed = objects.Pop().transform;
        DOTween.Kill(removed);
        RefreshMaxFlag();
        return removed;
    }

    private void RefreshMaxFlag()
    {
        if (maxFlag != null)
        {
            bool isShow = Count >= MaxStack;
            if (isShow != maxFlag.gameObject.activeSelf)
            {
                maxFlag.gameObject.SetActive(isShow);
            }
        }
    }
    public void ArrangeAddedObject(bool needfly)
    {
        if (IsShape)
        {
            int lastIndex = objects.Count - 1;
            Vector3 post = shapePoints[lastIndex];
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
        else
        {
            int lastIndex = objects.Count - 1;

            int row = (lastIndex / length) % width;
            int column = lastIndex % length;

            float xPos = column * spacing.x - pileCenter.x;
            float yPos = Mathf.FloorToInt(lastIndex / (length * width)) * spacing.y;
            float zPos = row * spacing.z - pileCenter.z - 0.01f * column;

            var latestObjectPushed = objects.Peek();
            if (needfly)
            {
                latestObjectPushed.transform.DOJump(transform.position + new Vector3(xPos, yPos, zPos), 3f, 1, Const.InvFlyTime);
            }
            else
            {
                latestObjectPushed.transform.position = transform.position + new Vector3(xPos, yPos, zPos);
            }
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
                if (RushManager.Instance.mainplayer.Stack.productIds.IndexOf(productId)==-1) return;
                if (RushManager.Instance.mainplayer.Stack.Count == 0) return;

                if (objects.Count >= MaxStack) return;

                var objToStack = RushManager.Instance.mainplayer.Stack.RemoveFromStack(productId);
                if (objToStack == null) return;
                AddToStack(objToStack.gameObject, true);
                PlayObjectSound();
            }
            else if(stype == Const.StackEnum.Get)
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
        pileCenter = new Vector3((length - 1) * spacing.x / 2f, 0f, (width - 1) * spacing.z / 2f);
        Gizmos.color = Color.yellow;
        for (int i = 0; i < length; i++)
        {
            for (int j = 0; j < width; j++)
            {
                Vector3 position = transform.position + new Vector3(i * spacing.x - pileCenter.x, spacing.y / 2f, j * spacing.z - pileCenter.z);
               Gizmos.DrawWireCube(position, spacing);
            }
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
            GameObject loadPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Resources/Prefabs/Product/kafeidou.prefab");
            outcube = Instantiate(loadPrefab);
            outcube.transform.localScale = Vector3.one;
        }
        return outcube;
    }
#endif
}

