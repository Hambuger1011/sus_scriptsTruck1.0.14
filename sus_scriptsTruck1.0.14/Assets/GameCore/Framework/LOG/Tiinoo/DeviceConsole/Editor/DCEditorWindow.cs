using Framework;

using UnityEngine;
using System.Collections;
using UnityEditor;

namespace Tiinoo.DeviceConsole
{
	public class DCEditorWindow : EditorWindow
	{
		private static TabSettings m_tabSettings;
		private static TabAbout m_tabAbout;
		private int m_currentTab = 0;

		private static string[] TAB_TITLES = {"Settings", "About"};

		public static DCEditorWindow GetInstance()
		{
			DCEditorWindow window = EditorWindow.GetWindowWithRect<DCEditorWindow>(new Rect(0, 0, 420, 360), true, "Device Console", true);
			window.Init();
			return window;
		}

		private void Init()
		{
			CreateTabs();

			// do other inits
		}

		private void CreateTabs()
		{
			if (m_tabSettings == null)
			{
				m_tabSettings = new TabSettings();
			}
			
			if (m_tabAbout == null)
			{
				m_tabAbout = new TabAbout();
			}
		}
		
		public void ShowSettings()
		{
			m_currentTab = 0;
		}

		public void ShowAbout()
		{
			m_currentTab = 1;
		}

		void OnGUI()
		{
			CreateTabs();

			GUILayout.BeginVertical();

			m_currentTab = GUILayout.Toolbar(m_currentTab, TAB_TITLES, GUILayout.Width(320));

			switch (m_currentTab)
			{
			case 0:
				if (m_tabSettings != null)
				{
					m_tabSettings.Draw();
				}
				break;

			case 1:
				if (m_tabAbout != null)
				{
					m_tabAbout.Draw();
				}
				break;
			}

			GUILayout.EndVertical();
		}
	}
}
