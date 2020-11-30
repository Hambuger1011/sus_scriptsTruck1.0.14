using AB;
using pb;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TestAnimation : MonoBehaviour {
    [Header("图片")]
    public Image AnimalImage;
    [Header("宠物ID")]
    public int pet_id;
    [Header("动作ID")]
    public int act_id;
    [Header("帧数")]
    public int frameCount;
    public Button StartBtn;
    public Button StopBtn;
    [Header("时间数组")]
    public float[] time;
    [Header("最小频率")]
    public int minFrequency;
    [Header("最大频率")]
    public int maxFrequency;

    [Header("每次循环次数(")]
    public int minTimes;
    [Header("每次循环次数(")]
    public int maxTimes;

    Coroutine co = null;
    // Use this for initialization
    void Start () {
       
    }
	
    void Awake()
    {
        StartBtn.onClick.AddListener(OnBtnStartClick);
        StopBtn.onClick.AddListener(OnBtnStopClick);


    }


    void OnDestroy()
    {
        StartBtn.onClick.RemoveListener(OnBtnStartClick);
        StopBtn.onClick.RemoveListener(OnBtnStopClick);
    }
    private void OnBtnStartClick()
    {
        LOG.Error("开始了！！！！");
        co = StartCoroutine(AnimationPlayThread(AnimalImage, pet_id, act_id, frameCount));
    }

   
    private void OnBtnStopClick()
    {
        LOG.Error("停止了！！！！");
        StopCoroutine(co);
    }



    /// <summary>
    /// 播放序列帧
    /// </summary>
    /// <param name="img">目标图片</param>
    /// <param name="cid">宠物ID</param>
    /// <param name="aid">动作ID</param>
    /// <param name="gif">动作数据</param>
    /// <param name="count">帧数</param>
    /// <returns></returns>
    IEnumerator AnimationPlayThread(Image img, int cid, int aid, int count)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("Cat/");
        sb.Append(cid.ToString("D2"));
        sb.Append(aid.ToString("D2"));
        int i = 1;
        int tmploop = 0;
        
        int loop = GetRandom(minTimes, maxTimes);
       
        int frequence = GetRandom(minFrequency, maxFrequency);
        bool flag = true;
        while (flag)
        {
            if (i <= count)
            {

               
                LOG.Info(sb);
                img.sprite = Resources.Load<Sprite>(sb.ToString()+ i.ToString("D2"));
                img.SetNativeSize();


            }
            else
            {
                i = 1;
                img.sprite = Resources.Load<Sprite>(sb.ToString() + i.ToString("D2"));
                img.SetNativeSize();
                tmploop += 1;
                if (tmploop == loop)
                {
                    tmploop = 0;
                    yield return new WaitForSeconds(frequence);
                }
            }

            float tmpflo = Convert.ToSingle(time[i - 1]);
            i++;
            yield return new WaitForSeconds(tmpflo);
        }
    }
    /// <summary>
    /// 获取随机数
    /// </summary>
    /// <param name="min">最小值</param>
    /// <param name="max">最大值</param>
    /// <returns></returns>
    private int GetRandom(int min, int max)
    {

        return UnityEngine.Random.Range(min, max + 1);
    }
}
