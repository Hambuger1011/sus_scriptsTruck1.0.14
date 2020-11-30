using Object= UnityEngine.Object;
using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEditor;
using UberLogger;

/// <summary>
/// The basic editor logger backend
/// This is seperate from the editor frontend, so we can have multiple frontends active if we wish,
///  and so that we catch errors even without the frontend active.
/// Derived from ScriptableObject so it persists across play sessions.
/// </summary>
[System.Serializable]
public class UberEditorLogger : ScriptableObject, UberLogger.ILogger
{
    List<LogInfo> LogInfo = new List<LogInfo>();

    public bool PauseOnError = false;
    public bool ClearOnPlay = true;
    public bool WasPlaying = false;
    public int NoErrors;
    public int NoWarnings;
    public int NoMessages;

    static public UberEditorLogger Create()
    {
        var editorDebug = ScriptableObject.FindObjectOfType<UberEditorLogger>();

        if(editorDebug==null)
        {
            editorDebug = ScriptableObject.CreateInstance<UberEditorLogger>();
        }

        editorDebug.NoErrors = 0;
        editorDebug.NoWarnings = 0;
        editorDebug.NoMessages = 0;
        
        return editorDebug;
    }

    public void OnEnable()
    {
        EditorApplication.playmodeStateChanged += OnPlaymodeStateChanged;

        //Make this scriptable object persist between Play sessions
        hideFlags = HideFlags.HideAndDontSave;
    }

    /// <summary>
    /// If we're about to start playing and 'ClearOnPlay' is set, clear the current logs
    /// </summary>
    public void ProcessOnStartClear()
    {
        if(!WasPlaying && EditorApplication.isPlayingOrWillChangePlaymode)
        {
            if(ClearOnPlay)
            {
                Clear();
            }
        }
        WasPlaying = EditorApplication.isPlayingOrWillChangePlaymode;
    }
    
    void OnPlaymodeStateChanged()
    {
        ProcessOnStartClear();
    }


    /// <summary>
    /// Interface for deriving new logger backends.
    /// Add a new logger via Logger.AddLogger()
    /// </summary>
    public interface ILoggerWindow
    {
        /// <summary>
        /// Logging backend entry point. logInfo contains all the information about the logging request.
        /// </summary>
        void OnLogChange(LogInfo logInfo);
    }
    List<ILoggerWindow> Windows = new List<ILoggerWindow>();

    public void AddWindow(ILoggerWindow window)
    {
        if(!Windows.Contains(window))
        {
            Windows.Add(window);
        }
    }

    public void Log(LogInfo logInfo)
    {
        lock(this)
        {
            LogInfo.Add(logInfo);
        }
        
        if(logInfo.Severity== UberLogger.LogSeverity.Error)
        {
            NoErrors++;
        }
        else if(logInfo.Severity== UberLogger.LogSeverity.Warning)
        {
            NoWarnings++;
        }
        else
        {
            NoMessages++;
        }

        foreach(var window in Windows)
        {
            window.OnLogChange(logInfo);
        }

        if(logInfo.Severity== UberLogger.LogSeverity.Error && PauseOnError)
        {
            UnityEngine.Debug.Break();
        }
    }

    public void Clear()
    {
        lock(this)
        {
            LogInfo.Clear();
            NoWarnings = 0;
            NoErrors = 0;
            NoMessages = 0;

            foreach(var window in Windows)
            {
                window.OnLogChange(null);
            }
        }
    }

    public List<LogInfo> CopyLogInfo()
    {
        lock(this)
        {
            return new List<LogInfo>(LogInfo);
        }
    }

    public List<LogInfo> GetLogInfo()
    {
        return LogInfo;
    }
}
