#if UNITY_IOS || UNITY_EDITOR
using Framework;
using System.Diagnostics;


using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using AOT;
using System;

public static class IOSUtils{

    // [DllImport("__Internal")]
    // private static extern void FixScreenFrame(float h);

 //   [DllImport("__Internal")]
 //   private static extern long GetAvailMemoryInternal();

 //   [DllImport("__Internal")]
	//private static extern long GetUseMemoryInternal();


    //public static long GetAvailMemory()
    //{
    //    long availMemory = GetAvailMemoryInternal();
    //    return availMemory;
    //}

    //public static long GetUseMemory()
    //{
    //    long useMemory = GetUseMemoryInternal();
    //    return useMemory;
    //}



    /*
	反向调用，即从非托管代码激发托管代码的时候，函数指针到委托包装类的转换程序基于 JIT 编译器，
	JIT 编译器在ios下处于禁用模式，这就使得该调用存在一些限制：所要传递的C#方法必须为静态类方法；
	方法必须标注有 MonoPInvokeCallbackAttribute
	Only static methods in C# code be called from native code like this.
	*/
    //[MonoPInvokeCallback(typeof(WLSDKLoginCallback))]
    //static void OnWLSDKLoginResult(string json)
    //{
    //	s_WLSDKLoginCallback (json);
    //}

#if false
    // [DllImport("__Internal")]
    // private static extern void SaveImageToGallery(string fileName, Action<string> callback);
#else
    // [DllImport("__Internal")]
    // private static extern int saveToGallery(string fileName);
#endif
    public static void SaveImageToGallery(string fileName, string description, Action<string> callback)
    {
        s_onSaveImageToGallery = callback;
#if false
        SaveImageToGallery(fileName, OnSaveImageToGalleryNotify);
#else
        // saveToGallery(fileName);
        OnSaveImageToGalleryNotify(null);
#endif
    }


    static Action<string> s_onSaveImageToGallery;
    [MonoPInvokeCallback(typeof(Action<string>))]
    static void OnSaveImageToGalleryNotify(string json)
    {
        if(s_onSaveImageToGallery == null)
        {
            return;
        }
        var tmp = s_onSaveImageToGallery;
        s_onSaveImageToGallery = null;
        tmp(json);
    }

    [Conditional("UNITY_IOS")]
    public static void Init()
    {
        FixScreenFrame();
    }


    private static void FixScreenFrame()
    {
#if UNITY_EDITOR
        return;
#endif
        //if(UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhoneX || UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhoneUnknown)
        //{
        //    FixScreenFrame(32);
        //}
    }
}
#endif
