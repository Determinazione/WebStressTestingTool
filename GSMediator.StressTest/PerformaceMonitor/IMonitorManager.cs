namespace GSMediator.StressTest.PerformaceMonitor
{
    public interface IMonitorManager
    {
        void OutputReport();
        void SaveEmptyUnit(MonitorUnitData unit);
        void SaveEndUnit(MonitorUnitData unit);
        bool IsUnitsDataAmountEqualTo(int amount);
    }
}