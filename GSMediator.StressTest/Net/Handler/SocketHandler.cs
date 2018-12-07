using GSMediator.StressTest.Net.Listener.Connect;
using GSMediator.StressTest.Net.Listener.Item;
using GSMediator.StressTest.PerformaceMonitor;
using System;
using System.Collections.Generic;

namespace GSMediator.StressTest.Net
{
    public enum EventCode
    {
        BagEvent,
        BagUpdateEvent,
        DisplayRewardInfoEvent,
        LoginFakeEvent,
        SignupFakeEvent,
        RedemptionActivationFakeEvent,
    }

    /// <summary>
    /// 可將有實作IGSocketListener的類別，透過AddListener監聽特定EventCode的網路事件。
    /// 並從OnEvent的ref TNetMsg Msg參數取得Server Push回來的資料。
    /// </summary>
    public class SocketHandler : ISocketHandler
    {
        public GSocket GSocket { get; }

        private const string GSIP = "192.168.37.23";
        private const int GSPort = 15800;
        private readonly Dictionary<EventCode, EventHandler> mSocketDictionary = new Dictionary<EventCode, EventHandler>();

        public SocketHandler(Action<bool> callback, MonitorUnit monitorUnit)
        {
            GSocket = new GSocket();
            GSocket.Initialization();

            GSocket.Cmds.NetMsgProcs[1, 71] = CreateEvent(EventCode.LoginFakeEvent);
            GSocket.Cmds.NetMsgProcs[1, 72] = CreateEvent(EventCode.SignupFakeEvent);
            GSocket.Cmds.NetMsgProcs[1, 111] = CreateEvent(EventCode.RedemptionActivationFakeEvent);
            GSocket.Cmds.NetMsgProcs[3, 1] = CreateEvent(EventCode.BagEvent);
            GSocket.Cmds.NetMsgProcs[3, 2] = CreateEvent(EventCode.BagUpdateEvent);
            GSocket.Cmds.NetMsgProcs[3, 100] = CreateEvent(EventCode.DisplayRewardInfoEvent);

            AddListener(EventCode.LoginFakeEvent, new LoginListener(GSocket, monitorUnit));
            AddListener(EventCode.SignupFakeEvent, new SignupListener(GSocket, monitorUnit));
            AddListener(EventCode.RedemptionActivationFakeEvent, new RedemptionActivationFakeListener(monitorUnit));
            AddListener(EventCode.BagEvent, new BagListener());
            AddListener(EventCode.BagUpdateEvent, new BagUpdateListener());
            AddListener(EventCode.DisplayRewardInfoEvent, new DisplayRewardInfoListener());

            GSocket.Connect(GSIP, GSPort, false, callback);
        }

        public void UpdateGSocket()
        {
            GSocket.OnSocketRead();
            TPacket.Handle_Commands(ref GSocket.Cmds);
        }

        /// <summary>
        /// Adds the listener.
        /// </summary>
        /// <param name="eventCode">The event code.</param>
        /// <param name="socketListener">The socket listener.</param>
        public void AddListener(EventCode eventCode, IGSocketListener socketListener)
        {
            if (mSocketDictionary.TryGetValue(eventCode, out var eventHandler))
            {
                eventHandler.AddListener(socketListener);
            }
            else
            {
                Console.WriteLine("於GSocket中並未定義該Event:" + eventCode);
            }
        }

        /// <summary>
        /// Removes the listener.
        /// </summary>
        /// <param name="eventCode">The event code.</param>
        /// <param name="socketListener">The socket listener.</param>
        public void RemoveListener(EventCode eventCode, IGSocketListener socketListener)
        {
            if (mSocketDictionary.TryGetValue(eventCode, out var eventHandler))
            {
                eventHandler.RemoveListener(socketListener);
            }
        }

        /// <summary>
        /// Creates the event.
        /// </summary>
        /// <param name="eventCode">The event code.</param>
        /// <returns></returns>
        public TNetMessageProc CreateEvent(EventCode eventCode)
        {
            // 新增該EventCode的Listener
            List<IGSocketListener> listenerList = new List<IGSocketListener>();
            EventHandler eventHandler = new EventHandler(eventCode, listenerList);
            mSocketDictionary.Add(eventCode, eventHandler);
            return eventHandler.HandleMessageProc;
        }

        /// <summary>
        /// Called when [event].
        /// </summary>
        /// <param name="eventCode">The event code.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">於GSocket中並未定義該Event:" + eventCode</exception>
        public TNetMessageProc OnEvent(EventCode eventCode)
        {
            if (mSocketDictionary.TryGetValue(eventCode, out var eventHandler))
            {
                return eventHandler.HandleMessageProc;
            }
            else
            {
                throw new Exception("於GSocket中並未定義該Event:" + eventCode);
            }
        }

        private struct EventHandler
        {
            private readonly EventCode _eventCode;
            private readonly List<IGSocketListener> _listenerList;

            public EventHandler(EventCode eventCode, List<IGSocketListener> listenerList)
            {
                _eventCode = eventCode;
                _listenerList = listenerList;
            }

            /// <summary>
            /// Adds the listener.
            /// </summary>
            /// <param name="socketListener">The socket listener.</param>
            public void AddListener(IGSocketListener socketListener)
            {
                _listenerList.Add(socketListener);
            }

            /// <summary>
            /// Removes the listener.
            /// </summary>
            /// <param name="socketListener">The socket listener.</param>
            public void RemoveListener(IGSocketListener socketListener)
            {
                bool isSuccess = _listenerList.Remove(socketListener);
                if (!isSuccess)
                {
                    Console.WriteLine("移除IGSocketListener失敗");
                }
            }

            /// <summary>
            /// Gets the event code.
            /// </summary>
            /// <returns></returns>
            public EventCode GetEventCode()
            {
                return _eventCode;
            }

            /// <summary>
            /// Handles the message proc.
            /// </summary>
            /// <param name="Msg">The MSG.</param>
            public void HandleMessageProc(ref TNetMsg Msg)
            {
                // 使用ToArray避免RemoveListener時產生Collection was modified;
                // enumeration operation may not execute的錯誤
                foreach (var listener in _listenerList.ToArray())
                {
                    listener.OnEvent(ref Msg);
                }
            }
        }
    }
}