#if (UNITY_IOS)
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Resources;
using System.Runtime.CompilerServices;
using Google;
using GooglePlayServices;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEngine;

namespace GameFramework
{
	[InitializeOnLoad]
	public class USDKDemoXcodeProjectPatcher: AssetPostprocessor
	{
		private const int BUILD_ORDER_ADD_CONFIG = 1;

		private const int BUILD_ORDER_PATCH_PROJECT = 2;

		private const string GOOGLE_SERVICES_INFO_PLIST_BASENAME = "GoogleService-Info";

		private const string GOOGLE_SERVICES_INFO_PLIST_FILE = "GoogleService-Info.plist";

		private static string[] PROJECT_KEYS;

		private static Dictionary<string, string> configValues;

		private static HashSet<string> allBundleIds;

		private static string configFile;

		private static bool spamguard;

		[CompilerGenerated]
		private static EventHandler<PlayServicesResolver.BundleIdChangedEventArgs> cache0;

		[CompilerGenerated]
		private static EventHandler<PlayServicesResolver.BundleIdChangedEventArgs> cache1;

		private static bool Enabled => (int)EditorUserBuildSettings.activeBuildTarget == 9 && IOSResolver.Enabled;

		static USDKDemoXcodeProjectPatcher()
		{
			PROJECT_KEYS = new string[6]
			{
				"CLIENT_ID",
				"REVERSED_CLIENT_ID",
				"BUNDLE_ID",
				"PROJECT_ID",
				"STORAGE_BUCKET",
				"DATABASE_URL"
			};
			configValues = new Dictionary<string, string>();
			allBundleIds = new HashSet<string>();
			configFile = null;
			IOSResolver.RemapXcodeExtension();
			RunOnMainThread.Run((Action)delegate
			{
				ReadConfigOnUpdate();
			}, false);
			PlayServicesResolver.BundleIdChanged += ((EventHandler<PlayServicesResolver.BundleIdChangedEventArgs>)OnBundleIdChanged);
			if (Enabled)
			{
				PlayServicesResolver.BundleIdChanged -= ((EventHandler<PlayServicesResolver.BundleIdChangedEventArgs>)OnBundleIdChanged);
				RunOnMainThread.Run((Action)delegate
				{
					CheckConfiguration();
				}, false);
			}
		}

		internal static void ReadConfigOnUpdate()
		{
			ReadConfig(errorOnNoConfig: false);
		}

		private static string GetIOSApplicationId()
		{
			return UnityCompat.GetApplicationId((BuildTarget)9);
		}

		private static void CheckConfiguration()
		{
			CheckBundleId(GetIOSApplicationId());
			CheckBuildEnvironment();
		}

		internal static void ReadConfig(bool errorOnNoConfig = true, string filename = null)
		{
			try
			{
				ReadConfigInternal(errorOnNoConfig, filename);
			}
			catch (Exception ex)
			{
				if (!(ex is FileNotFoundException) && !(ex is TypeInitializationException))
				{
					throw ex;
				}
				if (Enabled)
				{
					Debug.LogWarning("FailedToLoadIOSExtensions");
				}
			}
		}

		internal static void ReadConfigInternal(bool errorOnNoConfig, string filename = null)
		{
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Expected O, but got Unknown
			configValues = new Dictionary<string, string>();
			configFile = (filename ?? FindConfig(errorOnNoConfig));
			if (configFile == null)
			{
				return;
			}
			PlistDocument val = (PlistDocument)(object)new PlistDocument();
			val.ReadFromString(File.ReadAllText(configFile));
			PlistElementDict root = val.root;
			string[] pROJECT_KEYS = PROJECT_KEYS;
			foreach (string text in pROJECT_KEYS)
			{
				PlistElement val2 = root[text];
				if (val2 != null)
				{
					configValues[text] = val2.AsString();
					if (object.Equals(text, "BUNDLE_ID"))
					{
						allBundleIds.Add(val2.AsString());
					}
				}
			}
		}

		internal static Dictionary<string, string> GetConfig()
		{
			return configValues;
		}

		private static void CheckBuildEnvironment()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Invalid comparison between Unknown and I4
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Invalid comparison between Unknown and I4
			if ((int)EditorUserBuildSettings.activeBuildTarget == 9 && (int)Application.platform == 7)
			{
				Debug.LogError("IOSNotSupportedOnWindows");
			}
		}

		private static void OnBundleIdChanged(object sender, PlayServicesResolver.BundleIdChangedEventArgs args)
		{
			ReadConfig(errorOnNoConfig: false);
			CheckBundleId(GetIOSApplicationId());
		}

		private static string CheckBundleId(string bundleId, bool promptUpdate = true, bool logErrorOnMissingBundleId = true)
		{
			if (configFile == null)
			{
				return null;
			}
			Dictionary<string, string> config = GetConfig();
			if (!config.TryGetValue("BUNDLE_ID", out string value))
			{
				return null;
			}
			if (!value.Equals(bundleId) && logErrorOnMissingBundleId && allBundleIds.Count > 0)
			{
				string[] array = allBundleIds.ToArray();
				string errorMessage = ""+bundleId+"   value=="+value;
				if (promptUpdate && !spamguard)
				{
					Debug.LogError("promptUpdate && !spamguard");
					/*ChooserDialog.Show("Please fix your Bundle ID", "Select a valid Bundle ID from your Firebase configuration.", $"Your bundle ID {bundleId} is not present in your Firebase configuration.  A mismatched bundle ID will result in your application to fail to initialize.\n\nNew Bundle ID:", array, 0, "Apply", "Cancel", delegate(string selectedBundleId)
					{
						if (!string.IsNullOrEmpty(selectedBundleId))
						{
							UnityCompat.SetApplicationId((BuildTarget)9, selectedBundleId);
							Measurement.ReportWithBuildTarget("bundleidmismatch/apply", null, "Mismatched Bundle ID: Apply");
						}
						else
						{
							Measurement.ReportWithBuildTarget("bundleidmismatch/cancel", null, "Mismatched Bundle ID: Cancel");
							spamguard = true;
							Debug.LogError((object)errorMessage);
						}
						ReadConfig();
					});*/
				}
				else
				{
					Debug.LogError((object)errorMessage);
				}
			}
			return value;
		}

		private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromPath)
		{
			if (!Enabled)
			{
				return;
			}
			bool flag = false;
			foreach (string path in importedAssets)
			{
				if (Path.GetFileName(path) == "GoogleService-Info.plist")
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				spamguard = false;
				ReadConfig(errorOnNoConfig: false);
				CheckBundleId(GetIOSApplicationId());
			}
		}

		internal static string FindConfig(bool errorOnNoConfig = true)
		{
			string value = configFile;
			SortedDictionary<string, string> sortedDictionary = new SortedDictionary<string, string>();
			allBundleIds.Clear();
			string[] array = AssetDatabase.FindAssets("GoogleService-Info");
			foreach (string text in array)
			{
				string text2 = AssetDatabase.GUIDToAssetPath(text);
				if (Path.GetFileName(text2) == "GoogleService-Info.plist")
				{
					sortedDictionary[text2] = text2;
				}
			}
			string[] array2 = new string[sortedDictionary.Keys.Count];
			sortedDictionary.Keys.CopyTo(array2, 0);
			string text3 = (array2.Length < 1) ? null : array2[0];
			if (array2.Length == 0)
			{
				if (errorOnNoConfig && Enabled)
				{
					Debug.LogError("GoogleServicesIOSFileMissing");
				}
			}
			else if (array2.Length > 1)
			{
				string iOSApplicationId = GetIOSApplicationId();
				string text4 = null;
				string[] array3 = array2;
				foreach (string text5 in array3)
				{
					string filename = text5;
					ReadConfig(errorOnNoConfig: true, filename);
					string text6 = CheckBundleId(iOSApplicationId, promptUpdate: true, logErrorOnMissingBundleId: false);
					text4 = (text4 ?? text6);
					if (text6 == iOSApplicationId)
					{
						text3 = text5;
						text4 = iOSApplicationId;
					}
				}
				if (string.IsNullOrEmpty(value) || !text3.Equals(value))
				{
					Debug.LogWarning("GoogleServicesFileMultipleFiles");
				}
			}
			return text3;
		}

		[PostProcessBuild(1)]
		internal static void OnPostProcessAddGoogleServicePlist(BuildTarget buildTarget, string pathToBuiltProject)
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			Debug.Log($"PostProcessBuild1:{Enabled}");
			if (Enabled)
			{
//				Measurement.analytics.Report("ios/xcodepatch", "iOS Xcode Project Patcher: Start");
				AddGoogleServicePlist(buildTarget, pathToBuiltProject);
			}
		}

		internal static void AddGoogleServicePlist(BuildTarget buildTarget, string pathToBuiltProject)
		{
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Expected O, but got Unknown
			ReadConfig();
			if (configFile == null)
			{
//				Measurement.analytics.Report("ios/xcodepatch/config/failed", "Add Firebase Configuration File Failure");
				return;
			}
			
			Debug.Log($"configFile != null");
			CheckBundleId(GetIOSApplicationId(), promptUpdate: false);
			string fileName = Path.GetFileName(configFile);
			File.Copy(configFile, Path.Combine(pathToBuiltProject, fileName), overwrite: true);
			string projectPath = IOSResolver.GetProjectPath(pathToBuiltProject);
			Debug.Log($"projectPath:{projectPath}");
			PBXProject val = (PBXProject)(object)new PBXProject();
			val.ReadFromString(File.ReadAllText(projectPath));
			foreach (string xcodeTargetGuid in IOSResolver.GetXcodeTargetGuids((object)val))
			{
				Debug.Log($"configFile != null");
				val.AddFileToBuild(xcodeTargetGuid, val.AddFile(fileName, fileName, (PBXSourceTree)1));
			}
			File.WriteAllText(projectPath, val.WriteToString());
//			Measurement.analytics.Report("ios/xcodepatch/config/success", "Add Firebase Configuration File Successful");
		}

		[PostProcessBuild(2)]
		internal static void OnPostProcessPatchProject(BuildTarget buildTarget, string pathToBuiltProject)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			Debug.Log($"PostProcessBuild2:{Enabled}");
			if (Enabled)
			{
				ReadAndApplyFirebaseConfig(buildTarget, pathToBuiltProject);
//				ApplyNsUrlSessionWorkaround(buildTarget, pathToBuiltProject);
			}
		}

		internal static void ReadAndApplyFirebaseConfig(BuildTarget buildTarget, string pathToBuiltProject)
		{
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Expected O, but got Unknown
			string text = "Firebase.Invites.dll";
			HashSet<string> hashSet = new HashSet<string>();
			hashSet.Add("Firebase.Auth.dll");
			hashSet.Add("Firebase.DynamicLinks.dll");
			hashSet.Add(text);
			HashSet<string> hashSet2 = hashSet;
			bool flag = false;
			bool flag2 = false;
			string[] array = AssetDatabase.FindAssets("t:Object");
			foreach (string text2 in array)
			{
				string fileName = Path.GetFileName(AssetDatabase.GUIDToAssetPath(text2));
				if (hashSet2.Contains(fileName))
				{
					flag = true;
					flag2 = (fileName == text);
				}
			}
			if (!flag2 && !flag)
			{
				return;
			}
			ReadConfig();
			Dictionary<string, string> config = GetConfig();
			if (config.Count != 0)
			{
				string value = null;
				string value2 = null;
				if (!config.TryGetValue("REVERSED_CLIENT_ID", out value))
				{
//					Measurement.analytics.Report("ios/xcodepatch/reversedclientid/failed", "Add Reversed Client ID Failed");
					Debug.LogError("PropertyMissingForGoogleSignIn");
				}
				if (!config.TryGetValue("BUNDLE_ID", out value2))
				{
					Debug.LogError("PropertyMissingForGoogleSignIn");
				}
				string path = Path.Combine(pathToBuiltProject, "Info.plist");
				PlistDocument val = (PlistDocument)(object)new PlistDocument();
				val.ReadFromString(File.ReadAllText(path));
				PlistElementDict root = val.root;
				PlistElementArray val2 = null;
				if (root.values.ContainsKey("CFBundleURLTypes"))
				{
					val2 = root["CFBundleURLTypes"].AsArray();
				}
				if (val2 == null)
				{
					val2 = root.CreateArray("CFBundleURLTypes");
				}
				if (value != null)
				{
					PlistElementDict val3 = val2.AddDict();
					val3.SetString("CFBundleTypeRole", "Editor");
					val3.SetString("CFBundleURLName", "google");
					val3.CreateArray("CFBundleURLSchemes").AddString(value);
//					Measurement.analytics.Report("ios/xcodepatch/reversedclientid/success", "Add Reversed Client ID Successful");
				}
				if (value2 != null)
				{
					PlistElementDict val4 = val2.AddDict();
					val4.SetString("CFBundleTypeRole", "Editor");
					val4.SetString("CFBundleURLName", value2);
					val4.CreateArray("CFBundleURLSchemes").AddString(value2);
				}
				if (flag2 && !root.values.ContainsKey("NSContactsUsageDescription"))
				{
					root.SetString("NSContactsUsageDescription", "Invite others to use the app.");
				}
				File.WriteAllText(path, val.WriteToString());
			}
		}

		internal static void ApplyNsUrlSessionWorkaround(BuildTarget buildTarget, string pathToBuiltProject)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Expected O, but got Unknown
			string path = Path.Combine(pathToBuiltProject, "Info.plist");
			PlistDocument val = (PlistDocument)(object)new PlistDocument();
			val.ReadFromString(File.ReadAllText(path));
			PlistElementDict root = val.root;
			if (root.values.ContainsKey("NSAppTransportSecurity"))
			{
				PlistElementDict val2;
				try
				{
					val2 = root["NSAppTransportSecurity"].AsDict();
				}
				catch (InvalidCastException arg)
				{
					Debug.LogWarning((object)string.Format("Unable to parse NSAppTransportSecurity as a dictionary. ({0})\n{1}\nTo fix this issue make sure NSAppTransportSecurity is a dictionary in your Info.plist", arg, "Unable to apply NSURLSession workaround. If NSAllowsArbitraryLoads is set to a different value than NSAllowsArbitraryLoadsInWebContent in your Info.plist network operations will randomly fail on some versions of iOS"));
//					Measurement.analytics.Report("ios/xcodepatch/nsurlsessionworkaround/failed", "NSURLSession workaround Failed");
					return;
				}
				if (val2.values.ContainsKey("NSAllowsArbitraryLoads") && !val2.values.ContainsKey("NSAllowsArbitraryLoadsInWebContent"))
				{
					try
					{
						val2.SetBoolean("NSAllowsArbitraryLoadsInWebContent", val2["NSAllowsArbitraryLoads"].AsBoolean());
					}
					catch (InvalidCastException arg2)
					{
						Debug.LogWarning((object)string.Format("Unable to parse NSAllowsArbitraryLoads as a boolean value. ({0})\n{1}\nTo fix this problem make sure NSAllowsArbitraryLoads is YES or NO in your Info.plist or NSAllowsArbitraryLoadsInWebContent is set.", arg2, "Unable to apply NSURLSession workaround. If NSAllowsArbitraryLoads is set to a different value than NSAllowsArbitraryLoadsInWebContent in your Info.plist network operations will randomly fail on some versions of iOS"));
//						Measurement.analytics.Report("ios/xcodepatch/nsurlsessionworkaround/failed", "NSURLSession workaround Failed");
						return;
					}
//					Measurement.analytics.Report("ios/xcodepatch/nsurlsessionworkaround/success", "NSURLSession workaround Successful");
				}
			}
			File.WriteAllText(path, val.WriteToString());
		}
	}
}

#endif