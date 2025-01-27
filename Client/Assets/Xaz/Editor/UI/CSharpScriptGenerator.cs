//------------------------------------------------------------
// Xaz Framework
// Feedback: qq515688254
//------------------------------------------------------------
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace XazEditor.UI
{
    public class CSharpScriptGenerator : ScriptGenerator
    {
        public CSharpScriptGenerator()
        {
            extension = "cs";
        }

        private static string CompName(Component vt)
        {
            return vt.gameObject.name.ToUpperFirst();
        }

        private static string CompTypeName(Component vt)
        {
            return vt.GetType().Name;
        }
        public override void Generate(string file, List<KeyValuePair<Component, string>> properties, string prefabPath, List<UIViewExporter.UISubViewCollection> tableViews, List<UIViewExporter.UISubViewCollection> subgrops)
        {
            string className = Path.GetFileNameWithoutExtension(file);

            StringBuilder builder = new StringBuilder();
            builder.AppendLine("//------------------------------------------------------------");
            builder.AppendLine("// Xaz Framework\n// Auto Generate");
            builder.AppendLine("//------------------------------------------------------------");
            builder.AppendLine("using System.Collections.Generic;");
            builder.AppendLine("using UnityEngine;");
            builder.AppendLine("using UnityEngine.UI;");
            builder.AppendLine("using Xaz;");
            builder.AppendLine("using TMPro;");
            builder.AppendLine("");
            builder.AppendLine("");

            builder.AppendFormat("public class {0} : UIGameView", className).AppendLine();
            builder.AppendLine("{");
            if (properties.Count > 0)
            {
                foreach (var kv in properties)
                {
                    builder.Append("\t").AppendFormat("protected {0} m_{1};", CompTypeName(kv.Key), CompName(kv.Key)).AppendLine();
                }
                builder.AppendLine("");
            }
            //xiejie
            builder.Append("\t").AppendFormat("string _xprefabPath = \"{0}\";", prefabPath).AppendLine();
            builder.Append("\t").AppendLine("public override string prefabPath");
            builder.Append("\t").AppendLine("{");
            builder.Append("\t\t").AppendLine("get{ return _xprefabPath;}");
            builder.Append("\t").AppendLine("}");
            builder.AppendLine("");
            builder.Append("\t").AppendLine("sealed protected override void OnCreated()");
            builder.Append("\t").AppendLine("{");
            builder.Append("\t\t").AppendLine("base.OnCreated();");
            if (properties.Count > 0)
            {
                builder.Append("\t\t").AppendLine("var components = this.GetComponents(this.transform, true);");
                for (int i = 0; i < properties.Count; i++)
                {
                    var kv = properties[i];
                    builder.Append("\t\t").AppendFormat("this.m_{0} = components.Get<{1}>({2});", CompName(kv.Key), CompTypeName(kv.Key), i).AppendLine();
                }
            }
            builder.Append("\t").AppendLine("}");
            builder.AppendLine();
            bool hastatble = tableViews != null && tableViews.Count > 0;
            bool haschildtatble = subgrops != null && subgrops.Count > 0;
            if (hastatble)
            {
                builder.Append("\t").AppendLine("private Dictionary<Transform, object> mCachedViews = new Dictionary<Transform, object>();");
            }
            if (haschildtatble)
            {
                builder.Append("\t").AppendLine("private Dictionary<Transform, object> mCachedSubViews = new Dictionary<Transform, object>();");
            }
            builder.Append("\t").AppendLine("protected override void OnDestroyed()");
            builder.Append("\t").AppendLine("{");
            if (hastatble)
            {
                builder.Append("\t\t").AppendLine("mCachedViews.Clear();");
            }
            if (haschildtatble)
            {
                builder.Append("\t\t").AppendLine("mCachedSubViews.Clear();");
            }
            builder.Append("\t\t").AppendLine("base.OnDestroyed();");
            builder.Append("\t").AppendLine("}");

            if (hastatble)
            {
                builder.AppendLine();
                builder.Append("\t").AppendLine("protected interface Cell { };");
                builder.Append("\t").AppendLine("protected Cell GetCellView(BaseTable tableView, BaseTableCell tableCell)");
                builder.Append("\t").AppendLine("{");
                builder.Append("\t\t").AppendLine("object cell = null;");
                builder.Append("\t\t").AppendLine("if (mCachedViews.TryGetValue(tableCell.transform, out cell))");
                builder.Append("\t\t\t").AppendLine("return (Cell)cell;");
                for (int i = 0; i < tableViews.Count; i++)
                {
                    var collection = tableViews[i];
                    builder.Append("\t\t");
                    if (i > 0)
                    {
                        builder.Append(" else ");
                    }
                    builder.AppendFormat("if (tableView == m_{0})", collection.name);
                    builder.Append("\t\t").AppendLine("{");
                    builder.Append("\t\t\t").AppendFormat("cell = TV_{0}.Get(tableCell);", collection.name).AppendLine();
                    //嵌套list不支持多cell样式？
                    List<KeyValuePair<Component, string>> props = collection.views[0].properties;
                    if (props.Count > 0)
                    {
                        string hasproChildGroupName = null;
                        foreach (var p in props)
                        {
                            if (CompTypeName(p.Key) == "UISubGroup")
                            {
                                hasproChildGroupName = CompName(p.Key);
                                break;
                            }
                        }
                        if (hasproChildGroupName!=null)
                        {
                            builder.AppendFormat("TV_{0}.Cell0 cellv = cell as TV_{0}.Cell0;", collection.name).AppendLine();
                            builder.AppendFormat("cellv.{0}.onCellInit = OnSubCellInit;", hasproChildGroupName).AppendLine();
                            builder.AppendFormat("cellv.{0}.onCellClick = OnSubCellClick;", hasproChildGroupName).AppendLine();
                        }
                    }
                    builder.Append("\t\t").AppendLine("}");
                }
                builder.Append("\t\t").AppendLine("mCachedViews[tableCell.transform] = cell;");
                builder.Append("\t\t").AppendLine("return (Cell)cell;");
                builder.Append("\t").AppendLine("}");

                foreach (var collection in tableViews)
                {
                    builder.Append("\t").AppendFormat("protected class TV_{0}", collection.name).AppendLine();
                    builder.Append("\t").AppendLine("{");
                    for (int j = 0; j < collection.views.Count; j++)
                    {
                        var cell = collection.views[j];
                        builder.Append("\t\t").AppendFormat("public static string CELLSTR_{0} = \"{0}\";", cell.identifier).AppendLine();
                        builder.Append("\t\t").AppendFormat("public class Cell{0} : Cell", j).AppendLine();
                        builder.Append("\t\t").AppendLine("{");
                        foreach (var kv in cell.properties)
                        {
                            builder.Append("\t\t\t").AppendFormat("public {0} {1};", CompTypeName(kv.Key), CompName(kv.Key)).AppendLine();
                        }
                        builder.Append("\t\t").AppendLine("}");
                    }

                    builder.AppendLine("");
                    builder.Append("\t\t").AppendFormat("static public Cell Get(BaseTableCell tableCell)").AppendLine();
                    builder.Append("\t\t").AppendLine("{");
                    builder.Append("\t\t\t").AppendLine("Cell cell = null;");
                    builder.Append("\t\t\t");
                    for (int n = 0; n < collection.views.Count; n++)
                    {
                        var cell = collection.views[n];
                        if (n > 0)
                        {
                            builder.Append(" else ");
                        }
                        builder.AppendFormat("if (tableCell.identifier == CELLSTR_{0}) {{", cell.identifier).AppendLine();
                        builder.Append("\t\t\t\t").AppendFormat("//TV_{0}.Cell{1} cell = this.GetCellView(tableView, tableCell)  as TV_{0}.Cell{1};", collection.name,n).AppendLine();
                        builder.Append("\t\t\t\t").AppendFormat("var cell{0} = new Cell{0}();", n).AppendLine();
                        List<KeyValuePair<Component, string>> props = cell.properties;
                        if (props.Count > 0)
                        {
                            builder.Append("\t\t\t\t").AppendLine("var components = tableCell.transform.GetComponent<UIComponentCollection>();");
                            int c = 0;
                            foreach (var p in props)
                            {
                                builder.Append("\t\t\t\t").AppendFormat("cell{0}.{1} = components.Get<{2}>({3});", n, CompName(p.Key), CompTypeName(p.Key), c++).AppendLine();
                            }
                        }
                        builder.Append("\t\t\t\t").AppendFormat("cell = cell{0};", n).AppendLine();
                        builder.Append("\t\t\t").Append("}");
                    }
                    builder.AppendLine();
                    builder.Append("\t\t\t").AppendLine("return cell;");
                    builder.Append("\t\t").AppendLine("}");
                    builder.Append("\t").AppendLine("}");
                }
            }

            //嵌套list
            if (haschildtatble)
            {
                builder.AppendLine();
                builder.Append("\t").AppendLine("protected Cell GetSubCellView(UISubGroup tableView, BaseTableCell tableCell,string SubGroupname)");
                builder.Append("\t").AppendLine("{");
                builder.Append("\t\t").AppendLine("object cell = null;");
                builder.Append("\t\t").AppendLine("if (mCachedSubViews.TryGetValue(tableCell.transform, out cell))");
                builder.Append("\t\t\t").AppendLine("return (Cell)cell;");
                for (int i = 0; i < subgrops.Count; i++)
                {
                    var collection = subgrops[i];
                    builder.Append("\t\t");
                    if (i > 0)
                    {
                        builder.Append(" else ");
                    }
                    builder.AppendFormat("if (SubGroupname == TV_{0}.NAMESTR )", collection.name);
                    builder.Append("\t\t").AppendLine("{");
                    builder.Append("\t\t\t").AppendFormat("cell = TV_{0}.Get(tableCell);", collection.name).AppendLine();
                    builder.Append("\t\t").AppendLine("}");
                }
                builder.Append("\t\t").AppendLine("mCachedViews[tableCell.transform] = cell;");
                builder.Append("\t\t").AppendLine("return (Cell)cell;");
                builder.Append("\t").AppendLine("}");

                foreach (var collection in subgrops)
                {
                    builder.Append("\t").AppendFormat("protected class TV_{0}", collection.name).AppendLine();
                    builder.Append("\t").AppendLine("{");
                    for (int j = 0; j < collection.views.Count; j++)
                    {
                        var cell = collection.views[j];
                        builder.Append("\t\t").AppendFormat("public static string CELLSTR_{0} = \"{0}\";", cell.identifier).AppendLine();
                        builder.Append("\t\t").AppendFormat("public static string NAMESTR = \"{0}\";", collection.name).AppendLine();
                        builder.Append("\t\t").AppendFormat("public class Cell{0} : Cell", j).AppendLine();
                        builder.Append("\t\t").AppendLine("{");
                        foreach (var kv in cell.properties)
                        {
                            builder.Append("\t\t\t").AppendFormat("public {0} {1};", CompTypeName(kv.Key), CompName(kv.Key)).AppendLine();
                        }
                        builder.Append("\t\t").AppendLine("}");
                    }

                    builder.AppendLine("");
                    builder.Append("\t\t").AppendFormat("static public Cell Get(BaseTableCell tableCell)").AppendLine();
                    builder.Append("\t\t").AppendLine("{");
                    builder.Append("\t\t\t").AppendLine("Cell cell = null;");
                    builder.Append("\t\t\t");
                    for (int n = 0; n < collection.views.Count; n++)
                    {
                        var cell = collection.views[n];
                        if (n > 0)
                        {
                            builder.Append(" else ");
                        }
                        builder.AppendFormat("if (tableCell.identifier == CELLSTR_{0}) {{", cell.identifier).AppendLine();
                        builder.Append("\t\t\t\t").AppendFormat("var cell{0} = new Cell{0}();", n).AppendLine();
                        List<KeyValuePair<Component, string>> props = cell.properties;
                        if (props.Count > 0)
                        {
                            builder.Append("\t\t\t\t").AppendLine("var components = tableCell.transform.GetComponent<UIComponentCollection>();");
                            int c = 0;
                            foreach (var p in props)
                            {
                                builder.Append("\t\t\t\t").AppendFormat("cell{0}.{1} = components.Get<{2}>({3});", n, CompName(p.Key), CompTypeName(p.Key), c++).AppendLine();
                            }
                        }
                        builder.Append("\t\t\t\t").AppendFormat("cell = cell{0};", n).AppendLine();
                        builder.Append("\t\t\t").Append("}");
                    }
                    builder.AppendLine();
                    builder.Append("\t\t\t").AppendLine("return cell;");
                    builder.Append("\t\t").AppendLine("}");
                    builder.Append("\t").AppendLine("}");
                }
            }
            builder.AppendLine("}");

            FileUtil.WriteString(file, builder.ToString());
        }
    }

}
