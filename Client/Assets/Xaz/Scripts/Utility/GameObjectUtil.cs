//------------------------------------------------------------
// Xaz Framework
// Feedback: qq515688254
//------------------------------------------------------------
using System;
using System.Collections.Generic;
using UnityEngine;


public class GameObjectUtil
{
	static public T AddChild<T> (Transform parent)
		where T : Component
    {
		return AddChild<T>(parent,"");
	}

	static public T AddChild<T> (Transform parent,string path)
		where T : Component
	{
        return AddChild<T>(parent,Load<GameObject>(path));
	}

	static public T AddChild<T> (Transform parent,GameObject prefab)
		where T : Component
	{
        GameObject go =  AddChild(parent,prefab);
        T com = go.GetComponent<T>();
         if(!com){
            com = go.AddComponent<T>();
        }
        return com;
	}
    #if USE_LUA
    
    #endif
    static public List<T> AddChilds<T> (Transform parent,string path,int count)
        where T : Component
    {
        if(!string.IsNullOrEmpty(path)){
            return AddChilds<T>(parent,Load<GameObject> (path),count);
        }
        return new List<T>();
    }
    #if USE_LUA
    
    #endif
    static public List<T> AddChilds<T> (Transform parent,GameObject prefab,int count)
        where T : Component
    {
        List<T>datas = new List<T>();
        for(int i =0; i< count; i++){
            datas.Add(AddChild<T>(parent,prefab));
        }
        return datas;
    }

    //LUA
    static public  Component AddChildComponent (Type type,Component parent)
    {
        return AddChildComponent(type,parent,"");
    }

    static public  Component AddChildComponent (Type type,Component parent,string path)
    {
        return AddChildComponent(type,parent,Load<GameObject>(path));
    }

    static public  Component AddChildComponent (Type type,Component parent,GameObject prefab)
    {
        GameObject go = AddChild(parent,prefab);
        Component com = go.GetComponent(type);
        if(com == null)
            com = go.AddComponent(type);
        return com;
    }

    static public  List<Component> AddChildsComponent (Type type,Component parent,string path,int count)
    {
        if(!string.IsNullOrEmpty(path)){
            return AddChildsComponent(type,parent,Load<GameObject>(path),count);
        }
        return new List<Component>();
    }

    static public  List<Component> AddChildsComponent (Type type,Component parent,GameObject prefab,int count)
    {
        List<Component>datas = new List<Component>();
        for (int i = 0; i < count; i++)
        {
            datas.Add(AddChildComponent(type,parent,prefab));
        }
        return datas;
    }
  
    static public GameObject AddChild (Component parent)
	{
		return AddChild (parent, "");
	}

	static public GameObject AddChild (string path)
	{
		return AddChild(null,path);
	}

    static public GameObject AddChild (Component parent, string path)
	{
        GameObject prefab =  Load<GameObject>(path);
        if(prefab)
        {
            return AddChild(parent,prefab);
        }
        return null;
	}

    static public List<GameObject>  AddChilds (Component parent, string path,int count)
    {
        if(!string.IsNullOrEmpty(path))
        {
           return AddChilds(parent,  Load<GameObject> (path),count);
        }
        return new List<GameObject>();
    }

    static public List<GameObject>  AddChilds (Component parent, GameObject path,int count)
    {
        List<GameObject>gos = new List<GameObject>();
        for(int i =0; i< count;i++){
            gos.Add(AddChild(parent,path));
        }
        return gos;
    }

    static public void ClearChildren(Transform _tra)
    {
        int childcount = _tra.childCount;
        for (int i = 0; i < childcount; i++)
        {
            GameObject.DestroyImmediate(_tra.GetChild(0).gameObject);
        }
    }
    static public GameObject AddChild (Component parent, GameObject prefab)
	{
		GameObject go;
		if(prefab != null)
        {
			go = GameObject.Instantiate<GameObject> (prefab);
			go.name = prefab.name;
		}else{
            go = new GameObject("GameObject");
		}
		if(parent != null){
            if(parent.GetComponent<RectTransform>() != null){
                if(!(go.transform is RectTransform))
                   go.AddComponent<RectTransform>();
            }
            go.transform.SetParent(parent.transform);
		}
		go.transform.localPosition = Vector3.zero;
        go.transform.localEulerAngles = Vector3.zero;
		go.transform.localScale = Vector3.one;
		return go;
	}

    static public T Load<T>(string path) where T : UnityEngine.Object
    {
        if(!string.IsNullOrEmpty(path)){
            #if USE_LUA
            return Xaz.Assets.LoadAsset<T> (path);
            #else
            return Resources.Load<T>(path);
            #endif
        }
        return null;
    }

}
