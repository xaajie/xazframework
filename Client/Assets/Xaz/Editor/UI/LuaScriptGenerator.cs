//------------------------------------------------------------
// Xaz Framework
// Feedback: qq515688254
//------------------------------------------------------------
﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace XazEditor.UI
{
	public class LuaScriptGenerator : ScriptGenerator
	{
		private string m_BaseViewName;

		public LuaScriptGenerator() : this("UIView") { }

		public LuaScriptGenerator(string baseViewName)
		{
			extension = "lua";
			m_BaseViewName = baseViewName;
		} 

		public override void Generate(string file, List<KeyValuePair<Component, string>> properties, string prefabPath, List<UIViewExporter.UISubViewCollection> tableViews, List<UIViewExporter.UISubViewCollection> subgrops)
		{
			string className = Path.GetFileNameWithoutExtension(file);

			StringBuilder builder = new StringBuilder();

			bool hasCache = (tableViews != null && tableViews.Count > 0) ;

			builder.AppendFormat("local {0} = class(\"{0}\", {1})", className, m_BaseViewName).AppendLine();

			builder.AppendLine();
			builder.AppendLine("-- Properties");
			builder.AppendFormat("{0}.prefabPath = \"{1}\"", className, prefabPath).AppendLine();
			if (hasCache) {
				builder.AppendFormat("{0}.mCachedViews = nil", className).AppendLine();
			}
			builder.AppendLine();
			builder.AppendFormat("function {0}:OnCreated()", className).AppendLine();
			builder.Append("\t").AppendLine("UIView.OnCreated(self)");
			if (hasCache) {
				builder.Append("\t").AppendLine("self.mCachedViews = {}");
			}
			if (properties.Count > 0) {
				builder.Append("\t").AppendFormat("local coms = UIScriptableView.GetComponents(self, {0})", properties.Count).AppendLine();
				for (int i = 0; i < properties.Count; i++) {
					var kv = properties[i];
					builder.Append("\t").AppendFormat("self.m_{0} = coms[{1}]", kv.Key, i + 1).AppendLine();
				}
			}
			builder.AppendLine("end");
			builder.AppendLine();

			if (hasCache) {
				builder.AppendFormat("function {0}:OnDestroyed()", className).AppendLine();
				builder.Append("\t").AppendLine("self.mCachedViews = nil");
				builder.Append("\t").AppendLine("UIView.OnDestroyed(self)");
				builder.AppendLine("end");
				builder.AppendLine();
			}

			if (tableViews != null && tableViews.Count > 0) {
				// 通用的GetCellView方法
				builder.AppendFormat("function {0}:GetCellView(tableView, cell)", className).AppendLine();
				builder.Append("\t").AppendLine("local cellView = self.mCachedViews[cell]");
				builder.Append("\t").AppendLine("if cellView == nil then");
				builder.Append("\t\t").AppendLine("cellView = { }");
				int m = 0;
				for (int i = 0; i < tableViews.Count; i++) {
					var collection = tableViews[i];
					if (collection.views.Any((view) => view.properties.Count > 0)) {
						builder.Append("\t\t");
						if (m > 0) {
							builder.Append("else");
						}
						builder.AppendFormat("if tableView == self.m_{0} then", collection.name).AppendLine();
						if (collection.views.Count > 1) {
							int n = 0;
							for (int j = 0; j < collection.views.Count; j++) {
								var cell = collection.views[j];
								if (cell.properties.Count > 0) {
									builder.Append("\t\t\t");
									if (n > 0) {
										builder.Append("else");
									}
									builder.AppendFormat("if cell.identifier == \"{0}\" then", cell.identifier).AppendLine();
									builder.Append("\t\t\t\t").AppendFormat("local coms = UIScriptableView.GetComponents(cell, {0})", cell.properties.Count).AppendLine();
									for (int k = 0; k < cell.properties.Count; k++) {
										var kv = cell.properties[k];
										builder.Append("\t\t\t\t").AppendFormat("cellView.{0} = coms[{1}]", kv.Value.ToLowerFirst(), k + 1).AppendLine();
									}
									n++;
								}
							}
							if (n > 0) {
								builder.Append("\t\t\t").AppendLine("end");
							}
						} else {
							var cell = collection.views[0];
							builder.Append("\t\t\t").AppendFormat("local coms = UIScriptableView.GetComponents(cell, {0})", cell.properties.Count).AppendLine();
							for (int k = 0; k < cell.properties.Count; k++) {
								var kv = cell.properties[k];
								builder.Append("\t\t\t").AppendFormat("cellView.{0} = coms[{1}]", kv.Value.ToLowerFirst(), k + 1).AppendLine();
							}
						}
						m++;
					}
				}
				if (m > 0) {
					builder.Append("\t\t").AppendLine("end");
				}
				builder.Append("\t\t").AppendLine("self.mCachedViews[cell] = cellView");
				builder.Append("\t").AppendLine("end");
				builder.Append("\t").AppendLine("return cellView");
				builder.AppendLine("end");
				builder.AppendLine();
			}

			builder.AppendFormat("return {0}", className);

			FileUtil.WriteString(file, builder.ToString());
		}
	}
}
