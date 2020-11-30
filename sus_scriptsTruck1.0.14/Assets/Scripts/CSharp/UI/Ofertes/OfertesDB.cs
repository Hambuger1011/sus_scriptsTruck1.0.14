using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OfertesDB
{
    private List<ADSInfo> aDSInfo;
    private int hadReadADSNumber;    //记录已经领取到第几个广告

    public List<ADSInfo> GetADSInfoList()
    {
        if (aDSInfo==null)
        {
            aDSInfo = new List<ADSInfo>();
        }
        return aDSInfo;
    }

    public int HadReadADSNumber { get; set; }
    
}
