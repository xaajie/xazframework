//------------------------------------------------------------
// Xaz Framework
// Feedback: qq515688254
// UIImage组件
//-- @author xiejie
//----------------------------------------------------------------------------
using System;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

namespace Xaz
{
    [AddComponentMenu("UI/UIImage", 10)]
    public class UIImage : Image
    {
        [SerializeField]
        internal protected SpriteAtlas m_Atlas;

        [SerializeField]
        internal protected string m_SpriteName;

        [SerializeField]
        private bool m_ForceNativeSize;

        [SerializeField]
        private bool m_ShowBeforeLoad;

        //仅richtext用这个
        public string atlasName;
        protected override void Awake()
        {
            base.Awake();
            m_SpriteName = string.Empty;
        }

        public void SetSprite(string atlas, string spriteName, Action callfunc = null)
        {
            m_SpriteName = spriteName;
            if (atlas == string.Empty)
            {
                atlas = atlasName;
            }
            if (!string.IsNullOrEmpty(atlas) && atlas != this.atlasName)
            {
                this.atlasName = atlas;
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
                        m_Atlas = asset as SpriteAtlas;
                        if (!this.enabled)
                        {
                            this.enabled = true;
                        }
                    }
                    UpdateSprite();
                    if (callfunc != null)
                    {
                        callfunc();
                    }
                };
                if (!m_ShowBeforeLoad)
                {
                    this.enabled = false;
                }
                ResMgr.LoadSpriteAtlas(atlas, on_cfg);
            }
            else
            {
                UpdateSprite();
                if (callfunc != null)
                {
                    callfunc();
                }
            }
        }
        internal virtual protected void UpdateSprite()
        {
            if (m_Atlas == null || string.IsNullOrEmpty(m_SpriteName))
            {
                sprite = null;
            }
            else
            {
                sprite = m_Atlas.GetSprite(m_SpriteName);
                if (m_ForceNativeSize && sprite != null)
                {
                    SetNativeSize();
                }
            }
        }

        public void ResetComp()
        {
#if UNITY_EDITOR
            m_Atlas = null;
            m_SpriteName = string.Empty;
            sprite = null;
#endif
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            if (!Application.isPlaying && m_SpriteName != null)
            {
                m_SpriteName = m_SpriteName.Replace("(Clone)", "");
                if (m_Atlas != null)
                {
                    sprite = m_Atlas.GetSprite(m_SpriteName);
                }
                UpdateSprite();
                base.OnValidate();
            }
        }
#endif
    }
}
