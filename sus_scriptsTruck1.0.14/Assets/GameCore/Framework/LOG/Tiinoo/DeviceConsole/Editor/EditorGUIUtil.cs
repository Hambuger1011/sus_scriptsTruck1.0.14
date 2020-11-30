using Framework;

using UnityEngine;
using System.Collections;
using UnityEditor;

namespace Tiinoo.DeviceConsole
{
	public class EditorGUIUtil 
	{
		public static bool DrawHyperlinkLabel(string text, GUIStyle style)
		{
			GUIContent content = new GUIContent(text);
			return DrawHyperlinkLabel(content, style);
		}
		
		public static bool DrawHyperlinkLabel(GUIContent content, GUIStyle style)
		{
			Rect rect = EditorGUILayout.BeginVertical();
			GUILayout.Label(content, style);
			EditorGUILayout.EndVertical();
			
			if (Event.current.type == EventType.MouseUp)
			{
				if (rect.Contains(Event.current.mousePosition))
				{
					return true;
				}
			}
			
			return false;
		}
	}

}
