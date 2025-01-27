using System;
using System.Collections.Generic;
using Table;
using UnityEngine;

public class PoolManager : MonoSingleton<PoolManager>
{
    public enum PoolEnum
    {
        Money = 0,
        Product = 1,
        Package=2,
    }
    private static int[] poolSize = new int[] { 300, 5,10 };
    private static string[] prefaburl = new string[] { "Prefabs/Product/Money", "Prefabs/Product/" , "Prefabs/Product/Package" };
    private Dictionary<string, Queue<GameObject>> poolDictionary = new Dictionary<string, Queue<GameObject>>();
    private Dictionary<string, GameObject> rawList = new Dictionary<string, GameObject>();
    public static int[] PoolEnumUid = new int[] { -11,0,-21};
    private bool isFinishBegin=false;
    public void BeginStart()
    {
        if (isFinishBegin) return;
        isFinishBegin = true;
        GetRawGameObject(PoolEnum.Money, PoolEnumUid[(int)PoolEnum.Money], poolSize[(int)PoolEnum.Money]);
        GetRawGameObject(PoolEnum.Package, PoolEnumUid[(int)PoolEnum.Package], poolSize[(int)PoolEnum.Package]);
        foreach (product cha in StaticDataMgr.Instance.productInfo.Values)
        {
            GetRawGameObject(PoolEnum.Product, cha.id, poolSize[(int)PoolEnum.Product]);
        }
    }

    private void GetRawGameObject(PoolEnum enumId, int productId,int num)
    {
        int index = (int)enumId;
        string prefabName = GetPoolResKeyName(enumId, productId);
        string url = GetPrefabUrlPath(enumId, productId);
        Action<UnityEngine.Object> on_cfg = (asset) =>
        {
            rawList.Add(prefabName, (asset as GameObject));
            Queue<GameObject> objectPool = new Queue<GameObject>();
            for (int i = 0; i < num; i++)
            {
                GameObject obj = Instantiate(rawList[prefabName], transform);
                obj.name = prefabName;
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }
            poolDictionary.Add(prefabName, objectPool);
        };
        ResMgr.LoadAssetAsync(url, typeof(GameObject), on_cfg);
    }

    private string GetPrefabUrlPath(PoolEnum enumId, int productId)
    {
        if (enumId == PoolEnum.Product)
        {
            product vt = StaticDataMgr.Instance.productInfo[productId];
            return prefaburl[(int)enumId] + vt.prefab;
        }
        else
        {
            return prefaburl[(int)enumId];
        }
    }
    private string GetPoolResKeyName(PoolEnum enumId, int productId)
    {
        if (enumId == PoolEnum.Product)
        {
            return string.Format("{0}_{1}", enumId,productId);
        }
        else
        {
            return string.Format("{0}_{1}", enumId, PoolEnumUid[(int)enumId]);
        }
    }
    //public void Reset()
    //{
    //    foreach (Queue<GameObject> cha in poolDictionary.Values)
    //    {
    //        foreach (GameObject obj in cha)
    //        {
    //            obj.SetActive(false);
    //        }
    //    }
    //}

    public GameObject SpawnObject(PoolEnum enumId, int productId=-1)
    {
        string prefabName = GetPoolResKeyName(enumId, productId);
        if (!poolDictionary.ContainsKey(prefabName))
        {
            Debug.LogWarning("Pool with name " + prefabName + " doesn't exist!");
            return null;
        }
        Queue<GameObject> objectPool = poolDictionary[prefabName];
        GameObject objv = null;
        if (objectPool.Count == 0)
        {
            objv = Instantiate(rawList[prefabName]);
            objv.transform.localPosition = Vector3.zero;
            objv.transform.localScale = Vector3.one;
            objv.name = prefabName;
        }
        else
        {
            objv = objectPool.Dequeue();
        }
        objv.SetActive(true);
        return objv;
    }

    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
        obj.transform.SetParent(this.transform);
        obj.transform.localPosition = Vector3.zero;
        string prefabName = obj.name;
        if (poolDictionary.ContainsKey(prefabName))
        {
            poolDictionary[prefabName].Enqueue(obj);
        }
        else
        {
            Debug.LogWarning("No pool found for object: " + prefabName);
        }
    }
}

