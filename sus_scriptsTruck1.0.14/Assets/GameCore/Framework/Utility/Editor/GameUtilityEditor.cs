using Framework;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text.RegularExpressions;

public static class GameUtilityEditor
{
    [UnityEditor.InitializeOnLoadMethod]
    public static void OnEditorGameStartUp()
    {
        GameUtility.Init();
    }
}
