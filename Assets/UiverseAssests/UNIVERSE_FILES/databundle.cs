using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
public class BYTE_HELPER
{
    MemoryStream memstream = null;
    public BinaryReader reader = null;
    public BinaryWriter writer = null;
    public BYTE_HELPER(byte[] raw=null)
    {
        if (raw == null)
        {
            memstream = new MemoryStream();
        }
        else
        {
            memstream = new MemoryStream(raw);
        }
        reader = new BinaryReader(memstream);
        writer = new BinaryWriter(memstream);
    }
    public void set(byte[] raw)
    {
        memstream = new MemoryStream(raw);
        reader = new BinaryReader(memstream);
        writer = new BinaryWriter(memstream);
    }
    public byte[] get()
    {
        byte[] bytes = memstream.ToArray();
        return bytes;
    }
    public int getLength()
    {
        return (int)memstream.Length;
    }
    public override string ToString()
    {
        string return_value = "";
        byte[] bytes = get();
        for (int i = 0; i < bytes.Length; i++)
        {
            return_value += ((int)bytes[i]).ToString() ;
            if (i < bytes.Length - 1) return_value +=  ",";
        }
        return return_value;
    }
    
}
public class HASH_MESSAGE{
    public int Fuction = 0;
    public BYTE_HELPER Params = null;
    public HASH_MESSAGE()
    {
        Fuction = (int)CtosMessage.Response;
        Params = new BYTE_HELPER();
    }
}
public static class BinaryExtensions
{
    public static void WriteUnicode(this BinaryWriter writer, string text, int len)
    {
        byte[] unicode = Encoding.Unicode.GetBytes(text);
        byte[] result = new byte[len * 2];
        int max = len * 2 - 2;
        Array.Copy(unicode, result, unicode.Length > max ? max : unicode.Length);
        writer.Write(result);
    }

    public static string ReadUnicode(this BinaryReader reader, int len)
    {
        byte[] unicode = reader.ReadBytes(len * 2);
        string text = Encoding.Unicode.GetString(unicode);
        text = text.Substring(0, text.IndexOf('\0'));
        return text;
    }

    public static byte[] ReadToEnd(this BinaryReader reader)
    {
        return reader.ReadBytes((int)(reader.BaseStream.Length - reader.BaseStream.Position));
    }
}

