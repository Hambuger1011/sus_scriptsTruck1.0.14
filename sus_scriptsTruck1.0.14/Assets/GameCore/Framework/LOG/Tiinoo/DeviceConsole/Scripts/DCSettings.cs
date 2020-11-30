using Framework;

using UnityEngine;
using System.Collections;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Tiinoo.DeviceConsole
{
	public class DCSettings : ScriptableObject
	{
		// window access
		public Gesture openWithTouch = Gesture.SWIPE_DOWN_WITH_TWO_FINGERS;
		public KeyCode openWithKey = KeyCode.BackQuote;
		
		// display settings
		public int uiLayer = 5;
		//public bool useDebugCamera;

		[Range(-50, 50)]
		public float debugCameraDepth = 50;

		// console settings
		public bool showOnException = true;
		
		public enum Gesture
		{
			None,
			SWIPE_DOWN_WITH_ONE_FINGER,
			SWIPE_DOWN_WITH_TWO_FINGERS,
			SWIPE_DOWN_WITH_THREE_FINGERS,
		}

		private static DCSettings m_instance;

		public static DCSettings Instance
		{
			get
			{
				if (m_instance == null)
				{
					string pluginName = DCConst.PLUGIN_NAME;
					string pathInResources = DCConst.SETTINGS_ASSET_PATH_IN_RESOURCES;
					string filename = DCConst.SETTINGS_ASSET;

					m_instance = Resources.Load<DCSettings>(pathInResources);

					if (m_instance == null)
					{
						m_instance = ScriptableObject.CreateInstance<DCSettings>();

#if UNITY_EDITOR
						string parentFolder = DCConst.SETTINGS_ASSET_PARENT_FOLDER_PATH;

						if (!File.Exists(parentFolder))
						{
							Directory.CreateDirectory(parentFolder);
						}

						string path = parentFolder + "/" + filename;
						AssetDatabase.CreateAsset(m_instance, path);
						LOG.Info(string.Format("[{0}] {1} is lost, so create it! Path: {2}", pluginName, filename, path));
#endif
					}

					LOG.Info(string.Format("[{0}] Load {1}", pluginName, filename));
				}
				
				return m_instance;
			}
		}
	}
}
