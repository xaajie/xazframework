//------------------------------------------------------------
// Xaz Framework
// Feedback: qq515688254
//------------------------------------------------------------
﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Xaz
{
	public enum WireType
	{
		Variant = 0,
		Fixed64 = 1,
		String = 2,

		[Obsolete]
		StartGroup = 3,

		[Obsolete]
		EndGroup = 4,

		Fixed32 = 5,
	}

	public sealed class ProtoReader
	{
		private const int MSB_Int32 = ((int)1) << 31;
		private const long MSB_Int64 = ((long)1) << 63;

		private int m_Pos;
		private int m_Length;
		private byte[] m_Data;

		public ProtoReader(byte[] data)
		{
			m_Pos = 0;
			m_Data = data;
			m_Length = data.Length;
		}

		public int position
		{
			get
			{
				return m_Pos;
			}
		}

		public int ReadInt32()
		{
			int i = 0;
			int value = 0, chunk = 0;

			do {
				chunk = m_Data[m_Pos++];
				value |= (chunk & 0x7f) << (i * 7);
				if ((chunk & 0x80) == 0)
					return value;
			} while (++i < 4);

			chunk = m_Data[m_Pos++];
			value |= chunk << 28;

			if ((chunk & 0xf0) == 0 || ((chunk & 0xF0) == 0xF0 && m_Data[m_Pos++] == 0xFF && m_Data[m_Pos++] == 0xFF && m_Data[m_Pos++] == 0xFF && m_Data[m_Pos++] == 0xFF && m_Data[m_Pos++] == 0x01))
				return value;

			return value;
		}

		public uint ReadUInt32()
		{
			int i = 0;
			uint value = 0, chunk = 0;

			do {
				chunk = m_Data[m_Pos++];
				value |= (chunk & 0x7f) << (i * 7);
				if ((chunk & 0x80) == 0)
					return value;
			} while (++i < 4);

			chunk = m_Data[m_Pos++];
			value |= chunk << 28;
			if ((chunk & 0xf0) == 0)
				return value;

			return value;
		}

		public int ReadSInt32()
		{
			int value = (int)ReadUInt32();
			return (-(value & 0x01)) ^ ((value >> 1) & ~MSB_Int32);
		}

		public long ReadSInt64()
		{
			long value = (long)ReadUInt64();
			return (-(value & 0x01L)) ^ ((value >> 1) & ~MSB_Int64);
		}

		public long ReadInt64()
		{
			return (long)ReadUInt64();
		}

		public ulong ReadUInt64()
		{
			int i = 0;
			uint value = 0, chunk = 0;

			do {
				chunk = m_Data[m_Pos++];
				value |= (chunk & 0x7f) << (i * 7);
				if ((chunk & 0x80) == 0)
					return value;
			} while (++i < 9);

			chunk = m_Data[m_Pos++];
			value |= chunk << 63;

			if ((chunk & ~(ulong)0x01) != 0) {
				// error("");
			}

			return value;
		}

		public int ReadFixed32()
		{
			return ((int)m_Data[m_Pos++])
					| (((int)m_Data[m_Pos++]) << 8)
					| (((int)m_Data[m_Pos++]) << 16)
					| (((int)m_Data[m_Pos++]) << 24);
		}

		public long ReadFixed64()
		{
			return ((long)m_Data[m_Pos++])
					   | (((long)m_Data[m_Pos++]) << 8)
					   | (((long)m_Data[m_Pos++]) << 16)
					   | (((long)m_Data[m_Pos++]) << 24)
					   | (((long)m_Data[m_Pos++]) << 32)
					   | (((long)m_Data[m_Pos++]) << 40)
					   | (((long)m_Data[m_Pos++]) << 48)
					   | (((long)m_Data[m_Pos++]) << 56);
		}

		public float ReadFloat()
		{
			int value = ReadFixed32();
#if USE_UNSAFE
			return *(float*)&value;
#else
			return BitConverter.ToSingle(BitConverter.GetBytes(value), 0);
#endif
		}

		public double ReadDouble()
		{
			long value = ReadFixed64();
#if USE_UNSAFE
			return *(double*)&value;
#else
			return BitConverter.ToDouble(BitConverter.GetBytes(value), 0);
#endif
		}

		public bool ReadBoolean()
		{
			switch (ReadUInt32()) {
			case 0:
				return false;
			case 1:
				return true;
			default:
				throw new Exception("Unexpected boolean value");
			}
		}

		public byte[] ReadBytes()
		{
			int bytes = (int)ReadUInt32();
			byte[] result = new byte[bytes];
			Buffer.BlockCopy(m_Data, m_Pos, result, 0, bytes);
			m_Pos += bytes;
			return result;
		}

		public bool TryReadFieldHeader(out uint header)
		{
			if (m_Pos < m_Length) {
				header = ReadUInt32();
				return true;
			}
			header = 0;
			return false;
		}

		public string ReadString()
		{
			int bytes = (int)ReadUInt32();
			if (bytes == 0)
				return "";

			string result = UTF8Encoding.UTF8.GetString(m_Data, m_Pos, bytes);
			m_Pos += bytes;

			return result;
		}

		public void Skip(WireType type)
		{
			switch (type) {
			case WireType.Fixed32:
				m_Pos += 4;
				break;
			case WireType.Fixed64:
				m_Pos += 8;
				break;
			case WireType.Variant:
				ReadUInt64();
				break;
			case WireType.String:
				m_Pos += (int)ReadUInt32();
				break;
			default:
				throw new Exception("");
			}
		}
	}
}
