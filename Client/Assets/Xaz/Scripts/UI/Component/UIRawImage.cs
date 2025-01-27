//------------------------------------------------------------
// Xaz Framework
// Feedback: qq515688254
// UIRawImge组件
//-- @author xiejie
//----------------------------------------------------------------------------
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Xaz
{
    [AddComponentMenu("UI/UIRawImge", 10)]
    public class UIRawImge : RawImage
    {
        [SerializeField]
        private bool m_ShowBeforeLoad;

        [SerializeField]
        private bool m_ForceNativeSize;

        private string imgPath;

        //仅richtext用这个
        public string atlasName;
        private string curWaitSpritName;
        protected override void Awake()
        {
            base.Awake();
            imgPath = string.Empty;
        }

        public void SetInfo(string path,bool isWeb, Action callfunc = null)
        {
            if (isWeb)
            {
                SetUrlImg(path, callfunc); 
            }
            else
            {
                SetImg(path, callfunc);
            }
        }
        public void SetImg(string path, Action callfunc = null)
        {
            if (string.IsNullOrEmpty(path))
            {
                return;
            }
            if (path != this.imgPath)
            {
                this.imgPath = path;
                curWaitSpritName = path;
                Action<UnityEngine.Object> on_cfg = (asset) =>
                {
                    //避免异步加载资源后，组件已经被删除了
                    if (this == null)
                    {
                        if (callfunc != null)
                        {
                            callfunc();
                        }
                        return;
                    }
                    if (asset != null)
                    {
                        var loadspt = asset as Texture;
                        if (curWaitSpritName.IndexOf(loadspt.name)>=0)
                        {
                            texture = loadspt;
                            UpdateSprite();
                        }
                        if (!this.enabled)
                        {
                            this.enabled = true;
                        }
                    }
                    else
                    {
                        texture = null;
                        Debug.LogError("图片资源缺失：" + this.imgPath);
                    }
                    if (callfunc != null)
                    {
                        callfunc();
                    }
                };
                if (!m_ShowBeforeLoad)
                {
                    this.enabled = false;
                }
                ResMgr.LoadAssetAsync(path, typeof(Texture), on_cfg);
            }
            else
            {
                UpdateSprite();
            }
        }
        internal virtual protected void UpdateSprite()
        {
            if (m_ForceNativeSize && texture != null)
            {
                SetNativeSize();
            }
        }


        public void SetUrlImg(string url, Action callfunc = null)
        {
            if (imgPath == url)
            {
                return;
            }
            imgPath = url;
            StartCoroutine(DownloadImage(imgPath, callfunc));
        }

        private IEnumerator DownloadImage(string url, Action callfunc = null)
        {
            using (UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(url))
            {
                yield return webRequest.SendWebRequest();

                if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.LogError($"Error: {webRequest.error}");
                }
                else
                {
                    texture = ((DownloadHandlerTexture)webRequest.downloadHandler).texture;
                   // Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                }
                if (callfunc != null)
                {
                    callfunc();
                }
            }
        }
        public void ResetComp()
        {
#if UNITY_EDITOR
            imgPath = null;
            texture = null;
#endif
        }
    }
}
