using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TimeScalerWindow : EditorWindow
{
    [MenuItem("GameTools/TimeScale/Open", false, MenuPriority.TimeScaler + 100)]
    static void ShowWindow()
    {
        TimeScalerWindow window = (TimeScalerWindow)EditorWindow.GetWindow(typeof(TimeScalerWindow), false, "TimeScale", true);
        //try
        //{
        //    //设置居中
        //    Type type = Type.GetType("UnityEditor.MainWindow,UnityEditor");
        //    object obj = Resources.FindObjectsOfTypeAll(type)[0];
        //    Rect rect = (Rect)type.GetProperty("position").GetValue(obj, null);
        //    Rect rect2 = (Rect)type.GetProperty("screenPosition").GetValue(obj, null);
        //    window.position = new Rect(rect2.x + (rect.width - width) / 2f, rect2.y + (rect.height - height) / 2f, width, height);
        //}
        //catch (Exception ex)
        //{
        //    Debug.LogError(ex.Message);
        //}
        window.Show();
    }


    int selectedWin = 0;
    string[] winNames = new string[]
    {
        "x0.25","x0.5","x0.75","x1",
        "x1.25","x1.5","x1.75","x2",
    };
    void OnGUI()
    {
        Time.timeScale = GUILayout.HorizontalSlider(Time.timeScale, 0, 10,GUILayout.Height(35));
        GUILayout.Label("TimeScale:x" + Time.timeScale);
    }
}
