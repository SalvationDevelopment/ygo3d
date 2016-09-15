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

    public static bool Handshaking(NetworkStream stream, UInt64 cookie)
    {
        var rng = new Random();
        var mySeed = (UInt32)rng.Next();
        stream.Write(BitConverter.GetBytes(mySeed), 0, 4);
        MD5CryptoServiceProvider md5Maker = new MD5CryptoServiceProvider();
        var seed = BitConverter.ToUInt32(ReadFull(stream, 4), 0);
        var sum = cookie + seed;
        var md5Buf = md5Maker.ComputeHash(BitConverter.GetBytes(sum));
        stream.Write(md5Buf, 0, 16);
        md5Buf = ReadFull(stream, 16);
        var mySum = cookie + mySeed;
        var myMd5Buf = md5Maker.ComputeHash(BitConverter.GetBytes(mySum));
        bool sussess = true;
        if (myMd5Buf.Length != md5Buf.Length)
        {
            sussess = false;
        }
        else
        {
            for (int i = 0; i < myMd5Buf.Length;i++ )
            {
                if (myMd5Buf[i] != md5Buf[i])
                {
                    sussess=false;
                }
            }
        }
        return sussess;
    }

    public static byte[] HashString(string str)
    {
        MD5CryptoServiceProvider md5Maker = new MD5CryptoServiceProvider();
        return md5Maker.ComputeHash(Encoding.UTF8.GetBytes(str));
    }

    public static byte[] GetLoginBuffer(string user_name,string pass_word)
    {
        byte[] hashed_pw = HELPER_TCP.HashString(pass_word);
        var package = protos.Public.Types.Cts.Types.Login.CreateBuilder();
        package.Account = user_name;
        package.Password = Google.ProtocolBuffers.ByteString.CopyFrom(hashed_pw);
        string type = "protos.Public.Cts.Login";
        byte[] data=package.Build().ToByteArray();
        return WrappeBuffer(type, data);
    }

    public static byte[] WrappeBuffer(string type,byte[] buffer)
    {
        var package = protos.Gilgamesh.CreateBuilder();
        package.Type = type;
        package.Data = Google.ProtocolBuffers.ByteString.CopyFrom(buffer);
        return package.Build().ToByteArray();
    }

    public static string Unmarshal(byte[] d, out Google.ProtocolBuffers.IMessage m)
    {
        var g = protos.Gilgamesh.ParseFrom(d);
        var tt = Type.GetType(g.Type);
        var instance = Google.ProtocolBuffers.MessageUtil.GetDefaultMessage(tt);
        var obj = Google.ProtocolBuffers.DynamicMessage.ParseFrom(instance.DescriptorForType, g.Data.ToByteArray());
        m = obj as Google.ProtocolBuffers.IMessage;
        return g.Type;
    }

    public static byte[] Warper(Google.ProtocolBuffers.IMessage m)
    {
        byte[] data = m.ToByteArray();
        return WrappeBuffer(GetMessageName(m), data);
    }

    static string GetMessageName(Google.ProtocolBuffers.IMessage m)
    {
        return m.GetType().ToString().Replace("+", ".").Replace(".Types", "").Replace("Protos", "protos");
    }

    internal static byte[] GetRegistBuffer(string p1, string p2)
    {
        throw new NotImplementedException();
    }
}

