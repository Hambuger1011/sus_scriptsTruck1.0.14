using Framework;

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class EditorTools
{
    [MenuItem("GameTools/Scene/LaunchScene", false, MenuPriority.Scene + 100)]
    public static void PlayScene_StartSence()
    {
        OpenScene("Assets/Scenes/LaunchScene.unity");
        //EditorApplication.isPlaying = true;
    }

    public static void OpenScene(string sceneNameFullPath)
    {
        if (EditorApplication.isPlaying == true)
        {
            EditorApplication.isPlaying = false;
            return;
        }

        bool b = EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        if (!b) return;

        EditorSceneManager.OpenScene(sceneNameFullPath);
    }
    
}
