using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class UIBookTween : MonoBehaviour, IPointerUpHandler
{
    public Image uiImg;
    [SerializeField]
    public Sprite[] frames;
    public float tweenInterval = 0.25f;
    public float tweenCD = 0f;
    public bool isLoop = true;
    public bool playOnAwake = false;

    public bool isPlaying { get; private set; }

    float m_nextTweenTime = 0;
    int m_weenIdx = 0;

    private void Awake()
    {
        if(playOnAwake)
        {
            Play();
        }
    }

    public void Play()
    {
        if (isPlaying)
        {
            return;
        }
        isPlaying = true;
        m_weenIdx = 0;
    }

    void Update()
    {
        if(!isPlaying)
        {
            return;
        }
        if (m_nextTweenTime < Time.time)
        {
            m_nextTweenTime = Time.time + tweenInterval;
            PlayTween();
        }
    }

    void PlayTween()
    {
        if (m_weenIdx >= frames.Length)
        {
            if(isLoop)
            {
                m_nextTweenTime = Time.time + tweenCD;
                m_weenIdx = 0;
                return;
            }
            else
            {
                isPlaying = false;
                return;
            }
        }
        uiImg.sprite = frames[m_weenIdx];
        ++m_weenIdx;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Play();

#if PAY_TEST
        //GameHttpNet.Instance.ClearUserRecord(ClearRecordCallBack);
#endif
    }

    private void ClearRecordCallBack(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----GetOrderToSubmitCallBack---->" + result);
        JsonObject jo = JsonHelper.JsonToJObject(result);
        if (jo != null)
        {
            if(jo.code == 200)
            {
                UserDataManager.Instance.clearRecordResultInfo = JsonHelper.JsonToObject<HttpInfoReturn<ClearRecordResultInfo>>(result);
                if (UserDataManager.Instance.clearRecordResultInfo != null && UserDataManager.Instance.clearRecordResultInfo.data != null)
                {
                    LoomUtil.QueueOnMainThread((param) =>
                    {
                        UserDataManager.Instance.ResetMoney(1, UserDataManager.Instance.clearRecordResultInfo.data.bkey);
                        UserDataManager.Instance.ResetMoney(2, UserDataManager.Instance.clearRecordResultInfo.data.diamond);
                    }, null);
                }
            }
        }
    }
}
