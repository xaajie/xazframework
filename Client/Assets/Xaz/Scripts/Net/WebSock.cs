//------------------------------------------------------------
// Xaz Framework
// Feedback: qq515688254
//------------------------------------------------------------
﻿using UnityEngine;
using System.Collections;
using System;
using System.Text;
using System.Security.Cryptography;
using System.Net;

namespace Xaz
{
	public class WebSocket : HttpSock
	{
		internal static readonly RandomNumberGenerator m_RandomNumberGen;
		static WebSocket()
		{
			m_RandomNumberGen = new RNGCryptoServiceProvider();
		}
		static private string CreateBase64Key()
		{
			var src = new byte[16];
			m_RandomNumberGen.GetBytes(src);
			return Convert.ToBase64String(src);
		}

		private ByteStream m_Stream = new ByteStream(1024);

		public void Connect(string url, params string[] protocols)
		{
			if (url.StartsWith("ws://") || url.StartsWith("wss://")) {
				var headers = new WebHeaderCollection() {
					{HttpRequestHeader.Upgrade, "websocket"},
					{HttpRequestHeader.Connection, "Upgrade"},
					{HttpRequestHeader.SecWebSocketKey, CreateBase64Key()},
					{HttpRequestHeader.SecWebSocketVersion, "13"}
				};
				if (protocols.Length > 0) {
					headers[HttpRequestHeader.SecWebSocketProtocol] = string.Join(",", protocols);
				}
				Request(url, headers);
			} else {
				NotifyErrorEvent("必须是ws://或者wss://开头");
			}
		}

		protected override void OnOpen()
		{
			m_Stream.Clear(false);
			base.OnOpen();
		}

		protected override void OnResponseReceived(byte[] data)
		{
		}
	}
}