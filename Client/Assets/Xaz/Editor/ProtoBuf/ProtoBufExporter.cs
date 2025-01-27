//------------------------------------------------------------
// Xaz Framework
// Feedback: qq515688254
//------------------------------------------------------------
﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using Xaz;

namespace XazEditor
{
	namespace ProtoBuf
	{
		public abstract class ScriptGenerator
		{
			public string extension
			{
				get;
				protected set;
			}

			public abstract void Generate(Dictionary<string, ProtoBufExporter.PB.File> files, string scriptPath);
		}
	}

	public static class ProtoBufExporter
	{
		public class Token
		{
			public enum Type
			{
				Unknown,
				Whitespace,
				Newline,
				Comment,
				Operator,
				Keyword,
				Symbol,
				Identifier,
				String,
				Character,
				Number,
				End
			}

			static public readonly Token end = new Token(Type.End, "");
			static public readonly Token newline = new Token(Type.Newline, "\r\n");

			public Type type;
			public string value;

			public Token(Type type, string value)
			{
				this.type = type;
				this.value = value;
			}
		}

		class Scanner
		{
			static private Dictionary<string, bool> keywords = new Dictionary<string, bool>() {
			};

			private const char EOF = '\0';

			private int mPos;
			private string mCode;

			public Scanner(string code)
			{
				mPos = 0;
				mCode = code + EOF;
			}

			public bool IsDigit(char c)
			{
				return c >= '0' && c <= '9';
			}

			public bool IsWhitespace(char c)
			{
				return c == ' ' || c == '\t';
			}

			public bool IsIdentifierStart(char c)
			{
				return c == '_' || (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z');
			}

			public bool IsIdentifierPart(char c)
			{
				return IsIdentifierStart(c) || IsDigit(c);
			}

			public Token ParseNumber(char first, bool fraction, bool exponent)
			{
				int beginIndex = mPos;
				while (true) {
					char ch = mCode[mPos++];
					if (IsDigit(ch))
						continue;
					if (ch == '.') {
						if (fraction) {
							// error
						}
						fraction = true;
					} else if (ch == 'e' || ch == 'E') {
						if (exponent) {
							// error
						}
						exponent = true;
					} else if (ch == '+' || ch == '-') {
						if (!exponent) {
							mPos--;
							break;
						}
					} else {
						if (ch != 'f' && ch != 'F' && ch != 'd' && ch != 'D' && ch != 'm' && ch != 'M') {
							mPos--;
						}
						break;
					}
				}

				return new Token(Token.Type.Number, first + mCode.Substring(beginIndex, mPos - beginIndex));
			}

			public Token ParseString()
			{
				int beginIndex = mPos;
				while (true) {
					switch (mCode[mPos++]) {
					case EOF:
					case '\r':
					case '\n':
						break;
					case '\\':
						if (mCode[mPos] == EOF) {
							// error
						} else {
							mPos++;
						}
						break;
					case '"':
						return new Token(Token.Type.String, mCode.Substring(beginIndex, mPos - beginIndex - 1));
					}
				}
			}

			public Token ParseIdentifier(bool checkKeyword)
			{
				string identifier = "";
				int beginIndex = mPos - 1;
				while (true) {
					if (!IsIdentifierPart(mCode[mPos++])) {
						mPos--;
						identifier = mCode.Substring(beginIndex, mPos - beginIndex);
						break;
					}
				}
				if (checkKeyword && keywords.ContainsKey(identifier)) {
					return new Token(Token.Type.Keyword, identifier);
				}
				return new Token(Token.Type.Identifier, identifier);
			}

			public Token NextToken()
			{
				char ch;
				while (true) {
					switch (ch = mCode[mPos++]) {
					case EOF:
						return Token.end;
					case '\r':
						if (mCode[mPos] != '\n') {
							return Token.newline;
						}
						break;
					case '\n':
						return Token.newline;
					case '\"':
						// string
						return ParseString();
					case '.':
						// ., .0-9[f|d]
						ch = mCode[mPos++];
						if (IsDigit(ch)) {
							return ParseNumber(ch, true, false);
						}
						mPos--;
						return new Token(Token.Type.Symbol, ".");
					case '-':
						// -[0-9]
						ch = mCode[mPos++];
						if (IsDigit(ch)) {
							return ParseNumber(ch, false, false);
						}
						mPos--;
						return new Token(Token.Type.Unknown, "-");
					case '/':
						// //
						ch = mCode[mPos++];
						if (ch == '/') {
							// single comment
							int p = mPos;
							while (true) {
								switch (mCode[mPos++]) {
								case EOF:
								case '\r':
								case '\n':
									mPos--;
									return new Token(Token.Type.Comment, mCode.Substring(p, mPos - p + 1));
								}
							}
						}
						mPos--;
						return new Token(Token.Type.Unknown, "/");
					case '=':
						// =
						return new Token(Token.Type.Operator, "=");
					case '{':
					case '}':
					case '[':
					case ']':
					case ';':
						return new Token(Token.Type.Symbol, ch.ToString());
					case '0':
					case '1':
					case '2':
					case '3':
					case '4':
					case '5':
					case '6':
					case '7':
					case '8':
					case '9':
						// 0-9
						return ParseNumber(ch, false, false);
					default:
						// whitespace, identifier
						if (IsWhitespace(ch)) {
							do {
								ch = mCode[mPos++];
							} while (IsWhitespace(ch));
							mPos--;
							return new Token(Token.Type.Whitespace, "");
						} else if (IsIdentifierStart(ch)) {
							return ParseIdentifier(true);
						}
						return new Token(Token.Type.Unknown, ch.ToString());
					}
				}
			}
		}

		public class PB
		{
			public enum Modifier
			{
				Required,
				Optional,
				Repeated,
			}

			public enum FieldType
			{
				Unknown,
				Double,
				Float,
				Int32,
				Int64,
				UInt32,
				UInt64,
				SInt32,
				SInt64,
				Fixed32,
				Fixed64,
				SFixed32,
				SFixed64,
				Bool,
				String,
				Bytes,
				EnumType,
				MessageType,
			}

			public class ComplexType
			{
				public string name;
				public Message parent;

				public string fname
				{
					get
					{
						if (parent != null) {
							return parent.fname + "." + name;
						}
						return name;
					}
				}

				public bool CompareType(string name)
				{
					if (name == this.name)
						return true;
					string n = this.name;
					Message p = parent;
					while (p != null) {
						n = p.name + "." + n;
						if (n == name) {
							return true;
						}
						p = p.parent;
					}
					return false;
				}
			}

			public class Message : ComplexType
			{
				public bool isPackage = false;
				public List<Enum> enums = new List<Enum>();
				public List<Field> fields = new List<Field>();
				public List<Message> children = new List<Message>();
				public int totalIndex = 0;

				public string GetVarName(string name, int index)
				{
					string str = name + index;
					bool found = true;
					do {
						found = true;
						foreach (Field field in fields) {
							if (field.name == str) {
								str = name + (++index);
								found = false;
								break;
							}
						}
					} while (!found);
					return str;
				}

				public bool FindType(string name, ref FieldType type, ref string typeName)
				{
					if (this.name == name) {
						type = FieldType.MessageType;
						typeName = this.fname;
						return true;
					}
					foreach (Enum obj in enums) {
						if (obj.CompareType(name)) {
							type = FieldType.EnumType;
							typeName = obj.fname;
							return true;
						}
					}
					foreach (Message obj in children) {
						if (obj.CompareType(name)) {
							type = FieldType.MessageType;
							typeName = obj.fname;
							return true;
						} else if (obj.FindType(name, ref type, ref typeName)) {
							return true;
						}
					}
					return false;
				}
			}

			public class Enum : ComplexType
			{
				public bool allowAlias = false;
				public Dictionary<string, int> names = new Dictionary<string, int>();
			}

			public class Field
			{
				public FieldType type;
				public Modifier modifier;
				public int tag;
				public string typeName;
				public string name;
				public object defaultValue;
				public Token defaultValueToken;
				public bool packed;
				public bool deprecated;
				public int index;

				public WireType wireType
				{
					get
					{
						switch (type) {
						case FieldType.Bool:
						case FieldType.Int32:
						case FieldType.SInt32:
						case FieldType.Int64:
						case FieldType.SInt64:
						case FieldType.UInt32:
						case FieldType.UInt64:
						case FieldType.EnumType:
							return WireType.Variant;
						case FieldType.Fixed64:
						case FieldType.SFixed64:
						case FieldType.Double:
							return WireType.Fixed64;
						case FieldType.Float:
						case FieldType.Fixed32:
						case FieldType.SFixed32:
							return WireType.Fixed32;
						case FieldType.String:
						case FieldType.Bytes:
						case FieldType.MessageType:
							return WireType.String;
						}
						return WireType.Variant;
					}
				}

				public string nativeType
				{
					get
					{
						switch (type) {
						case FieldType.Bool:
							return "bool";
						case FieldType.Double:
							return "double";
						case FieldType.Float:
							return "float";
						case FieldType.Int32:
						case FieldType.SInt32:
						case FieldType.Fixed32:
						case FieldType.SFixed32:
							return "int";
						case FieldType.Int64:
						case FieldType.SInt64:
						case FieldType.Fixed64:
						case FieldType.SFixed64:
							return "long";
						case FieldType.String:
							return "string";
						case FieldType.UInt32:
							return "uint";
						case FieldType.UInt64:
							return "ulong";
						case FieldType.Bytes:
							return "byte[]";
						case FieldType.EnumType:
						case FieldType.MessageType:
							return typeName;
						}
						return "";
					}
				}
			}

			public class File
			{
				public string filePath;
				public Dictionary<File, bool> imports = null;
				public Dictionary<string, bool> importFiles = new Dictionary<string, bool>();
				public List<Token> tokens = new List<Token>();
				public Dictionary<string, Enum> enums = new Dictionary<string, Enum>();
				public Dictionary<string, Message> messages = new Dictionary<string, Message>();

				public File(string text)
				{
					Scanner scanner = new Scanner(text);
					while (true) {
						Token token = scanner.NextToken();
						switch (token.type) {
						case Token.Type.Comment:
						case Token.Type.Newline:
						case Token.Type.Whitespace:
							continue;
						}
						tokens.Add(token);
						if (token == Token.end)
							break;
					}

					int pos = 0;
					Message current = null;
					Enum enumValue = null;
					while (true) {
						Token token = tokens[pos++];
						if (token == Token.end)
							break;
						if (token.value == "import") {
							token = tokens[pos++];
							bool isPublic = false;
							if (token.type == Token.Type.Identifier) {
								XazEditorTools.Assert(token.value == "public", "Need 'public'.");
								token = tokens[pos++];
								isPublic = true;
							}
							XazEditorTools.Assert(token.type == Token.Type.String, "Need String Token.");
							XazEditorTools.Assert(tokens[pos++].value == ";", "Need ';'.");
							importFiles.Add(token.value, isPublic);
						} else if (token.value == "package") {
							// package
							StringBuilder sb = new StringBuilder();
							do {
								token = tokens[pos++];
								XazEditorTools.Assert(token.type == Token.Type.Identifier, "Need Identifier Token.");
								sb.Append(token.value);
								token = tokens[pos++];
								if (token.value == ";")
									break;
								XazEditorTools.Assert(token.value == ".", "Need '.'.");
								sb.Append(".");
							} while (true);
							foreach (string s in sb.ToString().Split('.')) {
								Message msg = new Message() {
									name = s, parent = current, isPackage = true
								};
								messages.Add(msg.fname, msg);
								if (current != null) {
									current.children.Add(msg);
								}
								current = msg;
							}
						} else if (token.value == "message") {
							token = tokens[pos++];
							XazEditorTools.Assert(token.type == Token.Type.Identifier, "Need Identifier Token.");
							XazEditorTools.Assert(tokens[pos++].value == "{", "Need '{'.");
							Message msg = new Message() {
								name = token.value, parent = current
							};
							messages.Add(msg.fname, msg);
							if (current != null) {
								current.children.Add(msg);
							}
							current = msg;
						} else if (token.value == "enum") {
							XazEditorTools.Assert(enumValue == null, "enum can't nestest.");
							token = tokens[pos++];
							XazEditorTools.Assert(token.type == Token.Type.Identifier, "Need Identifier Token.");
							XazEditorTools.Assert(tokens[pos++].value == "{", "Need '{'.");
							enumValue = new Enum() {
								name = token.value, parent = current
							};
							if (current != null) {
								current.enums.Add(enumValue);
							}
							enums.Add(enumValue.fname, enumValue);
						} else if (token.value == "required" || token.value == "optional" || token.value == "repeated") {
							XazEditorTools.Assert(enumValue == null, "modifier can't used in enum.");
							XazEditorTools.Assert(current != null, "No Message.");
							Modifier modifier = token.value == "required" ? Modifier.Required : (token.value == "optional" ? Modifier.Optional : Modifier.Repeated);

							StringBuilder sb = new StringBuilder();
							token = tokens[pos++];
							XazEditorTools.Assert(token.type == Token.Type.Identifier, "Need Identifier Token.");
							sb.Append(token.value);
							do {
								token = tokens[pos++];
								if (token.value != ".") {
									pos--;
									break;
								}
								sb.Append(".");
								token = tokens[pos++];
								XazEditorTools.Assert(token.type == Token.Type.Identifier, "Need Identifier Token.");
								sb.Append(token.value);
							} while (true);

							token = tokens[pos++];
							XazEditorTools.Assert(token.type == Token.Type.Identifier, "Need Identifier Token.");
							string fieldName = token.value;

							XazEditorTools.Assert(tokens[pos++].value == "=", "Need '='.");
							token = tokens[pos++];
							XazEditorTools.Assert(token.type == Token.Type.Number, "Need Number Token.");

							Field field = new Field() {
								modifier = modifier, typeName = sb.ToString(), name = fieldName, tag = Convert.ToInt32(token.value)
							};
							current.fields.Add(field);

							token = tokens[pos++];
							if (token.value == "[") {
								token = tokens[pos++];
								if (token.value == "default") {
									XazEditorTools.Assert(tokens[pos++].value == "=", "Need '='.");
									token = tokens[pos++];
									XazEditorTools.Assert(token.type != Token.Type.Symbol || token.value != "]", "Syntax Error.");
									field.defaultValueToken = token;
								} else if (token.value == "packed") {
									XazEditorTools.Assert(tokens[pos++].value == "=", "Need '='.");
									token = tokens[pos++];
									XazEditorTools.Assert(token.value == "true", "Need 'true'.");
									field.packed = true;
								} else if (token.value == "deprecated") {
									XazEditorTools.Assert(tokens[pos++].value == "=", "Need '='.");
									token = tokens[pos++];
									XazEditorTools.Assert(token.value == "true", "Need 'true'.");
									field.deprecated = true;
								}
								XazEditorTools.Assert(tokens[pos++].value == "]", "Need ']'.");
								token = tokens[pos++];
							}

							XazEditorTools.Assert(token.value == ";", "Need ';'.");
						} else if (token.value == "option") {
							XazEditorTools.Assert(enumValue != null, "'option' just support for enum.");
							token = tokens[pos++];
							XazEditorTools.Assert(token.value == "allow_alias", "Need 'allow_alias'.");
							XazEditorTools.Assert(tokens[pos++].value == "=", "Need '='.");
							token = tokens[pos++];
							if (token.value == "true") {
								enumValue.allowAlias = true;
							} else if (token.value == "false") {
								enumValue.allowAlias = false;
							} else {
								XazEditorTools.Assert("Need 'true' or 'false'.");
							}

							XazEditorTools.Assert(tokens[pos++].value == ";", "Need ';'.");
						} else if (token.value == "}") {
							if (enumValue != null) {
								enumValue = null;
							} else {
								if (current != null) {
									current = current.parent;
								}
							}
						} else if (token.type == Token.Type.Identifier) {
							XazEditorTools.Assert(enumValue != null, "Need 'enum'.");
							string enumValueName = token.value;

							XazEditorTools.Assert(tokens[pos++].value == "=", "Need '='.");

							token = tokens[pos++];
							XazEditorTools.Assert(token.type == Token.Type.Number, "Need Number Token.");
							XazEditorTools.Assert(tokens[pos++].value == ";", "Need ';'.");
							enumValue.names.Add(enumValueName, Convert.ToInt32(token.value));
						} else {
							Logger.Warning("Unknown Syntax:", token.type, token.value);
						}
					}
					XazEditorTools.Assert(enumValue == null && (current == null || current.isPackage), "Not Finished.");
				}

				public bool FindType(string name, ref FieldType type, bool any)
				{
					if (imports == null) {
						string dir = Path.GetDirectoryName(filePath);
						imports = new Dictionary<File, bool>();
						foreach (KeyValuePair<string, bool> entry in importFiles) {
							string path = Path.Combine(dir, entry.Key);
							Logger.Print("Path:", path);
							if (m_ProtoFiles.ContainsKey(path)) {
								imports[m_ProtoFiles[path]] = entry.Value;
							}
						}
					}

					bool found = false;
					if (enums.ContainsKey(name)) {
						type = FieldType.EnumType;
						found = true;
					} else if (messages.ContainsKey(name)) {
						type = FieldType.MessageType;
						found = true;
					} else {
						foreach (KeyValuePair<File, bool> entry in imports) {
							if (any || entry.Value) {
								if (entry.Key.FindType(name, ref type, false)) {
									found = true;
									break;
								}
							}
						}
					}
					return found;
				}

				public void CheckValid()
				{
					foreach (KeyValuePair<string, Message> entry in messages) {
						Message msg = entry.Value;
						int flagIndex = 0;
						foreach (Field field in msg.fields) {
							if (!field.deprecated && field.modifier != Modifier.Repeated) {
								field.index = flagIndex++;
							}
							string tname = field.typeName;
							if (tname == "double") {
								field.type = FieldType.Double;
								if (field.defaultValueToken != null) {
									XazEditorTools.Assert(field.defaultValueToken.type == Token.Type.Number, "Need Number Token.");
									field.defaultValue = Convert.ToDouble(field.defaultValueToken.value);
								}
							} else if (tname == "float") {
								field.type = FieldType.Float;
								if (field.defaultValueToken != null) {
									XazEditorTools.Assert(field.defaultValueToken.type == Token.Type.Number, "Need Number Token.");
									field.defaultValue = Convert.ToSingle(field.defaultValueToken.value);
								}
							} else if (tname == "int32") {
								field.type = FieldType.Int32;
								if (field.defaultValueToken != null) {
									XazEditorTools.Assert(field.defaultValueToken.type == Token.Type.Number, "Need Number Token.");
									field.defaultValue = Convert.ToInt32(field.defaultValueToken.value);
								}
							} else if (tname == "int64") {
								field.type = FieldType.Int64;
								if (field.defaultValueToken != null) {
									XazEditorTools.Assert(field.defaultValueToken.type == Token.Type.Number, "Need Number Token.");
									field.defaultValue = Convert.ToInt64(field.defaultValueToken.value);
								}
							} else if (tname == "uint32") {
								field.type = FieldType.UInt32;
								if (field.defaultValueToken != null) {
									XazEditorTools.Assert(field.defaultValueToken.type == Token.Type.Number, "Need Number Token.");
									field.defaultValue = Convert.ToUInt32(field.defaultValueToken.value);
								}
							} else if (tname == "uint64") {
								field.type = FieldType.UInt64;
								if (field.defaultValueToken != null) {
									XazEditorTools.Assert(field.defaultValueToken.type == Token.Type.Number, "Need Number Token.");
									field.defaultValue = Convert.ToUInt64(field.defaultValueToken.value);
								}
							} else if (tname == "sint32") {
								field.type = FieldType.SInt32;
								if (field.defaultValueToken != null) {
									XazEditorTools.Assert(field.defaultValueToken.type == Token.Type.Number, "Need Number Token.");
									field.defaultValue = Convert.ToInt32(field.defaultValueToken.value);
								}
							} else if (tname == "sint64") {
								field.type = FieldType.SInt64;
								if (field.defaultValueToken != null) {
									XazEditorTools.Assert(field.defaultValueToken.type == Token.Type.Number, "Need Number Token.");
									field.defaultValue = Convert.ToInt64(field.defaultValueToken.value);
								}
							} else if (tname == "fixed32") {
								field.type = FieldType.Fixed32;
								if (field.defaultValueToken != null) {
									XazEditorTools.Assert(field.defaultValueToken.type == Token.Type.Number, "Need Number Token.");
									field.defaultValue = Convert.ToInt32(field.defaultValueToken.value);
								}
							} else if (tname == "fixed64") {
								field.type = FieldType.Fixed64;
								if (field.defaultValueToken != null) {
									XazEditorTools.Assert(field.defaultValueToken.type == Token.Type.Number, "Need Number Token.");
									field.defaultValue = Convert.ToInt64(field.defaultValueToken.value);
								}
							} else if (tname == "sfixed32") {
								field.type = FieldType.SFixed32;
								if (field.defaultValueToken != null) {
									XazEditorTools.Assert(field.defaultValueToken.type == Token.Type.Number, "Need Number Token.");
									field.defaultValue = Convert.ToInt32(field.defaultValueToken.value);
								}
							} else if (tname == "sfixed64") {
								field.type = FieldType.SFixed64;
								if (field.defaultValueToken != null) {
									XazEditorTools.Assert(field.defaultValueToken.type == Token.Type.Number, "Need Number Token.");
									field.defaultValue = Convert.ToInt64(field.defaultValueToken.value);
								}
							} else if (tname == "bool") {
								field.type = FieldType.Bool;
								if (field.defaultValueToken != null) {
									XazEditorTools.Assert(field.defaultValueToken.value == "true" || field.defaultValueToken.value == "false", "Need 'true' or 'false'.");
									field.defaultValue = Convert.ToBoolean(field.defaultValueToken.value);
								}
							} else if (tname == "string") {
								field.type = FieldType.String;
								if (field.defaultValueToken != null) {
									XazEditorTools.Assert(field.defaultValueToken.type == Token.Type.String, "Need String Token.");
									field.defaultValue = Convert.ToString(field.defaultValueToken.value);
								}
							} else if (tname == "bytes") {
								field.type = FieldType.Bytes;
							} else {
								// enum or message
								if (!msg.FindType(tname, ref field.type, ref field.typeName)) {
									Message p = msg.parent;
									while (p != null) {
										if (p.FindType(tname, ref field.type, ref field.typeName))
											break;
										p = p.parent;
									}
									if (field.type == FieldType.Unknown) {
										if (!FindType(tname, ref field.type, true)) {
											XazEditorTools.Assert("'" + tname + "' is't a type.");
										}
									}
								}
								if (field.type == FieldType.EnumType) {
									if (field.defaultValueToken != null && field.defaultValueToken.type == Token.Type.Identifier) {
										field.defaultValue = field.typeName + "." + field.defaultValueToken.value;
									}
								}
							}
						}
						msg.totalIndex = flagIndex;
					}
				}
			}
		}

		static Dictionary<string, PB.File> m_ProtoFiles = new Dictionary<string, PB.File>();

		static public void Export(string protoPath, string scriptPath, ProtoBuf.ScriptGenerator scriptGenerator)
		{
			List<string> files = new List<string>();
			foreach (string file in Directory.GetFiles(protoPath)) {
				if (file.EndsWith(".proto")) {
					files.Add(file);
				}
			}
			if (files.Count == 0) {
				Logger.Warning("沒有找到格式为\"proto\"的协议文件");
				return;
			}

			m_ProtoFiles.Clear();
			foreach (string file in files) {
				Logger.Print("Parsing:", file);
				m_ProtoFiles[file] = new PB.File(FileUtil.ReadString(file)) {
					filePath = file
				};
			}

			foreach (KeyValuePair<string, PB.File> entry in m_ProtoFiles) {
				Logger.Print("Checking:", entry.Key);
				entry.Value.CheckValid();
			}

			if (scriptGenerator != null) {
				scriptGenerator.Generate(m_ProtoFiles, scriptPath);
			}

			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}
	}
}