//------------------------------------------------------------
// Xaz Framework
// Feedback: qq515688254
//------------------------------------------------------------
﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using System.IO;
using System.Net;

namespace Xaz
{
	abstract public class HttpSock : TcpSock
	{
		private Uri m_Uri;
		private int m_Port;
		private WebHeaderCollection m_RequestHeaders;

		private bool m_HeaderReceived;
		private ByteStream m_Stream = new ByteStream(1024);

		public HttpStatusCode statusCode
		{
			get;
			private set;
		}

		public WebHeaderCollection responseHeaders
		{
			get;
			private set;
		}

		public HttpSock()
		{
			responseHeaders = new WebHeaderCollection();
		}

		protected void Request(string url, WebHeaderCollection headers)
		{
			m_Uri = new Uri(url);
			m_RequestHeaders = headers;
			m_Port = m_Uri.Port == -1 ? (m_Uri.Scheme == "ws" ? 80 : (m_Uri.Scheme == "wss" ? 443 : -1)) : m_Uri.Port;
			Connect(m_Uri.Host, m_Port);
		}

		protected override void OnOpen()
		{
			StringBuilder builder = new StringBuilder();
			builder.AppendFormat("GET {0} HTTP/1.1\r\n", m_Uri.PathAndQuery);
			builder.AppendFormat("Host: {0}:{1}\r\n", m_Uri.Host, m_Port);
			builder.Append(m_RequestHeaders.ToString());
			Send(builder.ToString());

			m_Stream.Clear(false);
			m_HeaderReceived = false;
			responseHeaders.Clear();
			base.OnOpen();
		}

		sealed protected override void OnDataReceived(byte[] data)
		{
			if (m_HeaderReceived) {
				OnResponseReceived(data);
				return;
			}

			m_Stream.WriteBytes(data);

			while (true) {
				string header = m_Stream.ReadLine();
				if (header == null)
					break;
				if (header.Length == 0) { // \r\n
					m_HeaderReceived = true;
					OnHeadersReceived();
					if (m_Stream.bytesAvailable > 0) {
						data = new byte[m_Stream.bytesAvailable];
						m_Stream.ReadBytes(data);
						OnResponseReceived(data);
					}
					return;
				}

				if (header.StartsWith("HTTP")) {
					statusCode = (HttpStatusCode)Convert.ToInt32(header.Split(' ')[1]);
				} else if (header.IndexOf(":") >= 0) {
					responseHeaders.Add(header);
				} else {
					responseHeaders.Add(header, "(empty)");
				}
				Debug.Log(header);
			}
		}

		virtual protected void OnHeadersReceived()
		{
		}

		abstract protected void OnResponseReceived(byte[] data);
	}
}

namespace Xaz
{
	static internal partial class InternalExtensionMethod
	{
		static public string ReadLine(this ByteStream stream)
		{
			int index = stream.Find(10);
			if (index < 0)
				return null;
			string text = stream.ReadUTFBytes(index + 1 - 2);
			stream.Skip(2);
			return text;
		}

		// WebHeaderCollection
		public static bool Contains(this WebHeaderCollection collection, string name)
		{
			return collection != null && collection.Count > 0 ? collection[name] != null : false;
		}
		public static bool Contains(this WebHeaderCollection collection, string name, string value)
		{
			if (collection == null || collection.Count == 0)
				return false;

			var vals = collection[name];
			if (vals == null)
				return false;

			foreach (var val in vals.Split(','))
				if (val.Trim().Equals(value, StringComparison.OrdinalIgnoreCase))
					return true;

			return false;
		}
	}
}
