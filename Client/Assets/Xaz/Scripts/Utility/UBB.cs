//------------------------------------------------------------
// Xaz Framework
// Feedback: qq515688254
//------------------------------------------------------------
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
namespace Xaz{
	internal static class Ext {
		/// <summary>
		/// Extracts a string from a piece of string between index start and index end
		/// </summary>
		/// <param name="str"></param>
		/// <param name="indexStart"></param>
		/// <param name="indexEnd"></param>
		/// <returns></returns>
		[DebuggerStepThrough]
		public static string Extract(this string str, int indexStart, int indexEnd, bool inclusive) {
			if (str == null)
				return null;
			
			if (inclusive) {
				return str.Substring(indexStart, indexEnd - indexStart);
			}
			else {
				return str.Substring(indexStart + 1, indexEnd - indexStart - 1);
			}
		}
	}
	/// <summary>
	/// Provides dynamic methods to construct a bb code document
	/// </summary>
	public class BBDocument : BBNode {
		public List<BBFormattingError> Errors {
			get;
			private set;
		}
		/// <summary>
		/// Creates a new BB Document
		/// </summary>
		public BBDocument()
		: base(BBNodeTypes.Document) {
			this.Errors = new List<BBFormattingError>();
			base.Document = this;
		}
		/// <summary>
		/// Loads a BBDocument from an input string
		/// </summary>
		/// <param name="input"></param>
		public void Load(string input) {
			if (input == null)
				throw new ArgumentNullException("input");
			this.InnerText = input;
			base.ParseInner(input);
		}
		
		/// <summary>
		/// Saves the current BBDocument to a string
		/// </summary>
		/// <returns></returns>
		public string Save() {
			StringBuilder sb = new StringBuilder();
			foreach (var sub in this.ChildNodes) {
				sb.Append(sub.ToString());
			}
			return sb.ToString();
		}
	}
	public class BBFormattingError {
		public string Error {
			get;
			internal set;
		}
	}
	/// <summary>
	/// The type of BBNode
	/// </summary>
	public enum BBNodeTypes {
		/// <summary>
		/// Text only
		/// </summary>
		Text,
		/// <summary>
		/// Contains a tag element
		/// </summary>
		Tag,
		/// <summary>
		/// Document element
		/// </summary>
		Document
	}
	/// <summary>
	/// Represents a basic BB Node
	/// </summary>
	public class BBNode {
		/// <summary>
		/// Contains a list of child nodes this element has
		/// </summary>
		public BBNodeList ChildNodes {
			get;
			protected set;
		}
		/// <summary>
		/// Contains a reference to the parent bb node
		/// </summary>
		public BBNode ParentNode {
			get;
			protected set;
		}
		/// <summary>
		/// Contains the inner node text, excluding the [bbcode]
		/// </summary>
		public virtual string InnerText {
			get {
				return StripBBCode(this.InnerCode);
			}
			set {
				this.InnerCode = value;
			}
		}
		/// <summary>
		/// Contains the inner node code, including the [bbcode]
		/// </summary>
		public string InnerCode {
			get;
			set;
		}
		/// <summary>
		/// The lower-case name of the node
		/// </summary>
		public string Name {
			get;
			protected set;
		}
		/// <summary>
		/// Contains a collection of attributes of the node. Attributes are parsed separated by a comma.
		/// Ex: [url=attr0,attr1,attr2]google.com[/url]
		/// </summary>
		public BBNodeAttributeList Attributes {
			get;
			protected set;
		}
		/// <summary>
		/// Node type, whether it's a bb element, text, or document
		/// </summary>
		public BBNodeTypes Type {
			get;
			protected set;
		}
		/// <summary>
		/// Contains the reference to the bbdocument
		/// </summary>
		public BBDocument Document {
			get;
			protected set;
		}
		/// <summary>
		/// Creates a new instance of a bbnode with reference to a parent, and a type
		/// </summary>
		/// <param name="parentNode"></param>
		/// <param name="type"></param>
		public BBNode(BBNode parentNode, BBNodeTypes type)
		: this(type) {
			this.ParentNode = parentNode;
		}
		/// <summary>
		/// Creates a new bbnode with a type
		/// </summary>
		/// <param name="type"></param>
		public BBNode(BBNodeTypes type) {
			this.ParentNode = null;
			this.Type = type;
			this.ChildNodes = new BBNodeList();
			this.Attributes = new BBNodeAttributeList();
		}
		public override string ToString() {
			StringBuilder sb = new StringBuilder();
			if (this.Type == BBNodeTypes.Text)
				return this.InnerText;
			if (this.Attributes != null && this.Attributes.Count > 0) {
				StringBuilder attr = new StringBuilder();
				for (int i = 0; i < this.Attributes.Count; i++) {
					attr.Append(this.Attributes[i].Value);
					if (i + 1 < this.Attributes.Count)
						attr.Append(',');
				}
				sb.AppendFormat("[{0}={1}]", this.Name, attr.ToString());
			}
			else {
				sb.AppendFormat("[{0}]", this.Name);
			}
			if (this.ChildNodes != null && this.ChildNodes.Count > 0) {
				foreach (var sub in this.ChildNodes) {
					sb.Append(sub.ToString());
				}
			}
			sb.AppendFormat("[/{0}]", this.Name);
			return sb.ToString();
		}
		internal static string StripBBCode(string input) {
			if (string.IsNullOrEmpty(input))
				return input;
			char[] array = new char[input.Length];
			int arrayIndex = 0;
			bool inside = false;
			for (int i = 0; i < input.Length; i++) {
				char let = input[i];
				if (let == '[') {
					inside = true;
					continue;
				}
				if (let == ']') {
					inside = false;
					continue;
				}
				if (!inside) {
					array[arrayIndex] = let;
					arrayIndex++;
				}
			}
			return new string(array, 0, arrayIndex);
		}
		internal virtual void ParseInner(string innerText) {
			while (innerText != null && innerText.Length > 0) {
				int bbStartTag = -1;
				int bbEndTag = -1;
				bbStartTag = innerText.IndexOf('[');
				if (bbStartTag == -1) {
					// нет открытых бб кодов, просто текст внутри
					this.ChildNodes.Add(new BBTextNode(innerText, this));
					innerText = null;
					break;
				}
				else {
					// check if it's escaped
					if (bbStartTag > 0) {
						char before = innerText[bbStartTag - 1];
						if (before == '\\') {
							// escaped
							this.ChildNodes.Add(new BBTextNode(innerText.Substring(0, bbStartTag - 1) + '[', this));
							innerText = innerText.Remove(0, bbStartTag + 1);
						}
					}
					// ok, find a closing tag
					bbEndTag = innerText.IndexOf(']');
					if (bbEndTag == -1) {
						// not closed anywhere
						// write out as text node and break
						this.ChildNodes.Add(new BBTextNode(innerText, this));
						innerText = null;
						break;
					}
					else {
						if (bbEndTag > 0) {
							char before = innerText[bbEndTag - 1];
							if (before == '\\') {
								// escaped
								this.ChildNodes.Add(new BBTextNode(innerText.Substring(0, bbEndTag - 1) + ']', this));
								innerText = innerText.Remove(0, bbEndTag + 1);
								continue;
							}
						}
						// seems to be a valid tag
						// see if there are any text before the tag
						if (bbStartTag > 0) {
							this.ChildNodes.Add(new BBTextNode(innerText.Substring(0, bbStartTag), this));
							innerText = innerText.Remove(0, bbStartTag);
							continue;
						}
						BBNode sub = new BBNode(BBNodeTypes.Tag);
						sub.Document = this.Document;
						// get the start tag
						string innerTagText = innerText.Extract(bbStartTag, bbEndTag, false).ToLower().Trim();
						// tag attributes
						// TODO: reword tag attribute parsing
						string[] tagParts = innerTagText.Split('=');
						string tagName = tagParts[0].ToLower().Trim();
						sub.Name = tagName;
						sub.ParentNode = this;
						if (tagParts.Length > 1) {
							string[] attributes = string.Join("=", tagParts.Skip(1).ToArray()).Split(';'); 
							foreach (string attr in attributes) {
								sub.Attributes.Add(new BBNodeAttribute(attr.Trim()));
							}
						}
						innerText = innerText.Remove(0, bbEndTag + 1);
						// ok we have the name of the tag, now find the closing tag index
						int closebbStart = -1;
						string closeTagTemplate = "[/" + tagName + "]";
						closebbStart = innerText.IndexOf(closeTagTemplate, StringComparison.InvariantCultureIgnoreCase);
						if (closebbStart > -1) {
							sub.InnerCode = innerText.Extract(bbStartTag, closebbStart, true);
							sub.ParseInner(innerText.Extract(bbStartTag, closebbStart, true));
							this.ChildNodes.Add(sub);
							innerText = innerText.Remove(0, closebbStart + closeTagTemplate.Length);
						}
						else {
							// not a closed tag
							this.Document.Errors.Add(new BBFormattingError() {
								Error = "Element not closed: [" + tagName + "]"
							});
							this.ChildNodes.Add(new BBTextNode(innerText, this));
							innerText = null;
							break;
						}
					}
				}
			}
		}
	}
	public class BBNodeAttribute {
		public BBNodeAttribute(string value) {
			this.Value = value;
		}
		public string Value {
			get;
			set;
		}
	}
	
	public class BBNodeAttributeList : List<BBNodeAttribute> {
	}
	public class BBNodeList : List<BBNode> {
	}
	public class BBTextNode : BBNode {
		public BBTextNode(string text, BBNode parent) : base( BBNodeTypes.Text) {
			this.InnerText = text;
			this.ParentNode = parent;
		}
	}
}
