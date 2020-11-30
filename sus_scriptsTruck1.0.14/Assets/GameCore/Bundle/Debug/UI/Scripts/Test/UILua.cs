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
using UGUI;

public class UILua : MonoBehaviour {

    public Button btnClose;
    public Button btnRun;
    public InputField inScript;
    

	void Awake () {

        btnClose.onClick.AddListener(()=>
        {
            this.GetComponent<CUIForm>().Close();
        });

        btnRun.onClick.AddListener(() =>
        {
            XLuaManager.Instance.Execute(inScript.text);
        });
    }
    
}
