//------------------------------------------------------------
// Xaz Framework
// Feedback: qq515688254
//------------------------------------------------------------
using System;
using System.Text;
using System.Collections.Generic;

namespace Xaz
{
	public interface IDataOutput
	{
		Endian endian
		{
			get;
			set;
		}

		void WriteBoolean(bool value);

		void WriteByte(byte value);

		void WriteBytes(byte[] bytes);

		void WriteBytes(byte[] bytes, int offset);

		void WriteBytes(byte[] bytes, int offset, int length);

		void WriteDouble(double value);

		void WriteFloat(float value);

		void WriteInt(int value);

		void WriteLong(long value);

		void WriteMultiByte(string value, Encoding encoding);

		void WriteShort(short value);

		void WriteUTF(string value);

		void WriteUTFBytes(string value);
	}


}
