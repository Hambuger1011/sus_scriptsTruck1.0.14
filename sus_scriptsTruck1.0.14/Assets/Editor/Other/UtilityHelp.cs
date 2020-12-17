using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class UtilityHelp
{
    [MenuItem("Tools/UtilityHelp/ClearAllPlayerPrefs")]
    static void ClearAllPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }

    [MenuItem("Tools/UtilityHelp/ClearProgressBar")]
    static void ClearProgressBar()
    {
        EditorUtility.ClearProgressBar();
    }
}
