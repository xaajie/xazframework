//------------------------------------------------------------
// Xaz Framework
// Feedback: qq515688254
//------------------------------------------------------------
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace Xaz
{
	abstract public class Sock
	{
		public struct Message
		{
			public byte[] data;

			public string text
			{
				get
				{
					return System.Text.Encoding.UTF8.GetString(data);
				}
			}
		}

		protected enum ReadyState
		{
			None,
			Connecting,
			Connected,
			Closing,
			Closed
		}

		protected class SockMsg
		{
			public enum Type
			{
				Open,
				Close,
				Error,
				Message
			}
			public Type type;
			public byte[] data;
			public string text;
		}

		// Socket回调
		public delegate void OpenDelegate();
		public delegate void CloseDelegate();
		public delegate void ErrorDelegate(string txt);
		public delegate void MessageDelegate(Message msg);

		public OpenDelegate onOpen;
		public CloseDelegate onClose;
		public ErrorDelegate onError;
		public MessageDelegate onMessage;

		protected ReadyState m_ReadyState = ReadyState.None;

		private List<SockMsg> m_MsgQueue = new List<SockMsg>();
		
		public Sock()
		{
			Scheduler.Update(OnTick);
		}

		~Sock()
		{
			Scheduler.Remove(OnTick);
		}

		public bool connected
		{
			get
			{
				return m_ReadyState == ReadyState.Connected;
			}
		}

		abstract public void Close(bool fireEvent = false);

		virtual protected void OnOpen()
		{
			NotifyOpenEvent();
		}
		virtual protected void OnClose()
		{
			NotifyCloseEvent();
		}
		virtual protected void OnError(string text)
		{
			NotifyErrorEvent(text);
		}
		virtual protected void OnDataReceived(byte[] data)
		{
			NotifyMessageEvent(data);
		}

		protected void NotifyOpenEvent()
		{
			if (onOpen != null) {
				onOpen();
			}
		}
		protected void NotifyCloseEvent()
		{
			if (onClose != null) {
				onClose();
			}
		}
		protected void NotifyErrorEvent(string text)
		{
			if (onError != null) {
				onError(text);
			}
		}
		protected void NotifyMessageEvent(byte[] data)
		{
			if (onMessage != null) {
				onMessage(new Message() {
					data = data
				});
			}
		}

		protected void PushSocketMsg(SockMsg.Type type, byte[] data = null, string text = null)
		{
			SockMsg msg = new SockMsg() {
				type = type, data = data, text = text
			};
			lock (m_MsgQueue) {
				m_MsgQueue.Add(msg);
			}
		}

		private void OnTick()
		{
			List<SockMsg> list = null;
			lock (m_MsgQueue) {
				if (m_MsgQueue.Count == 0)
					return;
				list = new List<SockMsg>(m_MsgQueue);
				m_MsgQueue.Clear();
			}
			for (int i = 0; i < list.Count; i++) {
				SockMsg msg = list[i];
				switch (msg.type) {
				case SockMsg.Type.Open:
					m_ReadyState = ReadyState.Connected;
					OnOpen();
					break;
				case SockMsg.Type.Close:
					m_ReadyState = ReadyState.Closed;
					OnClose();
					break;
				case SockMsg.Type.Error:
					m_ReadyState = ReadyState.None;
					OnError(msg.text);
					break;
				case SockMsg.Type.Message:
					OnDataReceived(msg.data);
					break;
				}
			}
		}

	}
}
