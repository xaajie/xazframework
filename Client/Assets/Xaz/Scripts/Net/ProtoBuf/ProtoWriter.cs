//------------------------------------------------------------
// Xaz Framework
// Feedback: qq515688254
//------------------------------------------------------------
﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Xaz
{
	public sealed class ProtoWriter
	{
		private byte[] m_Buffer;
		private int m_Capacity = 0;
		private int m_Position = 0;

		public ProtoWriter()
		{
			m_Capacity = 0;
			m_Position = 0;
			m_Buffer = new byte[0];
		}

		private void DemandSpace(int size)
		{
			int length = m_Position + size;
			if (length > m_Capacity) {
				size = m_Capacity * 2;
				if (size < length) {
					size = length;
				}
				byte[] newBuffer = new byte[size];
				if (this.m_Position > 0) {
					Buffer.BlockCopy(this.m_Buffer, 0, newBuffer, 0, this.m_Position);
				}
				this.m_Buffer = newBuffer;
				this.m_Capacity = size;
			}
		}

		public void WriteInt32(int value)
		{
			if (value >= 0) {
				WriteUInt32((uint)value);
			} else {
				DemandSpace(10);
				m_Buffer[m_Position++] = (byte)(value | 0x80);
				m_Buffer[m_Position++] = (byte)((value >> 7) | 0x80);
				m_Buffer[m_Position++] = (byte)((value >> 14) | 0x80);
				m_Buffer[m_Position++] = (byte)((value >> 21) | 0x80);
				m_Buffer[m_Position++] = (byte)((value >> 28) | 0x80);
				m_Buffer[m_Position++] = (byte)0xFF;
				m_Buffer[m_Position++] = (byte)0xFF;
				m_Buffer[m_Position++] = (byte)0xFF;
				m_Buffer[m_Position++] = (byte)0xFF;
				m_Buffer[m_Position++] = (byte)0x01;
			}
		}

		public void WriteUInt32(uint value)
		{
			DemandSpace(5);
			do {
				m_Buffer[m_Position++] = (byte)((value & 0x7f) | 0x80);
			} while ((value >>= 7) != 0);
			m_Buffer[m_Position - 1] &= 0x7f;
		}

		public void WriteSInt32(int value)
		{
			WriteUInt32((uint)((value << 1) ^ (value >> 31)));
		}

		public void WriteString(string value)
		{
			int length = UTF8Encoding.UTF8.GetByteCount(value);
			DemandSpace(length + 5);
			WriteUInt32((uint)length);
			if (length > 0) {
				UTF8Encoding.UTF8.GetBytes(value, 0, value.Length, m_Buffer, m_Position);
				m_Position += length;
			}
		}

		public void WriteBytes(byte[] value)
		{
			int length = value.Length;
			DemandSpace(length + 5);
			WriteUInt32((uint)length);
			if (length > 0) {
				Buffer.BlockCopy(value, 0, m_Buffer, m_Position, length);
				m_Position += length;
			}
		}

		public void WriteInt64(long value)
		{
			if (value >= 0) {
				WriteUInt64((ulong)value);
			} else {
				DemandSpace(10);
				m_Buffer[m_Position++] = (byte)(value | 0x80);
				m_Buffer[m_Position++] = (byte)((int)(value >> 7) | 0x80);
				m_Buffer[m_Position++] = (byte)((int)(value >> 14) | 0x80);
				m_Buffer[m_Position++] = (byte)((int)(value >> 21) | 0x80);
				m_Buffer[m_Position++] = (byte)((int)(value >> 28) | 0x80);
				m_Buffer[m_Position++] = (byte)((int)(value >> 35) | 0x80);
				m_Buffer[m_Position++] = (byte)((int)(value >> 42) | 0x80);
				m_Buffer[m_Position++] = (byte)((int)(value >> 49) | 0x80);
				m_Buffer[m_Position++] = (byte)((int)(value >> 56) | 0x80);
				m_Buffer[m_Position++] = 0x01; // sign bit
			}
		}

		public void WriteUInt64(ulong value)
		{
			DemandSpace(10);
			do {
				m_Buffer[m_Position++] = (byte)((value & 0x7F) | 0x80);
			} while ((value >>= 7) != 0);
			m_Buffer[m_Position - 1] &= 0x7f;
		}

		public void WriteSInt64(long value)
		{
			WriteUInt64((ulong)((value << 1) ^ (value >> 63)));
		}

		public void WriteBoolean(bool value)
		{
			WriteUInt32(value ? (uint)1 : (uint)0);
		}

		public void WriteDouble(double value)
		{
#if USE_UNSAFE
			WriteFixed64(*(long*)&value, writer);
#else
			WriteFixed64(BitConverter.ToInt64(BitConverter.GetBytes(value), 0));
#endif
		}

		public void WriteFloat(float value)
		{
#if USE_UNSAFE
			WriteFixed32(*(int*)&value, writer);
#else
			WriteFixed32(BitConverter.ToInt32(BitConverter.GetBytes(value), 0));
#endif
		}

		public void WriteFixed32(int value)
		{
			DemandSpace(4);
			m_Buffer[m_Position++] = (byte)value;
			m_Buffer[m_Position++] = (byte)(value >> 8);
			m_Buffer[m_Position++] = (byte)(value >> 16);
			m_Buffer[m_Position++] = (byte)(value >> 24);
		}

		public void WriteFixed64(long value)
		{
			DemandSpace(8);
			m_Buffer[m_Position++] = (byte)value;
			m_Buffer[m_Position++] = (byte)(value >> 8);
			m_Buffer[m_Position++] = (byte)(value >> 16);
			m_Buffer[m_Position++] = (byte)(value >> 24);
			m_Buffer[m_Position++] = (byte)(value >> 32);
			m_Buffer[m_Position++] = (byte)(value >> 40);
			m_Buffer[m_Position++] = (byte)(value >> 48);
			m_Buffer[m_Position++] = (byte)(value >> 56);
		}

		public byte[] GetBytes()
		{
			byte[] result = new byte[m_Position];
			if (m_Position > 0) {
				Buffer.BlockCopy(m_Buffer, 0, result, 0, m_Position);
			}
			return result;
		}
	}
}
