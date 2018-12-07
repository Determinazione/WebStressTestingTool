using System;

namespace GSMediator.StressTest.Net.Listener
{
    public static class DelphiTool
    {
        public static DateTime ConvertToCSharpDateTime(double delphiDateTime)
        {
            DateTime delphiInitTime = new DateTime(1899, 12, 30, 0, 0, 0);
            DateTime cSharpInitTime = new DateTime();
            double period = new TimeSpan(delphiInitTime.Ticks - cSharpInitTime.Ticks).Days;

            //24 * 60 *60 *1000 * 1000  *10
            const double TIME_UNIT = 864000000000;

            //C# DateTime 為 delphi中所存的日期 + C#與Delphi日期格式初始值的差異
            DateTime date = new DateTime((long)((delphiDateTime + period) * TIME_UNIT));
            return date;
        }
    }
}