/*
 * Unity Camera的FOV是Vertical FOV的角度，fieldOfView = 60 即Far Play的高度为60
 * https://www.jianshu.com/p/95cb4621206e
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CameraAspectRatio : MonoBehaviour {

    public Vector2 designScreen = new Vector2(750,1334);
    public float designFOV = 60;

    Camera _camera;
    public new Camera camera
    {
        get
        {
            if(_camera == null)
            {
                _camera = this.GetComponent<Camera>();
            }
            return _camera;
        }
    }

    void Awake () {
        MatchScreen();
    }

    [ContextMenu("MatchScreen")]
    void MatchScreen()
    {
        if(camera.orthographic)
        {
            var physicalScreen = GetScreenSize();
            var designAspecRatio = designScreen.x / designScreen.y;
            var physicalAspecRatio = physicalScreen.y / physicalScreen.x;

            //1024/768 是编辑模式下固定的屏幕宽度和屏幕高度,Screen.Height和Screen.Width是实际屏幕高度和实际屏幕宽度
            Camera.main.orthographicSize = camera.orthographicSize * designAspecRatio * physicalAspecRatio;
        }
        else
        {
            var physicalScreen = GetScreenSize();
            var designAspecRatio = designScreen.y / designScreen.x;
            var physicalAspecRatio = physicalScreen.y / physicalScreen.x;

#if false
        //宽度匹配
        camera.fieldOfView = designFOV * (1 + (physicalAspecRatio - designAspecRatio)/2);
#else
            int manualHeight;
            //然后得到当前屏幕的高宽比 和 你自定义需求的高宽比。通过判断他们的大小，来不同赋值
            //*其中Convert.ToSingle（）和 Convert.ToFloat() 来取得一个int类型的单精度浮点数（C#中没有 Convert.ToFloat() ）；
            if (physicalAspecRatio > designAspecRatio)
            {
                //如果屏幕的高宽比大于自定义的高宽比 。则通过公式  ManualWidth * manualHeight = Screen.width * Screen.height；
                //来求得适应的  manualHeight ，用它待求出 实际高度与理想高度的比率 scale
                manualHeight = Mathf.RoundToInt(designScreen.x / Screen.width * Screen.height);
            }
            else
            {   //否则 直接给manualHeight 自定义的 ManualHeight的值，那么相机的fieldOfView就会原封不动
                manualHeight = Mathf.RoundToInt(designScreen.y);
            }

            float scale = Convert.ToSingle(manualHeight * 1.0f / designScreen.y);
            camera.fieldOfView = designFOV * scale;                      //Camera.fieldOfView 视野:  这是垂直视野：水平FOV取决于视口的宽高比，当相机是正交时fieldofView被忽略
                                                                         //把实际高度与理想高度的比率 scale乘加给Camera.fieldOfView。
                                                                         //这样就能达到，屏幕自动调节分辨率的效果
#endif
        }

    }

    Vector2 GetScreenSize()
    {
#if UNITY_EDITOR
        var type = Type.GetType("UnityEditor.GameView,UnityEditor.dll");

        //var gameView = (GameView)WindowLayout.FindEditorWindowOfType(typeof(GameView));
        //var p = type.GetProperty("targetSize", BindingFlags.Instance | BindingFlags.NonPublic);

        var method = type.GetMethod("GetMainGameViewTargetSize", BindingFlags.NonPublic | BindingFlags.Static);
        Vector2 gameViewSize = (Vector2)method.Invoke(null, null);
        //LOG.Info(gameViewSize);
        return gameViewSize;
#endif
        return new Vector2(Screen.width, Screen.height);
    }
}
