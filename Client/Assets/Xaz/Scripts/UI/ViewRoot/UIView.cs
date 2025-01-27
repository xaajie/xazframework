//------------------------------------------------------------
// Xaz Framework
// Feedback: qq515688254
//------------------------------------------------------------
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Xaz
{
    public abstract class UIView
	{
		private Scheduler.Proxy m_Scheduler;

        private  Dictionary<EventEnum, EventMgr.EventHandler> eves = new Dictionary<EventEnum, EventMgr.EventHandler>() { };
        public UIView()
		{
		}

        public virtual string prefabPath
        {
            get;
        }

        public Scheduler.Proxy scheduler
		{
			get
			{
				if (m_Scheduler == null) {
					m_Scheduler = Scheduler.Proxy.Get();
				}
				return m_Scheduler;
			}
		}

		public void AddEventListener(EventEnum name, EventMgr.EventHandler callBack)
		{
			eves.Add(name, callBack);
            EventMgr.AddEventListener(name, eves[name]);
        }

		public void RemoveEventListener(EventEnum name, EventMgr.EventHandler callBack = null)
		{
            EventMgr.RemoveEventListener(name, eves[name]);
            eves.Remove(name);
        }
        public void RemoveViewListener()
        {
            foreach (var kvp in eves)
            {
				EventMgr.RemoveEventListener(kvp.Key, kvp.Value);
            }
			eves.Clear();
        }
        public UIViewRoot root
		{
			get; internal set;
		}

		public UIContext context
		{
			get; private set;
		}

		public GameObject gameObject
		{
			get; private set;
		}

		public Transform transform
		{
			get; private set;
		}

		internal UIViewSettings setting
		{
			get; private set;
		}

		public virtual bool blockBackButton
		{
			get; protected set;
		}

        virtual protected void OnBackButtonPressed()
		{
		}

		virtual protected void OnLocalized()
		{
		}

		/******************************************************\
		| View Life Circle Methods                             |
        | 界面生命周期方法
		\******************************************************/
		virtual protected void OnCreated()
		{
		}
		virtual protected void OnOpened()
		{
		}
		virtual protected void OnFocusChanged(bool focus)
		{
		}
		virtual protected void OnClosed()
		{
			RemoveViewListener();

        }
		virtual protected void OnDestroyed()
		{
		}

		/*********************************************************\
		| UI Event Handlers                                       |
		\*********************************************************/
		virtual protected void OnButtonClick(Component com){}
        // Dropdown // Slider, ScrollBar// Toggle// InputField
        virtual protected void OnValueChanged(Component com, object value){}

		// UITableView
		virtual protected void OnTableViewCellInit(UITableView tableView, UITableViewCell tablecell, object data) { }
		virtual protected void OnTableViewSelected(UITableView tableView, object data) { }
		virtual protected void OnTableViewCellClick(UITableView tableView, UITableViewCell tablecell, GameObject target, object data) { }
		virtual protected void OnTableViewCellPress(UITableView tableView, UITableViewCell tablecell, GameObject target, object data) { }
        //UIFixTableView
        virtual protected void OnFixTableViewCellInit(UIFixTableView tableView, UIFixTableViewCell tablecell, object data) { }
        virtual protected void OnFixTableViewCellClick(UIFixTableView tableView, UIFixTableViewCell tablecell, GameObject target, object data) { }
        virtual protected void OnFixTableViewCellPress(UIFixTableView tableView, UIFixTableViewCell tablecell, GameObject target, object data) { }
        public void OnSubCellInit(UITableView tableView, UITableViewCell tablecell, object data) {
			OnSubGroupCellInit(tableView as UISubGroup,tablecell,data, tableView.name.ToUpperFirst());
        }
        public void OnSubCellClick(UITableView tableView, UITableViewCell tablecell, GameObject target, object data)
        {
            OnSubGroupCellClick(tableView as UISubGroup, tablecell, target,data,tableView.name.ToUpperFirst());
        }
        virtual protected void OnSubGroupCellInit(UISubGroup tableView, UITableViewCell tablecell, object data,string SubGroupname) { }
        virtual protected void OnSubGroupCellClick(UISubGroup tableView, UITableViewCell tablecell, GameObject target, object data, string SubGroupname) { }
        //// UIFixTableView
        //virtual protected void OnTableViewCellInit(UIFixTableView tableView, UIFixTableViewCell tablecell, object data) { }
        //virtual protected void OnTableViewSelected(UIFixTableView tableView, object data) { }
        //virtual protected void OnTableViewCellClick(UIFixTableView tableView, UIFixTableViewCell tablecell, GameObject target, object data) { }

        /*******************************************************************\
		| Internal Methods (Don't call these methods)                       |
		\*******************************************************************/
        static internal void InternalCreated(UIView view, GameObject prefab)
		{
#if Xaz_DEBUG
			Debug.Log("OnCreated: " + view);
#endif
			var gameObject = GameObject.Instantiate(prefab);
			view.gameObject = gameObject;
			view.transform = gameObject.transform;
			view.setting = gameObject.GetComponent<UIViewSettings>();
			if (view.setting == null) {
				view.setting = gameObject.AddComponent<UIViewSettings>();
			}
			view.OnCreated();
		}
		static internal void InternalOpened(UIView view, UIContext context)
		{
#if Xaz_DEBUG
			Debug.Log("OnOpened: " + view);
#endif
			view.context = context;
			view.OnOpened();
		}
		static internal void InternalFocusChanged(UIView view, bool active)
		{
#if Xaz_DEBUG
			Debug.Log("OnFocusChanged: " + view + "," + active);
#endif
			var raycaster = view.gameObject.GetComponent<GraphicRaycaster>();
			if (raycaster != null) {
				raycaster.enabled = active;
			}
			view.OnFocusChanged(active);
		}
		static internal void InternalClosed(UIView view)
		{
#if Xaz_DEBUG
			Debug.Log("OnClosed: " + view);
#endif
			view.OnClosed();
			foreach (var root in view.gameObject.GetComponentsInChildren<UIViewRoot>(true)) {
				root.CloseAll(false);
			}
			view.context = null;
			if (view.m_Scheduler != null) {
				Scheduler.Proxy.Release(view.m_Scheduler);
				view.m_Scheduler = null;
			}
		}
		static internal void InternalDestroyed(UIView view)
		{
#if Xaz_DEBUG
			Debug.Log("OnDestroyed: " + view);
#endif
			view.OnDestroyed();
		}

		static internal void InternalBackButtonPressed(UIView view)
		{
			view.OnBackButtonPressed();
		}
		
		internal protected UIComponentCollection GetComponents(Component component, bool bindEvents)
		{
			var componentCollection = component.GetComponent<UIComponentCollection>();
			if (bindEvents) {
				BindEvents(componentCollection.components);
			}
			return componentCollection;
		}

		void BindEvents(List<Component> coms)
		{
			for (int i = 0, count = coms.Count; i < count; i++) {
				Component comp = coms[i];
				if (comp is Button) {
					((Button)comp).onClick.AddListener(delegate {
						OnButtonClick(comp);
					});
				} else if (comp is Slider) {
					((Slider)comp).onValueChanged.AddListener(delegate (float value) {
						OnValueChanged(comp, (object)value);
					});
				} else if (comp is Scrollbar) {
					((Scrollbar)comp).onValueChanged.AddListener(delegate (float value) {
						OnValueChanged(comp, (object)value);
					});
				} else if (comp is Toggle) {
					((Toggle)comp).onValueChanged.AddListener(delegate (bool value) {
						OnValueChanged(comp, (object)value);
					});
				} else if (comp is Dropdown) {
					((Dropdown)comp).onValueChanged.AddListener(delegate (int value) {
						OnValueChanged(comp, (object)value);
					});
				} else if (comp is InputField) {
					((InputField)comp).onValueChanged.AddListener(delegate (string value) {
						OnValueChanged(comp, (object)value);
					});
                }
                else if (comp is UITabButton){
                    ((UITabButton)comp).onValueChanged.AddListener(delegate (bool value) {
                        OnValueChanged(comp, (object)value);
                    });
                } else if (comp is UIFixTableView) {
                    UIFixTableView table = (UIFixTableView)comp;
					table.onCellInit += OnFixTableViewCellInit;
					table.onCellClick += OnFixTableViewCellClick;
					table.onCellPress += OnFixTableViewCellPress;
                }
                else if (comp is UITableView)
                {
                    UITableView table = (UITableView)comp;
                    table.onCellInit += OnTableViewCellInit;
                    table.onCellClick += OnTableViewCellClick;
                    table.onCellPress += OnTableViewCellPress;
                }

            }
		}
	}
}
