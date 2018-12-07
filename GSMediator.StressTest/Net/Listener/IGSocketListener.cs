namespace GSMediator.StressTest.Net
{
    public interface IGSocketListener
    {
        void OnEvent(ref TNetMsg Msg);
    }
}