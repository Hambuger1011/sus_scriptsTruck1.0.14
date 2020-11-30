using Framework;

using UnityEngine;

namespace Tiinoo.DeviceConsole
{
	public class GUIUtil
	{
		public static GUIStyle label_rich;
		public static GUIStyle label_rich_wordWrap;
		public static GUIStyle label_rich_middleCenter;
		public static GUIStyle box_noPadding;
		public static GUIStyle line;
		public static string hex_color_link = "#3364a4";

		private static bool m_isInited;

		static GUIUtil()
		{
			Init();
		}

		public static void Init()
		{
			if (m_isInited)
			{
				return;
			}

			label_rich = new GUIStyle(GUI.skin.label);
			label_rich.richText = true;

			label_rich_wordWrap = new GUIStyle(label_rich);
			label_rich_wordWrap.wordWrap = true;

			label_rich_middleCenter = new GUIStyle(label_rich);
			label_rich_middleCenter.alignment = TextAnchor.MiddleCenter;

			box_noPadding = new GUIStyle(GUI.skin.box);
			box_noPadding.padding = new RectOffset();

			line = new GUIStyle(GUI.skin.box);
			line.padding.top = 1;
			line.padding.bottom = 1;
			line.margin.top = 1;
			line.margin.bottom = 1;
			line.border.top = 1;
			line.border.bottom = 1;

			m_isInited = true;
		}

		public static void DrawHorizontalLine()
		{
			GUILayout.Box(GUIContent.none, line, GUILayout.ExpandWidth(true), GUILayout.Height(1f));
		}

		public static void DrawVerticalLine()
		{
			GUILayout.Box(GUIContent.none, line, GUILayout.Width(1f), GUILayout.ExpandHeight(true));
		}
	}
}
