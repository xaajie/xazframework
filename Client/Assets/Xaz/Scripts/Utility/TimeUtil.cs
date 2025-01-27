using System;
using System.Data;
using UnityEngine;
namespace Xaz
{
    /// <summary>
    /// c#侧时间工具函数
    /// </summary>
    public class TimeUtil
    {
        public const int TIMESCALE = 10000000;
        public const int TIMESCALE2 = 10000;
        static public readonly DateTime TIME1970 = new DateTime(1970, 1, 1);
        static public TimeSpan timeOffest = new TimeSpan(0);
        static public TimeSpan dayOffest = new TimeSpan(3 - 8, 0, 0);

        public const int DaySpan = 86400;
        static private int startupTime;

        public const int DayTimeCount = 86400;
        public const int HourTimeCount = 3600;
        public const int MinuteTimeCount = 60;
        static public void SetServiceTime(int time)
        {
            startupTime = time;
            timeOffest = IntToDataTime(time) - System.DateTime.Now;
        }

        static public bool CheckCrossDay()
        {
            return IsSameDay(startupTime);
        }

        static public bool CheckCrossWeek()
        {
            return IsSameWeek(startupTime);
        }

        static public int GetDuration()
        {
            return (GetNowInt() - startupTime);
        }

        static public DateTime GetNow()
        {
            return DateTime.Now + timeOffest;
        }

        static public int GetNowInt()
        {
            return DataTimeToInt(GetNow());
        }

        static public long GetNowTicks()
        {
            return (long)((double)(GetNow().ToUniversalTime().Ticks - TIME1970.Ticks) / TIMESCALE2);
        }

        static public int DataTimeToInts(DateTime date)
        {
            return (int)((double)(date.ToUniversalTime().Ticks - TIME1970.Ticks) / TIMESCALE);
        }

        static public int TimeSpanToInt(TimeSpan time)
        {
            return (int)((double)(time.Ticks) / TIMESCALE);
        }

        static public DateTime IntToDataTime(int t)
        {
            return new DateTime(TIME1970.Ticks + (long)((double)t * TIMESCALE)).ToLocalTime();
        }

        static public TimeSpan IntToTimeSpan(int t)
        {
            return new TimeSpan((long)t * TIMESCALE);
        }

        static public DateTime GetNextDay()
        {
            return ((GetNow().ToUniversalTime() - dayOffest).Date + new TimeSpan(1, 0, 0, 0) + dayOffest).ToLocalTime();
        }

        static public int GetNextDayInt()
        {
            return DataTimeToInt(GetNextDay());
        }

        static public bool IsSameHour(DateTime v1, DateTime v2)
        {
            DateTime d1 = v1.ToUniversalTime() - dayOffest;
            DateTime d2 = v2.ToUniversalTime() - dayOffest;
            return d1.Date.Equals(d2.Date) && d1.Hour == d2.Hour;
        }

        static public bool IsSameHour(int v1, int v2)
        {
            return IsSameHour(IntToDataTime(v1), IntToDataTime(v2));
        }

        static public bool IsSameHour(int v1)
        {
            return IsSameHour(IntToDataTime(v1));
        }

        static public bool IsSameHour(DateTime v1)
        {
            return IsSameHour(v1, GetNow());
        }

        static public bool IsSameDay(DateTime v1, DateTime v2)
        {
            DateTime d1 = v1.ToUniversalTime() - dayOffest;
            DateTime d2 = v2.ToUniversalTime() - dayOffest;
            return d1.Date.Equals(d2.Date);
        }

        static public bool IsSameDay(int v1, int v2)
        {
            return IsSameDay(IntToDataTime(v1), IntToDataTime(v2));
        }

        static public bool IsSameDay(int v1)
        {
            return IsSameDay(IntToDataTime(v1));
        }

        static public bool IsSameDay(DateTime v1)
        {
            return IsSameDay(v1, GetNow());
        }

        static public bool IsSameWeek(int v)
        {
            int dbl = (int)(IntToDataTime(v).ToUniversalTime() - dayOffest).DayOfWeek;
            int intDow = (int)(GetNow().ToUniversalTime() - dayOffest).DayOfWeek;
            if (intDow == 0)
                intDow = 7;

            return dbl >= 7 || dbl >= intDow;
        }

        static private string formatStr(int s)
        {
            return s.ToString().PadLeft(2, '0');
        }

        public static DayOfWeek whichday()
        {
            return DateTime.Now.DayOfWeek;
        }

        public static int DayToInt(DayOfWeek day)
        {
            if (day == DayOfWeek.Friday)
                return 5;
            else if (day == DayOfWeek.Monday)
                return 1;
            else if (day == DayOfWeek.Saturday)
                return 6;
            else if (day == DayOfWeek.Sunday)
                return 7;
            else if (day == DayOfWeek.Tuesday)
                return 2;
            else if (day == DayOfWeek.Wednesday)
                return 3;
            else if (day == DayOfWeek.Thursday)
                return 4;
            return 0;

        }

        public static void SetTimeScale(float timeScale)
        {
            Time.timeScale = timeScale;
            Time.fixedDeltaTime = 1f / Application.targetFrameRate * timeScale;
            //	Time.maximumDeltaTime = Time.fixedDeltaTime * 10f;
        }
        public static string GetTimeStr(string time)
        {
            return GetTimeFormatStr(int.Parse(time), "yyyy-M-dd H:mm");
        }

        public static string GetTimeFormatStr(int time, string formatStr)
        {
            DateTime curTime = IntToDataTime(time);
            return curTime.ToString(formatStr);
        }

        //获取倒计时
        public static string GetTimeCountStr(int hours, int minutes, int seconds, string formatStr)
        {
            TimeSpan ts = new TimeSpan(hours, minutes, seconds);
            return ts.ToString();
        }

        public static string FormatTime(int seconds)
        {
            // 计算小时、分钟和秒
            int hours = seconds / 3600;
            seconds %= 3600;
            int minutes = seconds / 60;
            seconds %= 60;

            // 格式化成HH:mm:ss
            return string.Format("{0:D2}:{1:D2}:{2:D2}", hours, minutes, seconds);
        }
        public static string ConvertToLargestUnit(int totalSeconds)
        {
            // Calculate the time in various units
            int seconds = totalSeconds % 60;
            int minutes = (totalSeconds / 60) % 60;
            int hours = (totalSeconds / 3600) % 24;
            int days = totalSeconds / 86400;

            if (days > 0)
            {
                return $"{days}天";
            }
            else if (hours > 0)
            {
                return $"{hours}小时";
            }
            else if (minutes > 0)
            {
                return $"{minutes}分钟";
            }
            else
            {
                return $"{seconds}秒";
            }
        }
    }
}
