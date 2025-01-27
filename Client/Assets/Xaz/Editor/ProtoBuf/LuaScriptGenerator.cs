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

namespace XazEditor.ProtoBuf
{
	using System.Text.RegularExpressions;
	using PB = ProtoBufExporter.PB;

	public class LuaScriptGenerator : ScriptGenerator
	{
		public LuaScriptGenerator()
		{
			extension = "lua";
		}

		public override void Generate(Dictionary<string, PB.File> files, string scriptPath)
		{
			foreach (var kv in files) {
				GenerateProtoFile(kv.Key, kv.Value, scriptPath);
			}

			GenerateSendMethods(files, scriptPath);
		}

		private void GenerateProtoFile(string protoFile, PB.File pbFile, string scriptPath)
		{
			var protoName = "Proto" + Path.GetFileNameWithoutExtension(protoFile).ToUpperFirst();

			StringBuilder builder = new StringBuilder();
			builder.AppendFormat("local {0} = {{ }}", protoName).AppendLine();
			builder.AppendLine();
			foreach (var msg in pbFile.messages.Values) {
				if (msg.isPackage || msg.name.EndsWith("_S"))
					continue;
				builder.AppendFormat("function {0}.Pack{1}Msg(", protoName, Regex.Replace(msg.name.ToUpperFirst(), "_C$", ""));
				{
					int i = 0;
					foreach (var field in msg.fields) {
						if (i++ > 0)
							builder.Append(", ");
						builder.Append(field.name);
					}
				}
				builder.AppendLine(")");
				builder.Append("\t").Append("return { ");
				{
					int i = 0;
					foreach (var field in msg.fields) {
						if (i++ > 0)
							builder.Append(", ");
						builder.AppendFormat("{0} = {0}", field.name);
					}
				}
				builder.AppendLine(" }");
				builder.AppendLine("end");
				builder.AppendLine();
			}
			builder.AppendFormat("return {0}", protoName).AppendLine();

			FileUtil.WriteString(scriptPath + "/Protocol/" + protoName + ".lua", builder.ToString());
		}

		private void GenerateSendMethods(Dictionary<string, PB.File> files, string scriptPath)
		{
			List<string> protoFiles = new List<string>();

			StringBuilder builder = new StringBuilder();
			builder.AppendLine("local ProtoRequest = require(\"Net.ProtoRequest\")");
			builder.AppendLine();

			StringBuilder msgType = new StringBuilder();
			foreach (var kv in files) {
				string protoName = "Proto" + Path.GetFileNameWithoutExtension(kv.Key).ToUpperFirst();
				foreach (var msg in kv.Value.messages.Values) {
					if (msg.name.EndsWith("_S") || msg.name.EndsWith("_C")) {
						msgType.AppendFormat("Protocol.{0} = \"{1}\"", Regex.Replace(msg.fname, @"(^|\.)[a-z]", m => m.ToString().ToUpper()).Replace('.', '_'), msg.fname).AppendLine();
					}
					if (!msg.name.EndsWith("_C"))
						continue;
					if (!protoFiles.Contains(protoName)) {
						protoFiles.Add(protoName);
					}
					builder.AppendFormat("function Protocol.Send{0}(", Regex.Replace(Regex.Replace(msg.fname, @"(^|\.)[a-z]", m => m.ToString().ToUpper()), @"\.|_C$", ""));
					{
						int i = 0;
						foreach (var field in msg.fields) {
							if (i++ > 0)
								builder.Append(", ");
							builder.Append(field.name);
						}
					}
					builder.AppendLine(")");
					builder.Append("\t").AppendFormat("ProtoRequest.Request(\"{0}\", {1}.Pack{2}Msg(", msg.fname, protoName, Regex.Replace(msg.name.ToUpperFirst(), "_C$", ""));
					{
						int i = 0;
						foreach (var field in msg.fields) {
							if (i++ > 0)
								builder.Append(", ");
							builder.Append(field.name);
						}
					}
					builder.AppendLine("))");

					builder.AppendLine("end");
					builder.AppendLine();
				}
			}

			StringBuilder header = new StringBuilder();
			foreach (var protoName in protoFiles) {
				header.AppendFormat("local {0} = require(\"Net.Protocol.{0}\")", protoName).AppendLine();
			}
			header.AppendLine();
			header.AppendLine("local Protocol = {}");
			header.AppendLine();
			header.Append(msgType);
			header.AppendLine();
			header.Append(builder);
			header.AppendLine("return Protocol");

			FileUtil.WriteString(scriptPath + "/Protocol.lua", header.ToString());
		}

	}
}
