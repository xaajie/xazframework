//------------------------------------------------------------
// Xaz Framework
// Feedback: qq515688254
//------------------------------------------------------------
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xaz
{
	public class ByteStream : IDataInput, IDataOutput
	{
		private int m_Length;
		private int m_Position;
		private int m_Capacity;
		private byte[] m_Buffer;

		public int bytesAvailable
		{
			get
			{
				return m_Length - m_Position;
			}
		}

		public Endian endian
		{
			get;
			set;
		}

		public ByteStream()
			: this(0)
		{
		}

		public ByteStream(int capacity)
		{
			m_Capacity = capacity < 0 ? 0 : capacity;
			m_Buffer = new byte[m_Capacity];
			endian = Endian.LITTLE_ENDIAN;
		}

		public byte[] ToArray()
		{
			byte[] bytes = new byte[m_Length];
			if (m_Length > 0) {
				Buffer.BlockCopy(m_Buffer, 0, bytes, 0, m_Length);
			}
			return bytes;
		}

		public void Clear(bool cleanup)
		{
			if (cleanup) {
				m_Buffer = new byte[0];
			}
			m_Length = m_Position = m_Capacity = 0;
		}

		public void Skip(int count)
		{
			m_Position = Math.Min(m_Position + count, m_Length);
		}

		public int Find(byte b)
		{
			for (int i = m_Position; i < m_Length; i++) {
				if (m_Buffer[i] == b) {
					return i - m_Position;
				}
			}
			return -1;
		}

		private void CheckAvailable(int size)
		{
			if (size > bytesAvailable) {
				throw new OutOfMemoryException("bytesAvailable");
			}
		}

		#region Read Methods
		public bool ReadBoolean()
		{
			CheckAvailable(1);
			return ReadInt8(m_Buffer, m_Position++) != 0;
		}

		public sbyte ReadByte()
		{
			CheckAvailable(1);
			return (sbyte)ReadInt8(m_Buffer, m_Position++);
		}

		public void ReadBytes(byte[] bytes)
		{
			if (bytes == null) {
				throw new ArgumentNullException("bytes", "ArgumentNull_Buffer");
			}
			ReadBytes(bytes, 0, Math.Min(bytes.Length, bytesAvailable));
		}

		public void ReadBytes(byte[] bytes, int offset)
		{
			if (bytes == null) {
				throw new ArgumentNullException("bytes", "ArgumentNull_Buffer");
			}
			ReadBytes(bytes, offset, Math.Min(bytes.Length - offset, bytesAvailable));
		}

		public void ReadBytes(byte[] bytes, int offset, int length)
		{
			if (bytes == null) {
				throw new ArgumentNullException("bytes", "ArgumentNull_Buffer");
			}
			if (offset < 0) {
				throw new ArgumentOutOfRangeException("offset", "ArgumentOutOfRange_NeedNonNegNum");
			}
			if (length < 0) {
				throw new ArgumentOutOfRangeException("length", "ArgumentOutOfRange_NeedNonNegNum");
			}
			if (bytes.Length - offset < length) {
				throw new ArgumentException("Argument_InvalidOffLen");
			}
			if (length > 0) {
				CheckAvailable(length);
				Buffer.BlockCopy(m_Buffer, m_Position, bytes, offset, length);
				m_Position += length;
			}
		}

		public int ReadInt()
		{
			CheckAvailable(4);
			int value = ReadInt32(m_Buffer, m_Position, endian);
			m_Position += 4;
			return value;
		}

		public float ReadFloat()
		{
			CheckAvailable(4);
			float value = ReadFloat(m_Buffer, m_Position, endian);
			m_Position += 4;
			return value;
		}

		public long ReadLong()
		{
			CheckAvailable(8);
			long value = ReadInt64(m_Buffer, m_Position, endian);
			m_Position += 8;
			return value;
		}

		public double ReadDouble()
		{
			CheckAvailable(8);
			double value = ReadDouble(m_Buffer, m_Position, endian);
			m_Position += 8;
			return value;
		}

		public short ReadShort()
		{
			CheckAvailable(2);
			short value = ReadInt16(m_Buffer, m_Position, endian);
			m_Position += 2;
			return value;
		}

		public byte ReadUnsignedByte()
		{
			CheckAvailable(1);
			return ReadInt8(m_Buffer, m_Position++);
		}

		public uint ReadUnsignedInt()
		{
			return (uint)ReadInt();
		}

		public ulong ReadUnsignedLong()
		{
			return (ulong)ReadLong();
		}

		public ushort ReadUnsignedShort()
		{
			return (ushort)ReadShort();
		}

		public string ReadMultiByte(int length, Encoding encoding)
		{
			CheckAvailable(length);
			string value = encoding.GetString(m_Buffer, m_Position, length);
			m_Position += length;
			return value;
		}

		public string ReadUTF()
		{
			ushort length = ReadUnsignedShort();
			return ReadMultiByte((int)length, Encoding.UTF8);
		}

		public string ReadUTFBytes(int length)
		{
			return ReadMultiByte(length, Encoding.UTF8);
		}
		#endregion

		private void ExpandCapacity(int size)
		{
			if (m_Length + size <= m_Capacity)
				return;

			int count = bytesAvailable;
			if (m_Position > 0) {
				if (count > 0) {
					Buffer.BlockCopy(m_Buffer, m_Position, m_Buffer, 0, count);
				}
				m_Length -= m_Position;
				m_Position = 0;
			}

			if (m_Capacity - count < size) {
				size = (int)(Math.Ceiling(size / 256f) * 256);
				byte[] buffer = new byte[m_Capacity += size];
				if (count > 0) {
					Buffer.BlockCopy(m_Buffer, 0, buffer, 0, count);
				}
				m_Buffer = buffer;
			}
		}

		#region Write Methods
		public void WriteBoolean(bool value)
		{
			WriteByte((byte)(value ? 1 : 0));
		}

		public void WriteByte(byte value)
		{
			ExpandCapacity(1);
			WriteValue(value, m_Buffer, m_Length++);
		}

		private void Write(byte[] bytes, int capacity, int offset, int length)
		{
			if (length < 0) {
				throw new ArgumentOutOfRangeException("length", "ArgumentOutOfRange_NeedNonNegNum");
			}
			if (capacity - offset < length) {
				throw new ArgumentException("Argument_InvalidOffLen");
			}
			if (length > 0) {
				ExpandCapacity(length);
				Buffer.BlockCopy(bytes, offset, m_Buffer, m_Length, length);
				m_Length += length;
			}
		}

		public void WriteBytes(byte[] bytes)
		{
			if (bytes == null) {
				throw new ArgumentNullException("bytes", "ArgumentNull_Buffer");
			}
			Write(bytes, bytes.Length, 0, bytes.Length);
		}

		public void WriteBytes(byte[] bytes, int offset)
		{
			if (bytes == null) {
				throw new ArgumentNullException("bytes", "ArgumentNull_Buffer");
			}
			if (offset < 0) {
				throw new ArgumentOutOfRangeException("offset", "ArgumentOutOfRange_NeedNonNegNum");
			}
			Write(bytes, bytes.Length, offset, bytes.Length - offset);
		}

		public void WriteBytes(byte[] bytes, int offset, int length)
		{
			if (bytes == null) {
				throw new ArgumentNullException("bytes", "ArgumentNull_Buffer");
			}
			if (offset < 0) {
				throw new ArgumentOutOfRangeException("offset", "ArgumentOutOfRange_NeedNonNegNum");
			}
			Write(bytes, bytes.Length, offset, length);
		}

		public void WriteDouble(double value)
		{
			ExpandCapacity(8);
			WriteValue(BitConverter.GetBytes(value), m_Buffer, m_Length, endian);
			m_Length += 8;
		}

		public void WriteFloat(float value)
		{
			ExpandCapacity(4);
			WriteValue(BitConverter.GetBytes(value), m_Buffer, m_Length, endian);
			m_Length += 4;
		}

		public void WriteInt(int value)
		{
			ExpandCapacity(4);
			WriteValue(value, m_Buffer, m_Length, endian);
			m_Length += 4;
		}

		public void WriteLong(long value)
		{
			ExpandCapacity(8);
			WriteValue(value, m_Buffer, m_Length, endian);
			m_Length += 8;
		}

		public void WriteMultiByte(string value, Encoding encoding)
		{
			byte[] data = encoding.GetBytes(value);
			int length = data.Length;
			ExpandCapacity(length);
			Buffer.BlockCopy(data, 0, m_Buffer, m_Length, length);
			m_Length += length;
		}

		public void WriteShort(short value)
		{
			ExpandCapacity(2);
			WriteValue(value, m_Buffer, m_Length, endian);
			m_Length += 2;
		}

		public void WriteUTF(string value)
		{
			short length = (short)value.Length;
			ExpandCapacity(length + 2);
			WriteShort(length);
			WriteMultiByte(value, Encoding.UTF8);
		}

		public void WriteUTFBytes(string value)
		{
			WriteMultiByte(value, Encoding.UTF8);
		}
		#endregion

		#region ReadValue from Bytes
		static private byte ReadInt8(byte[] bytes, int offset)
		{
			return bytes[offset];
		}

		static private short ReadInt16(byte[] bytes, int offset, Endian endian)
		{
			return BitConverter.ToInt16(GetValueBytes(bytes, ref offset, 2, endian), offset);
		}

		static private short ReadInt32(byte[] bytes, int offset, Endian endian)
		{
			return BitConverter.ToInt16(GetValueBytes(bytes, ref offset, 4, endian), offset);
		}

		static private short ReadInt64(byte[] bytes, int offset, Endian endian)
		{
			return BitConverter.ToInt16(GetValueBytes(bytes, ref offset, 8, endian), offset);
		}

		static private float ReadFloat(byte[] bytes, int offset, Endian endian)
		{
			return BitConverter.ToSingle(GetValueBytes(bytes, ref offset, 4, endian), offset);
		}

		static private double ReadDouble(byte[] bytes, int offset, Endian endian)
		{
			return BitConverter.ToDouble(GetValueBytes(bytes, ref offset, 8, endian), offset);
		}

		static private byte[] GetValueBytes(byte[] bytes, ref int offset, int length, Endian endian)
		{
			bool isLittleEndian = endian == Endian.LITTLE_ENDIAN;
			if (isLittleEndian == BitConverter.IsLittleEndian) {
				return bytes;
			}
			byte[] value = new byte[length];
			for (int i = 0; i < length; i++) {
				value[i] = bytes[offset + length - i - 1];
			}
			offset = 0;
			return value;
		}
		#endregion

		#region WriteValue to Bytes
		static public void WriteValue(byte value, byte[] bytes, int offset)
		{
			bytes[offset] = value;
		}

		static private void WriteValue(short value, byte[] bytes, int offset, Endian endian)
		{
			WriteValue(BitConverter.GetBytes(value), bytes, offset, endian);
		}

		static private void WriteValue(int value, byte[] bytes, int offset, Endian endian)
		{
			WriteValue(BitConverter.GetBytes(value), bytes, offset, endian);
		}

		static private void WriteValue(long value, byte[] bytes, int offset, Endian endian)
		{
			WriteValue(BitConverter.GetBytes(value), bytes, offset, endian);
		}

		static private void WriteValue(byte[] value, byte[] bytes, int offset, Endian endian)
		{
			bool isLittleEndian = endian == Endian.LITTLE_ENDIAN;
			if (isLittleEndian == BitConverter.IsLittleEndian) {
				Buffer.BlockCopy(value, 0, bytes, offset, value.Length);
			} else {
				for (int i = 0, count = value.Length - 1; i <= count; i++) {
					bytes[offset + i] = value[count - i];
				}
			}
		}
		#endregion
	}
}
