//------------------------------------------------------------
// Xaz Framework
// Feedback: qq515688254
//------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.U2D;
using WeakReference = System.WeakReference;


namespace Xaz
{

    static public class Assets
	{

		public sealed class Async<T> : IEnumerator
			where T : Object
		{
			public T asset
			{
				get; internal set;
			}
			public bool isDone
			{
				get; internal set;
			}

			object IEnumerator.Current
			{
				get
				{
					return asset;
				}
			}
			bool IEnumerator.MoveNext()
			{
				return !isDone;
			}
			void IEnumerator.Reset()
			{
				asset = null;
			}
		}

		static private AssetsBehaviour m_AssetsBehaviour;

		static Assets()
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
				return;
#endif
			XazHelper.onInternalReload += () => {
				UnloadAllAssets();
			};

			SceneManager.sceneUnloaded += (scene) => {

				//项目修改
				if(!scene.name.Contains("empty"))
				{
					UnloadUnusedAssets();
				}

			};

			m_AssetsBehaviour = XazHelper.CreateModule<AssetsBehaviour>();
		}

		static private string[] m_ManifestAssetDirectories = new string[0];
		static private AssetBundleReference[] m_ManifestBundles = new AssetBundleReference[0];
		static private Dictionary<string, KeyValuePair<int, int>> m_ManifestAssets = new Dictionary<string, KeyValuePair<int, int>>();
		static private Dictionary<string, KeyValuePair<string, bool>> m_StoredAssets = new Dictionary<string, KeyValuePair<string, bool>>();
		static private readonly ObjectPool<WeakReference> m_ObjRefPool = new ObjectPool<WeakReference>(() => new WeakReference(null), null, (v) => v.Target = null);
		static private Dictionary<KeyValuePair<string, System.Type>, WeakReference> m_AssetTypeReferences = new Dictionary<KeyValuePair<string, System.Type>, WeakReference>();

		[DisallowMultipleComponent]
		private class PrefabObject : MonoBehaviour
		{
			[HideInInspector, SerializeField]
			internal string path = string.Empty;

			// 记录Prefab，防止Prefab被回收
			private Object m_Prefab = null;

			void Awake()
			{
				hideFlags = HideFlags.HideInInspector;
				AssetBundleReference bundle = null;
				var assetName = GetAssetName(ref path, out bundle);
				if (bundle != null) {
					m_Prefab = TryGetCacheValue(assetName, typeof(GameObject));
					bundle.CollectObject(gameObject);
				}
			}
		}

		
		public enum ResourceLocation
		{
			Local,
			HotUpdate,
			PkgUpdate
		}
		
		#region 项目修改
		static public void InitializeLocal()
		{
			Initialize(ResourceLocation.Local);
		}
		#endregion	

		static public void Initialize(ResourceLocation location)
		{
			UnloadAllAssets();
			m_StoredAssets.Clear();
			m_ManifestAssets.Clear();
			System.Array.Clear(m_ManifestBundles, 0, m_ManifestBundles.Length);

            if (location == ResourceLocation.Local)
            {
                Dictionary<string, string> caches = new Dictionary<string, string>();
                var cacheFile = XazHelper.assetsPath + "/CacheLocalIndex";
                if (File.Exists(cacheFile))
                {
                    var s = File.ReadAllText(cacheFile);
                    foreach (var l in s.Split('\n'))
                    {
                        string[] a = l.Split(',');

                        if (a.Length < 3)
                        {
                            Debug.Log("CacheLocalIndex=[" + l + "]  " + a.Length);
                            continue;
                        }

                        caches.Add(a[0], a[1]);
                    }
                }
            }
				

			KeyValuePair<string, bool> XazIndexPath;
			if (m_StoredAssets.TryGetValue(XazHelper.XazAssetIndexName, out XazIndexPath)) {
				var ab = AssetBundle.LoadFromFile(GetAssetPath(XazIndexPath));
				var manifest = ab.LoadAsset<AssetsManifest>(XazHelper.XazAssetIndexName);
				if (manifest != null) {
					var bundles = manifest.bundles;
					System.Array.Resize(ref m_ManifestBundles, bundles.Length);
					for (int i = 0, count = bundles.Length; i < count; i++) {
						var bundle = bundles[i];
						m_ManifestBundles[i] = new AssetBundleReference(bundle);
						foreach (var asset in bundle.assets) {
							m_ManifestAssets[asset.path] = new KeyValuePair<int, int>(asset.index, i);
						}
					}
					m_ManifestAssetDirectories = manifest.directories;
				}
				ab.Unload(false);
			}
		}

		static public T LoadAsset<T>(string path) where T : Object
		{
			return LoadAsset(path, typeof(T)) as T;
		}
				
		#region 项目修改
		private static HashSet<string> usedAssets =  new HashSet<string>();

		public static  HashSet<string> GetUsedAssets(){
			return usedAssets;
		}
		#endregion


	  #region 项目修改
        /** assetBundle里面是以Assets开始的路径来识别文件，程序调用的path是ResourcesAB之后的路径，需要转换一下 **/
        private static string AssetsHead = "Assets";
        private static string AssetBundlePath = "Assets/ResourcesAB/";
	    private static string origPath = null;


		static bool IsClass(System.Type type1,System.Type type2)
		{
			return (type1.IsSubclassOf(type2) || (type1 == type2));
		}

		static public string GetFileNameWithType(string path,System.Type type)
		{
			if (!Path.HasExtension(path)) {
				if (IsClass (type, typeof(ScriptableObject))) {
					path = Path.ChangeExtension (path, ".asset");
                }
                else if (IsClass(type, typeof(SpriteAtlas)))
                {
                    path = Path.ChangeExtension(path, ".spriteAtlas");
                } else if (IsClass (type, typeof(ShaderVariantCollection))) {
					path = Path.ChangeExtension (path, ".shadervariants");
				} else if (IsClass (type, typeof(Shader))) {
					path = Path.ChangeExtension (path, ".shader");
				} else if (IsClass (type, typeof(Sprite)) || IsClass (type, typeof(Texture))|| IsClass (type, typeof(Texture2D))) {
					path = Path.ChangeExtension (path, ".png");
				} else if (IsClass (type, typeof(TextAsset))) {
					path = Path.ChangeExtension (path, ".bytes");
				} else if (IsClass (type, typeof(Font))) {
					path = Path.ChangeExtension (path, ".ttf");
				} else if (IsClass(type, typeof(AnimationClip))) {
					path = Path.ChangeExtension(path, ".anim");
				} else if (IsClass(type, typeof(AudioClip))) {
					path = Path.ChangeExtension(path, XazConfig.AudioSuffix);
				} else if (IsClass(type, typeof(Material))) {
					path = Path.ChangeExtension(path, ".mat");
				} else {
					path = Path.ChangeExtension (path, ".prefab");
				}
			}
            //Debug.Log(path);
			path = path.Replace ("\\", "/"); 
			return path;
		}
		#endregion		


		static public Object LoadAsset(string path, System.Type type)
		{

			#region 项目修改
			origPath = path;
            path = GetFileNameWithType(path,type);
            if(!path.StartsWith(AssetsHead))
            {
                path = AssetBundlePath + path;
            }
			#endregion

			AssetBundleReference bundle = null;
			string assetName = GetAssetName(ref path, out bundle);
			Object obj = TryGetCacheValue(assetName, type);
			if (obj == null) {
#if !UNITY_EDITOR || USE_ASSETBUNDLE
				if (bundle != null) {
					obj = bundle.LoadAsset(path, assetName, type);
				} else
#endif
				{

					//项目修改  
					//读取Resource下面资源
 					obj = Resources.Load(Path.ChangeExtension(origPath, null), type);
                    if(obj == null)
                    {
						// Load From Resources/XazAssets
						int index = path.LastIndexOf("/Resources/");
						if (index > 0) {
							path = path.Substring(index + 11);
						}
						obj = Resources.Load(Path.ChangeExtension(path, null), type);
					}

#if UNITY_EDITOR && !USE_ASSETBUNDLE
					if (obj == null)
						obj = XazEditor.EditorAssets.LoadAsset(path, type);
#endif
#if Xaz_ASSET_DEBUG
					if (obj != null)
						Debug.LogWarningFormat("Assets.LoadAsset ({0}) from Resources.", assetName);
#endif
				}
				TrySetCacheValue(assetName, type, obj);
			}
			return obj;
		}

		static public void LoadScene(string path)
		{
			//Debug.Log ("===LoadScene===" + path);
			usedAssets.Add (path);
#if !UNITY_EDITOR || USE_ASSETBUNDLE
			AssetBundleReference bundle;
			GetAssetName(ref path, out bundle);
			if (bundle != null) {
				bundle.Ref(true);
			}
#endif
		}

		static private bool IsAlive(WeakReference reference)
		{
			var target = reference.Target;
			return target != null && !target.Equals(null);
		}

		static private Object TryGetCacheValue(string assetName, System.Type type)
		{
			WeakReference objRef;
			var key = new KeyValuePair<string, System.Type>(assetName, type);
			if (m_AssetTypeReferences.TryGetValue(key, out objRef)) {
				if (!IsAlive(objRef)) {
					m_ObjRefPool.Release(objRef);
					m_AssetTypeReferences.Remove(key);
				} else {
#if Xaz_ASSET_DEBUG
					Debug.LogWarningFormat("Assets.LoadFromCache ({0}): {1}", assetName, objRef.Target as Object);
#endif
					return objRef.Target as Object;
				}
			}
			return null;
		}
		static private void TrySetCacheValue(string assetName, System.Type type, Object obj)
		{
			if (obj == null) {
#if UNITY_EDITOR
				if (!assetName.EndsWith(".lua.bytes")) {
					Logger.Warning(string.Format("Cannot load resource '{0}'.", assetName));
				}
#endif
			} else {
				var objRef = m_ObjRefPool.Get();
				objRef.Target = obj;
				m_AssetTypeReferences[new KeyValuePair<string, System.Type>(assetName, type)] = objRef;
			}
		}
		static public void UnloadAllAssets()
		{
			m_AssetsBehaviour.StopAllCoroutines();
			foreach (var objRef in m_AssetTypeReferences.Values) {
				m_ObjRefPool.Release(objRef);
			}
			m_AssetTypeReferences.Clear();
			foreach (var bundle in m_ManifestBundles) {
				bundle.Clear();
			}
		}

		static public void UnloadUnusedAssets()
		{
			
#if Xaz_ASSET_DEBUG
			Debug.LogWarning("Assets.UnloadUnusedAssets ...");
#endif
			XazHelper.StartCoroutine(UnloadUnusedAssetsAsync());
		}


        #region 项目自定义的LOAD
        static public Async<T> LoadAssetAsyncView<T>(string path) where T : Object
        {
            Async<T> async = new Async<T>();
            LoadAssetAsync<T>(path, (obj,str) => {
                async.asset = obj;
                async.isDone = true;
            });
            return async;
        }
        static public IEnumerator LoadAssetAsync<T>(string path) where T : Object
        {
            return LoadAssetAsync<T>(path, typeof(T), null);
        }            

        static public void LoadAssetAsync<T>(string path, System.Action<T,string> callback,bool Instantiate = false) where T : Object
        {
            m_AssetsBehaviour.StartCoroutine(LoadAssetAsync<T>(path, typeof(T), callback,Instantiate));
        }
        static public IEnumerator LoadAssetAsync(string path, System.Type type)
        {
            return LoadAssetAsync<Object>(path, type, null);
        }
        static public void LoadAssetAsync(string path, System.Type type, System.Action<Object,string> callback)
        {
			if (m_AssetsBehaviour != null)
			{
                m_AssetsBehaviour.StartCoroutine(LoadAssetAsync<Object>(path, type, callback));
            }
        }

	
        static private IEnumerator LoadAssetAsync<T>(string path, System.Type type, System.Action<T,string> callback,bool Instantiate = false) where T : Object
        {

			for (int i = 0, limit = Random.Range(0, 3); i < limit; i++)
				yield return null; // 等待N帧
			//项目修改 start
			string resKey = path;
			origPath = path;


			if(!path.StartsWith(AssetsHead))
			{
				path = AssetBundlePath + path;
			}
			//项目修改 end


			AssetBundleReference bundle = null;
			string assetName = GetAssetName(ref path, out bundle);
			Object obj = TryGetCacheValue(assetName, type);
			if (obj == null) {
				#if !UNITY_EDITOR && USE_ASSETBUNDLE
				if (bundle != null) {
				yield return bundle.LoadAssetAsync(path, assetName, type, (asset) => {
				obj = asset;
				});
				} else
				#endif
				{

					//项目修改 start
					var req0  = Resources.LoadAsync(Path.ChangeExtension(origPath, null), type);
					yield return req0;
					obj = req0.asset;
					//项目修改 end

					if (obj == null) {

						// Load From Resources/XazAssets
						int index = path.LastIndexOf ("/Resources/");
						if (index > 0) {
							path = path.Substring (index + 11);
						}
						var req = Resources.LoadAsync (Path.ChangeExtension (path, null), type);
						yield return req;
						obj = req.asset;

					}

					#if UNITY_EDITOR && !USE_ASSETBUNDLE
					if (obj == null) {
						obj = XazEditor.EditorAssets.LoadAsset(path, type);
					}
					#endif
					#if Xaz_ASSET_DEBUG
					if (obj != null)
					Debug.LogWarningFormat("Assets.LoadAsset ({0}) from Resources.", assetName);
					#endif
				}
				if (obj != TryGetCacheValue(assetName, type)) {
					TrySetCacheValue(assetName, type, obj);
				}
			}

			if (callback != null) {
				if(Instantiate)
				{
					callback((T)Object.Instantiate(obj),resKey);
				}else{
					callback((T)obj,resKey);
				}
			}
				
        }

		private static string GetStaticAssetPath(string name)
		{
			return XazHelper.staticAssetsPath + "/" + name;
		}
			
		private static string GetStaticAssetPath2(KeyValuePair<string, bool> pair)
		{
			if (pair.Value) {
				return XazHelper.staticAssetsPath + "/" + pair.Key;
			}
			return XazHelper.staticAssetsPath + "/" + pair.Key;
		}
		#endregion




		static private IEnumerator UnloadUnusedAssetsAsync()
		{
			var asyncOp = Resources.UnloadUnusedAssets();
			yield return asyncOp;


			// Check Loaded Prefabs
			var temp = new Queue<KeyValuePair<string, System.Type>>();
			foreach (var entry in m_AssetTypeReferences) {
				var objRef = entry.Value;
				if (!IsAlive(objRef)) {
					m_ObjRefPool.Release(objRef);
					temp.Enqueue(entry.Key);
#if Xaz_ASSET_DEBUG
					Debug.LogWarningFormat("Assets.UnusedAsset: {0}", entry.Key);
#endif
				}
			}
			while (temp.Count > 0) {
				m_AssetTypeReferences.Remove(temp.Dequeue());
			}

			// Check Loaded AssetBundles
			foreach (var bundle in m_ManifestBundles) {
				bundle.Unref(true);
			}



#if Xaz_ASSET_DEBUG
			foreach (var bundle in m_ManifestBundles) {
				if (bundle.referenceCount > 0) {
					Debug.LogWarningFormat("Assets.UsedBundle: name={0}, ref={1}, ab={2}", bundle.name, bundle.referenceCount, bundle.assetBundle != null);
				}
			}
#endif
		}

		static private string GetAssetName(ref string path, out AssetBundleReference bundle)
		{
			if (!Path.HasExtension(path)) {
				path = Path.ChangeExtension(path, ".prefab");
			}
		
			usedAssets.Add (path);
			//Debug.Log ("===GetAssetName===" + path);

			KeyValuePair<int, int> kv;
			if (m_ManifestAssets.TryGetValue(path, out kv)) {
				bundle = m_ManifestBundles[kv.Value];
				return kv.Key == 0 ? path : (m_ManifestAssetDirectories[kv.Key - 1] + path);
			}

			bundle = null;
			return path;
		}

		static private string GetAssetPath(KeyValuePair<string, bool> pair)
		{
			if (pair.Value) {
				return XazHelper.assetsPath + "/" + pair.Key;
			}
			return XazHelper.staticAssetsPath + pair.Key;
		}

		#region 项目修改

		// wwise用通过原始文件名获得全路径
		static public string GetWwiseFullPath(string path)
		{
			string origin = path;

			KeyValuePair<string, bool> assetPath;
			if (m_StoredAssets.TryGetValue(AssetBundlePath + path, out assetPath))
			{
				// 获取热更文件路径
				if (assetPath.Value)
				{
					path = XazHelper.assetsPath + "/" + assetPath.Key;
				}
			}

			// StreamingAssets下的路径
			if (path.Equals(origin))
			{
				path = Application.streamingAssetsPath + "/" + path;
			}
			
			return path;
		}

		#endregion

		private class AssetBundleReference
		{
			private bool m_Used;
			public string name
			{
				get
				{
					return m_Bundle.name;
				}
			}
			public int referenceCount
			{
				get;
				private set;
			}
			public AssetBundle assetBundle
			{
				get;
				private set;
			}

			private Shader[] m_Shaders;
			private AssetsManifest.Bundle m_Bundle;
			private Queue<WeakReference> m_LoadedAssets;
			private Dictionary<KeyValuePair<string, System.Type>, AssetBundleRequest> m_Requests;

			public AssetBundleReference(AssetsManifest.Bundle bundle)
			{
				m_Bundle = bundle;
			}

			
			private Object LoadObjectFromAsset(string path, System.Type type, Object asset)
			{
				Object obj = null;
				if (type == typeof(GameObject)) {
					var prefab = asset as GameObject;
					if (prefab != null) {
						prefab.AddComponent<PrefabObject>().path = path;
						obj = prefab;
					}
				} else if (path.EndsWith(".prefab")) {
					// Hack: Unity5.x不能直接获取脚本对象
					var prefab = asset as GameObject;
					if (prefab != null) {
						obj = prefab.GetComponent(type);
					}
				} else {
					obj = asset;
				}
				if (obj != null) {
					CollectObject(obj);
				}
				//项目修改由于2017.4.2有单个图片加载不进来的BUG
#if UNITY_IOS
                Scheduler.Timeout(delegate()
                {
                UnloadUnusedBundle(obj);
                }, 0.02f);
#else
                UnloadUnusedBundle(obj);
#endif
				return obj;
			}

			private void UnloadUnusedBundle(Object obj)
			{
				if ((m_Requests == null || m_Requests.Count == 0) && m_Bundle.assets.Length == 1 && m_Bundle.dependents == 0) { // 只有一个资源且不被任何依赖直接unload(false)
					if (obj == null || obj.GetType() != typeof(AudioClip) || ((AudioClip)obj).loadType != AudioClipLoadType.Streaming) { // Streaming类型的AudioClip不能直接unload
						UnloadAssetBundle(false);
					}
				}
			}

			public Object LoadAsset(string path, string assetName, System.Type type)
			{
				Ref(true);
				var t = assetName.EndsWith(".prefab") ? typeof(GameObject) : type;
				Object asset = assetBundle.LoadAsset(assetName, t);
				return LoadObjectFromAsset(path, type, asset);
			}
			public void Ref(bool used)
			{
				if (!used || !m_Used) {
					foreach (var index in m_Bundle.depends) {
#if UNITY_EDITOR
						if (index >= m_ManifestBundles.Length) {
							throw new UnityException(string.Format("未找到AssetBundle[{0}]的依赖文件[index: {1}]", m_Bundle.name, index));
						}
#endif
						m_ManifestBundles[index].Ref(false);
					}
					++referenceCount;
					if (used) {
						m_Used = true;
						if (m_LoadedAssets == null) {
							m_LoadedAssets = new Queue<WeakReference>();
						}
					}
				}
				if (assetBundle == null) {
					KeyValuePair<string, bool> assetPath;
					if (m_StoredAssets.TryGetValue(m_Bundle.name, out assetPath)) {
						assetBundle = AssetBundle.LoadFromFile(GetAssetPath(assetPath));
						if (assetBundle != null && m_Bundle.name == XazHelper.XazUnityShadersName && m_Shaders == null) {
#if Xaz_ASSET_DEBUG
							Debug.LogWarningFormat("Assets.LoadAllShaders: {0}", m_Bundle.name);
#endif
							++referenceCount;
							m_Shaders = assetBundle.LoadAllAssets<Shader>();
						}
#if Xaz_ASSET_DEBUG
						if (assetBundle != null) {
							Debug.LogWarningFormat("Assets.LoadAssetBundle: {0}", m_Bundle.name);
						} else {
							throw new UnityException(string.Format("Can't Load AssetBundle File. [{0}: {1}]", assetPath, File.Exists(GetAssetPath(assetPath))));
						}
#endif
					} else {
						throw new UnityException(string.Format("Can't Find AssetBundle By Bundle. [{0}]", m_Bundle.name));
					}
				}
			}

			public void Unref(bool used)
			{
				if (used) {
					if (!m_Used)
						return;
					if (m_Requests != null && m_Requests.Count > 0)
						return;

					// 判定原始资源是否销毁
					var objRefs = m_LoadedAssets;
					for (int i = 0, count = objRefs.Count; i < count; i++) {
						var objRef = objRefs.Dequeue();
						if (IsAlive(objRef)) {
							objRefs.Enqueue(objRef);
						} else {
#if Xaz_ASSET_DEBUG
							Debug.LogWarningFormat("Unload Asset {0}", m_Bundle.name);
#endif
							m_ObjRefPool.Release(objRef);
						}
					}
					if (objRefs.Count > 0)
						return;

					// 判定场景是否存在
					if (m_Bundle.scenes.Length > 0) {
						var count = SceneManager.sceneCount;
						for (int i = 0; i < count; i++) {
							var scene = SceneManager.GetSceneAt(i);
							if (System.Array.IndexOf(m_Bundle.scenes, scene.name) >= 0)
								return;
						}
					}
					m_Used = false;
				}

#if Xaz_ASSET_DEBUG
				if (referenceCount <= 0) {
					throw new UnityException(string.Format("The reference count of AssetBundle ({0}) must be greater than zero.", m_Bundle.name));
				}
#endif
				if (--referenceCount == 0) {
					UnloadAssetBundle(true);
				}

				foreach (var path in m_Bundle.depends) {
					m_ManifestBundles[path].Unref(false);
				}
			}
			public void Clear()
			{
				m_Used = false;
				referenceCount = 0;
				if (m_Requests != null) {
					m_Requests.Clear();
				}
				m_Shaders = null;
				UnloadAssetBundle(true);
				if (m_LoadedAssets != null) {
					while (m_LoadedAssets.Count > 0) {
						m_ObjRefPool.Release(m_LoadedAssets.Dequeue());
					}
				}
			}
			private void UnloadAssetBundle(bool unloadAllLoadedObjects)
			{
				if (assetBundle != null) {
					assetBundle.Unload(unloadAllLoadedObjects);
					assetBundle = null;
#if Xaz_ASSET_DEBUG
					Debug.LogWarningFormat("Assets.UnloadAssetBundle({0}): {1}", unloadAllLoadedObjects, m_Bundle.name);
#endif
				}
			}
			public void CollectObject(Object obj)
			{
				var objRef = m_ObjRefPool.Get();
				objRef.Target = obj;
				m_LoadedAssets.Enqueue(objRef);
			}
		}

		class AssetsBehaviour : MonoBehaviour
		{
#if UNITY_EDITOR
			[UnityEditor.CustomEditor(typeof(AssetsBehaviour), true)]
			class Inspector : UnityEditor.Editor
			{
				public override void OnInspectorGUI()
				{
					foreach (var bundle in m_ManifestBundles) {
						if (bundle.referenceCount > 0) {
							UnityEditor.EditorGUILayout.LabelField(bundle.name);
						}
					}
				}
			}
#endif
		}
	}
}

#if UNITY_EDITOR
namespace XazEditor
{
    internal class EditorAssets : UnityEditor.AssetPostprocessor
	{
		private const string m_AssetFolderName = "XazAssets";
		static private List<string> m_AssetFolders = new List<string>();

		[UnityEditor.InitializeOnLoadMethod]
		static private void DetectAssetFolders()
		{
			m_AssetFolders.Clear();
			foreach (var path in Directory.GetDirectories("Assets", m_AssetFolderName, SearchOption.AllDirectories)) {
				m_AssetFolders.Add(path.Replace("\\", "/"));
			}
		}

		static private bool TryGetAssetFolderPath(string path, out string assetPath)
		{
			if (path.EndsWith("/" + m_AssetFolderName) && Directory.Exists(path)) {
				assetPath = path;
				return true;
			}

			int index = path.LastIndexOf("/" + m_AssetFolderName + "/");
			if (index > 0) {
				assetPath = path.Substring(0, index);
				return true;
			}

			assetPath = null;
			return false;
		}

		private static bool IsAssetFolderChanged(string[] assets)
		{
			string assetPath;
			foreach (var p in assets) {
				if (TryGetAssetFolderPath(p, out assetPath)) {
					if (!m_AssetFolders.Exists((path) => path == assetPath)) {
						return true;
					}
				}
			}
			return false;
		}

		private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromPath)
		{
			if (IsAssetFolderChanged(importedAssets) || IsAssetFolderChanged(deletedAssets) || IsAssetFolderChanged(movedAssets) || IsAssetFolderChanged(movedFromPath)) {
				DetectAssetFolders();
			}
		}

		static public UnityEngine.Object LoadAsset(string path, System.Type type)
		{
			UnityEngine.Object obj = null;
			if (path.StartsWith("Assets/")) {
				obj = LoadAssetAtPath(path, type);
				if (obj != null)
					return obj;
			}

			foreach (var folder in m_AssetFolders) {
				var assetPath = folder + "/" + path;
				if (File.Exists(assetPath)) {
					obj = LoadAssetAtPath(assetPath, type);
					if (obj != null)
						return obj;
				}
			}

			return null;
		}

		static public UnityEngine.Object LoadAssetAtPath(string path, System.Type type)
		{
#if UNITY_5 || UNITY_5_3_OR_NEWER
			return UnityEditor.AssetDatabase.LoadAssetAtPath(path, type);
#else
            return UnityEngine.Resources.LoadAssetAtPath(path, type);
#endif
		}
	}
}
#endif
