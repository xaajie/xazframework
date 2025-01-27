//------------------------------------------------------------
// Xaz Framework
// Feedback: qq515688254
//------------------------------------------------------------
using System;
using System.Text;
using System.Collections.Generic;

namespace Xaz
{
	public interface IDataInput
	{
		int bytesAvailable
		{
			get;
		}

		Endian endian
		{
			get;
			set;
		}

		bool ReadBoolean();

		sbyte ReadByte();

		void ReadBytes(byte[] bytes);

		void ReadBytes(byte[] bytes, int offset);

		void ReadBytes(byte[] bytes, int offset, int length);

		double ReadDouble();

		float ReadFloat();

		int ReadInt();

		long ReadLong();

		string ReadMultiByte(int length, Encoding encoding);

		short ReadShort();

		byte ReadUnsignedByte();

		uint ReadUnsignedInt();

		ushort ReadUnsignedShort();

		ulong ReadUnsignedLong();

		string ReadUTF();

		string ReadUTFBytes(int length);
	}

}
