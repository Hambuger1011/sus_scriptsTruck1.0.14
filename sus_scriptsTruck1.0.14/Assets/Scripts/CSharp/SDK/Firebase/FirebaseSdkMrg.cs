using System;
using System.Collections;
using System.Collections.Generic;
using Firebase.DynamicLinks;
using UnityEngine;

public class FirebaseSdkMrg : MonoBehaviour
{
    // Start is called before the first frame update
    void Start() {
        DynamicLinks.DynamicLinkReceived += OnDynamicLink;
    }

// Display the dynamic link received by the application.
    void OnDynamicLink(object sender, EventArgs args) {
        var dynamicLinkEventArgs = args as ReceivedDynamicLinkEventArgs;
        Debug.LogFormat("Received dynamic link {0}",
            dynamicLinkEventArgs.ReceivedDynamicLink.Url.OriginalString);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
