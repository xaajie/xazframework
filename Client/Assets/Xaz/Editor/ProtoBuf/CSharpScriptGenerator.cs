//------------------------------------------------------------
// Xaz Framework
// Feedback: qq515688254
//------------------------------------------------------------
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEditor;
using System.IO;
//using static Xaz.ProtoBufExporter;

namespace XazEditor.ProtoBuf
{
	using PB = ProtoBufExporter.PB;

	public class CSharpScriptGenerator : ScriptGenerator
	{
		public CSharpScriptGenerator()
		{
			extension = "cs";
		}

		public override void Generate(Dictionary<string, PB.File> files, string scriptPath)
		{
			// Protocol.cs
			StringBuilder builder = new StringBuilder();
			builder.AppendLine("using System;");
			builder.AppendLine("using System.Reflection;");
			builder.AppendLine("using System.Collections.Generic;");
			builder.AppendLine("using Xaz;");
			builder.AppendLine("");
			string nsspace = "";
			builder.Append(nsspace).AppendLine("public partial class Protocol");
			builder.Append(nsspace).AppendLine("{");
			builder.Append(nsspace).Append("\t").AppendLine("static Dictionary<string, MethodInfo> mMessageTypes;");
			builder.Append(nsspace).AppendLine("");
			builder.Append(nsspace).Append("\t").AppendLine("static Protocol()");
			builder.Append(nsspace).Append("\t").AppendLine("{");
			builder.Append(nsspace).Append("\t\t").AppendLine("Type[] types = new Type[] { typeof(byte[]) };");
			builder.Append(nsspace).Append("\t\t").AppendLine("mMessageTypes = new Dictionary<string, MethodInfo>() {");

			foreach (KeyValuePair<string, PB.File> entry in files) {
				GenerateClass(entry.Value, entry.Key, scriptPath, null);
				foreach (KeyValuePair<string, PB.Message> kv in entry.Value.messages) {
					builder.Append(nsspace).Append("\t\t\t").AppendFormat("{{ \"{0}\", typeof({0}).GetMethod(\"Decode\", types) }},", kv.Key).AppendLine("");
				}
			}

			builder.Append(nsspace).Append("\t\t").AppendLine("};");
			builder.Append(nsspace).Append("\t").AppendLine("}");
			builder.Append(nsspace).AppendLine("");
			builder.Append(nsspace).Append("\t").AppendLine("static object[] mParamValues = new object[1];");
			builder.Append(nsspace).Append("\t").AppendLine("static public object Decode(string name, byte[] data)");
			builder.Append(nsspace).Append("\t").AppendLine("{");
			builder.Append(nsspace).Append("\t\t").AppendLine("XazEditorTools.Assert(mMessageTypes.ContainsKey(name), \"没有找到消息类型: \" + name);");
			builder.Append(nsspace).Append("\t\t").AppendLine("mParamValues[0] = data;");
			builder.Append(nsspace).Append("\t\t").AppendLine("return mMessageTypes[name].Invoke(null, mParamValues);");
			builder.Append(nsspace).Append("\t").AppendLine("}");

			builder.Append(nsspace).AppendLine("}");
			FileUtil.WriteString(scriptPath + "/Protocol.cs", builder.ToString());
		}

		static void GenerateClass(PB.File file, string path, string scriptPath, string ns = null)
		{
			StringBuilder builder = new StringBuilder();
			builder.AppendLine("using System.Collections.Generic;");
			builder.AppendLine("using System.Text;");
			builder.AppendLine("using Xaz;");
			builder.AppendLine("");
			string nsspace = "";
			if (!string.IsNullOrEmpty(ns)) {
				nsspace = "\t";
				builder.AppendLine("namespace " + ns);
				builder.AppendLine("{");
			}
			builder.Append(nsspace).AppendLine("public partial class Protocol");
			builder.Append(nsspace).AppendLine("{");
			foreach (KeyValuePair<string, PB.Enum> entry in file.enums) {
				if (entry.Key.Contains("."))
					continue;
				builder.Append(nsspace).Append("\t").AppendFormat("public enum {0}", entry.Value.name).AppendLine("");
				builder.Append(nsspace).Append("\t").AppendLine("{");
				foreach (KeyValuePair<string, int> kv in entry.Value.names) {
					builder.Append(nsspace).Append("\t\t").AppendFormat("{0} = {1},", kv.Key, kv.Value).AppendLine("");
				}
				builder.Append(nsspace).Append("\t").AppendLine("}");
			}
			foreach (KeyValuePair<string, PB.Message> entry in file.messages) {
				if (entry.Key.Contains("."))
					continue;
				GenerateMessageClass(entry.Value, builder, nsspace + "\t");
				builder.Append(nsspace).AppendLine("");
			}
			builder.Append(nsspace).AppendLine("}");
			if (!string.IsNullOrEmpty(ns)) {
				builder.AppendLine("}");
			}

			FileUtil.WriteString(scriptPath + "/Protocol(" + Path.GetFileNameWithoutExtension(path) + ").cs", builder.ToString());
		}

		static Dictionary<PB.FieldType, string> m_DefaultValues = new Dictionary<PB.FieldType, string>() {
					{ PB.FieldType.Bool, "false" },
					{ PB.FieldType.Fixed32, "0" },
					{ PB.FieldType.SFixed32, "0" },
					{ PB.FieldType.Fixed64, "0" },
					{ PB.FieldType.SFixed64, "0" },
					{ PB.FieldType.Float, "0F" },
					{ PB.FieldType.Double, "0D" },
					{ PB.FieldType.Int32, "0" },
					{ PB.FieldType.Int64, "0" },
					{ PB.FieldType.SInt32, "0" },
					{ PB.FieldType.SInt64, "0" },
					{ PB.FieldType.UInt32, "0U" },
					{ PB.FieldType.UInt64, "0UL" },
					{ PB.FieldType.String, "\"\"" },
				};
		static Dictionary<PB.FieldType, string> m_ReadMethods = new Dictionary<PB.FieldType, string>() {
					{ PB.FieldType.Bool, "ReadBoolean" },
					{ PB.FieldType.Fixed32, "ReadFixed32" },
					{ PB.FieldType.SFixed32, "ReadFixed32" },
					{ PB.FieldType.Fixed64, "ReadFixed64" },
					{ PB.FieldType.SFixed64, "ReadFixed64" },
					{ PB.FieldType.Float, "ReadFloat" },
					{ PB.FieldType.Double, "ReadDouble" },
					{ PB.FieldType.Int32, "ReadInt32" },
					{ PB.FieldType.Int64, "ReadInt64" },
					{ PB.FieldType.SInt32, "ReadSInt32" },
					{ PB.FieldType.SInt64, "ReadSInt64" },
					{ PB.FieldType.UInt32, "ReadUInt32" },
					{ PB.FieldType.UInt64, "ReadUInt64" },
					{ PB.FieldType.Bytes, "ReadBytes" },
					{ PB.FieldType.String, "ReadString" },
				};
		static Dictionary<PB.FieldType, string> m_WriteMethods = new Dictionary<PB.FieldType, string>() {
					{ PB.FieldType.Bool, "WriteBoolean" },
					{ PB.FieldType.Fixed32, "WriteFixed32" },
					{ PB.FieldType.SFixed32, "WriteFixed32" },
					{ PB.FieldType.Fixed64, "WriteFixed64" },
					{ PB.FieldType.SFixed64, "WriteFixed64" },
					{ PB.FieldType.Float, "WriteFloat" },
					{ PB.FieldType.Double, "WriteDouble" },
					{ PB.FieldType.Int32, "WriteInt32" },
					{ PB.FieldType.Int64, "WriteInt64" },
					{ PB.FieldType.SInt32, "WriteSInt32" },
					{ PB.FieldType.SInt64, "WriteSInt64" },
					{ PB.FieldType.UInt32, "WriteUInt32" },
					{ PB.FieldType.UInt64, "WriteUInt64" },
					{ PB.FieldType.Bytes, "WriteBytes" },
					{ PB.FieldType.String, "WriteString" },
				};

		static void GenerateMessageClass(PB.Message message, StringBuilder builder, string space)
		{
			if (message.isPackage) {
				builder.Append(space).AppendFormat("public partial class {0}", message.name).AppendLine("");
			} else {
				builder.Append(space).AppendFormat("public class {0}", message.name).AppendLine("");
			}
			builder.Append(space).AppendLine("{");
			foreach (PB.Enum obj in message.enums) {
				builder.Append(space + "\t").AppendFormat("public enum {0}", obj.name).AppendLine("");
				builder.Append(space + "\t").AppendLine("{");
				foreach (KeyValuePair<string, int> kv in obj.names) {
					builder.Append(space + "\t\t").AppendFormat("{0} = {1},", kv.Key, kv.Value).AppendLine("");
				}
				builder.Append(space + "\t").AppendLine("}");
				builder.AppendLine("");
			}
			foreach (PB.Message child in message.children) {
				GenerateMessageClass(child, builder, space + "\t");
				builder.AppendLine("");
			}

			int count = message.fields.Count;
			if (count > 0) {
				foreach (PB.Field field in message.fields) {
					if (field.deprecated) {
						builder.Append(space + "\t").AppendLine("[System.Obsolete(\"\", true)]");
					}
					switch (field.modifier) {
					case PB.Modifier.Optional:
					case PB.Modifier.Required:
						if (field.type == PB.FieldType.MessageType) {
							builder.Append(space + "\t").AppendFormat("public {0} {1} = new {0}();", field.nativeType, field.name).AppendLine("");
						} else if (field.type == PB.FieldType.Bytes) {
							builder.Append(space + "\t").AppendFormat("public byte[] {0} = new byte[0];", field.name).AppendLine("");
						} else if (field.type == PB.FieldType.EnumType) {
							if (field.defaultValue != null) {
								builder.Append(space + "\t").AppendFormat("public {0} {1} = {2};", field.nativeType, field.name, field.defaultValue.ToString()).AppendLine("");
							} else {
								builder.Append(space + "\t").AppendFormat("public {0} {1};", field.nativeType, field.name).AppendLine("");
							}
						} else {
							string v = m_DefaultValues[field.type];
							if (field.defaultValue != null) {
								if (field.type == PB.FieldType.String) {
									v = "\"" + field.defaultValue + "\"";
								} else {
									v = field.defaultValue.ToString();
								}
							}
							builder.Append(space + "\t").AppendFormat("public {0} {1} = {2};", field.nativeType, field.name, v).AppendLine("");
						}
						break;
					case PB.Modifier.Repeated:
						builder.Append(space + "\t").AppendFormat("public readonly List<{0}> {1} = new List<{0}>();", field.nativeType, field.name).AppendLine("");
						break;
					}
				}
				builder.AppendLine("");
				if (message.totalIndex > 0) {
					for (int j = 0; j <= Mathf.FloorToInt(message.totalIndex / 32); j++) {
						builder.Append(space + "\t").AppendLine("[System.NonSerialized]");
						builder.Append(space + "\t").AppendFormat("private uint {0} = 0;", message.GetVarName("m_Flag", j)).AppendLine("");
					}
				}
				for (int j = 0; j < count; j++) {
					PB.Field field = message.fields[j];
					if (field.deprecated) {
						continue;
					}
					builder.AppendLine("");
					builder.Append(space + "\t").AppendFormat("public bool Has{0}()", field.name.Substring(0, 1).ToUpper() + field.name.Substring(1)).AppendLine("");
					builder.Append(space + "\t").AppendLine("{");
					if (field.modifier == PB.Modifier.Repeated) {
						builder.Append(space + "\t\t").AppendFormat("return {0}.Count > 0;", field.name).AppendLine("");
					} else {
						builder.Append(space + "\t\t").AppendFormat("return ({0} & {1}) != 0;", message.GetVarName("m_Flag", Mathf.FloorToInt(field.index / 32)), (uint)(1 << field.index)).AppendLine("");
					}
					builder.Append(space + "\t").AppendLine("}");
				}
			}

			builder.AppendLine("");
			builder.Append(space + "\t").AppendLine("public byte[] GetBytes()");
			builder.Append(space + "\t").AppendLine("{");
			builder.Append(space + "\t\t").AppendFormat("return {0}.Encode(", message.name);

			int usedCount = 0;
			for (int i = 0; i < message.fields.Count; i++) {
				PB.Field field = message.fields[i];
				if (field.deprecated)
					continue;
				if (usedCount > 0) {
					builder.Append(", ");
				}
				++usedCount;
				builder.Append(field.name);
			}
			builder.AppendLine(");");
			builder.Append(space + "\t").AppendLine("}");

			builder.AppendLine("");
			builder.Append(space + "\t").AppendFormat("public {0} Clone()", message.name).AppendLine("");
			builder.Append(space + "\t").AppendLine("{");
			builder.Append(space + "\t\t").AppendFormat("return {0}.Decode(GetBytes());", message.name).AppendLine("");
			builder.Append(space + "\t").AppendLine("}");
			builder.AppendLine("");
			builder.Append(space + "\t").AppendFormat("static public {0} Decode(byte[] data)", message.name).AppendLine("");
			builder.Append(space + "\t").AppendLine("{");
			builder.Append(space + "\t\t").AppendFormat("return Decode(data, new {0}());", message.name).AppendLine("");
			builder.Append(space + "\t").AppendLine("}");
			builder.AppendLine("");
			builder.Append(space + "\t").AppendFormat("static public {0} Decode(byte[] data, {0} msg)", message.name).AppendLine("");
			builder.Append(space + "\t").AppendLine("{");
			builder.Append(space + "\t\t").AppendLine("uint header = 0;");
			builder.Append(space + "\t\t").AppendLine("Xaz.ProtoBuf.ProtoReader reader = new Xaz.ProtoBuf.ProtoReader(data);");
			builder.Append(space + "\t\t").AppendLine("while (reader.TryReadFieldHeader(out header)) {");

			if (message.fields.Count > 0) {
				builder.Append(space + "\t\t\t").AppendLine("uint tag = header >> 3;");

				usedCount = 0;
				for (int i = 0; i < message.fields.Count; i++) {
					PB.Field field = message.fields[i];
					if (field.deprecated)
						continue;
					if (usedCount == 0) {
						builder.Append(space + "\t\t\t").AppendFormat("if (tag == {0}) {{", field.tag).AppendLine("");
					} else {
						builder.AppendFormat(" else if (tag == {0}) {{", field.tag).AppendLine("");
					}
					++usedCount;

					if (field.modifier == PB.Modifier.Repeated) {
						if (field.packed) {
							builder.Append(space + "\t\t\t\t").AppendLine("int limit = (int)reader.ReadUInt32() + reader.position;");
							builder.Append(space + "\t\t\t\t").AppendLine("while (reader.position < limit) {");
							if (field.type == PB.FieldType.EnumType) {
								builder.Append(space + "\t\t\t\t\t").AppendFormat("msg.{0}.Add(({1})reader.ReadUInt32());", field.name, field.nativeType).AppendLine("");
							} else {
								builder.Append(space + "\t\t\t\t\t").AppendFormat("msg.{0}.Add(reader.{1}());", field.name, m_ReadMethods[field.type]).AppendLine("");
							}
							builder.Append(space + "\t\t\t\t").AppendLine("}");
						} else {
							if (field.type == PB.FieldType.EnumType) {
								builder.Append(space + "\t\t\t\t").AppendFormat("msg.{0}.Add(({1})reader.ReadUInt32());", field.name, field.nativeType).AppendLine("");
							} else if (field.type == PB.FieldType.MessageType) {
								builder.Append(space + "\t\t\t\t").AppendFormat("msg.{0}.Add({1}.Decode(reader.ReadBytes()));", field.name, field.nativeType).AppendLine("");
							} else {
								builder.Append(space + "\t\t\t\t").AppendFormat("msg.{0}.Add(reader.{1}());", field.name, m_ReadMethods[field.type]).AppendLine("");
							}
						}
					} else {
						builder.Append(space + "\t\t\t\t").AppendFormat("msg.{0} |= {1};", message.GetVarName("m_Flag", Mathf.FloorToInt(field.index / 32)), (uint)(1 << field.index)).AppendLine("");
						if (field.type == PB.FieldType.EnumType) {
							builder.Append(space + "\t\t\t\t").AppendFormat("msg.{0} = ({1})reader.ReadUInt32();", field.name, field.nativeType).AppendLine("");
						} else if (field.type == PB.FieldType.MessageType) {
							builder.Append(space + "\t\t\t\t").AppendFormat("{0}.Decode(reader.ReadBytes(), msg.{1});", field.nativeType, field.name).AppendLine("");
						} else {
							builder.Append(space + "\t\t\t\t").AppendFormat("msg.{0} = reader.{1}();", field.name, m_ReadMethods[field.type]).AppendLine("");
						}
					}
					builder.Append(space + "\t\t\t").Append("}");
				}

				if (usedCount == 0) {
					builder.Append(space + "\t\t\t").AppendLine("reader.Skip((Xaz.ProtoBuf.WireType)(header & 0x07));");
				} else {
					builder.AppendLine(" else {");
					builder.Append(space + "\t\t\t\t").AppendLine("reader.Skip((Xaz.ProtoBuf.WireType)(header & 0x07));");
					builder.Append(space + "\t\t\t").AppendLine("}");
				}
			} else {
				builder.Append(space + "\t\t\t").AppendLine("reader.Skip((Xaz.ProtoBuf.WireType)(header & 0x07));");
			}
			builder.Append(space + "\t\t").AppendLine("}");
			builder.Append(space + "\t\t").AppendLine("return msg;");
			builder.Append(space + "\t").AppendLine("}");
			builder.AppendLine("");
			builder.Append(space + "\t").Append("static public byte[] Encode(");

			usedCount = 0;
			for (int i = 0; i < message.fields.Count; i++) {
				PB.Field field = message.fields[i];
				if (field.deprecated)
					continue;
				if (usedCount > 0) {
					builder.Append(", ");
				}
				++usedCount;
				if (field.modifier == PB.Modifier.Repeated) {
					builder.AppendFormat("List<{0}> {1}", field.nativeType, field.name);
				} else {
					builder.AppendFormat("{0} {1}", field.nativeType, field.name);
				}
			}
			builder.AppendLine(")");
			builder.Append(space + "\t").AppendLine("{");

			if (message.fields.Count > 0) {
				string writer = message.GetVarName("writer", 0);
				builder.Append(space + "\t\t").AppendFormat("Xaz.ProtoBuf.ProtoWriter {0} = new Xaz.ProtoBuf.ProtoWriter();", writer).AppendLine("");
				foreach (PB.Field field in message.fields) {
					if (field.deprecated)
						continue;
					if (field.modifier == PB.Modifier.Repeated) {
						string objKey = message.GetVarName("obj", 0);
						builder.Append(space + "\t\t").AppendFormat("if ({0} != null && {0}.Count > 0) {{", field.name).AppendLine("");
						if (field.packed) {
							string subWriter = message.GetVarName("writer", 1);
							builder.Append(space + "\t\t\t").AppendFormat("{0}.WriteUInt32((uint){1});", writer, (field.tag << 3) | ((int)Xaz.WireType.String)).AppendLine("");
							builder.Append(space + "\t\t\t").AppendFormat("Xaz.ProtoBuf.ProtoWriter {0} = new Xaz.ProtoBuf.ProtoWriter();", subWriter).AppendLine("");
							builder.Append(space + "\t\t\t").AppendFormat("foreach ({0} {1} in {2}) {{", field.nativeType, objKey, field.name).AppendLine("");
							if (field.type == PB.FieldType.EnumType) {
								builder.Append(space + "\t\t\t\t").AppendFormat("{0}.WriteUInt32((uint){1});", subWriter, objKey).AppendLine("");
							} else {
								builder.Append(space + "\t\t\t\t").AppendFormat("{0}.{1}({2});", subWriter, m_WriteMethods[field.type], objKey).AppendLine("");
							}
							builder.Append(space + "\t\t\t").AppendLine("}");
							builder.Append(space + "\t\t\t").AppendFormat("{0}.WriteBytes({1}.GetBytes());", writer, subWriter).AppendLine("");
						} else {
							builder.Append(space + "\t\t\t").AppendFormat("foreach ({0} {1} in {2}) {{", field.nativeType, objKey, field.name).AppendLine("");
							builder.Append(space + "\t\t\t\t").AppendFormat("{0}.WriteUInt32((uint){1});", writer, (field.tag << 3) | ((int)field.wireType)).AppendLine("");
							if (field.type == PB.FieldType.EnumType) {
								builder.Append(space + "\t\t\t\t").AppendFormat("{0}.WriteUInt32((uint){1});", writer, objKey).AppendLine("");
							} else if (field.type == PB.FieldType.MessageType) {
								builder.Append(space + "\t\t\t\t").AppendFormat("{0}.WriteBytes({1}.GetBytes());", writer, objKey).AppendLine("");
							} else {
								builder.Append(space + "\t\t\t\t").AppendFormat("{0}.{1}({2});", writer, m_WriteMethods[field.type], objKey).AppendLine("");
							}
							builder.Append(space + "\t\t\t").AppendLine("}");
						}
						builder.Append(space + "\t\t").AppendLine("}");
					} else {
						if (field.type == PB.FieldType.MessageType) {
							if (field.modifier == PB.Modifier.Required) {
								builder.Append(space + "\t\t").AppendFormat("{0}.WriteUInt32({1});", writer, (field.tag << 3) | ((int)field.wireType)).AppendLine("");
								builder.Append(space + "\t\t").AppendFormat("if ({0} != null) {{", field.name).AppendLine("");
								builder.Append(space + "\t\t\t").AppendFormat("{0}.WriteBytes({1}.GetBytes());", writer, field.name).AppendLine("");
								builder.Append(space + "\t\t").AppendLine("} else {");
								builder.Append(space + "\t\t\t").AppendFormat("{0}.WriteBytes(new {1}().GetBytes());", writer, field.nativeType).AppendLine("");
								builder.Append(space + "\t\t").AppendLine("}");
							} else {
								builder.Append(space + "\t\t").AppendFormat("if ({0} != null) {{", field.name).AppendLine("");
								builder.Append(space + "\t\t\t").AppendFormat("{0}.WriteUInt32({1});", writer, (field.tag << 3) | ((int)field.wireType)).AppendLine("");
								builder.Append(space + "\t\t\t").AppendFormat("{0}.WriteBytes({1}.GetBytes());", writer, field.name).AppendLine("");
								builder.Append(space + "\t\t").AppendLine("}");
							}
						} else {
							builder.Append(space + "\t\t").AppendFormat("{0}.WriteUInt32({1});", writer, (field.tag << 3) | ((int)field.wireType)).AppendLine("");
							if (field.type == PB.FieldType.EnumType) {
								builder.Append(space + "\t\t").AppendFormat("{0}.WriteUInt32((uint){1});", writer, field.name).AppendLine("");
							} else {
								builder.Append(space + "\t\t").AppendFormat("{0}.{1}({2});", writer, m_WriteMethods[field.type], field.name).AppendLine("");
							}
						}
					}
				}
				builder.Append(space + "\t\t").AppendFormat("return {0}.GetBytes();", writer).AppendLine("");
			} else {
				builder.Append(space + "\t\t").AppendLine("return new byte[0];");
			}
			builder.Append(space + "\t").AppendLine("}");

			builder.AppendLine("");
			builder.Append(space + "\t").AppendLine("public override string ToString()");
			builder.Append(space + "\t").AppendLine("{");
			if (message.fields.Count == 0) {
				builder.Append(space + "\t\t").AppendLine("return \"{}\";");
			} else {
				builder.Append(space + "\t\t").AppendLine("StringBuilder sb = new StringBuilder(\"{\");");
				for (int i = 0; i < message.fields.Count; i++) {
					PB.Field field = message.fields[i];
					if (field.deprecated)
						continue;
					builder.Append(space + "\t\t").AppendFormat("if (Has{0}()) ", field.name.Substring(0, 1).ToUpper() + field.name.Substring(1));
					if (field.modifier == PB.Modifier.Repeated) {
						builder.AppendLine("{");
						builder.Append(space + "\t\t\t").AppendFormat("sb.Append(\"\\\"{0}\\\":[\");", field.name).AppendLine("");
						builder.Append(space + "\t\t\t").AppendFormat("foreach ({0} obj in this.{1})", field.nativeType, field.name).AppendLine("");
						builder.Append(space + "\t\t\t\t");
						if (field.type == PB.FieldType.Bytes || field.type == PB.FieldType.String) {
							builder.Append("sb.Append(Xaz.JSON.Stringify(obj))");
						} else if (field.type == PB.FieldType.Bool) {
							builder.Append("sb.Append(obj ? \"true\" : \"false\")");
						} else {
							builder.Append("sb.Append(obj.ToString())");
						}
						builder.Append(".Append(\",\");").AppendLine("");
						builder.Append(space + "\t\t\t").AppendLine("sb.Remove(sb.Length - 1, 1).Append(\"],\");");
						builder.Append(space + "\t\t").AppendLine("}");
					} else {
						builder.AppendFormat("sb.Append(\"\\\"{0}\\\":\")", field.name);
						if (field.type == PB.FieldType.Bytes || field.type == PB.FieldType.String) {
							builder.AppendFormat(".Append(Xaz.JSON.Stringify(this.{0}))", field.name);
						} else if (field.type == PB.FieldType.Bool) {
							builder.AppendFormat(".Append(this.{0} ? \"true\" : \"false\")", field.name);
						} else {
							builder.AppendFormat(".Append(this.{0}.ToString())", field.name);
						}
						builder.Append(".Append(\",\");").AppendLine("");
					}
				}
				builder.Append(space + "\t\t").AppendLine("if (sb.Length > 1) sb.Remove(sb.Length - 1, 1);");
				builder.Append(space + "\t\t").AppendLine("return sb.Append(\"}\").ToString();");
			}
			builder.Append(space + "\t").AppendLine("}");

			builder.Append(space).AppendLine("}");
		}
	}
}
