using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

public class SFFormatter
{
    public static byte[] Serialize(object obj)
    {

        IFormatter formatter = new BinaryFormatter();
        using (MemoryStream stream = new MemoryStream())
        {
            formatter.Serialize(stream, obj);
            return stream.ToArray();
        }
    }

    public static object Deserialize(byte[] byteStream)
    {
        IFormatter formatter = new BinaryFormatter();
        using (MemoryStream stream = new MemoryStream(byteStream))
        {
            return formatter.Deserialize(stream);
        }
    }
}
