namespace GSMediator.StressTest.Net
{
    public interface ISocketHandler
    {
        void AddListener(EventCode eventCode, IGSocketListener socketListener);

        void RemoveListener(EventCode eventCode, IGSocketListener socketListener);

        TNetMessageProc CreateEvent(EventCode eventCode);

        TNetMessageProc OnEvent(EventCode eventCode);
    }
}