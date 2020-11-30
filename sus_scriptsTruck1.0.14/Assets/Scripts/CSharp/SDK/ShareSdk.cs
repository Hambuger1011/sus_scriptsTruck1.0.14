using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Framework;
using System;
#if UNITY_IOS
using System.Runtime.InteropServices;
#endif

/// <summary>
/// 分享SDK(弹出手机原生的分享)
/// </summary>
public class ShareSdk : MonoBehaviour
{

#if UNITY_ANDROID

    #region share picture
    public void ShareScreenShot()
    {
        string _name = "Screenshot_" + GetCurTime() + ".png";
        StartCoroutine(ShareScreenshot(_name));
    }

    IEnumerator ShareScreenshot(string vName)
    {
        yield return new WaitForEndOfFrame();
        ScreenCapture.CaptureScreenshot(vName, 2);
        string destination = Path.Combine(Application.persistentDataPath, vName);
        //LOG.Info("----path----->" + Application.persistentDataPath);
        yield return new WaitForSeconds(0.3f); 
        if (!Application.isEditor)
        {
            AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
            AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");
            intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
            AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");
            AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("parse", destination);
            intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_STREAM"),
            uriObject);
            intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"),
            "Can you beat my score?");
            intentObject.Call<AndroidJavaObject>("setType", "image/jpeg");
            AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject chooser = intentClass.CallStatic<AndroidJavaObject>("createChooser",
            intentObject, "Share Screenshot");
            currentActivity.Call("startActivity", chooser);
            yield return new WaitForSeconds(1f); //WaitForSecondsRealtime(1f);
        }
    }
    
    #endregion


    # region share txt

    public void ShareMsg(string vMsg)
    {

        StartCoroutine(TakeTextAndroid(vMsg));
        LOG.Info("====shareMsg---->" + vMsg);
    }

    private IEnumerator TakeTextAndroid(string value)
    {
        if (!Application.isEditor)
        {
            AndroidJavaObject sharingIntent = new AndroidJavaObject("android.content.Intent");
            sharingIntent.Call<AndroidJavaObject>("setAction", "android.intent.action.SEND");
            sharingIntent.Call<AndroidJavaObject>("setType", "text/plain");
            sharingIntent.Call<AndroidJavaObject>("putExtra", "android.intent.extra.TEXT", value);
            AndroidJavaObject createChooser = sharingIntent.CallStatic<AndroidJavaObject>("createChooser", sharingIntent, "Share to");
            currentActivity.Call("startActivity", createChooser);
            yield return new WaitForSeconds(1f);
        }
    }
    #endregion

#endif

    

#if UNITY_IOS
 //    [DllImport("__Internal")]
	// private static extern void Onyx_NativeShare(string text, string encodedMedia);
 //    [DllImport ("__Internal")]
 //    private static extern void copyTextToClipboard(string text);

    public delegate void OnShareSuccess(string platform);
	public delegate void OnShareCancel(string platform);
 
	public OnShareSuccess onShareSuccess = null;
	public OnShareCancel onShareCancel = null;


    public void SharePicture()
    {
        StartCoroutine(TakeScreenshotIos());
    }

    private IEnumerator TakeScreenshotIos()
    {
        //图片大小  
        Texture2D tex = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, true);
        yield return new WaitForEndOfFrame();
        tex.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0, true);
        tex.Apply();
        yield return tex;

        NativeShare("", tex);
    }

    public void NativeShare(string text, Texture2D texture = null) {
		LOG.Info("------NativeShare-----");

			if(texture != null) {
				//Debug.Log("NativeShare: Texture");
				byte[] val = texture.EncodeToPNG();
				string bytesString = System.Convert.ToBase64String (val);
                // Onyx_NativeShare(text, bytesString);
			} else {
				//Debug.Log("NativeShare: No Texture");
                // Onyx_NativeShare(text, "");
			}

	}
	private void OnNativeShareSuccess(string result){
		// Debug.Log("success: " + result);
		if (onShareSuccess!=null){
			onShareSuccess(result);
		}
	}
	private void OnNativeShareCancel(string result){
		// Debug.Log("cancel: " + result);
		if (onShareCancel != null){
			onShareCancel(result);
		}
	}

#endif


    private static AndroidJavaObject currentActivity
    {
        get
        {
            return new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
        }
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


    public void CopyToClipboard(string value)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        using (AndroidJavaClass PlayerActivity = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            AndroidJavaObject playerActivity = PlayerActivity.GetStatic<AndroidJavaObject>("currentActivity");

            // AndroidHelper.CallStaticMethod("com.game.gamelib.Scanner.PhotoScanner", "CopyToClipboard", value);
        }
#elif UNITY_IOS
        // copyTextToClipboard(value);
#endif

    }
	
}
