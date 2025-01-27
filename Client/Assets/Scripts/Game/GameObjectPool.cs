using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
//对象池
public class GameObjectPool : MonoSingleton<GameObjectPool>
{

    /// <summary>可能存放多个种类的对象，每个种类有多个对象 </summary>
    private Dictionary<string, List<GameObject>> Pool = new Dictionary<string, List<GameObject>>();
    /// <summary>增加物体进入池(按类别增加)</summary>
    public void Add(string key, GameObject go)
    {
        //1.如果key在容器中存在，则将go加入对应的列表
        //2.如果key在容器中不存在，是先创建一个列表，再加入
        if (!Pool.ContainsKey(key))
        {
            Pool.Add(key, new List<GameObject>());
        }
        Pool[key].Add(go);

    }

    private GameObject FindUsable(string key)
    {
        if (Pool.ContainsKey(key))
        {
            return Pool[key].Find((p) => !p.activeSelf);
        }
        return null;
    }
    /// <summary>销毁物体(将对象隐藏)</summary>
    public void CollectObject(GameObject destoryGo)
    {
        destoryGo.SetActive(false);
        destoryGo.transform.SetParent(transform);
    }

    /// <summary>将对象归入池中<summary>
    public void DelayDestory(GameObject tempGo, float delay)
    {
        //开启一个协程
        StartCoroutine(DelayDestoryco(tempGo, delay));
    }

    /// <summary>延迟销毁</summary>
    private IEnumerator DelayDestoryco(GameObject destoryGO, float delay)
    {
        //等待一个延迟的时间
        yield return new WaitForSeconds(delay);
        CollectObject(destoryGO);
    }


    /// <summary>创建一个游戏物体到场景 </summary>
    public void CreateObject(string key, System.Action<GameObject> callback)
    {
        GameObject tempGo = FindUsable(key);
        if (tempGo != null)
        {
            tempGo.SetActive(true);
            callback(tempGo);
        }
        else
        {
            Action<UnityEngine.Object> on_cfg = (asset) =>
            {
                if (asset != null)
                {
                    tempGo = GameObject.Instantiate(asset as GameObject);
                    tempGo.name = key;
                    tempGo.SetActive(true);
                    Add(key, tempGo);
                    callback(tempGo);
                }
            };
            ResMgr.LoadAssetAsync(key, typeof(GameObject), on_cfg);
        }
    }

    /// <summary>清空某类游戏对象</summary>
    public void Clear(string key)
    {
        if (Pool.ContainsKey(key))
        {
            for (int i = 0; i < Pool[key].Count; i++)
            {
                Destroy(Pool[key][i]);
            }
            Pool.Remove(key);
        }
    }

    /// <summary>清空池中所有游戏对象</summary>
    public void ClearAll()
    {
        StopAllCoroutines();
        List<string> list = new List<string>(Pool.Keys);
        for (int i = 0; i < list.Count; i++)
        {
            Clear(list[i]);
        }
    }
}