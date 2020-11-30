using Framework;

using UnityEngine;
using System.Collections;
using UnityEditor;
using Tiinoo.DeviceConsole;

namespace Tiinoo.DeviceConsole
{
	public class TabAbout
	{
		public TabAbout()
		{

		}

		public void Draw()
		{
			GUILayout.BeginVertical(GUIUtil.box_noPadding, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			
			GUILayout.BeginVertical();
			GUILayout.Space(50);

			GUILayout.Label("<size=20>" + DCConst.PLUGIN_NAME + "</size>", GUIUtil.label_rich_middleCenter);
			GUILayout.Space(4);

			GUILayout.Label("<b>" + DCConst.PLUGIN_VERSION + "</b>", GUIUtil.label_rich_middleCenter);
			GUILayout.Space(12);

			if (GUILayout.Button("Website"))
			{
				Application.OpenURL(DCConst.URL_WEBSITE);
			}
			GUILayout.Space(10);

			if (GUILayout.Button("Asset Store Page"))
			{
				Application.OpenURL(DCConst.URL_UAS_PLUGIN);
			}
			GUILayout.Space(10);

			if (GUILayout.Button("Unity Forum Thread"))
			{
				Application.OpenURL(DCConst.URL_UAS_FORUM);
			}
			GUILayout.Space(10);

			if (GUILayout.Button("All My Plugins"))
			{
				Application.OpenURL(DCConst.URL_UAS_PUBLISHER);
			}
			GUILayout.Space(6);

			EditorGUILayout.SelectableLabel("Email: " + DCConst.SUPPORT_EMAIL, GUIUtil.label_rich_middleCenter);
			
			GUILayout.EndVertical();
			
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();

			GUILayout.EndVertical();
		}
	}

}

