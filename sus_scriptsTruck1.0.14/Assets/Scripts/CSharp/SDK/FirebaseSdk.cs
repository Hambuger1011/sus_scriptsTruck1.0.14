using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Firebase;
//using Firebase.Analytics;
using System;

/// <summary>
/// Firebase SDK
/// </summary>
public class FirebaseSdk
{

    //DependencyStatus mDependencyStatus = DependencyStatus.UnavailableOther;

    protected bool mFirebaseInit = false;
    protected string mUserId;

    public void Init()
    {
        //FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        //{
        //    Debug.Log("-----firebase task init--------->");

        //    mDependencyStatus = task.Result;
        //    if (mDependencyStatus == DependencyStatus.Available)
        //    {
        //        InitFirebase();
        //    }
        //    else
        //    {
        //        Debug.LogError("Could not resolve all Firebase dependencies: " + mDependencyStatus);
        //    }
        //});
    }

    private void InitFirebase()
    {
        //FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
        //FirebaseAnalytics.SetUserProperty(FirebaseAnalytics.UserPropertySignUpMethod, "Google");



        //FirebaseAnalytics.SetMinimumSessionDuration(new TimeSpan(0, 0, 10));
        //FirebaseAnalytics.SetSessionTimeoutDuration(new TimeSpan(0, 30, 0));
        mFirebaseInit = true;
    }

    public void SetUserId(string vUserId)
    {
        mUserId = vUserId;
        //if (!string.IsNullOrEmpty(mUserId))
        //{
        //    FirebaseAnalytics.SetUserId(mUserId);
        //}
    }

    public void AnalyticsLogin()
    {
        // Log an event with no parameters.
        LOG.Info("Logging a login event.");
        //FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventLogin);
    }

    public void AnalyticsProgress()
    {
        // Log an event with a float.
        LOG.Info("Logging a progress event.");
        // FirebaseAnalytics.LogEvent("progress", "percent", 0.4f);
    }

    public void AnalyticsScore()
    {
        // Log an event with an int parameter.
        LOG.Info("Logging a post-score event.");
        //FirebaseAnalytics.LogEvent(
        //  FirebaseAnalytics.EventPostScore,
        //  FirebaseAnalytics.ParameterScore,
        //  42);
    }

    public void AnalyticsGroupJoin()
    {
        // Log an event with a string parameter.
        LOG.Info("Logging a group join event.");
        //FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventJoinGroup, FirebaseAnalytics.ParameterGroupId,
        //  "spoon_welders");
    }

    public void AnalyticsLevelUp()
    {
        // Log an event with multiple parameters.
        LOG.Info("Logging a level up event.");
        //FirebaseAnalytics.LogEvent(
        //  FirebaseAnalytics.EventLevelUp,
        //  new Parameter(FirebaseAnalytics.ParameterLevel, 5),
        //  new Parameter(FirebaseAnalytics.ParameterCharacter, "mrspoon"),
        //  new Parameter("hit_accuracy", 3.14f));
    }

    // Reset analytics data for this app instance.
    public void ResetAnalyticsData()
    {
        LOG.Info("Reset analytics data.");
        //FirebaseAnalytics.ResetAnalyticsData();
    }

}
