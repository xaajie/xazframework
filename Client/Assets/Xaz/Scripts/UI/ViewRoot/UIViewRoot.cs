//------------------------------------------------------------
// Xaz Framework
// Feedback: qq515688254
//------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static XazConfig;

namespace Xaz
{
    public partial class UIViewRoot : MonoBehaviour
	{
		private struct State
		{
			public object type; // Type or LuaTable
			public object next; // 下一个界面的类型：Type or LuaTable
			public UIStyle? style;
			public UIContext context;
		}

		private class Entity
		{
			public enum Phase
			{
				Opening,
				Running,
				Closing
			}

			public UIView view;
			public Phase? phase;
			public bool focused;
			public UIStyle? style;
			public UIContext context;
			// async vars
			public Coroutine coroutine;
			public Xaz.Assets.Async<GameObject> async;
		}

		[SerializeField]
		private Color m_DefaultMaskColor = new Color(0, 0, 0, 0.5f);

		internal enum InvisibleMode
		{
			SetActive,
			SetLayer,
			Close,
		}

		[SerializeField]
		private InvisibleMode m_InvisibleMode = InvisibleMode.SetActive;

		[SerializeField]
		private int m_InvisibleLayer = 0;
		private bool m_EntityViewChanged = false;
		private List<Entity> m_Entities = new List<Entity>();

		void Awake()
		{
		}

		#region Entity Functions
		private Entity CreateEntity(Type type, string ident, UIStyle? style, UIContext context)
		{
			UIView view;
			Entity entity;
			if (
#if USE_LUA
				//!string.IsNullOrEmpty(ident) ? TryGet(ident, true, out entity) :
#endif
				TryGet(type, true, out entity)) {
				view = entity.view;
				if (entity.coroutine != null) {
					StopCoroutine(entity.coroutine);
					entity.coroutine = null;
				}
				m_Entities.Remove(entity);
			} else {
				view = (UIView)Activator.CreateInstance(type);
				view.root = this;
				entity = new Entity() { view = view };
			}
			entity.style = style;
			if (entity.context == null) {
				entity.context = context ?? UIContext.Get();
			}
			if (!entity.phase.HasValue || entity.phase == Entity.Phase.Closing) {
				entity.phase = Entity.Phase.Opening;
			}
			m_Entities.Add(entity);
			return entity;
		}
		private void OpenEntity(Entity entity, GameObject prefab)
		{
			CleanEntity(entity);
			var view = entity.view;
			if (prefab != null) {
				UIView.InternalCreated(view, prefab);
				view.transform.SetParent(transform, false);
				if (view.transform.GetComponent<Canvas>() == null) {
					view.gameObject.AddComponent<Canvas>();
				}
				if (view.transform.GetComponent<GraphicRaycaster>() == null) {
					view.gameObject.AddComponent<GraphicRaycaster>();
				}
			}
			else
			{
				Logger.Print("prefab == null : xz todo", prefab == null);
			}

			// apply style
			var style = entity.style.HasValue ? entity.style.Value : view.setting.style;
			if (!style.overrideColor) {
				style.maskColor = m_DefaultMaskColor;
			}
			entity.style = view.setting.style = style;

			var siblingIndex = 0;
			if (style.topmost) {
				siblingIndex = transform.childCount;
				for (int i = m_Entities.IndexOf(entity) + 1, count = m_Entities.Count; i < count; i++) {
					var e = m_Entities[i];
					if (e.phase != Entity.Phase.Closing && e.view.gameObject != null && e.style.HasValue && e.style.Value.topmost) {
						siblingIndex = e.view.transform.GetSiblingIndex();
						break;
					}
				}
			} else {
				for (int i = m_Entities.IndexOf(entity) - 1; i >= 0; i--) {
					var e = m_Entities[i];
					if (e.phase != Entity.Phase.Closing && e.view.gameObject != null && e.style.HasValue && !e.style.Value.topmost) {
						siblingIndex = e.view.transform.GetSiblingIndex() + 1;
						break;
					}
				}
			}
			SetSiblingIndex(view.transform, siblingIndex, false);
			SetViewVisible(view, true);

			// opening
			if (entity.phase == Entity.Phase.Opening) {
				UIView.InternalOpened(view, entity.context);
			}

			// running
			if (entity.phase != Entity.Phase.Closing) {
				entity.phase = Entity.Phase.Running;
			}
			m_EntityViewChanged = true;
		}
		private void CleanEntity(Entity entity)
		{
			if (entity.coroutine != null) {
				StopCoroutine(entity.coroutine);
				entity.coroutine = null;
			}
			entity.async = null;
		}
		private void DestroyEntity(Entity entity)
		{
			CleanEntity(entity);
			var view = entity.view;
			m_Entities.Remove(entity);
			if (view.gameObject != null) {
				GameObject.Destroy(entity.view.gameObject);
				UIView.InternalDestroyed(view);
			}
		}
		#endregion

		#region OpenAsync
		public void OpenAsync<T>(Action<T> callback)
			where T : UIView, new()
		{
			OpenAsync(null, null, null, callback);
		}
		public void OpenAsync<T>(UIContext context, Action<T> callback)
			where T : UIView, new()
		{
			OpenAsync(null, null, context, callback);
		}
		public void OpenAsync<T>(UIStyle style, Action<T> callback)
			where T : UIView, new()
		{
			OpenAsync(null, style, null, callback);
		}
		public void OpenAsync<T>(UIStyle style, UIContext context, Action<T> callback)
			where T : UIView, new()
		{
			OpenAsync(null, style, context, callback);
		}
		private void OpenAsync<T>(string ident, UIStyle? style, UIContext context, Action<T> callback)
			where T : UIView, new()
		{
			Entity entity = CreateEntity(typeof(T), ident, style, context);
            GameObject prefab = null;
            UIView view = entity.view;
            if (view.gameObject == null)
            {
                var coroutine = StartCoroutine(OpenAsync(entity, callback));
                if (entity.async != null)
                {
                    entity.coroutine = coroutine;
				}
			}
			else
			{
                OpenEntity(entity, prefab);
            }
		}
		private IEnumerator OpenAsync<T>(Entity entity, Action<T> callback)
			where T : UIView, new()
		{
			GameObject prefab = null;
			UIView view = entity.view;
			if (view.gameObject == null) {
				//if (entity.placeholder == null) {
				//	entity.placeholder = m_PlaceholderPool.Get();
				//} else {
				//	entity.placeholder.Show();
				//}
				if (entity.async == null) {
					entity.async =  Xaz.Assets.LoadAssetAsyncView<GameObject>(XazConfig.UIPrefabPath+view.prefabPath);
				}
				yield return entity.async;
				prefab = entity.async.asset;
			}
			OpenEntity(entity, prefab);
			if (callback != null) {
				callback((T)view);
			}
		}
        #endregion

        #region Close
        public void Close<T>(bool destroy, bool checkState=true)
			where T : UIView
		{
			Entity entity;
			if (TryGet(typeof(T), false, out entity)) {
                Close(entity, destroy, checkState, false);
            }
		}

		
		public void Close(UIView view, bool destroy = false)
		{
			Close(view, destroy, true);
		}

		private void Close(UIView view, bool destroy, bool checkState)
		{
			for (int i = m_Entities.Count - 1; i >= 0; i--) {
				Entity entity = m_Entities[i];
				if (entity.view == view) {
					Close(entity, destroy, checkState, false);
					return;
				}
			}
		}

		public void CloseAll(bool destroy)
		{
			if (destroy) {
				while (m_Entities.Count > 0) {
					Close(m_Entities.Last(), destroy, false, false);
				}
			} else {
				while (m_Entities.Count > 0) {
					int count = m_Entities.Count;
					do {
						Entity entity = m_Entities[--count];
						if (entity.phase == Entity.Phase.Closing) {
							if (count > 0)
								continue;
							return;
						}
						Close(entity, destroy, false, false);
						break;
					} while (true);
				}
			}
		}

		
		public void DestroyAll(params Type[] excludeTypes)
		{
			if (excludeTypes.Length == 0) {
				CloseAll(true);
			} else {
				while (m_Entities.Count > 0) {
					int count = m_Entities.Count;
					do {
						Entity entity = m_Entities[--count];
						var type = entity.view.GetType();
						var excluded = Array.Exists(excludeTypes, (t) => t == type);
						if (excluded) {
							if (entity.phase == Entity.Phase.Closing) {
								if (count > 0)
									continue;
								return;
							}
						}
						Close(entity, !excluded, false, false);
						break;
					} while (true);
				}
			}
		}

		private void Close(Entity entity, bool destroy, bool checkState, bool pushState)
		{
			if (entity.phase == Entity.Phase.Closing) {
				if (destroy) {
					DestroyEntity(entity);
				}
				return;
			}

			var context = entity.context;
			entity.context = null;
			entity.phase = Entity.Phase.Closing;

			var view = entity.view;
			if (view.gameObject != null) {
				if (entity.focused) {
					entity.focused = false;
					UIView.InternalFocusChanged(view, false);
				}
				m_EntityViewChanged = true;
				if (entity.phase != Entity.Phase.Closing)
					return;
				UIView.InternalClosed(view);
			} else {
				CleanEntity(entity);
			}

			if (!pushState) {
				UIContext.Release(context);
			}

			if (entity.phase == Entity.Phase.Closing) {
				if (destroy) {
					DestroyEntity(entity);
				} else if (view.gameObject != null) {
					SetViewVisible(view, false);
				}
			}
		}
		#endregion

		public bool Exists(Type vt,bool checkInvisible = false)
		{
            Entity entity;
            if (TryGet(vt, checkInvisible, out entity))
            {
				if(entity.phase != Entity.Phase.Closing)
				{
                    return true;
                }
            }
            return false;
        }

		public T Get<T>(bool checkInvisible = false)
			where T : UIView
		{
			Entity entity;
			if (TryGet(typeof(T), checkInvisible, out entity)) {
				return (T)entity.view;
			}
			return null;
		}

        public bool IsFocused<T>()
		{
			Entity entity;
			if (TryGet(typeof(T), true, out entity)) {
				return entity.focused;
			}
			return false;
		}

		public bool IsOnTop<T>()
		{
			for (int i = m_Entities.Count - 1; i >= 0; i--) {
				Entity entity = m_Entities[i];
				if (entity.phase == Entity.Phase.Running) {
                    if (entity.style.HasValue && entity.style.Value.overlapped)
                    {
                        continue;
                    }
                    else
                    {
                        if (entity.view.GetType() == typeof(T))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
				}
			}
			return false;
		}

		private bool TryGet(Type type, bool checkInvisible, out Entity entity)
		{
			for (int i = m_Entities.Count - 1; i >= 0; i--) {
				entity = m_Entities[i];
				if (entity.view.GetType() == type) {
					if (checkInvisible || entity.phase != Entity.Phase.Closing) {
						return true;
					}
					break;
				}
			}
			entity = null;
			return false;
		}

		private Layer m_MaskLayer, m_EventLayer;

		private void SetViewVisible(UIView view, bool visible)
		{
			if ((view.setting.overrideMode ? view.setting.invisibleMode : m_InvisibleMode) == InvisibleMode.SetActive) {
				view.gameObject.SetActive(visible);
			} else {
				XazHelper.SetLayer(view.gameObject, visible ? gameObject.layer : (view.setting.overrideMode ? view.setting.invisibleLayer : m_InvisibleLayer));
				var raycaster = view.gameObject.GetComponent<GraphicRaycaster>();
				if (raycaster != null)
					raycaster.enabled = visible;
			}
		}

		static private void SetSiblingIndex(Transform transform, int siblingIndex, bool below)
		{
			var oldIndex = transform.GetSiblingIndex();
			var compareIndex = siblingIndex - (below ? 1 : 0);

			if (oldIndex == compareIndex)
				return;

			if (oldIndex > siblingIndex) {
				transform.SetSiblingIndex(siblingIndex);
			} else {
				transform.SetSiblingIndex(siblingIndex - 1);
			}
		}

		private void SetViewFocused(Entity entity, ref bool focused, ref bool maskLayerUsed, ref bool eventLayerUsed)
		{
			if (entity.focused != focused) {
				entity.focused = focused;
				UIView.InternalFocusChanged(entity.view, focused);
			}
			//addby xiejie 26/6/26
			bool checkNotHideView = entity.view.gameObject.layer != LayerMask.NameToLayer(LayerDefine.UIINVISIBLE);
            var style = entity.style.Value;
			if (focused && checkNotHideView && (!style.overlapped || (!eventLayerUsed && style.popup ))) {
				if (!style.overlapped)
					focused = false;
				if (!eventLayerUsed) {
					eventLayerUsed = true;
					if (m_EventLayer == null) {
						m_EventLayer = Layer.Create<EventLayer>(transform);
					}
					m_EventLayer.Show(entity);
				}
			}
			if (!maskLayerUsed && !style.overlapped && style.maskColor.a != 0f && checkNotHideView) {
				maskLayerUsed = true;
				if (m_MaskLayer == null) {
					m_MaskLayer = Layer.Create<MaskLayer>(transform);
				}
				m_MaskLayer.Show(entity);
			}
		}

		void Update()
		{
			if (!m_EntityViewChanged)
				return;
			m_EntityViewChanged = false;

			bool focused = true, maskLayerUsed = false, eventLayerUsed = false;

			for (int i = m_Entities.Count - 1; !m_EntityViewChanged && i >= 0; i--) {
				Entity entity = m_Entities[i];
				if (entity.phase != Entity.Phase.Running || !entity.style.HasValue || !entity.style.Value.topmost)
					continue;
				SetViewFocused(entity, ref focused, ref maskLayerUsed, ref eventLayerUsed);
			}
			for (int i = m_Entities.Count - 1; !m_EntityViewChanged && i >= 0; i--) {
				Entity entity = m_Entities[i];
				if (entity.phase != Entity.Phase.Running || !entity.style.HasValue || entity.style.Value.topmost)
					continue;
				SetViewFocused(entity, ref focused, ref maskLayerUsed, ref eventLayerUsed);
			}

			if (!maskLayerUsed && m_MaskLayer != null)
				m_MaskLayer.Hide();
			if (!eventLayerUsed && m_EventLayer != null)
				m_EventLayer.Hide();
		}

#if UNITY_EDITOR || UNITY_ANDROID
		static private List<UIViewRoot> m_ViewRootList = new List<UIViewRoot>();

		void OnEnable()
		{
			if (transform.parent != null) {
				if (transform.parent.GetComponentInParent<UIViewRoot>() != null) {
					return;
				}
			}
			m_ViewRootList.Add(this);
		}

		void OnDisable()
		{
			m_ViewRootList.Remove(this);
		}

		static internal bool OnBackButtonPressed()
		{
            //modify by xiejie
            //sdk点击返回键，会弹sdk确认框，弹的同时会关掉上层界面，一旦sdk确认框选择了取消，确认框关闭后，上层界面永远无法恢复了？
            //10.4.3.1/index.php?m=bug&f=view&bugID=36107
            //foreach (var root in m_ViewRootList) {
            //    int i = root.m_Entities.Count;
            //    while (--i >= 0) {
            //        var entity = root.m_Entities[i];
            //        if (entity.phase == Entity.Phase.Opening) {
            //            return true;
            //        } else if (entity.phase == Entity.Phase.Running) {
            //            if (entity.view.blockBackButton) {
            //                UIView.InternalBackButtonPressed(entity.view);
            //                return true;
            //            } else {
            //                var style = entity.style.Value;
            //                if (!style.overlapped || style.popup) {
            //                    root.CloseAndBack(entity);
            //                    return true;
            //                }
            //            }
            //        }
            //    }
            //}
			return false;
		}
#endif
		#region Layer
		private class EventLayer : Layer
		{
			protected override void Awake()
			{
				base.Awake();
				gameObject.name = "__UI_EVENT_LAYER__";
				gameObject.AddComponent<UIRect>();
				var button = gameObject.AddComponent<Button>();
				button.transition = Selectable.Transition.None;
				button.onClick.AddListener(OnBlockerClick);
			}
			private void OnBlockerClick()
			{
				if (entity != null) {
					if (entity.style.Value.popup) {
						UIMgr.Close(entity.view);
                    }
				}
			}
		}
		private class MaskLayer : Layer
		{
			private Image m_Image;
			protected override void Awake()
			{
				base.Awake();
				gameObject.name = "__UI_MASK_LAYER__";
				m_Image = gameObject.AddComponent<Image>();
				m_Image.raycastTarget = false;
			}

			public override void Show(Entity entity)
			{
				base.Show(entity);
				m_Image.color = entity.style.Value.maskColor;
			}
		}

		private abstract class Layer : UnityEngine.EventSystems.UIBehaviour
		{
			public Entity entity
			{
				get; private set;
			}

           public void CheckLayer()
            {
                if (entity != null && RayUtil.IsUIHide(entity.view.transform))
                {
                    gameObject.SetActive(false);
                }
            }			

			public virtual void Show(Entity entity)
			{
				this.entity = entity;

                if (!RayUtil.IsUIHide(entity.view.transform))
                {
                    gameObject.SetActive(true);
                }
				SetSiblingIndex(transform, entity.view.transform.GetSiblingIndex(), true);
			}

			public virtual void Hide()
			{
				entity = null;
				gameObject.SetActive(false);
			}

			static public T Create<T>(Transform parent)
				where T : Layer
			{
				var go = new GameObject();
				var rect = go.AddComponent<RectTransform>();
				rect.SetParent(parent, false);
				rect.anchorMin = Vector2.zero;
				rect.anchorMax = Vector2.one;
				rect.sizeDelta = Vector2.zero;
				go.layer = parent.gameObject.layer;
				return go.AddComponent<T>();
			}
		}
		#endregion
	}
}
