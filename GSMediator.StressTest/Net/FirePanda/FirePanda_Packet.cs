using ComponentAce.Compression.Libs.zlib;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;

public delegate void TNetMessageProc(ref TNetMsg Msg);

public delegate void TLogProc(string text);

public delegate bool TDecodePacketProc(ref TNetMsg Cmd, ref ByteArray RecBuf);

public struct ByteArray
{
    public int Index;
    public int Size;
    public byte[] Data;
}

public struct TNetMsg
{
    public int Who;
    public byte MsgNo;
    public byte SubNo;
    public ushort GSWho;
    public int GSSN;
    public ByteArray Value;
}

public struct TCommands
{
    public TNetMessageProc[,] NetMsgProcs;
    public TNetMsg[] Cmds;
    public int CmdHead;
    public int CmdTail;
    public int Who;
    public byte MsgNo;
    public byte SubNo;
}

public class TPacket
{
    public ByteArray SendBuf;
    public ByteArray TempBuf;
    public ByteArray SendTempBuf;
    public const int Max_SendBuf = 1024 * 1024 * 1 - 1;
    public const int Max_RecBufSize = 1024 * 256 - 1;
    public const int Max_ReceiveSize = 1024 * 256;

    public static string HttpPost(string uri, string data)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(data);
        string queryString = sb.ToString();
        byte[] byteArray = Encoding.ASCII.GetBytes(queryString);
        WebRequest hw = WebRequest.Create(new Uri(uri));
        hw.Method = "POST";
        hw.ContentType = "application/x-www-form-urlencoded";
        hw.ContentLength = byteArray.Length;
        try
        {
            using (Stream stream = hw.GetRequestStream())
            {
                stream.Write(byteArray, 0, byteArray.Length);
                stream.Close();

                using (WebResponse hr = hw.GetResponse())
                {
                    using (StreamReader sr = new StreamReader(hr.GetResponseStream(), Encoding.GetEncoding("UTF-8")))
                    {
                        return sr.ReadToEnd();
                    }
                }
            }
        }
        catch (WebException ex)
        {
            return ex.ToString();
        }
    }

    public void PushByte(byte Data)
    {
        if (SendBuf.Size <= SendBuf.Data.Length - 1)
        {
            SendBuf.Data[SendBuf.Size] = Data;
            SendBuf.Size++;
        }
    }

    public void PushByte(sbyte Data)
    {
        int size = 1;
        if (SendBuf.Size <= SendBuf.Data.Length - 1)
        {
            Array.Copy(BitConverter.GetBytes(Data), 0, SendBuf.Data, SendBuf.Size, size);
            SendBuf.Size++;
        }
    }

    public void PushByte(bool Data)
    {
        if (SendBuf.Size <= SendBuf.Data.Length - 1)
        {
            if (Data)
                SendBuf.Data[SendBuf.Size] = 1;
            else
                SendBuf.Data[SendBuf.Size] = 0;

            SendBuf.Size++;
        }
    }

    public void PushWord(ushort Data)
    {
        int size = 2;
        if (SendBuf.Size <= SendBuf.Data.Length - size)
        {
            Array.Copy(BitConverter.GetBytes(Data), 0, SendBuf.Data, SendBuf.Size, size);
            SendBuf.Size += size;
        }
    }

    public void PushWord(short Data)
    {
        int size = 2;
        if (SendBuf.Size <= SendBuf.Data.Length - size)
        {
            Array.Copy(BitConverter.GetBytes(Data), 0, SendBuf.Data, SendBuf.Size, size);
            SendBuf.Size += size;
        }
    }

    public void PushDWord(uint Data)
    {
        int size = 4;
        if (SendBuf.Size <= SendBuf.Data.Length - size)
        {
            Array.Copy(BitConverter.GetBytes(Data), 0, SendBuf.Data, SendBuf.Size, size);
            SendBuf.Size += size;
        }
    }

    public void PushDWord(int Data)
    {
        int size = 4;
        if (SendBuf.Size <= SendBuf.Data.Length - size)
        {
            Array.Copy(BitConverter.GetBytes(Data), 0, SendBuf.Data, SendBuf.Size, size);
            SendBuf.Size += size;
        }
    }

    public void PushFloat(float Data)
    {
        int size = 4;
        if (SendBuf.Size <= SendBuf.Data.Length - size)
        {
            Array.Copy(BitConverter.GetBytes(Data), 0, SendBuf.Data, SendBuf.Size, size);
            SendBuf.Size += size;
        }
    }

    public void PushULong(ulong Data)
    {
        int size = 8;
        if (SendBuf.Size <= SendBuf.Data.Length - size)
        {
            Array.Copy(BitConverter.GetBytes(Data), 0, SendBuf.Data, SendBuf.Size, size);
            SendBuf.Size += size;
        }
    }

    public void PushDouble(double Data)
    {
        int size = 8;
        if (SendBuf.Size <= SendBuf.Data.Length - size)
        {
            Array.Copy(BitConverter.GetBytes(Data), 0, SendBuf.Data, SendBuf.Size, size);
            SendBuf.Size += size;
        }
    }

    public void PushString(string Data)
    {
        ushort len = (ushort)Data.Length;
        if (SendBuf.Size <= SendBuf.Data.Length - len * 2)
        {
            PushWord(len);
            byte[] buf = System.Text.Encoding.Unicode.GetBytes(Data);
            Array.Copy(buf, 0, SendBuf.Data, SendBuf.Size, len * 2);
            SendBuf.Size += len * 2;
        }
    }

    public void PushString(string Data, int len)
    {
        if (SendBuf.Size <= SendBuf.Data.Length - len * 2)
        {
            byte[] buf = Encoding.Unicode.GetBytes(Data);
            Array.Clear(SendBuf.Data, SendBuf.Size, len * 2);
            Array.Copy(buf, 0, SendBuf.Data, SendBuf.Size, buf.Length);
            SendBuf.Size += len * 2;
        }
    }

    private bool PushData(object Data, FieldInfo Info)
    {
        IConvertible convertible = Data as IConvertible;

        if (convertible != null)
        {
            switch (convertible.GetTypeCode())
            {
                case TypeCode.Boolean:
                    PushByte((bool)Data);
                    break;

                case TypeCode.SByte:
                    PushByte((sbyte)Data);
                    break;

                case TypeCode.Byte:
                    PushByte((byte)Data);
                    break;

                case TypeCode.Int16:
                    PushWord((short)Data);
                    break;

                case TypeCode.UInt16:
                    PushWord((ushort)Data);
                    break;

                case TypeCode.Int32:
                    PushDWord((int)Data);
                    break;

                case TypeCode.UInt32:
                    PushDWord((uint)Data);
                    break;

                case TypeCode.UInt64:
                    PushULong((ulong)Data);
                    break;

                case TypeCode.Single:
                    PushFloat((float)Data);
                    break;

                case TypeCode.Double:
                    PushDouble((double)Data);
                    break;

                case TypeCode.DateTime:
                    PushDouble(((DateTime)Data).ToOADate());
                    break;

                case TypeCode.String:
                    int len = GetTypeSize(Info);
                    PushString((string)Data, len);
                    break;

                default:
                    return false;
            }

            return true;
        }

        return false;
    }

    private static BindingFlags CheckFieldFlags =
        BindingFlags.Public | BindingFlags.NonPublic |
            BindingFlags.Static | BindingFlags.Instance |
            BindingFlags.DeclaredOnly;

    private void PushType(object Data)
    {
        FieldInfo[] fieldInfos = Data.GetType().GetFields(CheckFieldFlags);

        for (int i = 0; i < fieldInfos.Length; i++)
        {
            if (fieldInfos[i].FieldType.IsArray)
            {
                Array array = (Array)fieldInfos[i].GetValue(Data);

                if (array != null)
                {
                    for (int j = 0; j < array.Length; j++)
                    {
                        PushType(array.GetValue(j));
                    }
                }
            }
            else if (!fieldInfos[i].IsLiteral)
            {
                object tempValue = fieldInfos[i].GetValue(Data);

                if (!PushData(tempValue, fieldInfos[i]))
                    PushType(tempValue);
            }
        }
    }

    public void PushType<T>(T Data)
    {
        object tempValue = Data;
        PushType(tempValue);
    }

    public void PushBuf(ByteArray buf)
    {
        if (buf.Size == 0)
            return;

        int size = buf.Size - buf.Index;
        if (SendBuf.Size <= SendBuf.Data.Length - size)
        {
            Array.Copy(buf.Data, buf.Index, SendBuf.Data, SendBuf.Size, size);
            SendBuf.Size += size;
        }
    }

    private static sbyte GetBufToSByte(ref ByteArray buf)
    {
        sbyte result;
        int size = 1;
        if (buf.Index + size <= buf.Size)
        {
            result = (sbyte)buf.Data[buf.Index];
            buf.Index += size;
        }
        else
            result = 0;

        return result;
    }

    public static byte GetBufToByte(ref ByteArray buf)
    {
        byte result = 0;
        int size = 1;
        if (buf.Index + size <= buf.Size)
        {
            result = buf.Data[buf.Index];
            buf.Index += size;
        }

        return result;
    }

    public static bool GetBufToBool(ref ByteArray buf)
    {
        bool result = false;
        int size = 1;
        if (buf.Index + size <= buf.Size)
        {
            result = BitConverter.ToBoolean(buf.Data, buf.Index);
            buf.Index += size;
        }

        return result;
    }

    public static ushort GetBufToWord(ref ByteArray buf)
    {
        ushort result = 0;
        int size = 2;
        if (buf.Index + size <= buf.Size)
        {
            result = BitConverter.ToUInt16(buf.Data, buf.Index);
            buf.Index += size;
        }

        return result;
    }

    private static short GetBufToShort(ref ByteArray buf)
    {
        short result = 0;
        int size = 2;
        if (buf.Index + size <= buf.Size)
        {
            result = BitConverter.ToInt16(buf.Data, buf.Index);
            buf.Index += size;
        }

        return result;
    }

    public static uint GetBufToDWord(ref ByteArray buf)
    {
        uint result = 0;
        int size = 4;
        if (buf.Index + size <= buf.Size)
        {
            result = BitConverter.ToUInt32(buf.Data, buf.Index);
            buf.Index += size;
        }

        return result;
    }

    public static int GetBufToInt(ref ByteArray buf)
    {
        int result = 0;
        int size = 4;
        if (buf.Index + size <= buf.Size)
        {
            result = BitConverter.ToInt32(buf.Data, buf.Index);
            buf.Index += size;
        }

        return result;
    }

    public static float GetBufToFloat(ref ByteArray buf)
    {
        float result = 0;
        int size = 4;
        if (buf.Index + size <= buf.Size)
        {
            result = BitConverter.ToSingle(buf.Data, buf.Index);
            buf.Index += size;
        }

        return result;
    }

    public static ulong GetBufToULong(ref ByteArray buf)
    {
        ulong result = 0;
        int size = 8;
        if (buf.Index + size <= buf.Size)
        {
            result = BitConverter.ToUInt64(buf.Data, buf.Index);
            buf.Index += size;
        }

        return result;
    }

    public static double GetBufToDouble(ref ByteArray buf)
    {
        double result = 0;
        int size = 8;
        if (buf.Index + size <= buf.Size)
        {
            result = BitConverter.ToDouble(buf.Data, buf.Index);
            buf.Index += size;
        }

        return result;
    }

    public static string GetBufToStr(ref ByteArray buf)
    {
        string result = "";
        ushort size = GetBufToWord(ref buf);
        if ((size > 0) && (buf.Index + size * 2 <= buf.Size))
        {
            result = Encoding.Unicode.GetString(buf.Data, buf.Index, size * 2);
            buf.Index += size * 2;
        }

        return result;
    }

    public static string GetBufToStr(ref ByteArray buf, int Len, bool GetRealStr = false)
    {
        string result = "";
        if ((Len > 0) && (buf.Index + Len * 2 <= buf.Size))
        {
            result = Encoding.Unicode.GetString(buf.Data, buf.Index, Len * 2);
            buf.Index += Len * 2;
        }

        if (GetRealStr)
            result = GetRealString(result);

        return result;
    }

    /// <summary>
    /// 取得不含空白字元的文字 (進行String的拼字有問題時使用)
    /// </summary>
    public static string GetRealString(string text)
    {
        if (text == null)
            return string.Empty;

        string real = text.Replace("\0", "");

        return real;
    }

    private static int GetTypeSize(FieldInfo Info)
    {
        object[] objs = Info.GetCustomAttributes(false);
        if (objs.Length > 0)
        {
            Myatt attr = (Myatt)objs[0];
            return attr.arrayLen;
        }
        else
            return 0;
    }

    private static bool PopData(ref ByteArray buf, ref object Data, FieldInfo Info)
    {
        System.IConvertible convertible = Data as System.IConvertible;

        if (convertible != null)
        {
            switch (convertible.GetTypeCode())
            {
                case TypeCode.Boolean:
                    Data = GetBufToBool(ref buf);
                    return true;

                case TypeCode.SByte:
                    Data = GetBufToSByte(ref buf);
                    return true;

                case TypeCode.Byte:
                    Data = GetBufToByte(ref buf);
                    return true;

                case TypeCode.Int16:
                    Data = GetBufToShort(ref buf);
                    return true;

                case TypeCode.UInt16:
                    Data = GetBufToWord(ref buf);
                    return true;

                case TypeCode.Int32:
                    Data = GetBufToInt(ref buf);
                    return true;

                case TypeCode.UInt32:
                    Data = GetBufToDWord(ref buf);
                    return true;

                case TypeCode.UInt64:
                    Data = GetBufToULong(ref buf);
                    return true;

                case TypeCode.Single:
                    Data = GetBufToFloat(ref buf);
                    return true;

                case TypeCode.Double:
                    Data = GetBufToDouble(ref buf);
                    return true;

                case TypeCode.DateTime:
                    Data = DateTime.FromOADate(GetBufToDouble(ref buf));
                    return true;

                case TypeCode.String:
                    int len = GetTypeSize(Info);
                    Data = GetBufToStr(ref buf, len, true);
                    return true;

                default:
                    return false;
            }
        }

        return false;
    }

    private static int get_marshalnum(FieldInfo fInf)
    {
        object[] objs = fInf.GetCustomAttributes(false);

        for (int i = 0; i < objs.Length; i++)
            if (objs[i].GetType() == typeof(Myatt))
                return ((Myatt)objs[i]).arrayLen;

        return 0;
    }

    private static bool GetBufToType(ref ByteArray buf, ref object Data)
    {
        bool result = false;
        FieldInfo[] fieldInfos = Data.GetType().GetFields();

        result = true;

        for (int i = 0; i < fieldInfos.Length; i++)
        {
            if (fieldInfos[i].FieldType.IsArray)
            {
                int num = get_marshalnum(fieldInfos[i]);
                Type subtp = fieldInfos[i].FieldType.GetElementType();

                Array tempAy = Array.CreateInstance(subtp, num);

                for (int j = 0; j < tempAy.Length; j++)
                {
                    object tempValue = tempAy.GetValue(j);

                    if (PopData(ref buf, ref tempValue, fieldInfos[i]))
                    {
                        tempAy.SetValue(tempValue, j);
                    }
                    else
                    {
                        object aStruct = Activator.CreateInstance(subtp);
                        if (GetBufToType(ref buf, ref aStruct))
                        {
                            tempAy.SetValue(aStruct, j);
                        }
                        else
                        {
                            result = false;
                            break;
                        }
                    }
                }

                fieldInfos[i].SetValue(Data, tempAy);
            }
            else if (!fieldInfos[i].IsLiteral)
            {
                object tempValue = fieldInfos[i].GetValue(Data);

                if (PopData(ref buf, ref tempValue, fieldInfos[i]))
                {
                    fieldInfos[i].SetValue(Data, tempValue);
                }
                else
                {
                    object aStruct = Activator.CreateInstance(fieldInfos[i].FieldType);
                    if (GetBufToType(ref buf, ref aStruct))
                    {
                        fieldInfos[i].SetValue(Data, aStruct);
                    }
                    else
                    {
                        result = false;
                        break;
                    }
                }
            }
        }

        return result;
    }

    public static bool GetBufToType<T>(ref ByteArray buf, ref T Data)
    {
        bool result = false;
        object tempValue = Data;

        if (GetBufToType(ref buf, ref tempValue))
        {
            Data = (T)tempValue;
            result = true;
        }

        return result;
    }

    public static bool GetBufToZip(ref ByteArray buf, ref ByteArray data)
    {
        try
        {
            if (buf.Index + 2 <= buf.Size)
            {
                ushort len = GetBufToWord(ref buf);
                data.Data = DeCompress(buf.Data, buf.Index, len);
                data.Index = 0;
                data.Size = data.Data.Length;
                buf.Index += len;
                return true;
            }

            return false;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
            return false;
        }
    }

    public void SendBufToTemp()
    {
        TempBuf.Index = SendBuf.Index;
        TempBuf.Size = SendBuf.Size;
        Array.Copy(SendBuf.Data, TempBuf.Data, SendBuf.Size);
        SendBuf.Size = 0;
    }

    public void SendBufToSendTemp()
    {
        SendTempBuf.Index = SendBuf.Index;
        SendTempBuf.Size = SendBuf.Size;
        Array.Copy(SendBuf.Data, SendTempBuf.Data, SendBuf.Size);
        SendBuf.Size = 0;
    }

    public void SendBufToTemp(ref byte[] buf)
    {
        if (buf.Length >= SendBuf.Size)
            Array.Copy(SendBuf.Data, buf, SendBuf.Size);

        SendBuf.Size = 0;
    }

    private static byte[] mSocketTempBuf = new byte[Max_ReceiveSize];

    public static void SocketRead(ref Socket Socket, ref TCommands RcvCmds, ref ByteArray RecBuf, TDecodePacketProc DecodeProc, byte XorByte)
    {
        if (Socket == null)
            return;
        if (Socket.Connected == false)
            return;
        if (Socket != null && Socket.Available > 0)
        {
            //野汪修正 續傳Buf不夠問題
            Array.Clear(mSocketTempBuf, 0, mSocketTempBuf.Length);

            int count = Socket.Receive(mSocketTempBuf);
            int size = Math.Min(mSocketTempBuf.GetUpperBound(0) + 1, count);

            if ((size > 0) && (size + RecBuf.Size <= RecBuf.Data.Length))
            {
                if (XorByte > 0)
                {
                    for (int i = 0; i < mSocketTempBuf.Length; i++)
                        mSocketTempBuf[i] = (byte)(mSocketTempBuf[i] ^ XorByte);
                }

                Array.Copy(mSocketTempBuf, 0, RecBuf.Data, RecBuf.Size, size);
                RecBuf.Size += size;

                while (RecBuf.Size > 0)
                {
                    if (DecodeProc(ref RcvCmds.Cmds[RcvCmds.CmdTail], ref RecBuf))
                    {
                        RcvCmds.CmdTail++;
                        if (RcvCmds.CmdTail == RcvCmds.CmdHead)
                        {
                            RcvCmds.CmdTail = 0;
                            RcvCmds.CmdHead = 0;
                        }

                        if (RcvCmds.CmdTail > RcvCmds.Cmds.GetUpperBound(0))
                            RcvCmds.CmdTail = 0;

                        if (RecBuf.Index > 0 && RecBuf.Index < RecBuf.Size)
                        {
                            Array.Copy(RecBuf.Data, RecBuf.Index, RecBuf.Data, 0, RecBuf.Size - RecBuf.Index);
                            RecBuf.Size -= RecBuf.Index;
                        }
                        else
                            RecBuf.Size = 0;

                        RecBuf.Index = 0;
                    }
                    else
                        return;
                }
            }
        }
    }

    public static void UdpSocketRead(ref UdpClient UdpListener, ref TCommands RcvCmds, ref ByteArray RecBuf, TDecodePacketProc DecodeProc, byte XorByte, IPEndPoint _endPoint)
    {
        if (UdpListener != null && UdpListener.Available > 0)
        {
            byte[] udpTempBuf = new byte[Max_ReceiveSize];

            udpTempBuf = UdpListener.Receive(ref _endPoint);
            int size = udpTempBuf.Length;
            if ((size > 0) && (size + RecBuf.Size <= RecBuf.Data.Length))
            {
                if (XorByte > 0)
                {
                    for (int i = 0; i < udpTempBuf.Length; i++)
                        udpTempBuf[i] = (byte)(udpTempBuf[i] ^ XorByte);
                }

                Array.Copy(udpTempBuf, 0, RecBuf.Data, RecBuf.Index, size);
                RecBuf.Size += size;

                while (RecBuf.Size > 0)
                {
                    if (DecodeProc(ref RcvCmds.Cmds[RcvCmds.CmdTail], ref RecBuf))
                    {
                        RcvCmds.CmdTail++;
                        if (RcvCmds.CmdTail == RcvCmds.CmdHead)
                        {
                            RcvCmds.CmdTail = 0;
                            RcvCmds.CmdHead = 0;
                        }

                        if (RcvCmds.CmdTail > RcvCmds.Cmds.GetUpperBound(0))
                            RcvCmds.CmdTail = 0;

                        if (RecBuf.Index > 0 && RecBuf.Index < RecBuf.Size)
                        {
                            Array.Copy(RecBuf.Data, RecBuf.Index, RecBuf.Data, 0, size - RecBuf.Index);
                            RecBuf.Size -= RecBuf.Index;
                        }
                        else
                            RecBuf.Size = 0;

                        RecBuf.Index = 0;
                    }
                    else
                        return;
                }
            }
        }
    }

    public static void Handle_Commands(ref TCommands RcvCmds)
    {
        int cnt;
        int CmdIndex;
        cnt = 0;
        while (RcvCmds.CmdHead != RcvCmds.CmdTail)
        {
            CmdIndex = RcvCmds.CmdHead;
            RcvCmds.Who = RcvCmds.Cmds[CmdIndex].Who;
            RcvCmds.MsgNo = RcvCmds.Cmds[CmdIndex].MsgNo;
            RcvCmds.SubNo = RcvCmds.Cmds[CmdIndex].SubNo;
            try
            {
                if (RcvCmds.NetMsgProcs[RcvCmds.MsgNo, RcvCmds.SubNo] != null)
                {
                    RcvCmds.NetMsgProcs[RcvCmds.MsgNo, RcvCmds.SubNo](ref RcvCmds.Cmds[CmdIndex]);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(string.Format("protocol error {0} {1}", RcvCmds.Cmds[CmdIndex].MsgNo, RcvCmds.Cmds[CmdIndex].SubNo)
                    + ", " + e.Message + ", " + e.StackTrace);
            }

            RcvCmds.Who = 0;
            RcvCmds.MsgNo = 0;
            RcvCmds.SubNo = 0;
            RcvCmds.Cmds[CmdIndex].Who = 0;
            RcvCmds.Cmds[CmdIndex].MsgNo = 0;
            RcvCmds.Cmds[CmdIndex].SubNo = 0;
            RcvCmds.Cmds[CmdIndex].GSWho = 0;
            RcvCmds.Cmds[CmdIndex].GSSN = 0;
            RcvCmds.Cmds[CmdIndex].Value.Index = 0;
            RcvCmds.CmdHead++;
            if (RcvCmds.CmdHead > RcvCmds.Cmds.GetUpperBound(0))
                RcvCmds.CmdHead = 0;

            cnt++;
            if ((cnt > 100))
                return;
        }
    }

    #region Zlib

    public static MemoryStream CompressStream(MemoryStream SourceStream)
    {
        try
        {
            MemoryStream stmOutput = new MemoryStream();
            ZOutputStream outZStream = new ZOutputStream(stmOutput, zlibConst.Z_BEST_COMPRESSION);
            SourceStream.Position = 0;
            outZStream.Position = 0;
            byte[] tempbuf = new byte[SourceStream.Length];
            SourceStream.Read(tempbuf, 0, (int)SourceStream.Length);
            outZStream.Write(tempbuf, 0, (int)SourceStream.Length);
            outZStream.finish();

            return stmOutput;
        }
        catch
        {
            return null;
        }
    }

    public static byte[] Compress(byte[] SourceByte, int index, int count)
    {
        try
        {
            MemoryStream stmInput = new MemoryStream(SourceByte, index, count);
            MemoryStream stmOutPut = CompressStream(stmInput);
            byte[] bytOutPut = new byte[stmOutPut.Length];
            stmOutPut.Position = 0;
            stmOutPut.Read(bytOutPut, 0, bytOutPut.Length);
            return bytOutPut;
        }
        catch
        {
            return null;
        }
    }

    private static MemoryStream DecompressStream(MemoryStream SourceStream)
    {
        try
        {
            MemoryStream stmOutput = new MemoryStream();
            ZOutputStream outZStream = new ZOutputStream(stmOutput);
            SourceStream.Position = 0;
            outZStream.Position = 0;
            byte[] tempbuf = new byte[SourceStream.Length];
            SourceStream.Read(tempbuf, 0, (int)SourceStream.Length);
            outZStream.Write(tempbuf, 0, (int)SourceStream.Length);
            outZStream.finish();

            return stmOutput;
        }
        catch
        {
            return null;
        }
    }

    public static byte[] DeCompress(byte[] SourceByte, int index, int count)
    {
        try
        {
            MemoryStream stmInput = new MemoryStream(SourceByte, index, count);
            MemoryStream stmOutPut = DecompressStream(stmInput);
            byte[] bytOutPut = new byte[stmOutPut.Length];
            stmOutPut.Position = 0;
            stmOutPut.Read(bytOutPut, 0, bytOutPut.Length);
            stmInput.Close();
            stmOutPut.Close();

            return bytOutPut;
        }
        catch
        {
            return null;
        }
    }

    #endregion

    protected void Init_Buf()
    {
        SendBuf.Data = new byte[Max_SendBuf];
        SendBuf.Size = 0;
        SendBuf.Index = 0;

        TempBuf.Data = new byte[Max_SendBuf];
        TempBuf.Size = 0;
        TempBuf.Index = 0;

        SendTempBuf.Data = new byte[Max_SendBuf];
        SendTempBuf.Size = 0;
        SendTempBuf.Index = 0;
    }

    protected void Free_Buf()
    {
        SendBuf.Data = null;
        TempBuf.Data = null;
        SendTempBuf.Data = null;
    }
}