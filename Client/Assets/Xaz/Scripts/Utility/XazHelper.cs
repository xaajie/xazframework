//------------------------------------------------------------
// Xaz Framework
// Feedback: qq515688254
//------------------------------------------------------------
﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


static public partial class XazHelper
{
    public static WaitForEndOfFrame waitFrame = new WaitForEndOfFrame();
    public const string version = "3.4.1";

	public delegate void OnScreenSizeChanged(int width, int height);
	/// <summary>
	// 屏幕发生变化的代理，只在UnityEditor下生效。
	/// </summary>
	static public OnScreenSizeChanged onScreenSizeChanged;

	static public Action onReload;
	static internal Action onInternalReload;

	static public Action onAppQuit;
	static public Action<bool> onAppPause;
	static public Action<bool> onAppFocus;

	/// <summary>
	/// 设备返回键事件，比如Android、WP8等支持返回键的设备
	/// 参数指定该事件是否被UI截获了
	/// </summary>
	static public Action<bool> onBackButtonPressed;

	static private GameObject m_XazObject;
	static private XazBehaviour m_XazBehaviour;

	public const string XazAssetExtension = ".awb";
	public const string XazAssetIndexName = "XazAssetIndex" + XazAssetExtension;
	public const string XazUpdateIndexName = "XazUpdateIndex";
	public const string XazUnityShadersName = "XazUnityShaders" + XazAssetExtension;

	static XazHelper()
	{
#if UNITY_EDITOR
		if (!Application.isPlaying)
			return;
#endif

		System.IO.Directory.CreateDirectory(dataPath);
		System.IO.Directory.CreateDirectory(assetsPath);

		// init #obj#
		GameObject prefab = Resources.Load("__Xaz__", typeof(GameObject)) as GameObject;
		if (prefab != null) {
			m_XazObject = GameObject.Instantiate(prefab) as GameObject;
			m_XazObject.name = "__Xaz__";
		} else {
			m_XazObject = new GameObject("__Xaz__");
		}
		GameObject.DontDestroyOnLoad(m_XazObject);

		// Add Module
		m_XazBehaviour = CreateModule<XazBehaviour>();
	}

	//static private string m_PersistentDataPath = string.Empty;
	static public string persistentDataPath
	{
		get
		{
			return Application.persistentDataPath;
		}
	}
	static public string dataPath
	{
		get
		{
			return persistentDataPath + "/data";
		}
	}
	static public string assetsPath
	{
		get
		{
			return persistentDataPath + "/assets";
		}
	}
	static public string staticAssetsPath
	{
		get
		{
			return Application.streamingAssetsPath + "/StaticAssets";
		}
	}
	static public string staticAssetsIndexPath
	{
		get
		{
			return staticAssetsPath + "/XazPkgAssets.dat";
		}
	}

	/// <summary>
	/// 触发reload事件，外部可根据此事件清除相关状态
	/// </summary>
	static public void Reload()
	{
		Xaz.Scheduler.Timeout(() => {
			if (onInternalReload != null)
				onInternalReload();
			if (onReload != null)
				onReload();
		});
	}

	static internal T CreateModule<T>() where T : MonoBehaviour
	{
#if UNITY_EDITOR
		if (!Application.isPlaying)
			return null;
#endif
		return m_XazObject.AddComponent<T>();
	}

		static public bool IsChild(Transform parent, Transform child)
		{
			if (parent == null || child == null)
				return false;

			while (child != null) {
				if (child == parent)
					return true;
				child = child.parent;
			}
			return false;
		}

		/// <summary>
		/// 添加新的GameObject到父节点
		/// </summary>
		static public GameObject AddChild(GameObject parent)
		{
			GameObject go = new GameObject();
			if (parent != null) {
				go.transform.SetParent(parent.transform, false);
				go.layer = parent.layer;
			}
			return go;
		}

		/// <summary>
		/// 实例化prefab到父节点
		/// </summary>
		static public GameObject AddChild(GameObject parent, GameObject prefab)
		{
			GameObject go = GameObject.Instantiate(prefab) as GameObject;
			if (go != null && parent != null) {
				go.transform.SetParent(parent.transform, false);
				go.layer = parent.layer;
			}
			return go;
		}

		/// <summary>
		/// 添加特定组件的GameObject到父节点
		/// </summary>
		static public T AddChild<T>(GameObject parent)
			where T : Component
		{
			GameObject go = AddChild(parent);
			string name = typeof(T).ToString();
			go.name = name.Substring(name.LastIndexOf(".") + 1);
			return go.AddComponent<T>();
		}

		/// <summary>
		/// 移除所有子节点
		/// </summary>
		/// <param name="parent"></param>
		static public void RemoveChildren(GameObject parent)
		{
			RemoveChildren(parent.transform);
		}

		static public void RemoveChildren(Transform parent)
		{
			for (int i = parent.childCount - 1; i >= 0; i--) {
				Destroy(parent.GetChild(i).gameObject);
			}
		}

		/// <summary>
		/// 销毁对象，在Editor下，采用DestroyImmediate方法，运行时采用Destroy方法。
		/// </summary>
		static public void Destroy(UnityEngine.Object obj)
		{
	#if UNITY_EDITOR
			if (Application.isPlaying) {
				UnityEngine.Object.Destroy(obj);
			} else {
				UnityEngine.Object.DestroyImmediate(obj);
			}
	#else
			UnityEngine.Object.Destroy(obj);
	#endif
		}

	/// <summary>
	/// 设置所有子节点的Layer
	/// </summary>
	static public void SetLayer(GameObject go, int layer)
	{
		foreach (Transform t in go.GetComponentsInChildren<Transform>(true)) {
			t.gameObject.layer = layer;
		}
	}

	static private List<Graphic> s_TempComponents;

    /// <summary>
    /// 启动协程
    /// </summary>
    static public Coroutine StartCoroutine(IEnumerator routine)
    {
        if (m_XazBehaviour != null)
        {
            return m_XazBehaviour.StartCoroutine(routine);
        }
        return null;
    }

    /// <summary>
    /// 终止协程
    /// </summary>
    static public void StopCoroutine(IEnumerator routine)
    {
        if (m_XazBehaviour != null)
        {
            m_XazBehaviour.StopCoroutine(routine);
        }
    }

    /// <summary>
    /// MaterialStorage
    /// </summary>
    class MaterialStorage : MonoBehaviour
	{
		public Material material;

		void Awake()
		{
			hideFlags = HideFlags.HideInInspector;
		}
	}

	/// <summary>
	/// XazBehaviour
	/// </summary>
	class XazBehaviour : MonoBehaviour
	{
#if UNITY_EDITOR
		private int m_ScreenWidth;
		private int m_ScreenHeight;

		void Awake()
		{
			m_ScreenWidth = Screen.width;
			m_ScreenHeight = Screen.height;
		}
#endif

#if UNITY_EDITOR || UNITY_ANDROID
		void Update()
		{
#if UNITY_EDITOR
			int width = Screen.width;
			int height = Screen.height;
			if (width != m_ScreenWidth || height != m_ScreenHeight) {
				m_ScreenWidth = width;
				m_ScreenHeight = height;
				if (onScreenSizeChanged != null) {
					onScreenSizeChanged(width, height);
				}
			}
#endif
			if (Input.GetKeyUp(KeyCode.Escape)) {
				var blocked = Xaz.UIViewRoot.OnBackButtonPressed();
				if (onBackButtonPressed != null)
					onBackButtonPressed(blocked);
			}
		}
#endif

		void OnApplicationQuit()
		{
			if (onAppQuit != null)
				onAppQuit();
		}

		void OnApplicationPause(bool isPaused)
		{
			if (onAppPause != null)
				onAppPause(isPaused);
		}
		void OnApplicationFocus(bool isFocused)
		{
			if (onAppFocus != null)
				onAppFocus(isFocused);
		}
	}
}

namespace Xaz
{
	static public partial class ExtensionMethod
	{
		static public string ToUpperFirst(this string str)
		{
			if (str == string.Empty)
				return str;
			return str.Substring(0, 1).ToUpper() + str.Substring(1);
		}
		static public string ToLowerFirst(this string str)
		{
			if (str == string.Empty)
				return str;
			return str.Substring(0, 1).ToLower() + str.Substring(1);
		}
	}
}