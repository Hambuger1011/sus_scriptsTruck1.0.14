//#define USE_PLUGIN
using System;
using Mopsicus.Plugins;
using UGUI;
using UnityEngine;
using UnityEngine.UI;
using Framework;

/// <summary>
/// 移动设备输入框的自适应组件
/// </summary>
public class ChatViewAdaptMobileKeyBoard : MonoBehaviour
{
    public InputField _inputField;
    public RectTransform target;
    private CUIForm uiform;

    /// <summary>
    /// 自适应（弹出输入框后整体抬高）的面板的初始位置
    /// </summary>
    private Vector2 _adaptPanelOriginPos;

    public static ChatViewAdaptMobileKeyBoard Create(GameObject attachRoot, InputField inputField)
    {
        ChatViewAdaptMobileKeyBoard instance = null;
        instance = attachRoot.AddComponent<ChatViewAdaptMobileKeyBoard>();
        instance._inputField = inputField;
        return instance;
    }

    private void Start()
    {
        //Debuger.Log("ChatViewAdaptMobileKeyBoard.start()");
        _inputField.onEndEdit.AddListener(OnEndEdit);
        _inputField.onValueChanged.AddListener(OnValueChanged);
        //_adaptPanelRt = transform.Find("TabControl/Panels").GetComponent<RectTransform>();

        uiform = this.GetComponentInParent<CUIForm>();
        _adaptPanelOriginPos = target.anchoredPosition;
#if USE_PLUGIN
        MobileInput.OnPrepareKeyboard += OnPrepareKeyboard;
        MobileInput.OnShowKeyboard += OnShowKeyboard;
#endif
    }

#if USE_PLUGIN
    float m_keyboardHeight;
    bool isHideSoftKeyboard = false;
    private void OnShowKeyboard(bool isShow, float height)
    {
        m_keyboardHeight = height;
        if (isShow)
        {
            if (!isHideSoftKeyboard)
            {
                isHideSoftKeyboard = true;
                AndroidUtils.HideUnitySoftKeyboard();
            }
        }else
        {
            isHideSoftKeyboard = false;
        }
        UpdatePosition();
    }

    private void OnPrepareKeyboard()
    {
    }
#endif

#if UNITY_EDITOR || !USE_PLUGIN
    private void Update()
    {
        UpdatePosition();
    }
#endif

    void UpdatePosition()
    {
        if (!_inputField.isFocused)
        {
            target.anchoredPosition = _adaptPanelOriginPos;
            return;
        }
#if USE_PLUGIN
        //float height = 0.4f* Display.main.systemHeight;// m_keyboardHeight;//GetKeyboardHeight();
        float height = m_keyboardHeight;//GetKeyboardHeight();
#else
        float height = ResolutionAdapter.GetKeyboardHeight();
#endif
        height = uiform.yPixel2View(height);
        //var viewSize = uiform.m_referenceResolution;
        var pos = _adaptPanelOriginPos;
        if (height > this._adaptPanelOriginPos.y)
        {
            //pos.y = height - viewSize.y * 0.5f + target.rect.size.y * 0.5f;
            pos.y = height;// + target.rect.size.y * 0.5f;
                           //Debug.LogFormat("安卓平台检测到InputField.isFocused为真，获取键盘高度：{0}, pos：{1}", height, pos);
        }
        target.anchoredPosition = pos;
    }

    private void OnValueChanged(string arg0) { }


    /// <summary>
    /// 结束编辑事件，TouchScreenKeyboard.isFocused为false的时候
    /// </summary>
    /// <param name="currentInputString"></param>
    private void OnEndEdit(string currentInputString)
    {
        //Debuger.LogFormat("OnEndEdit, 输入内容：{0}, 结束时间：{1}", currentInputString, Time.realtimeSinceStartup);
        target.anchoredPosition = _adaptPanelOriginPos;
    }

}
