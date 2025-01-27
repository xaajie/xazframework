//------------------------------------------------------------
//导表工具
// Xaz Framework
// Feedback: qq515688254
//------------------------------------------------------------
using Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using UnityEditor;
using UnityEngine;
using static DG.DemiEditor.DeGUIKey;

/// <summary>
/// Excel生成bytes和cs工具
/// </summary>
public class Excel2CsBytesTool
{
    static string ExcelDataPath = Application.dataPath + "/../ExcelData";//源Excel文件夹,xlsx格式
    static string BytesDataPathpos = "/Resources/" + XazConfig.DBFolderName;//生成的bytes文件夹
    static string BytesDataPath = Application.dataPath + BytesDataPathpos;//生成的bytes文件夹
    static string CsClassPath = Application.dataPath + "/Scripts/DataTable";//生成的c#脚本文件夹
    static string XmlDataPath = ExcelDataPath + "/tempXmlData";//生成的xml(临时)文件夹..
    static string AllCsHead = "alls";//序列化结构体的数组类.类名前缀

    static char ArrayTypeSplitChar = ';';//数组类型值拆分符: int[] 1#2#34 string[] 你好#再见 bool[] true#false ...
    static bool IsDeleteXmlInFinish = false;//生成bytes后是否删除中间文件xml
    static string intStr = "int";
    static string boolStr = "bool";

    // [MenuItem("DataTool/Excel2Bytes")]
    public static void Excel2Xml2Bytes()
    {
        Init();
        Excel2CsOrXml();
        EditorApplication.delayCall += ProcessTwo;
        //Excel2CsOrXml();
        ////生成bytes
        //WriteBytes();
    }

    private static void ProcessTwo()
    {
        WriteBytes();
        EditorApplication.delayCall -= ProcessTwo; // 确保只执行一次
    }
    public static void OpenExcelDirectory()
    {
        System.Diagnostics.Process.Start(ExcelDataPath);
    }
    static void Init()
    {
        if (!Directory.Exists(CsClassPath))
        {
            Directory.CreateDirectory(CsClassPath);
        }
        if (!Directory.Exists(XmlDataPath))
        {
            Directory.CreateDirectory(XmlDataPath);
        }
        if (!Directory.Exists(BytesDataPath))
        {
            Directory.CreateDirectory(BytesDataPath);
        }
    }

    static void WriteCs(string className, string[] names, string[] types, string[] descs)
    {
        try
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("//【DataTool】Auto Generate code");
            stringBuilder.AppendLine("using System;");
            stringBuilder.AppendLine("using System.Collections.Generic;");
            stringBuilder.AppendLine("using System.IO;");
            stringBuilder.AppendLine("using System.Runtime.Serialization.Formatters.Binary;");
            stringBuilder.AppendLine("using System.Xml.Serialization;");
            stringBuilder.AppendLine("using UnityEngine;");
            stringBuilder.Append("\n");
            stringBuilder.AppendLine("namespace Table");
            stringBuilder.AppendLine("{");
            stringBuilder.AppendLine("    [Serializable]");
            stringBuilder.AppendLine("    public class " + className);
            stringBuilder.AppendLine("    {");
            for (int i = 0; i < names.Length; i++)
            {
                stringBuilder.AppendLine("        /** " + descs[i] + "**/");
                stringBuilder.AppendLine("        [XmlAttribute(\"" + names[i] + "\")]");

                string type = types[i];
                if (type.Contains("[]"))
                {
                    //type = type.Replace("[]", "");
                    //stringBuilder.AppendLine("        public List<" + type + "> " + names[i] + ";");

                    //可选代码：
                    //用_name字段去反序列化，name取_name.item的值,直接返回list<type>。
                    //因为xml每行可能有多个数组字段，这样就多了一层变量item，所以访问的时候需要.item才能取到list<type>
                    //因此用额外的一个变量直接返回List<type>。
                    type = type.Replace("[]", "");
                    stringBuilder.AppendLine("        public List<" + type + "> " + names[i] + "");
                    stringBuilder.AppendLine("        {");
                    stringBuilder.AppendLine("            get");
                    stringBuilder.AppendLine("            {");
                    stringBuilder.AppendLine("                if (_" + names[i] + " != null)");
                    stringBuilder.AppendLine("                {");
                    stringBuilder.AppendLine("                    return _" + names[i] + ".item;");
                    stringBuilder.AppendLine("                }");
                    stringBuilder.AppendLine("                return null;");
                    stringBuilder.AppendLine("            }");
                    stringBuilder.AppendLine("        }");
                    stringBuilder.AppendLine("        [XmlElementAttribute(\"" + names[i] + "\")]");
                    stringBuilder.AppendLine("        public " + type + "Array _" + names[i] + ";");
                }
                else
                {
                    stringBuilder.AppendLine("        public " + type + " " + names[i] + ";");
                }

                stringBuilder.Append("\n");
            }
            stringBuilder.AppendLine("        public static List<" + className + "> LoadBytes()");
            stringBuilder.AppendLine("        {");
            stringBuilder.AppendLine("            TextAsset asset = Xaz.Assets.LoadAsset<TextAsset>(\"" + XazConfig.DBFolderName + "/" + className + "\");");
            stringBuilder.AppendLine("            if (asset != null)");
            stringBuilder.AppendLine("            {");
            stringBuilder.AppendLine("                BinaryFormatter binaryFormatter = new BinaryFormatter();");
            stringBuilder.AppendLine("                using (MemoryStream mStream = new MemoryStream())");
            stringBuilder.AppendLine("                {");
            stringBuilder.AppendLine("                    mStream.Write(asset.bytes, 0, asset.bytes.Length);");
            stringBuilder.AppendLine("                    mStream.Flush();");
            stringBuilder.AppendLine("                    mStream.Seek(0, SeekOrigin.Begin);");
            stringBuilder.AppendFormat("                    {0}{1} table = binaryFormatter.Deserialize(mStream) as {0}{1};", AllCsHead, className).AppendLine();
            stringBuilder.AppendLine("                    return table." + className + "s;");
            stringBuilder.AppendLine("                }");
            stringBuilder.AppendLine("              }");
            stringBuilder.AppendLine("              return null;");
            stringBuilder.AppendLine("        }");
            stringBuilder.AppendLine("    }");
            stringBuilder.Append("\n");
            stringBuilder.AppendLine("    [Serializable]");
            stringBuilder.AppendFormat("    public class {0}{1}", AllCsHead, className).AppendLine();
            stringBuilder.AppendLine("    {");
            stringBuilder.AppendLine("        public List<" + className + "> " + className + "s;");
            stringBuilder.AppendLine("    }");
            stringBuilder.AppendLine("}");

            string csPath = CsClassPath + "/" + className + ".cs";
            if (File.Exists(csPath))
            {
                File.Delete(csPath);
            }
            using (StreamWriter sw = new StreamWriter(csPath))
            {
                sw.Write(stringBuilder);
                Debug.Log("生成:" + csPath);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("写入CS失败:" + e.Message);
            throw;
        }
    }

    static void WriteXml(string className, string[] names, string[] types, List<string[]> datasList)
    {
        try
        {
            string xmlPath = XmlDataPath + "/" + className + ".xml";

            // 创建XmlWriterSettings，设置编码和缩进等属性
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = Encoding.UTF8;
            settings.Indent = true;

            // 使用XmlWriter创建XML文件
            using (XmlWriter writer = XmlWriter.Create(xmlPath, settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement(AllCsHead + className); // 根节点

                writer.WriteStartElement(className + "s"); // 类节点

                foreach (var datas in datasList)
                {
                    writer.WriteStartElement(className); // 每条数据节点

                    // 填充属性节点
                    for (int c = 0; c < datas.Length; c++)
                    {
                        string type = types[c];
                        string name = names[c];
                        string value = datas[c];

                        if (!type.Contains("[]"))
                        {
                            // 处理非数组类型字段
                            if (String.IsNullOrEmpty(value))
                            {
                                if (type == intStr)
                                {
                                    value = "-1";
                                }
                                else if (type == boolStr)
                                {
                                    value = bool.FalseString.ToLower();
                                }
                            }
                            else
                            {
                                if (type == boolStr)
                                {
                                    value = value.ToLower();
                                }
                                if (type == intStr)
                                {
                                    try
                                    {
                                        int vt = int.Parse(value);
                                    }
                                    catch (Exception)
                                    {
                                        Debug.LogError(string.Format("error:格式错误:content:{0},字段名称{1},表{2},", value, name, className));
                                        throw;
                                    }
                                }
                            }

                            // 写入属性
                            writer.WriteAttributeString(name, value);
                        }
                    }

                    // 填充子元素节点(数组类型字段)
                    for (int c = 0; c < datas.Length; c++)
                    {
                        string type = types[c];
                        if (type.Contains("[]"))
                        {
                            string name = names[c];
                            string value = datas[c];
                            string[] values = value.Split(ArrayTypeSplitChar);

                            writer.WriteStartElement(name); // 数组节点

                            if (!String.IsNullOrEmpty(value))
                            {
                                foreach (var item in values)
                                {
                                    writer.WriteStartElement("item");
                                    writer.WriteCData(item); // 使用CData包裹HTML标记内容
                                    writer.WriteEndElement();
                                }
                            }

                            writer.WriteEndElement(); // 关闭数组节点
                        }
                    }

                    writer.WriteEndElement(); // 关闭每条数据节点
                }

                writer.WriteEndElement(); // 关闭类节点
                writer.WriteEndElement(); // 关闭根节点
                writer.WriteEndDocument(); // 结束文档写入

                Debug.Log("生成文件:" + xmlPath);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("写入Xml失败:" + e.Message);
        }
    }

    static string EscapeXmlString(string input)
    {
        // 对特殊字符进行XML转义
        return System.Security.SecurityElement.Escape(input);
    }
    static void Excel2CsOrXml()
    {
        string[] excelPaths = Directory.GetFiles(ExcelDataPath, "*.xlsx");
        for (int e = 0; e < excelPaths.Length; e++)
        {
            //0.读Excel
            string className;//类型名
            List<string> names;//字段名
            List<string> types;//字段类型
            List<string> descs;//字段描述
            List<string[]> datasList;//数据

            try
            {
                string excelPath = excelPaths[e];//excel路径  
                className = Path.GetFileNameWithoutExtension(excelPath).ToLower();
                if (className.StartsWith("~$"))
                {
                    continue;
                }
                className = Regex.Replace(className, @"\[.*?\]", string.Empty);

                using (FileStream fileStream = new FileStream(excelPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (IExcelDataReader excelDataReader = ExcelReaderFactory.CreateOpenXmlReader(fileStream))
                    {
                        DataSet result = excelDataReader.AsDataSet();
                        if (className.IndexOf("(") >= 0)
                        {

                        }
                        else if (className.Equals("constant", StringComparison.OrdinalIgnoreCase))
                        {
                            DataTable table = result.Tables[0];
                            GenerateConstantsClass(table);
                        }
                        else
                        {
                            // 获取表格列数
                            int columns = result.Tables[0].Columns.Count;
                            // 获取表格行数
                            int rows = result.Tables[0].Rows.Count;
                            // 根据行列依次读取表格中的每个数据
                            names = new List<string>();
                            types = new List<string>();
                            descs = new List<string>();
                            datasList = new List<string[]>();
                            for (int r = 0; r < rows; r++)
                            {
                                List<string> curRowData = new List<string>();
                                for (int c = 0; c < columns; c++)
                                {
                                    //解析：获取第一个表格中指定行指定列的数据
                                    string value = result.Tables[0].Rows[r][c].ToString();
                                    string keyname = result.Tables[0].Rows[0][c].ToString();
                                    //清除前两行的变量名、变量类型 首尾空格
                                    if (r < 2)
                                    {
                                        value = value.TrimStart(' ').TrimEnd(' ');
                                    }

                                    if (!keyname.StartsWith("_"))
                                    {
                                        curRowData.Add(value);
                                    }
                                }
                                //解析：第一行类变量名
                                if (r == 0)
                                {
                                    names = curRowData;
                                }//解析：第二行类变量类型
                                else if (r == 1)
                                {
                                    types = curRowData;
                                }//解析：第三行类变量描述
                                else if (r == 2)
                                {
                                    descs = curRowData;
                                }//解析：第三行开始是数据
                                else
                                {
                                    datasList.Add(curRowData.ToArray());
                                }
                            }
                            //写Cs
                            WriteCs(className, names.ToArray(), types.ToArray(), descs.ToArray());
                            //写Xml
                            WriteXml(className, names.ToArray(), types.ToArray(), datasList);
                        }

                    }
                }
                AssetDatabase.Refresh();
            }
            catch (System.Exception exc)
            {
                Debug.LogError("请关闭Excel:" + exc.Message);
                return;
            }
        }

        AssetDatabase.Refresh();
    }


    private static void GenerateConstantsClass(DataTable table)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("public static class Constant");
        sb.AppendLine("{");

        foreach (DataRow row in table.Rows)
        {
            string identifier = row[0].ToString();
            string value = row[1].ToString();
            string type = row[2].ToString();

            switch (type)
            {
                case "int":
                    sb.AppendLine($"    public const int {identifier} = {value};");
                    break;
                case "string":
                    sb.AppendLine($"    public const string {identifier} = \"{value}\";");
                    break;
                case "float":
                    sb.AppendLine($"    public const float {identifier} = {value}f;");
                    break;
                case "int[]":
                    string[] intValues = value.Split(';');
                    sb.AppendLine($"    public static readonly int[] {identifier} = new int[] {{{string.Join(", ", intValues)}}};");
                    break;
                case "float[]":
                    string[] floatValues = value.Split(';');
                    sb.AppendLine($"    public static readonly float[] {identifier} = new float[] {{{string.Join(", ", floatValues.Select(v => v + "f"))}}};");
                    break;
                case "string[]":
                    string[] stringValues = value.Split(';').Select(v => $"\"{v}\"").ToArray();
                    sb.AppendLine($"    public static readonly string[] {identifier} = new string[] {{{string.Join(", ", stringValues)}}};");
                    break;
            }
        }

        sb.AppendLine("}");

        File.WriteAllText(CsClassPath + "/Constant.cs", sb.ToString());
        Debug.Log("Constants class generated successfully!");
    }
    static void WriteBytes()
    {
        string csAssemblyPath = Application.dataPath + "/../Library/ScriptAssemblies/Assembly-CSharp.dll";
        Assembly assembly = Assembly.LoadFile(csAssemblyPath);
        if (assembly != null)
        {
            Type[] types = assembly.GetTypes();
            for (int i = 0; i < types.Length; i++)
            {
                Type type = types[i];
                if (type.Namespace == "Table" && type.Name.Contains(AllCsHead))
                {
                    string className = type.Name.Replace(AllCsHead, "");

                    //读取xml数据
                    string xmlPath = XmlDataPath + "/" + className + ".xml";
                    if (!File.Exists(xmlPath))
                    {
                        Debug.LogError("Xml文件读取失败:" + xmlPath);
                        continue;
                    }

                    object table = null;
                    using (Stream reader = new FileStream(xmlPath, FileMode.Open))
                    {
                        Debug.Log("Xml文件:" + xmlPath);
                        XmlSerializer xmlSerializer = new XmlSerializer(type);
                        try
                        {
                            table = xmlSerializer.Deserialize(reader);
                        }
                        catch (Exception ex)
                        {
                            // 打印具体内容
                            reader.Position = 0; // 重置流的位置
                            using (StreamReader sr = new StreamReader(reader))
                            {
                                string xmlContent = sr.ReadToEnd();
                                Debug.LogError($"格式转化失败: {ex.Message}\nStackTrace: {ex.StackTrace}\n");
                                //Debug.LogError($"格式转化失败: {ex.Message}\nStackTrace: {ex.StackTrace}\nXML内容:\n{xmlContent}");
                            }
                            throw;
                        }
                    }

                    //obj序列化二进制
                    string bytesPath = BytesDataPath + "/" + className + ".bytes";
                    if (File.Exists(bytesPath))
                    {
                        File.Delete(bytesPath);
                    }
                    using (FileStream fileStream = new FileStream(bytesPath, FileMode.Create))
                    {
                        try
                        {
                            BinaryFormatter binaryFormatter = new BinaryFormatter();
                            binaryFormatter.Serialize(fileStream, table);
                            Debug.Log("生成:" + bytesPath);
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError($"二进制序列化失败: {ex.Message}\nStackTrace: {ex.StackTrace}");
                            throw;
                        }
                    }

                    if (IsDeleteXmlInFinish)
                    {
                        File.Delete(xmlPath);
                        Debug.Log("删除:" + xmlPath);
                    }
                }
            }
        }

        if (IsDeleteXmlInFinish)
        {
            Directory.Delete(XmlDataPath);
            Debug.Log("删除:" + XmlDataPath);
        }
    }
}
