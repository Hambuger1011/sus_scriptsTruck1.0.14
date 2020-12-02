using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase;
using Firebase.DynamicLinks;
using Firebase.Extensions;
using UnityEngine;

public class FirebaseSdkMrg : MonoBehaviour
{
    // private static FirebaseSdkMrg instance;
    //
    // public static FirebaseSdkMrg ShareInstance()
    // {
    //     return instance;
    // }

    DependencyStatus dependencyStatus = DependencyStatus.UnavailableOther;
    public bool firebaseInitialized = false;

    public string kDomainUriPrefix;

    // Start is called before the first frame update
    void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                InitializeFirebase();
            }
            else
            {
                Debug.LogError(
                    "Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        });
    }

    private void FBShareLinkSucced(string postId)
    {
        TalkingDataManager.Instance.ShareRecord(2);
        LOG.Info("--ShareSucc--->postId:" + postId);

        var Localization = GameDataMgr.Instance.table.GetLocalizationById(142);
        UITipsMgr.Instance.PopupTips(Localization, false);
    }

    private void FBShareLinkFaild(bool isCancel, string errorInfo)
    {
        var Localization = GameDataMgr.Instance.table.GetLocalizationById(143);
        UITipsMgr.Instance.PopupTips(Localization, false);

        //UITipsMgr.Instance.PopupTips("Share Failed!", false);
    }

    private DynamicLinkComponents components;
    private Uri uri;

    private void OnGUI()
    {
        if (GUI.Button(new Rect(400, 100f, 100f, 100f), "测试1"))
        {
            uri = CreateAndDisplayLongLink();
            SdkMgr.Instance.shareSDK.ShareMsg(uri.ToString());
        }

        if (GUI.Button(new Rect(400, 200f, 100f, 100f), "测试2"))
        {
            CreateAndDisplayShortLinkAsync();

            // if (null != uri &&!string.IsNullOrEmpty(uri.ToString())) 
            //   Application.OpenURL(uri.ToString());

            // SdkMgr.Instance.shareSDK.ShareScreenShot();
        }

        if (GUI.Button(new Rect(400, 300f, 100f, 100f), "测试3"))
        {
            CreateAndDisplayUnguessableShortLinkAsync();

            // if (null != uri &&!string.IsNullOrEmpty(uri.ToString()))
            //   SdkMgr.Instance.facebook.FBShareLink(uri.ToString(), "Secrets of game choices", "Welcome to Secrets", "", FBShareLinkSucced, FBShareLinkFaild);

            // SdkMgr.Instance.shareSDK.CopyToClipboard("==========CopyToClipboard========");
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnDestroy()
    {
        DynamicLinks.DynamicLinkReceived -= OnDynamicLink;
    }

    // Handle initialization of the necessary firebase modules:
    void InitializeFirebase()
    {
        DynamicLinks.DynamicLinkReceived += OnDynamicLink;
        Debug.LogError("Firebase初始化完成===============================");
        firebaseInitialized = true;
    }

    // Display the dynamic link received by the application.
    void OnDynamicLink(object sender, EventArgs args)
    {
        Debug.LogError("OnDynamicLink===============================");
        var dynamicLinkEventArgs = args as ReceivedDynamicLinkEventArgs;
        Debug.LogErrorFormat("Received dynamic link {0}",
            dynamicLinkEventArgs.ReceivedDynamicLink.Url.OriginalString);
    }

    DynamicLinkComponents CreateDynamicLinkComponents()
    {
#if UNITY_5_6_OR_NEWER
        string appIdentifier = Application.identifier;
#else
      string appIdentifier = Application.bundleIdentifier;
#endif

        return new DynamicLinkComponents(
            // The base Link.
            new System.Uri("https://google.com"),
            // The dynamic link domain.
            kDomainUriPrefix)
        {
            GoogleAnalyticsParameters = new Firebase.DynamicLinks.GoogleAnalyticsParameters()
            {
                Source = "mysource",
                Medium = "mymedium",
                Campaign = "mycampaign",
                Term = "myterm",
                Content = "mycontent"
            },
            IOSParameters = new Firebase.DynamicLinks.IOSParameters(appIdentifier)
            {
                FallbackUrl = new System.Uri("https://mysite/fallback"),
                CustomScheme = "mycustomscheme",
                MinimumVersion = "1.1.0",
                IPadBundleId = appIdentifier,
                IPadFallbackUrl = new System.Uri("https://mysite/fallbackipad")
            },
            ITunesConnectAnalyticsParameters =
                new Firebase.DynamicLinks.ITunesConnectAnalyticsParameters()
                {
                    AffiliateToken = "abcdefg",
                    CampaignToken = "hijklmno",
                    ProviderToken = "pq-rstuv"
                },
            AndroidParameters = new Firebase.DynamicLinks.AndroidParameters(appIdentifier)
            {
                FallbackUrl = new System.Uri("https://mysite/fallback"),
                MinimumVersion = 1
            },
            SocialMetaTagParameters = new Firebase.DynamicLinks.SocialMetaTagParameters()
            {
                Title = "My App!",
                Description = "My app is awesome!",
                ImageUrl = new System.Uri("https://mysite.com/someimage.jpg")
            },
        };
    }

    public Uri CreateAndDisplayLongLink()
    {
        var longLink = CreateDynamicLinkComponents().LongDynamicLink;
        Debug.Log(String.Format("Long dynamic link {0}", longLink));
        return longLink;
    }

    public Task<ShortDynamicLink> CreateAndDisplayShortLinkAsync()
    {
        return CreateAndDisplayShortLinkAsync(new DynamicLinkOptions());
    }

    public Task<ShortDynamicLink> CreateAndDisplayUnguessableShortLinkAsync()
    {
        return CreateAndDisplayShortLinkAsync(new DynamicLinkOptions
        {
            PathLength = DynamicLinkPathLength.Unguessable
        });
    }

    private Task<ShortDynamicLink> CreateAndDisplayShortLinkAsync(DynamicLinkOptions options)
    {
        var components = CreateDynamicLinkComponents();
        return DynamicLinks.GetShortLinkAsync(components, options)
            .ContinueWithOnMainThread((task) =>
            {
                if (task.IsCanceled)
                {
                    Debug.Log("Short link creation canceled");
                }
                else if (task.IsFaulted)
                {
                    Debug.Log(String.Format("Short link creation failed {0}", task.Exception.ToString()));
                }
                else
                {
                    ShortDynamicLink link = task.Result;
                    Debug.Log(String.Format("Generated short link {0}", link.Url));
                    var warnings = new System.Collections.Generic.List<string>(link.Warnings);
                    if (warnings.Count > 0)
                    {
                        Debug.Log("Warnings:");
                        foreach (var warning in warnings)
                        {
                            Debug.Log("  " + warning);
                        }
                    }
                }

                return task.Result;
            });
    }
}