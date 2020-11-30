using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework;
using UGUI;

public abstract class UIFrame : CUIComponent
{
    #region 控件
    UIConfigLegacy _uiConfig;
    public UIConfigLegacy uiConfig
    {
        get
        {
            if (_uiConfig == null)
            {
                _uiConfig = new UIConfigLegacy(this.transform);
            }
            return _uiConfig;
        }
    }

    public GameObject Get(string name, bool ignoreNull = false)
    {
        return uiConfig[name, ignoreNull];
    }

    public T Get<T>(string name, bool ignoreNull = false) where T : Component
    {
        return uiConfig.Get<T>(name, ignoreNull);
    }

    public GameObject this[string name, bool ignoreNull = false]
    {
        get
        {
            return uiConfig[name, ignoreNull];
        }
    }

    #endregion
    #region 状态机
    private Dictionary<uint, UIState> m_registedState = new Dictionary<uint, UIState>();
    private Stack<UIState> m_stateStack = new Stack<UIState>();

    public void RegisterState(uint stateID,UIState state)
    {
        m_registedState.Add(stateID, state);
        state.OnRegister();
    }

    public void UnregisterState(uint stateID)
    {
        UIState state;
        if(m_registedState.TryGetValue(stateID,out state))
        {
            m_registedState.Remove(stateID);
            state.OnRemove();
        }
    }

    public UIState PushState(uint stateID)
    {
        UIState state;
        if(!m_registedState.TryGetValue(stateID,out state))
        {
            LOG.Error("未注册UIState:stateID = "+ stateID);
            return null;
        }
        if (this.m_stateStack.Count > 0)
        {
            this.m_stateStack.Peek().OnPause();
        }
        this.m_stateStack.Push(state);
        if(state.IsEnter)
        {
            state.OnResume();
        }
        else
        {
            state.IsEnter = true;
            state.OnEnter();
        }
        return state;
    }

    public UIState PopState()
    {
        if (this.m_stateStack.Count <= 0)
        {
            return null;
        }
        var state = this.m_stateStack.Pop(); 
        state.IsEnter = false;
        state.OnExit();
        if (this.m_stateStack.Count > 0)
        {
            this.m_stateStack.Peek().OnResume();
        }
        return state;
    }

    public int GetStateCount()
    {
        return m_stateStack.Count;
    }

    public UIState GetTopState()
    {
        return m_stateStack.Peek();
    }
    #endregion


    void Awake()
    {
        OnInitState();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        while (m_stateStack.Count > 0)
        {
            this.PopState();
        }
        foreach (var itr in m_registedState)
        {
            itr.Value.OnRemove();
        }
        m_registedState.Clear();
    }

    protected virtual void OnInitState()
    {

    }
}


public abstract class UIState
{
    public bool IsEnter { get; set; }

    public virtual void OnRegister()
    {
    }

    public virtual void OnRemove()
    {
    }

    public abstract void OnEnter();

    public abstract void OnExit();

    public virtual void OnPause() { }

    public virtual void OnResume() { }
}
