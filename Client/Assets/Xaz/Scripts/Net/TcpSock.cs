//------------------------------------------------------------
// Xaz Framework
// Feedback: qq515688254
//------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

#pragma warning disable 0168 // variable declared but not used.

namespace Xaz
{
	abstract public class TcpSock : Sock
	{
		class AsyncState
		{
			public int size;
			public int offset;
			public byte[] data;
			public Socket socket;
		}

		private Socket m_Socket = null;
		private byte[] m_SocketBuffer = new byte[8192];

		protected void Connect(string host, int port)
		{
			CloseSocket(false);
			m_ReadyState = ReadyState.Connecting;
			// 在没有网的时候，在小米3手机上，new操作会导致 "SocketException: Access denied" 异常。
			try {
#if (UNITY_IOS || UNITY_IPHONE)
				//add ipv6 support
				IPAddress[] addressList = Dns.GetHostEntry(host).AddressList;
				if (addressList.Length == 0) {
					PushSocketMsg(SockMsg.Type.Error, null, "Can't found IPAddress.");
				} else {
					IPAddress address = addressList[0];
					m_Socket = new Socket(address.AddressFamily == AddressFamily.InterNetworkV6 ? AddressFamily.InterNetworkV6 : AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
					m_Socket.BeginConnect(address, port, new AsyncCallback(OnConnectCallback), m_Socket);
				}
#else
				m_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				m_Socket.BeginConnect(host, port, new AsyncCallback(OnConnectCallback), m_Socket);
#endif
			} catch (Exception e) {
				// ArgumentNullException, ObjectDisposedException, NotSupportedException, ArgumentOutOfRangeException, InvalidOperationException, ArgumentException, SocketException
				PushSocketMsg(SockMsg.Type.Error, null, e.Message);
#if Xaz_DEBUG
				Logger.Warning("[Connect] Exception:", e.ToString());
#endif
			}
		}

		protected void Send(string data)
		{
			Send(Encoding.UTF8.GetBytes(data));
		}
		protected void Send(byte[] data)
		{
			Send(data, 0, data.Length);
		}
		protected void Send(byte[] data, int offset, int size)
		{
			if (m_ReadyState != ReadyState.Connected)
				return;
			if (data == null || offset < 0 || size <= 0 || offset + size > data.Length)
				return;
			try {
				m_Socket.BeginSend(data, offset, size, SocketFlags.None, new AsyncCallback(OnSendCallback), new AsyncState() {
					socket = m_Socket, data = data, offset = offset, size = size
				});
			} catch (Exception e) {
				// ArgumentNullException, SocketException, ArgumentOutOfRangeException, ObjectDisposedException
				CloseSocket(true);
#if Xaz_DEBUG
				Logger.Warning("[Send] Exception:", e.ToString());
#endif
			}
		}

		public override void Close(bool fireEvent = false)
		{
			CloseSocket(fireEvent);
		}

		private void OnConnectCallback(IAsyncResult result)
		{
			Socket socket = (Socket)result.AsyncState;

			try {
				socket.EndConnect(result);
				PushSocketMsg(SockMsg.Type.Open);

				socket.BeginReceive(m_SocketBuffer, 0, m_SocketBuffer.Length, SocketFlags.None, new AsyncCallback(OnReceiveCallback), socket);
			} catch (Exception e) {
				// ObjectDisposedException, SocketException, ArgumentNullException, ArgumentException
#if Xaz_DEBUG
				Logger.Warning("[OnConnectCallback] Exception:", e is SocketException ? ((SocketException)e).SocketErrorCode.ToString() : e.Message);
#endif
				if (e is ObjectDisposedException)
					return;
				PushSocketMsg(SockMsg.Type.Error, null, e.Message);
			}
		}

		private void OnSendCallback(IAsyncResult result)
		{
			AsyncState state = (AsyncState)result.AsyncState;
			Socket socket = state.socket;

			try {
				SocketError socketError;
				int sent = socket.EndSend(result, out socketError);
#if Xaz_DEBUG
				Logger.Print(string.Format("[OnSendCallback] sent:{0}, size:{1}, err:{2}", sent, state.size, socketError));
#endif
				if (socketError != SocketError.Success && socketError != SocketError.WouldBlock && socketError != SocketError.InProgress) {
					CloseSocket(true);
				} else if (sent < state.size) {
					state.size -= sent;
					state.offset += sent;
					socket.BeginSend(state.data, state.offset, state.size, SocketFlags.None, new AsyncCallback(OnSendCallback), state);
				}
			} catch (Exception e) {
				// ObjectDisposedException, ArgumentNullException, ArgumentException, SocketException, InvalidOperationException
				if (e is ObjectDisposedException)
					return;
#if Xaz_DEBUG
				Logger.Warning("[OnSendCallback] Exception:", e is SocketException ? ((SocketException)e).SocketErrorCode.ToString() : e.Message);
#endif
			}
		}

		private void OnReceiveCallback(IAsyncResult result)
		{
			Socket socket = (Socket)result.AsyncState;

			try {
				SocketError socketError;
				int size = socket.EndReceive(result, out socketError);
				if (size > 0) {
					byte[] data = new byte[size];
					Array.Copy(m_SocketBuffer, 0, data, 0, size);
					PushSocketMsg(SockMsg.Type.Message, data);
				}
#if Xaz_DEBUG
				Logger.Print(string.Format("[OnReceiveCallback] err:{0}, size:{1}", socketError, size));
#endif
				if ((socketError == SocketError.Success && size == 0) || (socketError != SocketError.Success && socketError != SocketError.WouldBlock && socketError != SocketError.InProgress)) {
					CloseSocket(true);
				} else {
					socket.BeginReceive(m_SocketBuffer, 0, m_SocketBuffer.Length, SocketFlags.None, new AsyncCallback(OnReceiveCallback), socket);
				}
			} catch (Exception e) {
				// ObjectDisposedException, ArgumentNullException, ArgumentException, SocketException, InvalidOperationException
				if (e is ObjectDisposedException)
					return;
#if Xaz_DEBUG
				Logger.Warning("[OnReceiveCallback] Exception:", e is SocketException ? ((SocketException)e).SocketErrorCode.ToString() : e.Message);
#endif
			}
		}

		private void OnDisconnectCallback(IAsyncResult result)
		{
			Socket socket = (Socket)result.AsyncState;

			try {
				socket.EndDisconnect(result);
			} catch (Exception e) {
#if Xaz_DEBUG
				Logger.Warning("[OnDisconnectCallback] Exception:", e.ToString());
#endif
			}

			try {
				socket.Close();
			} catch (Exception e) {
#if Xaz_DEBUG
				Logger.Warning("[Close] Exception:", e.ToString());
#endif
			}
		}

		private void CloseSocket(bool fireEvent)
		{
			if (m_ReadyState != ReadyState.Connected && m_ReadyState != ReadyState.Connecting)
				return;

			m_ReadyState = ReadyState.Closing;

			try {
				if (m_Socket.Connected) {
					m_Socket.BeginDisconnect(true, new AsyncCallback(OnDisconnectCallback), m_Socket);
				} else {
					m_Socket.Close();
				}
			} catch (Exception e) {
#if Xaz_DEBUG
				Logger.Warning("[Shutdown] Exception:", e.ToString());
#endif
			} finally {
				m_Socket = null;
				if (fireEvent) {
					PushSocketMsg(SockMsg.Type.Close);
				}
			}
		}

	}
}
