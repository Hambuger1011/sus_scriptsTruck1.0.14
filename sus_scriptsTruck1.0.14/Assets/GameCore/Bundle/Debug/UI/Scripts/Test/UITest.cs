using Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Reflection;
using UnityEngine.SceneManagement;
using System.Diagnostics;
using System.Threading;
using Common;
using System.Runtime.InteropServices;
using AOT;
using UnityEngine.Networking;
using System.IO;
using Object = UnityEngine.Object;
using Framework;
using UGUI;
using AB;
using Tiinoo.DeviceConsole;
//using Tiinoo.DeviceConsole;
using XLua;
#if UNITY_2017_1_OR_NEWER
using UnityEngine.Video;
#endif

#if USE_MINIGAME_SDK
using Facebook.Unity;
#endif

public class UITest : MonoBehaviour {

    public Button btnClose;
    public GameObject prefabButton;

    public class TestAttribute : Attribute
    {
        public string name;

        public TestAttribute(string strName)
        {
            this.name = strName;
        }
    }

	void Awake () {
        btnClose.onClick.AddListener(()=>
        {
            this.GetComponent<CUIForm>().Close();
        });
        prefabButton.SetActiveEx(false);
        Type t = this.GetType();
        var methods = t.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);
        foreach(var method in methods)
        {
            if(method.IsDefined(typeof(TestAttribute),true))
            {
                var attr = method.GetCustomAttributes(typeof(TestAttribute), true)[0] as TestAttribute;
                UnityAction func = Delegate.CreateDelegate(typeof(UnityAction),this,method.Name) as UnityAction;
                GameObject go = GameObject.Instantiate(prefabButton);
                go.transform.SetParent(prefabButton.transform.parent, false);
                go.SetActiveEx(true);
                go.GetComponentInChildren<Text>().text = attr.name;
                go.GetComponent<Button>().onClick.AddListener(func);
            }
        }
    }


    [Test("显示日志控制台")]
    private void ShowWindowConsole()
    {
        if (WindowConsole.isVisible)
        {
            return;
        }
        
        if (UIWindowMgr.Instance == null)
        {
            LOG.Error("UIWindowMgr is not found!!!");
            return;
        }
        
        UIWindowMgr.Instance.PopUpWindow(Tiinoo.DeviceConsole.UIWindow.Id.Console, false);
    }


    [Test("资源检测")]
    void AbResCheck()
    {
        CUIManager.Instance.OpenForm(CUIID.Canvas_Dump_Res, useCameraRenderMode: false);
    }

    [Test("Google SDK")]
    void GoogleSDK()
    {
        ABSystem.ui.bundle(AbResBundle_DataTable.IsDataTableAsset(CUIID.Canvas_GoogleSDK)).LoadAsync("UITest", enResType.ePrefab, CUIID.Canvas_GoogleSDK,  (_) =>
        {
            _.Retain(this);
            CUIManager.Instance.OpenForm(CUIID.Canvas_GoogleSDK, useCameraRenderMode: false);
        });
    }

    [Test("Lua面板")]
    void RunLua()
    {
        CUIManager.Instance.OpenForm(CUIID.Canvas_Lua);
    }
    
    [Test("支付验证开关")]
    void PayFailure()
    {
        UIAlertMgr.Instance.Show("支付验证开关", SdkMgr.Instance._Test_PayOn ? "关闭" : "开启", AlertType.SureOrCancel, (isOK) =>
        {
            if (!isOK)
            {
                return;
            }
            SdkMgr.Instance._Test_PayOn = !SdkMgr.Instance._Test_PayOn;
        });
    }


    [Test("Keyboard")]
    void Keyboard()
    {
        //string inputMsg = "this is my string";
        //TouchScreenKeyboard keyboard;
        //keyboard = TouchScreenKeyboard.Open(
        //    inputMsg, 
        //    TouchScreenKeyboardType.Default, 
        //    false, false, false, false, 
        //    "Please Enter Name"
        //    );
        //if (keyboard.active)
        //{
        //    inputMsg = keyboard.text;
        //}
        //LOG.Error(inputMsg);


        var luaenv = XLuaManager.Instance.GetLuaEnv();
        var res = luaenv.DoString(@"
return function()
    local uiView = logic.UIMgr:Open(logic.uiid.Story_Keyboard)
end");
        using (var func = (LuaFunction)res[0])
        {
            var f = (Action)func.Cast(typeof(Action));
            f();
        }

    }

    [Test("SafeArea")]
    void QuerySkuDetails()
    {
        // LOG.Error(Screen.width+","+Screen.height);
        // LOG.Error(Screen.currentResolution.width + "," + Screen.currentResolution.height);
        // LOG.Error(Display.main.systemWidth + "," + Display.main.systemHeight);
        // LOG.Error(ResolutionAdapter.GetSafeArea());

        //        var luaenv = XLuaManager.Instance.GetLuaEnv();

        //        var res = luaenv.DoString(@"
        //return function()
        //    local go = CS.UnityEngine.GameObject('Test')
        //    CS.UnityEngine.GameObject.DestroyImmediate(go)
        //    print(go == nil)
        //    --print(go:Equals(nil))
        //end");
        //        using (var func = (LuaFunction)res[0])
        //        {
        //            var f = (Action)func.Cast(typeof(Action));
        //            f();
        //        }
        //UserDataManager.Instance.ResetMoney(2, UserDataManager.Instance.UserData.DiamondNum + 100);


        // LOG.Error("decorHeight=" + ResolutionAdapter.GetKeyboardHeight());
    }



    [Test("HideUnitySoftKeyboard")]
    void HideUnitySoftKeyboard()
    {
        var uiform = GameObject.Find("Canvas_BookReading(Clone)");
        var ps = uiform.GetComponentsInChildren<ParticleSystemRenderer>();
        foreach(var itr in ps)
        {
            var shader = itr.material.shader;
            // LOG.Error(shader + "\n" + shader.isSupported);
        }

    }

    [Test("测试奖励广告")]
    void gplogin()
    {
        // SdkMgr.Instance.ads.ShowRewardBasedVideo("test", bsuc => {
        //     // LOG.Error("测试奖励广告:" + bsuc);
        // });
    }
#if CHANNEL_SPAIN
    [Test("测试广告")]
    void TestAds()
    {
        LabCaveMediation.InitTest(LabCaveMediationSdk.appHash);
    }

    [Test("fb登录")]
    void fblogin()
    {
        SdkMgr.Instance.facebook.Login(1);
    }
    



#endif
}