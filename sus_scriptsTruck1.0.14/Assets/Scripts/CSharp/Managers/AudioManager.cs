using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class AudioManager : Singleton<AudioManager> {

    private GameObject m_audioTonesSource;

    protected override void Init()
    {
        base.Init();
        initAudioSource();
        initTonesSound();
        initCatAudioSource();
        initBGMTransition();
    }

    protected override void UnInit()
    {
        base.UnInit();

        UnityEngine.Object.Destroy(m_audioTonesSource);
    }

    public void PlayTones(AudioTones audioTones)
    {
        if (m_audioTonesDic.ContainsKey(audioTones))
        {
            m_audioTonesDic[audioTones].Stop();
            m_audioTonesDic[audioTones].Play();
        }
    }

    public void StopTones(AudioTones audioTones)
    {
        if (m_audioTonesDic.ContainsKey(audioTones))
        {
            m_audioTonesDic[audioTones].Stop();
        }
    }

    private Dictionary<AudioTones, AudioSource> m_audioTonesDic;
    private void initAudioSource()
    {
        m_audioTonesSource = new GameObject("AudioTonesSource");
        UnityEngine.Object.DontDestroyOnLoad(m_audioTonesSource);
    }

    private void initTonesSound()
    {
        m_audioTonesDic = new Dictionary<AudioTones, AudioSource>();
        m_audioTonesSource.transform.position = Vector3.zero;


        var AudioTonesPath = new Dictionary<string, string>();
        AudioTonesPath["book_click"] = "AudioTones/book_click.mp3";
        AudioTonesPath["dialog_click"] = "AudioTones/dialog_click.mp3";
        AudioTonesPath["dialog_choice_click"] = "AudioTones/dialog_choice_click.mp3";
        AudioTonesPath["diamond_click"] = "AudioTones/diamond_click.mp3";
        AudioTonesPath["RewardWin"] = "AudioTones/RewardWin.mp3";
        AudioTonesPath["LoseFail"] = "AudioTones/LoseFail.mp3";
        //AudioTonesPath["RouletteSpin"] = "AudioTones/RouletteSpin.mp3";

        Array array = Enum.GetValues(typeof(AudioTones));
        foreach (AudioTones item in array)
        {
            var clipName = AudioTonesPath[item.ToString()];
            if (clipName != null)
            {
                AudioSource audioSource = m_audioTonesSource.AddComponent<AudioSource>();
                audioSource.clip = ResourceManager.Instance.GetAudioTones(clipName);
                if (UserDataManager.Instance.UserData != null)
                    audioSource.volume = UserDataManager.Instance.UserData.TonesIsOn;
                else
                    audioSource.volume = 1;

                m_audioTonesDic.Add(item, audioSource);
            }
        }
    }


    private AudioSource mCatAudioSource;
    private void initCatAudioSource()
    {
        mCatAudioSource = m_audioTonesSource.AddComponent<AudioSource>();
        if (UserDataManager.Instance.UserData != null)
            mCatAudioSource.volume = UserDataManager.Instance.UserData.TonesIsOn;
        else
            mCatAudioSource.volume = 1;
    }

    /// <summary>
    /// 猫咪的音效
    /// </summary>
    /// <param name="clip"></param>
    public void PlayCatTones(AudioClip clip)
    {
        mCatAudioSource.clip = clip;
        mCatAudioSource.Play();
    }


    //这个是音效的音量控制
    public void ChangeTonesVolume(float volume)
    {
        foreach (var item in m_audioTonesDic)
        {
            item.Value.volume = volume;
        }

        if (mCatAudioSource != null)
            mCatAudioSource.volume = volume;
    }

   
    private float m_fBGMVolume;
    private AudioSource source_1;
    private AudioSource source_2;
    private bool pointToFist;
    private bool isBGMTransiting;
    private void initBGMTransition()
    {
        source_1 = m_audioTonesSource.AddComponent<AudioSource>();
        source_2 = m_audioTonesSource.AddComponent<AudioSource>();
        source_1.loop = true;
        source_2.loop = true;
        pointToFist = true;
        isBGMTransiting = false;
        m_fBGMVolume = 1;

        if(UserDataManager.Instance.UserData != null && UserDataManager.Instance.UserData.BgMusicIsOn == 0)
        {
            source_1.Stop();
            source_2.Stop();
        }
    }

    private AudioClip mLastClip;
    public void PlayBGM(AudioClip clip)
    {
        mLastClip = clip;
        if (UserDataManager.Instance.UserData != null && UserDataManager.Instance.UserData.BgMusicIsOn == 0) return;

        if (pointToFist)
        {
            source_2.clip = clip;
            transition(source_1, source_2);
        }
        else
        {
            source_1.clip = clip;
            transition(source_2, source_1);
        }
        pointToFist = !pointToFist;
    }

    public void CleanClip()
    {
        if (source_1 != null)
            source_1.clip = null;

        if (source_2 != null)
            source_2.clip = null;
    }
    //这个是停止播放背景音乐
    public void StopBGM()
    {
        source_1.DOKill();
        source_2.DOKill();
        source_1.DOFade(0,1f).SetEase(Ease.Flash).OnComplete(() => { source_1.Stop(); });
        source_2.DOFade(0, 1f).SetEase(Ease.Flash).OnComplete(() => { source_2.Stop(); });
    }

    //这个是停止播放背景音乐后启动背景音乐播放
    public void PlayBGMAgain()
    {
        source_1.DOKill();
        source_2.DOKill();
        if (pointToFist)
        {
            source_1.clip = mLastClip;
            source_1.DOFade(1, 1f).SetEase(Ease.Flash).OnComplete(() => { source_1.Play(); });
        }
        else
        {
            source_2.clip = mLastClip;
            source_2.DOFade(1, 1f).SetEase(Ease.Flash).OnComplete(() => { source_2.Play(); });
        }
    }

    public void StopBGMQuick()
    {
        source_1.Stop();
        source_2.Stop();
    }

    private void transition(AudioSource volumeToLow, AudioSource volumeToHeigh)
    {
        source_1.DOKill();
        source_2.DOKill();
        volumeToLow.volume = m_fBGMVolume;
        volumeToLow.DOFade(0, 2f).SetEase(Ease.Flash)
                .OnStart(() => isBGMTransiting = true)
                .OnComplete(() => {
                    volumeToLow.Stop();
                });
        volumeToHeigh.volume = 0;
        volumeToHeigh.DOFade(1, 2f).SetDelay(1f).SetEase(Ease.Flash)
            .OnStart(() =>
            {
                volumeToHeigh.Play();
            })
            .OnComplete(() => {
                isBGMTransiting = false;
            });
    }
}

public enum AudioTones
{
    book_click,             //点击书本
    dialog_click,           //点击对话的音效
    diamond_click,          //点击钻石的音效
    dialog_choice_click,    //选择选项的音效
    RewardWin,          //成功音效
    LoseFail,           //失败音效
    // RouletteSpin,       //转盘启动的音效
}