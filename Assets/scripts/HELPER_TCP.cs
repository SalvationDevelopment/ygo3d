using System;
using System.IO;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
public class HELPER_TCP
{
    public static byte[] ReadFull(NetworkStream stream, int length)
    {
        var buf = new byte[length];
        int rlen = 0;
        while (rlen < buf.Length)
        {
            rlen += stream.Read(buf, rlen, buf.Length - rlen);
        }

        return buf;
    }

    public static byte[] ReadPacket(NetworkStream stream)
    {
        var hdr = ReadFull(stream, 4);
        var plen = BitConverter.ToInt32(hdr, 0);
        var buf = ReadFull(stream, plen);
        return buf;
    }

    public static void WritePacket(NetworkStream stream, byte[] d)
    {
        var hdr = BitConverter.GetBytes((UInt32)d.Length);
        stream.Write(hdr, 0, 4);
        stream.Write(d, 0, d.Length);
        return;
    }

    public static byte[] ToMd5String(string str)
    {
        MD5CryptoServiceProvider md5Maker = new MD5CryptoServiceProvider();
        return md5Maker.ComputeHash(Encoding.UTF8.GetBytes(str));
    }

    public static byte[] GetLoginBuffer(string user_name,string pass_word)
    {
        var package = protos.Auth.Types.AuthRequest.CreateBuilder();
        package.Account = user_name;
        package.Password = Google.ProtocolBuffers.ByteString.CopyFrom(HELPER_TCP.ToMd5String(pass_word));
        return Warper(package.Build());
    }

    public static string Unmarshal(byte[] d, out Google.ProtocolBuffers.IMessage m)
    {
        var g = protos.Box.ParseFrom(d);
        var tt = Type.GetType(g.Type);
        var instance = Google.ProtocolBuffers.MessageUtil.GetDefaultMessage(tt);
        var obj = Google.ProtocolBuffers.DynamicMessage.ParseFrom(instance.DescriptorForType, g.Data.ToByteArray());
        m = obj as Google.ProtocolBuffers.IMessage;
        return g.Type;
    }

    public static byte[] Warper(Google.ProtocolBuffers.IMessage m)
    {
        var package = protos.Box.CreateBuilder();
        package.Type = GetMessageName(m);
        package.Data = Google.ProtocolBuffers.ByteString.CopyFrom(m.ToByteArray());
        return package.Build().ToByteArray();
    }

    static string GetMessageName(Google.ProtocolBuffers.IMessage m)
    {
        return m.GetType().ToString().Replace("+", ".").Replace(".Types", "").Replace("Protos", "protos");
    }
}

