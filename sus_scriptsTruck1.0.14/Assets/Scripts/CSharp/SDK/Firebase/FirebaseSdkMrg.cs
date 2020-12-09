using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    // private void OnGUI()
    // {
    //     if (GUI.Button(new Rect(400, 100f, 100f, 100f), "测试1"))
    //     {
    //         Uri uri = CreateAndDisplayLongLink();
    //         // Uri uri = CreateAndDisplayLongLink2();
    //         // SdkMgr.Instance.shareSDK.ShareMsg(uri.ToString());
    //         LOG.Log(uri.ToString());
    //     }
    //
    //     if (GUI.Button(new Rect(400, 200f, 100f, 100f), "测试2"))
    //     {
    //         var uu = CreateAndDisplayShortLinkAsync();
    //         LOG.Log(uu.ToString());
    //         // if (null != uri &&!string.IsNullOrEmpty(uri.ToString())) 
    //         //   Application.OpenURL(uri.ToString());
    //
    //         // SdkMgr.Instance.shareSDK.ShareScreenShot();
    //     }
    //
    //     if (GUI.Button(new Rect(400, 300f, 100f, 100f), "测试3"))
    //     {
    //         var oo = CreateAndDisplayUnguessableShortLinkAsync();
    //         LOG.Log(oo.ToString());
    //
    //         // SdkMgr.Instance.shareSDK.CopyToClipboard("==========CopyToClipboard========");
    //     }
    // }

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
    
    /// <summary>
    /// 获取一个类指定的属性值
    /// </summary>
    /// <param name="info">object对象</param>
    /// <param name="field">属性名称</param>
    /// <returns></returns>
    public static object GetPropertyValue(object info, string field)
    {
        if (info == null) return null;
        Type t = info.GetType();
        IEnumerable<System.Reflection.PropertyInfo> property = from pi in t.GetProperties() where pi.Name.ToLower() == field.ToLower() select pi;
        return property.First().GetValue(info, null);
    }

    // Display the dynamic link received by the application.
    void OnDynamicLink(object sender, EventArgs args)
    {
        Debug.LogWarning("OnDynamicLink===============================");
        var dynamicLinkEventArgs = args as ReceivedDynamicLinkEventArgs;
        Debug.LogWarningFormat("Received dynamic link {0}", dynamicLinkEventArgs.ReceivedDynamicLink.Url.OriginalString);
        Debug.LogWarningFormat("sender {0}", sender);
        var refferer = GetPropertyValue(sender, "refferer");
        Debug.LogWarningFormat("refferer {0}", refferer);
        
        //数组 转 object
        object[] objectArray = new object[2];//这里的2就是改成你要传递几个参数
        objectArray[0] = true;
        objectArray[1] = 5f;
        object objArr = (object)objectArray;
        Debug.LogWarningFormat("objArr {0}", objArr);
        // object[] objArr = (object[])sender;
        // Debug.LogWarningFormat("objArr.Length {0}", objArr.Length);
        // for (int i = 0; i < objArr.Length; i++)
        // {
        //     Debug.LogWarningFormat("objArr {0}==={1}",i, objArr[i]);
        // }
    }

    public Uri CreateAndDisplayLongLink2()
    {
#if UNITY_5_6_OR_NEWER
        string appIdentifier = Application.identifier;
#else
      string appIdentifier = Application.bundleIdentifier;
#endif
        var components = new Firebase.DynamicLinks.DynamicLinkComponents(
            // The base Link.
            new System.Uri("https://www.baidu.com/?TestId=" + 88888888),
            // The dynamic link URI prefix.
            kDomainUriPrefix) {
            IOSParameters = new Firebase.DynamicLinks.IOSParameters(appIdentifier),
            AndroidParameters = new Firebase.DynamicLinks.AndroidParameters(
                appIdentifier),
        };
// do something with: components.LongDynamicLink

        var options = new Firebase.DynamicLinks.DynamicLinkOptions {
            PathLength = DynamicLinkPathLength.Unguessable
        };
        
        Firebase.DynamicLinks.DynamicLinks.GetShortLinkAsync(components, options).ContinueWith(task => {
            if (task.IsCanceled) {
                Debug.LogError("GetShortLinkAsync was canceled.");
                return;
            }
            if (task.IsFaulted) {
                Debug.LogError("GetShortLinkAsync encountered an error: " + task.Exception);
                return;
            }
        
            // Short Link has been created.
            Firebase.DynamicLinks.ShortDynamicLink link = task.Result;
            Debug.LogFormat("Generated short link {0}", link.Url);
        
            var warnings = new System.Collections.Generic.List<string>(link.Warnings);
            if (warnings.Count > 0) {
                // Debug logging for warnings generating the short link.
            }
        });

        return components.LongDynamicLink;
    }

    public Uri CreateAndDisplayLongLink()
    {
        var longLink = CreateDynamicLinkComponents().LongDynamicLink;
        Debug.Log(String.Format("Long dynamic link {0}", longLink));
        return longLink;
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