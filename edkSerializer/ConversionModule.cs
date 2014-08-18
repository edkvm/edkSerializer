using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace edkSerializer.Core {
  public static class ConversionModule {

    private static byte[] ComputeSerialVersionUID(Object o) {
      return Guid.NewGuid().ToByteArray();
    }

    public static byte[] ConvertShortToByte(short length) {

      byte[] result = BitConverter.GetBytes(length);
      if (BitConverter.IsLittleEndian)
        Array.Reverse(result);

      return result;
    }
    
    public static int WriteStringTypeProperty(string name, string value, Stream stream) {

      // Property Type
      stream.Write(new byte[] { (byte)TypeCode.String }, 0, 1);

      // Property Name
      WriteString(name, stream);

      // Write value
      WriteString(value, stream);



      return 0;
    }

    public static int WriteString(string name, Stream stream) {


      stream.Write(ConvertShortToByte((short)name.Length), 0, 2);

      byte[] bValue = Encoding.ASCII.GetBytes(name);

      // Write class name
      stream.Write(bValue, 0, name.Length);


      return 0;

    }

    public static int WriteRefTypeProperty(string name, short value, Stream stream, byte command) {

      // Property Type
      stream.Write(new byte[] { command }, 0, 1);

      // Property Name
      WriteString(name, stream);

      // Write value
      stream.Write(ConvertShortToByte((short)value), 0, 2);

      return 0;

    }

    public static int WriteValueTypeProperty(string name, object value, Type type, Stream stream) {

      byte[] typeSymbole = new byte[] { 0 };
      byte[] valueByte = new byte[] { 0 };
      int size = 0;

      Type propertyType = type;

      System.TypeCode typeCode = Type.GetTypeCode(propertyType);
      typeSymbole = new byte[] { (byte)typeCode };

      switch (typeCode) {
        case TypeCode.Int16:
          valueByte = BitConverter.GetBytes((System.Int32)value);
          size = sizeof(System.Int32);
          break;
        case TypeCode.Int32:
          valueByte = BitConverter.GetBytes((System.Int32)value);
          size = sizeof(System.Int32);
          break;
        case TypeCode.Double:

          valueByte = BitConverter.GetBytes((System.Double)value);
          break;
        case TypeCode.UInt32:

          valueByte = BitConverter.GetBytes((System.UInt32)value);
          break;
        case TypeCode.DateTime:

          DateTime tempDate = (System.DateTime)value;
          valueByte = BitConverter.GetBytes(tempDate.ToBinary());
          break;
        default:
          typeSymbole = new byte[] { (byte)TypeCode.Int32 };
          break;

      }

      if (BitConverter.IsLittleEndian)
        Array.Reverse(valueByte);

      // Write property type
      stream.Write(typeSymbole, 0, 1);

      // Property name 
      WriteString(name, stream);

      // Write Property value
      stream.Write(valueByte, 0, size);
      return 0;
    }

    public static byte ReadByte(Stream stream) {

      byte[] currentByte = new byte[1];
      stream.Read(currentByte, 0, 1);

      return currentByte[0];
    }

    public static short ReadShort(Stream stream) {
      byte[] currentByte = new byte[2];

      stream.Read(currentByte, 0, 2);

      return ReadShort(currentByte);

    }

    public static int ReadInt(Stream stream) {
      byte[] currentByte = new byte[4];

      stream.Read(currentByte, 0, 4);

      if (BitConverter.IsLittleEndian)
        Array.Reverse(currentByte);

      return BitConverter.ToInt32(currentByte, 0);


    }

    public static object ReadValue(TypeCode typeCode, Stream stream) {
      int readBytes = 1;
      byte[] byteResult = new byte[1];
      object res = null;
      switch (typeCode) {
        case TypeCode.Int16:

          readBytes = sizeof(System.Int32);
          break;
        case TypeCode.Int32:

          readBytes = sizeof(System.Int32);
          res = ReadInt(stream);
          break;
        case TypeCode.Double:

          readBytes = sizeof(System.Double);
          break;
        case TypeCode.UInt32:

          readBytes = sizeof(System.UInt32);
          break;
        case TypeCode.DateTime:
          readBytes = sizeof(System.Int64);
          byteResult = new byte[readBytes];
          stream.Read(byteResult, 0, readBytes);
          if (BitConverter.IsLittleEndian)
            Array.Reverse(byteResult);
          long ticks = BitConverter.ToInt64(byteResult, 0);
          res = new DateTime(ticks);
          break;
        default:

          break;

      }

      return res;

    }

    public static string ReadString(Stream stream) {


      int readBytes = 2;
      byte[] currentByte = new byte[readBytes];
      // Read string size
      stream.Read(currentByte, 0, readBytes);
      readBytes = ReadShort(currentByte);

      // Read string value
      currentByte = new byte[readBytes];
      stream.Read(currentByte, 0, readBytes);

      return Encoding.ASCII.GetString(currentByte);

    }

    public static short ReadShort(byte[] array) {

      if (BitConverter.IsLittleEndian)
        Array.Reverse(array);

      return BitConverter.ToInt16(array, 0);

    }

  }
}
