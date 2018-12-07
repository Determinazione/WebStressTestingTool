using System;
using System.Net;
using System.Net.Sockets;

public struct TGateHeader
{
    public ushort Header;
    public ushort PacketLen;
    public byte MsgNo;
    public byte SubNo;
}

public class GSocket : TPacket
{
    private int FMaxRecBuf = 1024 * 512;
    private int FMaxCmds = 1024;
    private TGateHeader FRecHeader;
    private ByteArray FRecBuf;
    private bool mProtocalFlag = false;
    public TCommands Cmds;
    public Socket FSocket = null;
    public int ServerPort = 15800;

    private bool Decode_Packet(ref TNetMsg Cmd, ref ByteArray RecBuf)
    {
        int size = 6;
        if (RecBuf.Size >= size)
        {
            FRecHeader.Header = BitConverter.ToUInt16(RecBuf.Data, 0);
            if (FRecHeader.Header == 46061)
            {
                FRecHeader.PacketLen = BitConverter.ToUInt16(RecBuf.Data, 2);
                FRecHeader.MsgNo = RecBuf.Data[4];
                FRecHeader.SubNo = RecBuf.Data[5];

                RecBuf.Index = Math.Min(RecBuf.Size, FRecHeader.PacketLen);
                //檢查命令長度 封包是否已經完全取得
                if (RecBuf.Size < FRecHeader.PacketLen)
                    return false;

                Cmd.MsgNo = FRecHeader.MsgNo;
                Cmd.SubNo = FRecHeader.SubNo;
                Cmd.Value.Index = 0;
                Cmd.Value.Size = Math.Max(0, FRecHeader.PacketLen - size);
                if (Cmd.Value.Size > 0)
                {
                    if (Cmd.Value.Data == null || Cmd.Value.Size > Cmd.Value.Data.Length)
                        Cmd.Value.Data = new byte[Cmd.Value.Size];

                    Array.Copy(RecBuf.Data, size, Cmd.Value.Data, 0, Cmd.Value.Size);
                }

                return true;
            }
            else
            {
                RecBuf.Size = 0;
                return false;
            }
        }
        else
            return false;
    }

    public void OnSocketRead()
    {
        CheckConnect();
        SocketRead(ref FSocket, ref Cmds, ref FRecBuf, Decode_Packet, 0);
    }

    public void SendMsg(byte Kind1, byte Kind2, bool UiLock = true, System.Action<byte> TimeLockEvent = null)
    {
        // 斷線重連
        if (FSocket == null)
        {
            SendBuf.Size = 0;
            return;
        }

        if (FSocket != null)
        {
            if (FSocket.Connected)
            {
                try
                {
                    SendBufToSendTemp();

                    PushByte(237);
                    PushByte(179);
                    PushWord((ushort)(SendTempBuf.Size + 6));
                    PushByte(Kind1);
                    PushByte(Kind2);
                    PushBuf(SendTempBuf);
                    FSocket.Send(SendBuf.Data, SendBuf.Size, SocketFlags.None);
                    SendBuf.Size = 0;
                }
                catch (ArgumentNullException ex)
                {
                    Console.WriteLine(ex.ToString());
                    SendError("1");
                }
                catch (SocketException ex)
                {
                    Console.WriteLine(ex.ToString());
                    SendError("2");
                }
                catch (ObjectDisposedException ex)
                {
                    Console.WriteLine(ex.ToString());
                    SendError("3");
                }
            }
            else
            {
                SendError("0");
            }
        }
    }

    private void SendError(string ex)
    {
        SendBuf.Size = 0;
        Disconnect();
    }

    public void Connect(string ip, int port, bool testconnect, System.Action<bool> callback)
    {
        Disconnect();

        IPAddress[] ipaddressay = Dns.GetHostAddresses(ip);
        IPEndPoint ipendpoint = new IPEndPoint(ipaddressay[0], port);

        FSocket = new Socket(ipendpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        FSocket.ReceiveBufferSize = TPacket.Max_SendBuf;
        FSocket.SendTimeout = 10000;
        FSocket.ReceiveTimeout = 10000;

        if (!FSocket.Connected)
        {
            mTestconnect = testconnect;
            mConnectCallback = callback;

            mTryConnect = true;
            mGetCallback = false;
            mConnectResult = false;
            FSocket.BeginConnect(ipaddressay[0], port, new AsyncCallback(ConnectCallback), null);
        }
    }

    private bool mTryConnect = false;
    private bool mGetCallback = false;
    private bool mConnectResult = false;
    private bool mTestconnect = false;
    private Action<bool> mConnectCallback = null;

    private void CheckConnect()
    {
        if (!mTryConnect)
            return;

        if (!mGetCallback)
            return;

        if (!mConnectResult)
        {
            Disconnect();

            if (!mTestconnect)
            {
            }
        }

        bool result = mConnectResult;
        Action<bool> callback = mConnectCallback;

        mTryConnect = false;
        mGetCallback = false;
        mTestconnect = false;
        mConnectResult = false;
        mConnectCallback = null;

        callback(result);
    }

    public void ConnectCallback(IAsyncResult ar)
    {
        try
        {
            FSocket.EndConnect(ar);
            mConnectResult = true;
        }
        catch
        {
            mConnectResult = false;
        }

        mGetCallback = true;
    }

    public void Disconnect(bool isCloseSocket = false)
    {
        if (FSocket != null)
        {
            try
            {
                if (FSocket.Connected)
                    FSocket.Shutdown(SocketShutdown.Both);

                if (isCloseSocket)
                {
                    FSocket.Close();
                    FSocket = null;
                }
                else
                {
                    FSocket.Close();
                    FSocket = null;
                }
            }
            catch
            {
                FSocket = null;
            }

            ProtocalFlag = false;
        }
    }

    public void Initialization()
    {
        Init_Buf();

        FRecBuf.Data = new byte[FMaxRecBuf];
        Cmds.NetMsgProcs = new TNetMessageProc[byte.MaxValue, byte.MaxValue];
        Cmds.Cmds = new TNetMsg[FMaxCmds];
    }

    public void Finalization()
    {
        Disconnect(true);
        Free_Buf();

        FRecBuf.Data = null;
        Cmds.NetMsgProcs = null;
        Cmds.Cmds = null;
    }

    public bool ProtocalFlag
    {
        get
        {
            return mProtocalFlag;
        }
        set
        {
            mProtocalFlag = value;
        }
    }
}