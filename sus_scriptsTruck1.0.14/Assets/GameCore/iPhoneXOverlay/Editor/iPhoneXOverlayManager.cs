using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public static class iPhoneXOverlayManager
{
	//const GameViewSizeGroupType GROUP_TYPE = GameViewSizeGroupType.iOS;
	const float LANDSCAPE_ASPECT = 2436f / 1125f;
	const float PORTRAIT_ASPECT = 1125f / 2436f;

	struct OrientationMetadata
	{
		internal string sizeName;
		internal string textureName;
		internal int width, height;
	}

	/// <summary>
	/// Defines metadata for creating custom game view sizes.
	/// Texture names corespond to textures that are used to mask areas of the game view.
	/// </summary>
	static List<OrientationMetadata> metadata = new List<OrientationMetadata> {
		new OrientationMetadata {
			sizeName = "iPhone X Wide",
			textureName = "iPhoneX-landscape.psd",
			width = 2436,
			height = 1125
		},
		new OrientationMetadata {
			sizeName = "iPhone X Tall",
			textureName = "iPhoneX-portrait.psd",
			width = 1125,
			height = 2436
		}
	};

	#region Lifecycle
	[InitializeOnLoadMethod]
	static void InitializeOnLoad()
	{
		if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS)
		{
			metadata.ForEach(EnsureSizeExists);//添加分辨率
		}
		else
        {
            metadata.ForEach(EnsureSizeExists);//添加分辨率
        }
	}


	#endregion

	#region Private
	
	static void EnsureSizeExists(OrientationMetadata orientationMetadata)
	{
        GameViewSizeGroupType GROUP_TYPE = GameViewSizeGroupType.Standalone;
        switch (EditorUserBuildSettings.activeBuildTarget)
        {
            case BuildTarget.iOS:
                {
                    GROUP_TYPE = GameViewSizeGroupType.iOS;
                }
                break;
            case BuildTarget.Android:
                {
                    GROUP_TYPE = GameViewSizeGroupType.Android;
                }
                break;
        }
        if (GameViewUtils.SizeExists(GROUP_TYPE, orientationMetadata.sizeName)) return;
		GameViewUtils.AddCustomSize(GameViewUtils.GameViewSizeType.FixedResolution, GROUP_TYPE, 
		                            orientationMetadata.width, 
		                            orientationMetadata.height, 
		                            orientationMetadata.sizeName);
	}
	#endregion
}