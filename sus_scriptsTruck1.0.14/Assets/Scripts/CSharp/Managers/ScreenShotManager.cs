using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UGUI;
using Framework;
using System.IO;
using System;
using System.Runtime.InteropServices;


/// <summary>
/// 屏幕截图功能
/// </summary>

public class ScreenShotManager : CMonoSingleton<ScreenShotManager>
{

#if UNITY_IOS 
    // [DllImport("__Internal")]
    // private static extern void _SavePhoto(string readAddr);
#endif

    protected override void Init()
    {
        base.Init();

    }

    /// <summary>
    /// 保存截屏图片，并且刷新相册（Android和iOS）
    /// </summary>
    /// <param name="name">若空就按照时间命名</param>
    public void CaptureScreenshot(string name = "")
    {
        string _name = "";
        if (string.IsNullOrEmpty(name))
        {
            _name = "Screenshot_" + GetCurTime() + ".jpg";
        }
        
        //Android版本
        StartCoroutine(CutImage(_name));
        LOG.Info("图片保存地址" + _name);

    }
    //截屏并保存
    IEnumerator CutImage(string name)
    {
        //图片大小  
        Texture2D tex = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, true);
        yield return new WaitForEndOfFrame();
        tex.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0, true);
        tex.Apply();


        yield return tex;
        //byte[] byt = tex.EncodeToPNG();
        byte[] byt = tex.EncodeToJPG();
        string path ="ScreenShotOut";//Application.dataPath;
#if UNITY_EDITOR
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
#endif
        string resultPath = path + "/" + name;

#if UNITY_ANDROID && !UNITY_EDITOR
        path = Application.persistentDataPath.Substring(0, Application.persistentDataPath.IndexOf("Android"));
        resultPath = path + "/DCIM/Camera/" + name;
        if (!Directory.Exists(path + "/DCIM/Camera"))
        {
            Directory.CreateDirectory(path + "/DCIM/Camera");
        }
#endif

#if UNITY_IOS && !UNITY_EDITOR
        resultPath = Application.persistentDataPath + "/" + name;
        ScreenCapture.CaptureScreenshot(name);
        // _SavePhoto(resultPath);
        yield break;
#endif


        File.WriteAllBytes(resultPath, byt);


        string[] paths = new string[1];
        string[] mimeTypes = new string[1];
        paths[0] = resultPath;
        mimeTypes[0] = "image/jpeg";
        ScanFile(paths, mimeTypes);
    }

    /// <summary>
    /// 添加水印
    /// </summary>
    /// <param name="background"></param>
    /// <param name="watermark"></param>
    /// <returns></returns>
    public Texture2D AddWatermark(Texture2D background, Texture2D watermark)
    {
        int startX = background.width - watermark.width - 10;
        int endX = startX + watermark.width;
        int startY = 8;
        int endY = startY + watermark.height;

        for (int x = startX; x < endX; x++)
        {
            for (int y = startY; y < endY; y++)
            {
                Color bgColor = background.GetPixel(x, y);
                Color wmColor = watermark.GetPixel(x - startX, y - startY);
                Color final_color = Color.Lerp(bgColor, wmColor, wmColor.a / 1.0f);
                background.SetPixel(x, y, final_color);
            }
        }
        background.Apply();
        return background;

    }

    //刷新图片，显示到相册中
    void ScanFile(string[] path,string[] mimeType)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        using (AndroidJavaClass PlayerActivity = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            AndroidJavaObject playerActivity = PlayerActivity.GetStatic<AndroidJavaObject>("currentActivity");

            // AndroidHelper.CallStaticMethod("com.game.gamelib.Scanner.PhotoScanner", "ToScanner", new object[] { playerActivity, path, mimeType });

            //using (AndroidJavaObject Conn = new AndroidJavaObject("android.media.MediaScannerConnection", playerActivity, null))
            //{
            //    Conn.CallStatic("scanFile", playerActivity, path, null, null);
            //}
        }
#endif
    }
    /// <summary>
    /// 获取当前年月日时分秒，如201803081916
    /// </summary>
    /// <returns></returns>
    string GetCurTime()
    {
        return DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString()
            + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();
    }
	
}
