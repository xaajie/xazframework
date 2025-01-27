//------------------------------------------------------------
// Xaz Framework
// Feedback: qq515688254
//------------------------------------------------------------
using System;
using System.Text;
using System.Collections.Generic;

namespace Xaz
{
	public enum Endian
	{
		BIG_ENDIAN,
		LITTLE_ENDIAN
	}

	public class ByteArray : IDataInput, IDataOutput
	{
		private int m_Length;
		private int m_Position;
		private int m_Capacity;
		private byte[] m_Buffer;

		public int bytesAvailable
		{
			get
			{
				return m_Length <= m_Position ? 0 : (m_Length - m_Position);
			}
		}

		public Endian endian
		{
			get;
			set;
		}

		public int length
		{
			get
			{
				return m_Length;
			}
			set
			{
				if (value < 0) {
					value = 0;
				}
				CheckCapacity(value);
			}
		}

		public int position
		{
			get
			{
				return m_Position;
			}
			set
			{
				m_Position = value <= 0 ? 0 : value;
			}
		}

		public ByteArray()
			: this(0)
		{
		}

		public ByteArray(int capacity)
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

		public byte[] GetBuffer()
		{
			return m_Buffer;
		}

		public void Clear()
		{
			this.m_Buffer = new byte[0];
			this.m_Length = this.m_Position = this.m_Capacity = 0;
		}

		///// Compress.
		//public void Compress(Compression.Algorithm algorithm = Compression.Algorithm.GZIP)
		//{
		//	rawData = Compression.Compress(rawData, 0, m_Length, algorithm);
		//	m_Position = m_Length = m_Capacity = rawData.Length;
		//}

		///// Uncompress.
		//public void Uncompress(Compression.Algorithm algorithm = Compression.Algorithm.GZIP)
		//{
		//	rawData = Compression.Uncompress(rawData, 0, m_Length, algorithm);
		//	m_Position = m_Length = m_Capacity = rawData.Length;
		//}

		public void Skip(int count)
		{
			m_Position += count;
		}

		public void Move(int srcOffset, int dstOffset, int length)
		{
			if (srcOffset < 0) {
				throw new ArgumentOutOfRangeException("srcOffset", "ArgumentOutOfRange_NeedNonNegNum");
			}
			if (dstOffset < 0) {
				throw new ArgumentOutOfRangeException("dstOffset", "ArgumentOutOfRange_NeedNonNegNum");
			}
			if (m_Length - srcOffset < length) {
				throw new ArgumentException("Argument_InvalidOffLen");
			}
			CheckCapacity(dstOffset + length - m_Length);
			Buffer.BlockCopy(m_Buffer, srcOffset, m_Buffer, dstOffset, length);
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

		public void ReadByteArray(ByteArray bytes)
		{
			if (bytes == null) {
				throw new ArgumentNullException("bytes", "ArgumentNull_Buffer");
			}
			ReadByteArray(bytes, 0, bytesAvailable);
		}

		public void ReadByteArray(ByteArray bytes, int offset)
		{
			if (bytes == null) {
				throw new ArgumentNullException("bytes", "ArgumentNull_Buffer");
			}
			ReadByteArray(bytes, offset, bytesAvailable);
		}

		public void ReadByteArray(ByteArray bytes, int offset, int length)
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
			CheckAvailable(length);
			if (offset + length > bytes.m_Capacity) {
				bytes.CheckCapacity(offset + length);
			}
			Buffer.BlockCopy(m_Buffer, m_Position, bytes.m_Buffer, offset, length);
			m_Position += length;
			bytes.length = offset + length;
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

		private void CheckCapacity(int size)
		{
			if (size <= m_Capacity) {
				m_Length = size;
				return;
			}

			int length = size;
			if (size < 0x100) {
				size = 0x100;
			}
			if (size < m_Capacity * 2) {
				size = m_Capacity * 2;
			}
			byte[] dst = new byte[size];
			if (m_Length > 0) {
				Buffer.BlockCopy(m_Buffer, 0, dst, 0, m_Length);
			}
			m_Buffer = dst;
			m_Capacity = size;
			m_Length = length;
		}

		#region Write Methods
		public void WriteBoolean(bool value)
		{
			WriteByte((byte)(value ? 1 : 0));
		}

		public void WriteByte(byte value)
		{
			CheckCapacity(m_Position + 1);
			WriteValue(value, m_Buffer, m_Position++);
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
				CheckCapacity(m_Position + length);
				Buffer.BlockCopy(bytes, offset, m_Buffer, m_Position, length);
				m_Position += length;
			}
		}

		public void WriteByteArray(ByteArray bytes)
		{
			if (bytes == null) {
				throw new ArgumentNullException("bytes", "ArgumentNull_Buffer");
			}
			Write(bytes.m_Buffer, bytes.m_Length, 0, bytes.length);
		}

		public void WriteByteArray(ByteArray bytes, int offset)
		{
			if (bytes == null) {
				throw new ArgumentNullException("bytes", "ArgumentNull_Buffer");
			}
			if (offset < 0) {
				throw new ArgumentOutOfRangeException("offset", "ArgumentOutOfRange_NeedNonNegNum");
			}
			Write(bytes.m_Buffer, bytes.m_Length, offset, bytes.m_Length - offset);
		}

		public void WriteByteArray(ByteArray bytes, int offset, int length)
		{
			if (bytes == null) {
				throw new ArgumentNullException("bytes", "ArgumentNull_Buffer");
			}
			if (offset < 0) {
				throw new ArgumentOutOfRangeException("offset", "ArgumentOutOfRange_NeedNonNegNum");
			}
			Write(bytes.m_Buffer, bytes.m_Length, offset, length);
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
			CheckCapacity(m_Position + 8);
			WriteValue(BitConverter.GetBytes(value), m_Buffer, m_Position, endian);
			m_Position += 8;
		}

		public void WriteFloat(float value)
		{
			CheckCapacity(m_Position + 4);
			WriteValue(BitConverter.GetBytes(value), m_Buffer, m_Position, endian);
			m_Position += 4;
		}

		public void WriteInt(int value)
		{
			CheckCapacity(m_Position + 4);
			WriteValue(value, m_Buffer, m_Position, endian);
			m_Position += 4;
		}

		public void WriteLong(long value)
		{
			CheckCapacity(m_Position + 8);
			WriteValue(value, m_Buffer, m_Position, endian);
			m_Position += 8;
		}

		public void WriteMultiByte(string value, Encoding encoding)
		{
			byte[] data = encoding.GetBytes(value);
			int length = data.Length;
			this.CheckCapacity(m_Position + length);
			Buffer.BlockCopy(data, 0, m_Buffer, m_Position, length);
			m_Position += length;
		}

		public void WriteShort(short value)
		{
			CheckCapacity(m_Position + 2);
			WriteValue(value, m_Buffer, m_Position, endian);
			m_Position += 2;
		}

		public void WriteUTF(string value)
		{
			short length = (short)value.Length;
			CheckCapacity(m_Position + length + 2);
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
