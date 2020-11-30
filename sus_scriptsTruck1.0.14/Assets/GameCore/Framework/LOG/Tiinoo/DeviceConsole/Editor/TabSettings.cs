using Framework;

using UnityEngine;
using System.Collections;
using UnityEditor;
using Tiinoo.DeviceConsole;

namespace Tiinoo.DeviceConsole
{
	public class TabSettings
	{
		public TabSettings()
		{

		}

		public void Draw()
		{
			GUILayoutOption CONTENT_WIDTH = GUILayout.Width(240);

			GUILayout.BeginVertical(GUIUtil.box_noPadding, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

			EditorGUI.BeginChangeCheck();

			// Open
			GUILayout.Label("<b>Open</b>", GUIUtil.label_rich);

			GUILayout.BeginHorizontal();
			GUIContent contentOpenWithTouch = new GUIContent("Open With Touch", "Select the gesture to open the console on mobile.");
			EditorGUILayout.PrefixLabel(contentOpenWithTouch);
			DCSettings.Instance.openWithTouch = (DCSettings.Gesture)EditorGUILayout.EnumPopup(DCSettings.Instance.openWithTouch, CONTENT_WIDTH);
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			GUIContent contentOpenWithKey = new GUIContent("Open With Key", "Select the key to open the console on desktop.");
			EditorGUILayout.PrefixLabel(contentOpenWithKey);
			DCSettings.Instance.openWithKey = (KeyCode)EditorGUILayout.EnumPopup(DCSettings.Instance.openWithKey, CONTENT_WIDTH);
			GUILayout.EndHorizontal();

			GUILayout.Space(8);
			GUIUtil.DrawHorizontalLine();
			GUILayout.Space(8);

			// Display
			GUILayout.Label("<b>Display</b>", GUIUtil.label_rich);

			GUILayout.BeginHorizontal();
			GUIContent contentLayer = new GUIContent("UI Layer", "The layer which the Device Console UI will be placed in. Default is the build-in UI layer.");
			EditorGUILayout.PrefixLabel(contentLayer);
			DCSettings.Instance.uiLayer = EditorGUILayout.LayerField(DCSettings.Instance.uiLayer, CONTENT_WIDTH);
			GUILayout.EndHorizontal();

			//GUIContent contentUseDebugCamera = new GUIContent ("Use Debug Camera", "Device Console uses Overlay render mode by default. If you choose this option, it will create a camera with the given depth. So you can decide what is rendered over it.");
			//DCSettings.Instance.useDebugCamera = EditorGUILayout.Toggle(contentUseDebugCamera, DCSettings.Instance.useDebugCamera);
			
			//EditorGUI.BeginDisabledGroup(!DCSettings.Instance.useDebugCamera);
			//GUIContent contentDebugCamera = new GUIContent("Debug Camera Depth", "Depth of the debug camera.");
			//DCSettings.Instance.debugCameraDepth = EditorGUILayout.Slider(contentDebugCamera, DCSettings.Instance.debugCameraDepth, -50, 50);
			//EditorGUI.EndDisabledGroup();

			GUILayout.Space(8);
			GUIUtil.DrawHorizontalLine();
			GUILayout.Space(8);

			// Console
			GUILayout.Label("<b>Console</b>", GUIUtil.label_rich);
			GUIContent contentShowOnException = new GUIContent("Show On Exception", "Show the console when an exception occurs even if the console is closed.");
			DCSettings.Instance.showOnException = EditorGUILayout.Toggle(contentShowOnException, DCSettings.Instance.showOnException);

			GUILayout.Space(8);
			GUIUtil.DrawHorizontalLine();
			GUILayout.Space(8);

			if (EditorGUI.EndChangeCheck())
			{
				EditorUtility.SetDirty(DCSettings.Instance);
			}

			GUILayout.EndVertical();

			// Hint
			GUILayout.Space(4);

			string hint = string.Format("It's really important to know your opinion, please consider leaving a rating on the <color={0}><b>Asset Store</b></color>.", GUIUtil.hex_color_link);
			bool isClickHint = EditorGUIUtil.DrawHyperlinkLabel(hint, GUIUtil.label_rich_wordWrap);
			if (isClickHint)
			{
				Application.OpenURL(DCConst.URL_UAS_PLUGIN);
			}

			GUILayout.Space(4);
		}
	}

}
