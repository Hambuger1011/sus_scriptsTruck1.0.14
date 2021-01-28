using System;
using System.Globalization;
using System.Text.RegularExpressions;
using UnityEngine;

public sealed  class DateUtil
{
   public const string  yyyyMMdd = "yyyy-MM-dd";
   public const string  HHmmss = "HH:mm:ss";
    public const string HHmm = "HH:mm";
    public const string  yyyyMMdd_HHmmss = "yyyy/MM/dd HH:mm:ss";
    public const string yyyyMMdd_HHmm = "yyyy/MM/dd HH:mm";
    public const string yyyyMMddHHmmss = "yyyyMMddHHmmss";
    public const string yyyyMMdd__HHmmss = "yyyy-MM-dd HH:mm:ss";

    private const string TIME_PATTERN_ONE = "[0-9]{3}[1-9]|[0-9]{2}[1-9][0-9]{1}|[0-9]{1}[1-9][0-9]{2}|[1-9][0-9]{3})-(((0[13578]|1[02])-(0[1-9]|[12][0-9]|3[01]))|((0[469]|11)-(0[1-9]|[12][0-9]|30))|(02-(0[1-9]|[1][0-9]|2[0-8])))\\s([0-1][0-9]|2[0-3]):([0-5][0-9]):([0-5][0-9]";

    /// <summary>
    // 得到当前的日期，格式为：2009-07-16
    /// </summary>
    /// <returns></returns>
    public static string getCurrentDate()
   {
       DateTime now = DateTime.Now;
       return now.ToString(yyyyMMdd);
   }

  /// <summary>
   // 得到当前的日期和时间，格式为：2009-07-16 00:42:53
  /// </summary>
  /// <returns></returns>
   public static string getCurrentDateTime()
   {
       DateTime now = DateTime.Now;
       return now.ToString(yyyyMMdd_HHmmss);
	}

    /// <summary>
    // 得到当前的日期和时间，格式为：08:53
    /// </summary>
    /// <returns></returns>
    public static string getCurrentDateTime2()
    {
        DateTime now = DateTime.Now;
        return now.ToString(HHmm);
    }

    public static string getCurrentDateTime5()
    {
        DateTime now = DateTime.Now;
        return now.ToString(yyyyMMddHHmmss);
    }
    /// <summary>
    /// 得到当前月份
    /// </summary>
    /// <returns></returns>
    public static int getCurrentMouth()
    {
        DateTime now = DateTime.Now;
        return now.Month;
    }
    /// <summary>
    /// 得到当前月份包含的天数
    /// </summary>
    /// <returns></returns>
    public static int getCurrentMouthDays()
    {
        DateTime now = DateTime.Now;
        return DateTime.DaysInMonth(now.Year,now.Month);
    }
    /// <summary>
    /// 得到当前周几
    /// </summary>
    /// <returns></returns>
    public static int getCurrentWeek()
    {
        DateTime now = DateTime.Now;
        return (int)now.DayOfWeek;
    }


    public static DateTime GetCurTime()
    {
        //展示时间
        DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
        DateTime curDateT = startTime.AddSeconds(GameDataMgr.Instance.GetCurrentUTCTime());
        return curDateT;
    }

    /// <summary>
    /// 计算当前时间和指定时间戳的秒差
    /// </summary>
    /// <param name="timeStamp">指定时间戳</param>
    /// <returns>差值总秒</returns>
    public static long StampToDateTime(string timeStamp)
    {
        DateTime nowT = DateTime.Now;
        DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
        long mTime = long.Parse(timeStamp + "0000000");
        TimeSpan toNow = new TimeSpan(mTime);
        //startTime.Add(toNow).ToString("yyyy/MM/dd HH:mm:ss");//把传入的时间戳转换为制定时间格式
        return (long)(nowT - startTime.Add(toNow)).TotalSeconds;//计算传入时间和现在时间差   秒

    }

    /// <summary>
    /// 计算指定时间戳1   和指   定时间戳2的秒差           指定时间戳1 - 指定时间戳2
    /// </summary>
    /// <returns>差值总秒</returns>
    public static long StampToDateTime2(DateTime timeStamp1, DateTime timeStamp2)
    {
        //startTime.Add(toNow).ToString("yyyy/MM/dd HH:mm:ss");//把传入的时间戳转换为制定时间格式
        return (long)(timeStamp1 - timeStamp2).TotalSeconds;//计算传入时间和现在时间差   秒
    }


    /// <summary>
    /// 计算当前时间和指定时间戳的差值 小于一天显示时间戳的时：分； 大于一天的显示N天前
    /// </summary>
    /// <param name="timeStamp">指定时间戳</param>
    /// <returns>小于一天显示时间戳的时：分； 大于一天的显示N天前</returns>
    public static string StampToDateTime1(string timeStamp)
    {

        long unixTimeStamp = long.Parse(timeStamp);
        System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区
        DateTime dt = startTime.AddSeconds(unixTimeStamp);
        TimeSpan ts = DateTime.Now - dt;
        //判断是否是同一天
        if (ts.Days == 0)
        {
            return dt.ToString("HH:mm");
        }
        else
        {
            return ts.Days + "天前";
        }
    }



    /// <summary>

    /// 获取当前时间戳
    /// </summary>
    /// <param name="bflag">为真时获取10位时间戳,为假时获取13位时间戳.</param>
    /// <returns></returns>
    public static long GetTimeStamp(bool bflag = true)
    {
        TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
        long ret;
        if (bflag)
            ret = Convert.ToInt64(ts.TotalSeconds);
        else
            ret = Convert.ToInt64(ts.TotalMilliseconds);
        return ret;
    }


    /// <summary>
    /// 时间戳转成字符串
    /// </summary>
    static DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
    public static string NormalizeTimpstamp0(long timpStamp)
    {
        long unixTime = timpStamp * 10000000L;
        TimeSpan toNow = new TimeSpan(unixTime);
        DateTime dt = dtStart.Add(toNow);
        return dt.ToString(yyyyMMdd__HHmmss);
    }

    public static DateTime TimpstampToDateTime(long timpStamp)
    {
        long unixTime = timpStamp * 10000000L;
        TimeSpan toNow = new TimeSpan(unixTime);
        DateTime dt = dtStart.Add(toNow);
        return dt;
    }

    /// <summary>
    /// 将字符串时间格式     转成 DateTime    例如：字符串  "2019-09-04 07:35" 转成 Detetime格式
    /// </summary>
    /// <param name="dateString"></param>
    /// <returns></returns>
    public static DateTime ConvertToDateTime(string dateString)
    {
        DateTime dt = new DateTime();
        DateTime.TryParse(dateString, out dt);
       
        return dt;
    }


    /// <summary>
    /// GMT-5  时间 转换成 当前系统时区时间
    /// 将字符串时间格式     转成 DateTime    例如：字符串  "2019-09-04 07:35" 转成 Detetime格式
    /// </summary>
    /// <returns></returns>
    public static DateTime GMT5ToLocalTime(string dateString)
    {
        DateTime dt = new DateTime();
        DateTime.TryParse(dateString, out dt);

        DateTime utcTime1 = TimeZoneInfo.ConvertTimeToUtc(dt, TimeZoneInfo.Local);

        // long timpStamp = GetTimeStampFromDate(utcTime1);
        // //时间戳转成 DateTime
        // dt = DateUtil.TimpstampToDateTime(timpStamp);
        return utcTime1;
    }
    public void InitFromString(string formatDateTime)
    {
        string str = formatDateTime;
    }

    /// <summary>
    /// 时钟式倒计时
    /// </summary>
    /// <param name="second"></param>
    /// <returns></returns>
    public string GetSecondString(int second)
    {
        return string.Format("{0:D2}", second / 3600) + string.Format("{0:D2}", second % 3600 / 60) + ":" + string.Format("{0:D2}", second % 60);
    }


    /// 将Unix时间戳转换为DateTime类型时间
    /// </summary>
    /// <param name="d">double 型数字</param>
    /// <returns>DateTime</returns>
    public static DateTime ConvertIntDateTime(long d)
    {
        DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
        DateTime time = startTime.AddSeconds(d);
        return time;
    }


    /// <summary>
    /// 将c# DateTime时间格式转换为Unix时间戳格式
    /// </summary>
    /// <param name="time">时间</param>
    /// <returns>double</returns>
    public static double ConvertDateTimeInt(System.DateTime time)
    {
        double intResult = 0;
        System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
        intResult = (time - startTime).TotalSeconds;
        return intResult;
    }

    /// <summary>
    /// 日期转换成unix时间戳  本地时区
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    public static long GetTimeStampFromDate(DateTime dateTime)
    {
        DateTime localTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
        return (long)(dateTime - localTime).TotalSeconds;
    }

    /// <summary>
    /// 日期转换成unix时间戳
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    public static long DateTimeToUnixTimestamp(DateTime dateTime)
    {
        var start = new DateTime(1970, 1, 1, 0, 0, 0, dateTime.Kind);
        return Convert.ToInt64((dateTime - start).TotalSeconds);
    }


    /// <summary>
    /// unix时间戳转换成日期
    /// </summary>
    /// <param name="unixTimeStamp">时间戳（秒）</param>
    /// <returns></returns>
    public static DateTime UnixTimestampToDateTime(DateTime target, long timestamp)
    {
        DateTime start = new DateTime(1970, 1, 1, 0, 0, 0, target.Kind);
        return start.AddSeconds(timestamp);
    }



    /// <summary>根据日期，获得星期几</summary>
    public static int GetWeekDay(long timestamp)
    {
        DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
        DateTime time = startTime.AddSeconds(timestamp);
        return (int) time.GetDayOfWeek();
    }

}

