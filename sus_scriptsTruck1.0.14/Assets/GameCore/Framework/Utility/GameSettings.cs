using Framework;


using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
	public class GameSettings
	{
        public static int c_renderFPS = 60;
        public static int c_NormalFPS = 60;
        public static int c_HighPerfomanceFPS = 60;


        public const string str_renderQuality = "gameSettings_RenderQuality";
        public const string str_particleQuality = "gameSettings_ParticleQuality";
        private const string GameSettingEnableHDMode = "GameSettingEnableHDMode_New2";

        public const int maxScreenWidth = 750;
		public const int maxScreenHeight = 1334;

		public static enGameRenderQuality RenderQuality;

		public static enGameRenderQuality ParticleQuality;

		public static enGameRenderQuality DeviceLevel;

		private static bool m_dynamicParticleLOD;
        
        
		private static bool _EnableHDMode;
        
        

		public static enGameRenderQuality MaxShadowQuality;

		public static int DefaultScreenWidth;

		public static int DefaultScreenHeight;
        
		public static bool EnableHDMode
		{
			get
			{
				return GameSettings._EnableHDMode;
			}
			set
			{
				if (GameSettings._EnableHDMode != value)
				{
					GameSettings._EnableHDMode = value;
				}
			}
		}
        

		public static bool IsHighQuality
		{
			get
			{
				return GameSettings.RenderQuality == enGameRenderQuality.eHigh;
			}
		}

		public static bool DynamicParticleLOD
		{
			get
			{
				return GameSettings.m_dynamicParticleLOD;
			}
		}

		public static int ModelLOD
		{
			get
			{
				return (int)GameSettings.RenderQuality;
			}
			set
			{
				GameSettings.RenderQuality = (enGameRenderQuality)Mathf.Clamp(value, 0, 2);
			}
		}

		public static int ParticleLOD
		{
			get
			{
				return (int)GameSettings.ParticleQuality;
			}
			set
			{
				GameSettings.ParticleQuality = (enGameRenderQuality)Mathf.Clamp(value, 0, 2);
			}
		}
        
		public static enGameRenderQuality ShadowQuality
		{
			get
			{
				return (enGameRenderQuality)Mathf.Max((int)GameSettings.MaxShadowQuality, GameSettings.ModelLOD);
			}
			set
			{
				enGameRenderQuality shadowQuality = GameSettings.ShadowQuality;
				GameSettings.MaxShadowQuality = value;
				//if (shadowQuality != GameSettings.MaxShadowQuality)
				//{
				//	GameSettings.ApplyShadowSettings();
				//}
			}
		}

		public static bool AllowRadialBlur
		{
			get
			{
				return GameSettings.DeviceLevel != enGameRenderQuality.eLow;
			}
		}

		static GameSettings()
		{
			GameSettings.DeviceLevel = enGameRenderQuality.eLow;
			GameSettings.m_dynamicParticleLOD = true;
		}
        

		public static void Init()
		{
			GameSettings.DeviceLevel = enGameRenderQuality.eHigh;
            if(SystemInfo.systemMemorySize <= 1024)
            {
                GameSettings.DeviceLevel = enGameRenderQuality.eLow;
            }else if (SystemInfo.systemMemorySize <= 2048)
            {
                GameSettings.DeviceLevel = enGameRenderQuality.eMedium;
            }

            if (PlayerPrefs.HasKey(str_renderQuality))
			{
				int val = PlayerPrefs.GetInt(str_renderQuality, 0);
				GameSettings.RenderQuality = (enGameRenderQuality)Mathf.Clamp(val, 0, 2);
			}
			else
			{
				GameSettings.RenderQuality = GameSettings.DeviceLevel;
			}

			if (PlayerPrefs.HasKey(str_particleQuality))
			{
				int val = PlayerPrefs.GetInt(str_particleQuality, 0);
				GameSettings.ParticleQuality = (enGameRenderQuality)Mathf.Clamp(val, 0, 2);
			}
			else
			{
				GameSettings.ParticleQuality = GameSettings.RenderQuality;
			}

            //if (GameSettings.DeviceLevel == SGameRenderQuality.Low)
            //{
            //	GameSettings.cameraHeight = CameraHeightType.Low;
            //}
            //else
            //{
            //	GameSettings.cameraHeight = CameraHeightType.Medium;
            //}


            if (GameSettings.RenderQuality == enGameRenderQuality.eLow)
            {
                QualitySettings.blendWeights = BlendWeights.OneBone;
                QualitySettings.SetQualityLevel((int)enQualityLevel.eFastest, true);
                QualitySettings.vSyncCount = 0;
            }
            else
            {
                QualitySettings.blendWeights = BlendWeights.TwoBones;
                QualitySettings.SetQualityLevel((int)enQualityLevel.eFantastic, true);
                QualitySettings.vSyncCount = 0;
            }
            RefreshResolution();
        }

		public static void Save()
		{
			PlayerPrefs.SetInt(str_renderQuality, (int)GameSettings.RenderQuality);
			PlayerPrefs.SetInt(str_particleQuality, (int)GameSettings.ParticleQuality);

			bool flag = PlayerPrefs.GetInt(GameSettingEnableHDMode, 0) == 1;
			if (flag != GameSettings._EnableHDMode)
			{
				GameSettings.SetHDMode(GameSettings._EnableHDMode);
			}
			PlayerPrefs.SetInt(GameSettingEnableHDMode, (!GameSettings._EnableHDMode) ? 0 : 1);

			PlayerPrefs.Save();
		}


        #region 分辨率设置
        public static void RefreshResolution()
        {
            GameSettings.InitResolution();
            if (PlayerPrefs.HasKey(GameSettingEnableHDMode))
            {
                int _EnableHDMode = PlayerPrefs.GetInt(GameSettingEnableHDMode, 0);
                GameSettings._EnableHDMode = (_EnableHDMode > 0);
            }
            else
            {
                bool bSupportHDMode = GameSettings.SupportHDMode();
                if (bSupportHDMode)
                {
                    GameSettings._EnableHDMode = false;
                }
                else
                {
                    GameSettings._EnableHDMode = !GameSettings.ShouldReduceResolution();
                }
            }
            GameSettings.SetHDMode(GameSettings._EnableHDMode);
        }

        public static bool ShouldReduceResolution()
		{
			int width = (GameSettings.DefaultScreenWidth <= GameSettings.DefaultScreenHeight) ? GameSettings.DefaultScreenHeight : GameSettings.DefaultScreenWidth;
			int height = (GameSettings.DefaultScreenWidth <= GameSettings.DefaultScreenHeight) ? GameSettings.DefaultScreenWidth : GameSettings.DefaultScreenHeight;
			return width >= maxScreenWidth || height >= maxScreenHeight;
        }

		public static bool SupportHDMode()
		{
			int width = (GameSettings.DefaultScreenWidth <= GameSettings.DefaultScreenHeight) ? GameSettings.DefaultScreenHeight : GameSettings.DefaultScreenWidth;
			int height = (GameSettings.DefaultScreenWidth <= GameSettings.DefaultScreenHeight) ? GameSettings.DefaultScreenWidth : GameSettings.DefaultScreenHeight;
			return width >= maxScreenWidth || height >= maxScreenHeight;
		}

		private static void InitResolution()
		{
			if (GameSettings.DefaultScreenWidth == 0 || GameSettings.DefaultScreenHeight == 0)
			{
				int width = Screen.width;
				int height = Screen.height;
#if 横屏
                GameSettings.DefaultScreenWidth = Mathf.Max(width, height);
				GameSettings.DefaultScreenHeight = Mathf.Min(width, height);
#else
                GameSettings.DefaultScreenWidth = Mathf.Min(width, height);
                GameSettings.DefaultScreenHeight = Mathf.Max(width, height);
#endif
            }
		}

		public static void SetHDMode(bool enable)
		{
			GameSettings.InitResolution();
			int width = GameSettings.DefaultScreenWidth;
			int height = GameSettings.DefaultScreenHeight;
			if (!enable)
			{
				width = maxScreenWidth;
				height = width * GameSettings.DefaultScreenHeight / GameSettings.DefaultScreenWidth;
			}
			if (width != Screen.width || height != Screen.height)
			{
				Screen.SetResolution(width, height, true);
				//Sprite3D.SetRatio(width, height);
                LOG.Info("分辨率:"+ width+" x "+height);
			}
		}

        /// <summary>
        /// 是否支持描边（根据分辨率来判断）
        /// </summary>
		public static bool supportOutline()
		{
			int width = (Screen.width <= Screen.height) ? Screen.height : Screen.width;
			int height = (Screen.width <= Screen.height) ? Screen.width : Screen.height;
			return width >= 960 && height >= 540 && GameSettings.DeviceLevel != enGameRenderQuality.eLow;
		}
#endregion
    }


#region 枚举
    public enum enGameRenderQuality
    {
        eHigh,
        eMedium,
        eLow
    }

    public enum enQualityLevel
    {
        eFastest = 0,
        eFast = 1,
        eSimple = 2,
        eGood = 3,
        eBeautiful = 4,
        eFantastic = 5
    }

#endregion

}
