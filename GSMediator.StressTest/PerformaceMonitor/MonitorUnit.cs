using System;
using System.Diagnostics;

namespace GSMediator.StressTest.PerformaceMonitor
{
    /// <summary>
    /// 紀錄單一Request的執行狀態
    /// </summary>
    public struct MonitorUnitData
    {
        public string Tag { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public TimeSpan ProcessTimeSpan { get; set; }
        public bool isRun { get; set; }
        public bool isSuccess { get; set; }
        public string ProcessResult { get; set; }
    }

    public class MonitorUnit
    {
        private readonly IMonitorManager _monitorManager;
        private Stopwatch _stopWatch;
        private MonitorUnitData _monitorUnitData;

        public MonitorUnit(IMonitorManager monitorManager)
        {
            _stopWatch = new Stopwatch();
            _monitorManager = monitorManager;
        }

        /// <summary>
        /// 設置Unit的起跑點 (相當於設置賽跑選手在起跑線上，但還沒啟動)
        /// </summary>
        public void SetStartPoint()
        {
            // 初始化紀錄資料
            _monitorUnitData = new MonitorUnitData();
            _monitorUnitData.isRun = false;
            _monitorManager.SaveEmptyUnit(_monitorUnitData);
        }

        /// <summary>
        /// 開始監測運行時間
        /// </summary>
        public void StartMonitor()
        {
            // 啟動碼錶
            _stopWatch.Reset();
            _stopWatch = Stopwatch.StartNew();
            // 初始化啟動時間
            _monitorUnitData.isRun = true;
            _monitorUnitData.StartTime = DateTime.Now;
        }

        /// <summary>
        /// 抵達終點，儲存運行速度與結果
        /// </summary>
        public void StopAndSaveToMonitorManager(bool isSuccess, string processResult)
        {
            _stopWatch.Stop();
            _monitorUnitData.EndTime = DateTime.Now;

            TimeSpan el = _stopWatch.Elapsed;
            _monitorUnitData.ProcessTimeSpan = el;
            _monitorUnitData.isSuccess = isSuccess;
            _monitorUnitData.ProcessResult = processResult;
            // 將單筆Request結果存入MonitorManager
            _monitorManager.SaveEndUnit(_monitorUnitData);
            // 請求確認目前存入任務數量，是否相等於本次高併發的需求次數
            if (_monitorManager.IsUnitsDataAmountEqualTo(Program.MultiTaskTimes))
            {
                // 若已完成，輸出報告
                _monitorManager.OutputReport();
            }
        }

        /// <summary>
        /// Gets the tag.
        /// </summary>
        /// <returns></returns>
        public string GetTag()
        {
            return _monitorUnitData.Tag;
        }

        /// <summary>
        /// Sets the tag.
        /// </summary>
        /// <param name="tag">The tag.</param>
        public void SetTag(string tag)
        {
            _monitorUnitData.Tag = tag;
        }
    }
}