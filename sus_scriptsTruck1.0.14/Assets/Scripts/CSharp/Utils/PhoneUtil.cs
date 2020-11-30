using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhoneUtil
{
    private static PhoneUtil _Instance = null;

    public static PhoneUtil Instance
    {
        get
        {
            if (_Instance == null)
            {
                _Instance = new PhoneUtil();
            }
            return _Instance;
        }
    }


    public enum EnumPhoneLevel
    { 
        High,
        Medium,
        Low,
    }

    /// <summary>
    /// 获取手机等级
    /// </summary>
    ///1、主要判断： 现有标准如下：
    ///高端机：内存>=6G && cpu主频>= 2.36g && >=8核
    ///低端机：内存<=4G || cpu主频<= 1.8g
    ///中端机：其它机型

    public EnumPhoneLevel GetPhoneLevel()
    {
        EnumPhoneLevel level = EnumPhoneLevel.Low;

        //内存 SystemInfo.systemMemorySize
        //内存大小，单位M                               **内存例如是8G ，真实内存数是不足8*1024的  8G是理论数值，所以1G，按950算
        int memorysize = SystemInfo.systemMemorySize;
        Debug.LogError("内存大小  内存大小  内存大小:" + memorysize);


        //1 SystemInfo.processorCount
        //CPU处理核数
        int cpu_count = SystemInfo.processorCount;
        Debug.LogError("CPU处理核数  CPU处理核数  CPU处理核数:" + cpu_count);


        //1 SystemInfo.processorFrequency
        //处理器的频率
        int cpu_level = SystemInfo.processorFrequency;
        Debug.LogError("处理器的频率  处理器的频率  处理器的频率:" + cpu_level);



        if (memorysize > 8 * 950 && cpu_level >= 2.36 * 1024 && cpu_count >= 8)
        {
            //高端机
            level = EnumPhoneLevel.High;


        }
        else if (memorysize <= 4 * 1024 && cpu_level <= 1.8 * 1024)
        {
            //低端机
            level = EnumPhoneLevel.Low;
        }
        else
        {
            //中端机
            level = EnumPhoneLevel.Medium;
        }


#if !UNITY_EDITOR && UNITY_IOS
        string modelStr = SystemInfo.deviceModel;

        //高端机
        //[iPhone 12 Pro Max]   iPhone13,4
        //[iPhone 12 Pro]       iPhone13,3
        //[iPhone 12]           iPhone13,2
        //[iPhone 12 mini]      iPhone13,1
        //[iPhone SE 2]         iPhone12,8
        //[iPhone 11 Pro Max]   iPhone12,5
        //[iPhone 11 Pro]       iPhone12,3
        //[iPhone 11]           iPhone12,1
        //[iPhone XR]           iPhone11,8
        //[iPhone XS Max]       iPhone11,6
        //[iPhone XS Max]       iPhone11,4
        //[iPhone XS]           iPhone11,2
        //[iPhone X]            iPhone10,6
        //[iPhone X]            iPhone10,3

        bool isHigh = modelStr.Equals("iPhone13,4") || modelStr.Equals("iPhone13,3") || modelStr.Equals("iPhone13,2") 
            || modelStr.Equals("iPhone13,1") || modelStr.Equals("iPhone12,8") || modelStr.Equals("iPhone12,5")
            || modelStr.Equals("iPhone12,3") || modelStr.Equals("iPhone12,1") || modelStr.Equals("iPhone11,8")
            || modelStr.Equals("iPhone11,6") || modelStr.Equals("iPhone11,4") || modelStr.Equals("iPhone11,2")
            || modelStr.Equals("iPhone10,6") || modelStr.Equals("iPhone10,3");


        //中端机
        //[iPhone 8 Plus]       iPhone10,5
        //[iPhone 8]            iPhone10,4
        //[iPhone 8 Plus]       iPhone10,2
        //[iPhone 8]            iPhone10,1
        //[iPhone 7 Plus]       iPhone9,4
        //[iPhone 7]            iPhone9,3
        //[iPhone 7 Plus]       iPhone9,2
        //[iPhone 7]            iPhone9,1
        //[iPhone SE]           iPhone8,4
        //[iPhone 6s Plus]      iPhone8,2
        //[iPhone 6s]           iPhone8,1
        //// [iPhone 6]            iPhone7,2
        //// [iPhone 6 Plus]       iPhone7,1

        bool isMedium = modelStr.Equals("iPhone10,5") || modelStr.Equals("iPhone10,4") || modelStr.Equals("iPhone10,2")
            || modelStr.Equals("iPhone10,1") || modelStr.Equals("iPhone9,4") || modelStr.Equals("iPhone9,3")
            || modelStr.Equals("iPhone9,2") || modelStr.Equals("iPhone9,1") || modelStr.Equals("iPhone8,4")
            || modelStr.Equals("iPhone8,2") || modelStr.Equals("iPhone8,1");




        //如果是高端机
        if (isHigh)
        {
            //高端机
            level = EnumPhoneLevel.High;
        }
        else if (isMedium)
        {
            //中端机
            level = EnumPhoneLevel.Medium;
        }
        else
        {
             //低端机
            level = EnumPhoneLevel.Low;
        }
#endif

        return level;
    }


}
