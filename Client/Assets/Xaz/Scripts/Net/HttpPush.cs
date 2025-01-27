//------------------------------------------------------------
// Xaz Framework
// Feedback: qq515688254
//------------------------------------------------------------
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using UnityEngine;

namespace Xaz
{
	class HttpPush : HttpSock
	{
		private int m_ContentSize = 0;
		private ByteStream m_Stream = new ByteStream(1024);

		public void Connect(string url)
		{
			if (url.StartsWith("http://") || url.StartsWith("https://")) {
				Request(url, new WebHeaderCollection(){
					{HttpRequestHeader.AcceptCharset,"UTF-8,*;q=0.5"},
					{HttpRequestHeader.Connection, "Keep-Alive"},
					{HttpRequestHeader.CacheControl, "no-cache"}
				});
			} else {
				NotifyErrorEvent("必须是http://或者https://开头");
			}
		}

		protected override void OnOpen()
		{
			m_Stream.Clear(false);
			base.OnOpen();
		}

		protected override void OnResponseReceived(byte[] data)
		{
			if (!responseHeaders.Contains(HttpResponseHeader.TransferEncoding, "chunked")) {
				if (responseHeaders.Contains(HttpResponseHeader.ContentLength)) {
					m_ContentSize = Convert.ToInt32(responseHeaders[HttpResponseHeader.ContentLength]);
				} else {
					NotifyErrorEvent("响应头中没有发现 Content-Length 或者 TransferEncoding != 'chunked', 关闭该连接！");
					Close();
					return;
				}
			}

			m_Stream.WriteBytes(data);
			while (true) {
				if (m_ContentSize == 0) {
					string text = m_Stream.ReadLine();
					if (text == null)
						break;

					if (text.Length == 0) { // \r\n
						continue;
					}

					int index = text.IndexOf(";");
					m_ContentSize = Convert.ToInt32(index < 0 ? text : text.Substring(0, index), 16);
					if (m_ContentSize == 0)
						break;
				}

				if (m_Stream.bytesAvailable < m_ContentSize)
					break;

				data = new byte[m_ContentSize];
				m_Stream.ReadBytes(data);
				m_ContentSize = 0;
				NotifyMessageEvent(data);
			}
		}
	}
}
