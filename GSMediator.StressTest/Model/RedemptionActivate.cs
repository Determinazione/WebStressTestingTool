namespace GSMediator.StressTest.Model
{
    /// <summary>
    /// 激活禮包碼時，需紀錄的玩家相關資訊
    /// </summary>
    public class RedemptionActivate
    {
        public string UID { get; set; }
        public string ServerID { get; set; }
        public int PlatformID { get; set; }
        public string PackageName { get; set; }
        public string RedemptionCode { get; set; }
        public int SocketWho { get; set; }
    }
}
