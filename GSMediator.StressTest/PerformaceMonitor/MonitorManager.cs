using System;

using System.Collections.Generic;
using System.Linq;

namespace GSMediator.StressTest.PerformaceMonitor
{
    public class MonitorManager : IMonitorManager
    {
        private static readonly List<MonitorUnitData> _emptyMonitorUnits = new List<MonitorUnitData>();
        private static readonly List<MonitorUnitData> _monitorUnits = new List<MonitorUnitData>();

        /// <summary>
        /// 儲存空的Unit (用作輸出報告時，計算有初始化Unit，卻沒有等到反應結果的滯留Unit)
        /// </summary>
        /// <param name="unit">The unit.</param>
        public void SaveEmptyUnit(MonitorUnitData unit)
        {
            lock (_emptyMonitorUnits)
            {
                _emptyMonitorUnits.Add(unit);
            }
        }

        /// <summary>
        /// 儲存已具備結果的Unit
        /// </summary>
        /// <param name="unit">The unit.</param>
        public void SaveEndUnit(MonitorUnitData unit)
        {
            lock (_monitorUnits)
            {
                _monitorUnits.Add(unit);
            }
        }

        /// <summary>
        /// Determines whether [is units data amount equal to] [the specified amount].
        /// </summary>
        /// <param name="amount">The amount.</param>
        /// <returns>
        ///   <c>true</c> if [is units data amount equal to] [the specified amount]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsUnitsDataAmountEqualTo(int amount)
        {
            return _monitorUnits.Count == amount;
        }

        /// <summary>
        /// Outputs the report.
        /// </summary>
        public void OutputReport()
        {
            Console.WriteLine("===壓力測試報告===");
            var maxCount = _emptyMonitorUnits.Count;
            Console.WriteLine($"高併發記錄次數: {maxCount}");

            Console.WriteLine("===時間===");
            TimeSpan allTimeSpan = new TimeSpan(0);

            var index = 1;
            foreach(var unit in _monitorUnits)
            {
                var outputRatio = 20;
                var onePageDataCount = (int)(maxCount * (outputRatio / 100f));
                float completedPercentage = (index / (float)maxCount) * 100;
                // 輸出每隔20%完成紀錄時的時間
                if(completedPercentage % outputRatio == 0)
                {
                    Console.WriteLine($"===已完成{completedPercentage}%時的時間===");
                    var previousDataCount = index - onePageDataCount;
                    TimeSpan maxTimeSpan = _monitorUnits.Skip(previousDataCount).Take(onePageDataCount).Max(x => x.ProcessTimeSpan);
                    TimeSpan minTimeSpan = _monitorUnits.Skip(previousDataCount).Take(onePageDataCount).Min(x => x.ProcessTimeSpan);
                    TimeSpan avgTimeSpan = allTimeSpan.Divide(index);
                    Console.WriteLine($"平均單筆執行時間: {avgTimeSpan}");
                    Console.WriteLine($"單筆最小執行時間: {minTimeSpan}");
                    Console.WriteLine($"單筆最大執行時間: {maxTimeSpan}");
                    Console.WriteLine();
                }
                // 儲存總執行時間
                allTimeSpan = allTimeSpan.Add(unit.ProcessTimeSpan);
                index++;
            }


            Console.WriteLine($"總執行時間: {allTimeSpan}");

            Console.WriteLine("===結果===");
            var successTimes = _monitorUnits.Count(x => x.isSuccess);
            var failedTimes = _monitorUnits.Count(x => x.isSuccess == false);
            var noResponseTimes = _emptyMonitorUnits.Count - _monitorUnits.Count;
            Console.WriteLine($"執行成功次數: {successTimes}");
            Console.WriteLine($"執行失敗次數: {failedTimes}");
            Console.WriteLine($"滯留Unit個數: {noResponseTimes}");

            Console.WriteLine("===執行失敗原因列表===");
            index = 0;
            foreach(var unit in _monitorUnits)
            {
                index++;
                if(unit.isSuccess == false)
                {
                    Console.WriteLine($"執行第{index}筆發生失敗，原因為: {unit.ProcessResult}");
                }
            }
        }
    }
}
