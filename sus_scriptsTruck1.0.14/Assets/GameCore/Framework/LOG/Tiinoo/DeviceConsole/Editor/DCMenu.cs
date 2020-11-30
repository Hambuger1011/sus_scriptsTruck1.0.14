using Framework;

using UnityEngine;
using System.Collections;
using UnityEditor;

namespace Tiinoo.DeviceConsole
{
	public class DCMenu
	{
		[MenuItem("Window/Tiinoo/Device Console/Settings")]
		public static void ShowSettings()
		{
			DCEditorWindow.GetInstance().ShowSettings();
		}

		[MenuItem("Window/Tiinoo/Device Console/About")]
		public static void ShowAbout()
		{
			DCEditorWindow.GetInstance().ShowAbout();
		}
	}
}

