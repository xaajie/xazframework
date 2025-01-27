//------------------------------------------------------------
// Xaz Framework
// Feedback: qq515688254
//------------------------------------------------------------
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xaz
{
	/// <summary>
	/// 界面的参数数据类
	/// </summary>
	
	public class UIContext
	{
		static private readonly ObjectPool<UIContext> m_ContextPool;

		static UIContext()
		{
			m_ContextPool = new ObjectPool<UIContext>(() => new UIContext(), null, (context) => {
				context.m_ValueDict.Clear();
			});
		}

		public static UIContext Get()
		{
			return m_ContextPool.Get();
		}

		public static UIContext Get(Dictionary<string, object> values)
		{
			UIContext context = Get();
			if (values != null) {
				var map = context.m_ValueDict;
				foreach (var kv in values) {
					map[kv.Key] = kv.Value;
				}
			}
			return context;
		}

#if USE_LUA
		public static UIContext Get(SLua.LuaTable values)
		{
			UIContext context = Get();
			if (values != null) {
				var map = context.m_ValueDict;
				foreach (var pair in values) {
					map[pair.key.ToString()] = pair.value;
				}
			}
			return context;
		}
#endif

		internal static void Release(UIContext context)
		{
			if (context != null) {
				m_ContextPool.Release(context);
			}
		}

		private readonly Dictionary<string, object> m_ValueDict;

		private UIContext()
		{
			m_ValueDict = new Dictionary<string, object>();
		}

		public T Get<T>(string name)
		{
			object value;
			if (m_ValueDict.TryGetValue(name, out value)) {
				return (T)value;
			}
			return default(T);
		}

		public bool TryGet<T>(string name, out T value)
		{
			object ret;
			if (m_ValueDict.TryGetValue(name, out ret)) {
				value = (T)ret;
				return true;
			}
			value = default(T);
			return false;
		}

		public object Get(string name)
		{
			return Get<object>(name);
		}

		public bool TryGet(string name, out object value)
		{
			return TryGet<object>(name, out value);
		}

		public UIContext Set(string name, object value)
		{
			m_ValueDict[name] = value;
			return this;
		}

		public UIContext Remove(string name)
		{
			if (m_ValueDict.ContainsKey(name))
				m_ValueDict.Remove(name);

			return this;
		}

		public bool Contains(string name)
		{
			return m_ValueDict.ContainsKey(name);
		}
	}
}
