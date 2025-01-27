//------------------------------------------------------------
// Xaz Framework
// 加载固定prefab组件（编辑态可视，运行态才加载）
// Feedback: qq515688254
//------------------------------------------------------------
using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Xaz
{
    public class PrefabPathLoad : MonoBehaviour,IControl
    {
        // 导出时只保留路径字符串
        [HideInInspector]
        public string _prefabPath;

        public bool isUI = true;

        private Action<UnityEngine.Object> LoadAction;

        public delegate void LoadCallback();

        //是否改变宽高依据加载的所有内容
        public bool modifySizebyAll = false;
        //是否改变宽高依据加载的第一层级
        public bool modifySizebyOne = false;
        [SerializeField]
        private GameObject loadPrefab;
        // 在编辑器中显示预制件
#if UNITY_EDITOR
        void OnValidate()
        {
            if (loadPrefab != null)
            {
                string newpath = AssetDatabase.GetAssetPath(loadPrefab);
                if (newpath != _prefabPath)
                {
                    _prefabPath = newpath;
                    EditorApplication.update += CoroutineUpdate;
                }
            }
        }

        void CoroutineUpdate()
        {
            SetPrefabPath(_prefabPath);
            EditorApplication.update -= CoroutineUpdate;
        }



#endif


        //#endif
        public virtual void SetPrefabPath(string path = null, LoadCallback finCallback = null)
        {
            ClearLoadPrefab();
            if (string.IsNullOrEmpty(path))
            {
                path = _prefabPath;
            }
            if (!string.IsNullOrEmpty(path))
            {
                _prefabPath = path;
                LoadPrefabNow(finCallback);
            }
        }

        private bool isLoad = false;

        public void SetInit()
        {
            if (!isLoad)
            {
                LoadPrefabNow();
                isLoad = true;
            }
        }
        private void OnEnable()
        {
            SetInit();
        }
        private void LoadPrefabNow(LoadCallback finCallback = null)
        {
            if (!string.IsNullOrEmpty(_prefabPath))
            {
                LoadAction = (asset) =>
                {
                    if (asset != null)
                    {
                        loadPrefab = asset as GameObject;
                        GameObject runPrefab = Instantiate<GameObject>(loadPrefab);
                        runPrefab.transform.SetParent(this.transform);
                        runPrefab.transform.localPosition = Vector3.zero;
                        runPrefab.transform.localScale = Vector3.one;
                        if (isUI)
                        {
                            RayUtil.SetLayer(runPrefab,XazConfig.LayerDefine.UILAYER);
                        }
                        if (GetRectTrans() != null)
                        {
                            if (modifySizebyAll)
                            {
                                Bounds bounds = RectTransformUtility.CalculateRelativeRectTransformBounds(this.transform);
                                GetRectTrans().sizeDelta = new Vector2(bounds.size.x, bounds.size.y);
                            }
                            if (modifySizebyOne)
                            {
                                Transform vt = this.transform.GetChild(0);
                                RectTransform cvt = vt.GetComponent<RectTransform>();
                                if (cvt)
                                {
                                    GetRectTrans().sizeDelta = cvt.sizeDelta;
                                }
                            }
                        }
                        finCallback?.Invoke();
                        finCallback = null;
                    }
                };
                ResMgr.LoadAssetAsync(_prefabPath.Replace(XazConfig.AssetsPath, string.Empty), typeof(GameObject), LoadAction);
            }
        }

        public void ClearLoadPrefab()
        {
//#if UNITY_EDITOR
            if (loadPrefab != null)
            {
                loadPrefab = null;
            }
//#endif
            if (this.gameObject != null)
            {
                XazHelper.RemoveChildren(gameObject);
            }
        }

        private RectTransform _rect;
        public RectTransform GetRectTrans()
        {
            if (_rect == null)
            {
                _rect = GetComponent<RectTransform>();
            }
            return _rect;
        }

        void OnDestroy()
        {
            ClearLoadPrefab();
            LoadAction = null;
        }
    }
}
