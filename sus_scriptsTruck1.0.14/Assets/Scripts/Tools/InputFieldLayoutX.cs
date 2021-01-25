using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class InputFieldLayoutX : MonoBehaviour
{
    public InputField inputField;
    public RectTransform inputFieldRect;
    public RectTransform moveRect;
    bool isFocused = false;
    Vector2 moveRectOriginPos;
    Vector2 screenPos;
    int keyboardHeight_last = 0;
    // Start is called before the first frame update
    void Start()
    {
        moveRectOriginPos = moveRect.anchoredPosition;
        //Deug.Log($"systemHeight:{Display.main.systemHeight} renderingHeight:{Display.main.renderingHeight}");
        //Deug.Log($"Screen.height:{Screen.height}");
        //Deug.Log($"TouchScreenKeyboard.isSupported:{TouchScreenKeyboard.isSupported}");
        //Deug.Log($"moveRect.anchoredPosition:{moveRectOriginPos}");
    }
    private void Update()
    {
        if (isFocused != inputField.isFocused)
        {
            isFocused = inputField.isFocused;
            if (isFocused)
            {
                onGetFocused();
            }
            else
            {
                onLostFocused(); 
                isFocused = inputField.isFocused;
            }
        }
    }

    IEnumerator DelayGetKeyboardHeight()
    {
        yield return null;
        while (inputField.isFocused)
        {
            int keyboardHeight_local = GetKeyboardHeight();
            if(keyboardHeight_local != 0 )
            {
                if(keyboardHeight_local != keyboardHeight_last)
                {
                    keyboardHeight_last = keyboardHeight_local;
                    SetInputUIHeight(keyboardHeight_local);
                }
                else
                {
                    yield break;
                }
            }
            yield return null;
        }
    }

    void SetInputUIHeight(int _keyboardHeight)
    {
        float keyboardHeight = ((float)_keyboardHeight) * Display.main.systemHeight / Screen.height;
        float k = gameObject.GetComponentInParent<CanvasScaler>().GetComponent<RectTransform>().sizeDelta.y;
        //Deug.Log($"SetInputUIHeight k:{k}");
        float keyboardHeightUi = keyboardHeight * k / Display.main.systemHeight;
        //Deug.Log($"SetInputUIHeight keyboardHeightUi:{keyboardHeightUi}");

        screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, inputFieldRect.transform.position);
        screenPos.y -= inputFieldRect.GetComponent<RectTransform>().rect.height / 2;
        //Deug.Log($"SetInputUIHeight screenPos.y:{screenPos.y}");
        if (keyboardHeightUi > screenPos.y)
        {
            keyboardHeightUi = keyboardHeightUi - screenPos.y;
            moveRect.anchoredPosition = moveRectOriginPos + Vector2.up * keyboardHeightUi;
        }
    }

    void onGetFocused(int _keyboardHeight_local=0)
    {
        //Deug.Log($"InputField.onGetFocused");
        StartCoroutine(DelayGetKeyboardHeight());
    }

    void onLostFocused()
    {
        //Deug.Log($"InputField.onLostFocused");
        keyboardHeight_last = 0;
        moveRect.anchoredPosition = moveRectOriginPos;
    }
       
    int GetKeyboardHeight()
    {
#if !UNITY_EDITOR && UNITY_ANDROID
        using (var unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                var unityPlayer = unityClass.GetStatic<AndroidJavaObject>("currentActivity").Get<AndroidJavaObject>("mUnityPlayer");
                var view = unityPlayer.Call<AndroidJavaObject>("getView");
                var dialog = unityPlayer.Get<AndroidJavaObject>("b");

                if (view == null || dialog == null)
                {
                    return 0;
                }

                var decorHeight = 0;

                if (true)//includeInput
                {
                    var decorView = dialog.Call<AndroidJavaObject>("getWindow").Call<AndroidJavaObject>("getDecorView");

                    if (decorView != null)
                        decorHeight = decorView.Call<int>("getHeight");
                }

                using (var rect = new AndroidJavaObject("android.graphics.Rect"))
                {
                    view.Call("getWindowVisibleDisplayFrame", rect);
                    return Display.main.systemHeight - rect.Call<int>("height") + decorHeight;
                }
            }
#elif !UNITY_EDITOR && UNITY_IOS
        return TouchScreenKeyboard.area.height
#else
        return 0;
#endif
    }

}
