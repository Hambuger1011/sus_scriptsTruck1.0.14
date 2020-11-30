using System;
using UnityEngine;
using UnityEngine.UI;

public class InputFieldLayout : MonoBehaviour
{
    public InputField mainInputField;
    public RectTransform mainInputTrans;
    private float keyboardHeight;
    private float k;
    private float height = 0;
    private float PosY;
    private float PosX;
    private bool isFocused = false;

    // Start is called before the first frame update
    void Start()
    {
        PosY = mainInputTrans.localPosition.y;
        k = mainInputTrans.GetComponentInParent<CanvasScaler>().GetComponent<RectTransform>().sizeDelta.y;
    }

    // Update is called once per frame
    void Update()
    {
// #if UNITY_ANDROID
//         if (mainInputField.isFocused)
//         {
//             if (isFocused == false)
//             {
//                 isFocused = true;
//                 Invoke("UpdateTransHeight", 0.2f);
//                 Invoke("UpdateTransHeight", 0.5f);
//             }
//         }
//         else
//         {
//             if (isFocused)
//             {
//                 isFocused = false;
//                 height = 0f;
//                 PosX = mainInputTrans.localPosition.x;
//                 mainInputTrans.localPosition = new Vector3(PosX,PosY);
//             }
//         }
// #elif UNITY_IOS
         UpdateTransHeight();
// #endif
    }

    void UpdateTransHeight()
    {
        keyboardHeight = GetKeyboardHeight();
        if (Math.Abs(keyboardHeight - height) > 0)
        {
            height = keyboardHeight;
            if (Math.Abs(keyboardHeight) < 0.01f)
            {
                PosX = mainInputTrans.localPosition.x;
                mainInputTrans.localPosition = new Vector3(PosX,PosY);
                return;
            }
            float keyboardHeightUi = height * k / Display.main.systemHeight;
            mainInputTrans.anchoredPosition = Vector3.up * keyboardHeightUi;
        }
    }
    
    public float GetKeyboardHeight()
    {
#if UNITY_EDITOR
        return 0;
#elif UNITY_ANDROID
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
#elif UNITY_IOS
         return TouchScreenKeyboard.area.height * Display.main.systemHeight / Screen.height;
#else
        return 0;
#endif
    }
}
