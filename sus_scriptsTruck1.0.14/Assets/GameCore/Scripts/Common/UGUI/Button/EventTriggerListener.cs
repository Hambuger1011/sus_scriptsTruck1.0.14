using Framework;

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;


public class EventTriggerListener :
        MonoBehaviour,
        IPointerEnterHandler,
        IPointerExitHandler,
        IPointerDownHandler,
        IPointerUpHandler,
        IPointerClickHandler
{
    public delegate void VoidDelegate(GameObject go = null);

    public VoidDelegate onClick;

	public VoidDelegate onDown;

	public VoidDelegate onUp;


    public VoidDelegate onEnter;

	public VoidDelegate onExit;
    


    [Header("点击间隔")]
    public float Interval = 0.25f;

    [Header("长按时间阈值")]
    public float longPressThreshold = -1;

    bool m_isDown = false;
    bool m_isUp = false;
    bool m_isLongPress = false;
    float m_downTime;
    float m_lastClickTime;
    public object parameter;
    const int TYPE_NUM = 6;

    VoidDelegate _onLongPress;
    public VoidDelegate onLongPress
    {
        get
        {
            return _onLongPress;
        }
        set
        {
            if(longPressThreshold <= 0)
            {
                longPressThreshold = 1;
            }
            _onLongPress = value;
        }
    }

    public static EventTriggerListener Get(GameObject go)
	{
        EventTriggerListener eventTriggerListener = AddTrigger(go);
        return eventTriggerListener;
	}
    static EventTriggerListener AddTrigger(GameObject go)
    {
        EventTriggerListener listener = go.GetComponent<EventTriggerListener>();
        if (listener == null)
        {
            listener = go.AddComponent<EventTriggerListener>();
            listener.Init();
        }
        return listener;
    }

    void Awake()
    {
        Init();
    }

    void OnDestroy()
    {
        UnInit();
    }

    void Init()
    {
        if (eventActives != null)
        {
            return;
        }
        eventActives = new bool[TYPE_NUM];//Pathfinding.Util.ArrayPool<bool>.ClaimWithExactLength(TYPE_NUM);
        for(int i=0; i < TYPE_NUM; ++i)
        {
            eventActives[i] = true;
        }
        //eventAudioClips = Pathfinding.Util.ArrayPool<AudioClip>.ClaimWithExactLength(TYPE_NUM);
    }

    void UnInit()
    {
        //Pathfinding.Util.ArrayPool<bool>.Release(ref eventActives,true);
        //if(eventAudioClips != null)
        //{
        //    for(int i = 0; i < TYPE_NUM; ++i)
        //    {
        //        eventAudioClips[i] = null;
        //    }
        //    Pathfinding.Util.ArrayPool<AudioClip>.Release(ref eventAudioClips, true);
        //}
    }

    void ResetState()
    {
        m_isDown = false;
        m_isUp = false;
        m_isLongPress = false;
        //m_downTime = 0;
    }

    //优化级:donw->up->click->exit
	public void Clear()
	{
        ResetState();
        
		this.onUp = null;
		this.onDown = null;
		this.onClick = null;
		this.onEnter = null;
		this.onExit = null;
	}

    void FixedUpdate()
    {
        if(longPressThreshold > 0 && m_isDown && !m_isUp && !m_isLongPress)
        {
            if (Time.time - m_downTime >= longPressThreshold)
            {
                m_isLongPress = true;
                if (onLongPress != null)
                {
                    onLongPress(this.gameObject);
                }
            }
        }
    }

	public void OnPointerClick(PointerEventData eventData)
    {
        if (!m_isDown)
        {
            return;
        }
        if (m_isLongPress)
        {
            return;
        }
        if(!EnableTrigger(UIEventType.OnClick))
        {
            return;
        }
        if (Interval > 0 && Time.time - m_lastClickTime < Interval)
        {
            return;
        }
        m_lastClickTime = Time.time;
        if (this.onClick != null)
        {
            this.onClick.Invoke(gameObject);
        }
	}

	public void OnPointerDown(PointerEventData eventData)
    {
        if (!EnableTrigger(UIEventType.OnDown))
        {
            return;
        }
        ResetState();
        this.m_isDown = true;
        this.m_downTime = Time.time;
        if (this.onDown != null)
        {
            this.onDown.Invoke(gameObject);
        }
	}

	public void OnPointerUp(PointerEventData eventData)
    {
        if (!EnableTrigger(UIEventType.OnUp))
        {
            return;
        }
        if (!m_isDown)
        {
            return;
        }
        m_isUp = true;
        if (this.onUp != null)
        {
            this.onUp.Invoke(gameObject);
        }
        //ResetState();
    }

	public void OnPointerEnter(PointerEventData eventData)
    {
        if (!EnableTrigger(UIEventType.OnEnter))
        {
            return;
        }
        if (!m_isDown)
        {
            return;
        }
        if (this.onEnter != null)
        {
            this.onEnter.Invoke(gameObject);
        }
	}

	public void OnPointerExit(PointerEventData eventData)
    {
        if (!EnableTrigger(UIEventType.OnExit))
        {
            return;
        }
        if (this.onExit != null)
        {
            this.onExit.Invoke(gameObject);
        }
	}


    
    /// <summary>
    /// 触发绑定事件
    /// </summary>
    /// <param name="obj">事件绑定对象</param>
    /// <param name="ev">事件类型</param>
    public static void TriggerEvent(GameObject obj, UIEventType ev)
    {
        if (obj == null)
        {
            LOG.Error("触发事件不能传入空对象!");
            return;
        }
        EventTriggerListener listener = Get(obj);
        VoidDelegate func = null;
        switch (ev)
        {
            case UIEventType.OnDown:
                func = listener.onDown;
                break;
            case UIEventType.OnUp:
                func = listener.onUp;
                break;
            case UIEventType.OnClick:
                func = listener.onClick;
                break;
            case UIEventType.OnEnter:
                func = listener.onEnter;
                break;
            case UIEventType.OnExit:
                func = listener.onExit;
                break;
            case UIEventType.OnLongPress:
                func = listener.onLongPress;
                break;
            //case UIEventType.OnSelect:
            //    //func = listener.onSelect;
            //    break;
            //case UIEventType.OnUpdateSelect:
            //    //func = listener.onUpdateSelect;
            //    break;
        }
        if (func != null)
            func(obj);
    }

    private bool[] eventActives = null;
    private AudioClip[] eventAudioClips = null;
    /// <summary>
    /// 设置事件是否激活
    /// </summary>
    public void SetEventEnable(UIEventType type, bool enable)
    {
        int idx = (int)type;
        eventActives[idx] = enable;
    }

    public bool EnableTrigger(UIEventType type)
    {
        int idx = (int)type;
        return eventActives[idx];
    }
    
    public void SetAudio(UIEventType type, AudioClip clip)
    {
        return;
        int idx = (int)type;
        eventAudioClips[idx] = clip;
    }

    public void PlayAudio(UIEventType type)
    {
        return;
        //int idx = (int)type;
        //var clip = eventAudioClips[idx];
        //if(clip != null)
        //{
        //    Main.audioMgr.PlayAudio(clip);
        //}
    }
}
public enum UIEventType : uint
{
    OnClick = 0,
    OnDown = 1,
    OnUp = 2,
    OnLongPress = 3,
    OnEnter = 4,
    OnExit = 5,
    //OnSelect = 6,
    //OnUpdateSelect = 7
}
