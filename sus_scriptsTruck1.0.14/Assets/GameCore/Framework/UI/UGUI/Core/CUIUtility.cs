using Object= UnityEngine.Object;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using GameLogic;
using Framework;

namespace UGUI
{
	public static class CUIUtility
	{
		public const int c_hideLayer = 31;

		public const int c_uiLayer = 5;

		public const int c_defaultLayer = 0;

		public const int c_UIBottomBg = 18;

		public const int c_formBaseWidth = 960;

		public const int c_formBaseHeight = 640;

		public const string s_Form_Battle_Dir = "UGUI/Form/Battle/";

		public const string s_Form_System_Dir = "UGUI/Form/System/";

		public const string s_Form_Common_Dir = "UGUI/Form/Common/";

		public const string s_Form_Part_Dir = "UGUI/Form/Part/";

		public const string s_Sprite_Battle_Dir = "UGUI/Sprite/Battle/";

		public const string s_Sprite_System_Dir = "UGUI/Sprite/System/";

		public const string s_Sprite_Common_Dir = "UGUI/Sprite/Common/";

		public const string s_Sprite_Dynamic_Dir = "UGUI/Sprite/Dynamic/";

		public const string s_Sprite_Dynamic_Quality_Dir = "UGUI/Sprite/Dynamic/Quality/";

		public const float c_floatEqualPrecision = 0.001f;

		public static Color[] campColor = new Color[5]
		{
			Color.red,
			new Color(0.325f, 0.411f, 0.58f),
			Color.green,
			Color.cyan,
			Color.yellow
		};

		public static string s_Form_Activity_Dir = "UGUI/Form/System/OpActivity/";

		public static string s_Form_Lobby_Dir = "UGUI/Form/System/Lobby/";

		public static string s_Sprite_Activity_Dir = "UGUI/Sprite/System/OpActivity/";

		public static string s_Sprite_HeroInfo_Dir = "UGUI/Sprite/Dynamic/HeroInfo/";

		public static string s_Sprite_Invite_Tag_Dir = "UGUI/Sprite/Dynamic/Invite/";

		public static string s_IDIP_Form_Dir = "UGUI/Form/System/IDIPNotice/";

		public static string s_Animation3D_Dir = "UGUI/Animation/";

		public static string s_Particle_Dir = "UGUI/Particle/";

		public static string s_heroSceneBgPath = "UIScene_HeroInfo";

		public static string s_heroSkinNewSceneBgPath = "UIScene_NewHeroOrSkin";

		public static string s_heroSelectBgPath = "UIScene_HeroSelect";

		public static string s_recommendHeroInfoBgPath = "UIScene_Recommend_HeroInfo";

		public static string s_lotterySceneBgPath = "UIScene_Lottery";

		public static string s_battleResultBgPath = "UIScene_BattleResult";

		public static string s_Sprite_Dynamic_Icon_Dir = "UGUI/Sprite/Dynamic/Icon/";

		public static string s_Sprite_Dynamic_BustHero_Dir = "UGUI/Sprite/Dynamic/BustHero/";

		public static string s_Sprite_Dynamic_BustCircle_Dir = "UGUI/Sprite/Dynamic/BustCircle/";

		public static string s_Sprite_Dynamic_BustCircleSmall_Dir = "UGUI/Sprite/Dynamic/BustCircleSmall/";

		public static string s_Sprite_Dynamic_BustHeroLarge_Dir = "UGUI/Sprite/Dynamic/BustHeroLarge/";

		public static string s_Sprite_Dynamic_ActivityPve_Dir = "UGUI/Sprite/Dynamic/ActivityPve/";

		public static string s_Sprite_Dynamic_FucUnlock_Dir = "UGUI/Sprite/Dynamic/FunctionUnlock/";

		public static string s_Sprite_Dynamic_Dialog_Dir = "UGUI/Sprite/Dynamic/Dialog/";

		public static string s_Sprite_Dynamic_Dialog_Dir_Head = s_Sprite_Dynamic_Dialog_Dir + "Heads/";

		public static string s_Sprite_Dynamic_Dialog_Dir_Portrait = s_Sprite_Dynamic_Dialog_Dir + "Portraits/";

		public static string s_Sprite_Dynamic_Map_Dir = "UGUI/Sprite/Dynamic/Map/";

		public static string s_Sprite_Dynamic_Talent_Dir = "UGUI/Sprite/Dynamic/Skill/";

		public static string s_Sprite_Dynamic_Adventure_Dir = "UGUI/Sprite/Dynamic/Adventure/";

		public static string s_Sprite_Dynamic_Task_Dir = "UGUI/Sprite/Dynamic/Task/";

		public static string s_Sprite_Dynamic_Skill_Dir = "UGUI/Sprite/Dynamic/Skill/";

		public static string s_Sprite_Dynamic_PvPTitle_Dir = "UGUI/Sprite/Dynamic/PvPTitle/";

		public static string s_Sprite_Dynamic_GuildHead_Dir = "UGUI/Sprite/Dynamic/GuildHead/";

		public static string s_Sprite_Dynamic_Guild_Dir = "UGUI/Sprite/Dynamic/Guild/";

		public static string s_Sprite_Dynamic_Profession_Dir = "UGUI/Sprite/Dynamic/Profession/";

		public static string s_Sprite_Dynamic_Pvp_Settle_Dir = "UGUI/Sprite/System/PvpIcon/";

		public static string s_Sprite_Dynamic_Pvp_Happy_Dir = "UGUI/Sprite/System/PvpIconHappyHouse/";

		public static string s_Sprite_Dynamic_Pvp_Settle_Large_Dir = "UGUI/Sprite/System/PvpIconLarge/";

		public static string s_Sprite_Dynamic_Purchase_Dir = "UGUI/Sprite/Dynamic/Purchase/";

		public static string s_Sprite_Dynamic_BustPlayer_Dir = "UGUI/Sprite/Dynamic/BustPlayer/";

		public static string s_Sprite_Dynamic_AddedSkill_Dir = "UGUI/Sprite/Dynamic/AddedSkill/";

		public static string s_Sprite_Dynamic_Proficiency_Dir = "UGUI/Sprite/Dynamic/HeroProficiency/";

		public static string s_Sprite_Dynamic_PvpEntry_Dir = "UGUI/Sprite/Dynamic/PvpEntry/";

		public static string s_Sprite_Dynamic_SkinQuality_Dir = "UGUI/Sprite/Dynamic/SkinQuality/";

		public static string s_Sprite_Dynamic_ExperienceCard_Dir = "UGUI/Sprite/Dynamic/ExperienceCard/";

		public static string s_Sprite_Dynamic_PvpAchievementShare_Dir = "UGUI/Sprite/Dynamic/PvpShare/";

		public static string s_Sprite_Dynamic_UnionBattleBaner_Dir = "UGUI/Sprite/Dynamic/UnionBattleBaner/";

		public static string s_Sprite_Dynamic_Nobe_Dir = "UGUI/Sprite/Dynamic/Nobe/";

		public static string s_Sprite_Dynamic_Newbie_Dir = "UGUI/Sprite/Dynamic/Newbie/";

		public static string s_Sprite_Dynamic_NewbieBannerGuide_Dir = "UGUI/Sprite/Dynamic/NewbieBannerGuide/";

		public static string s_Sprite_Dynamic_SkinFeature_Dir = "UGUI/Sprite/Dynamic/SkinFeature/";

		public static string s_Sprite_Dynamic_Signal_Dir = "UGUI/Sprite/Dynamic/Signal/";

		public static string s_Sprite_Dynamic_Mall_Dir = "UGUI/Sprite/Dynamic/Mall/";

		public static string s_Sprite_Dynamic_LBS_Dir = "UGUI/Sprite/Dynamic/LBS/";

		public static string s_Sprite_Dynamic_KillSkinView_Dir = "UGUI/Sprite/Dynamic/KillSkinView/";

		public static string s_Sprite_Dynamic_KillSkin_Dir = "UGUI/Sprite/Dynamic/KillSkin/";

		public static string s_Sprite_Dynamic_CustomCompetition_Dir = "UGUI/Sprite/Dynamic/CustomCompetition/";

		public static string s_Sprite_Dynamic_GiftSkinView_Dir = "UGUI/Sprite/Dynamic/GiftSkinView/";

		public static string s_Sprite_Dynamic_Ladder_Dir = "UGUI/Sprite/Dynamic/Ladder/";

		public static string s_Sprite_Dynamic_BranchRoad_Dir = "UGUI/Sprite/Dynamic/BranchRoad/";

		public static string s_Sprite_Dynamic_HeroHeadFrame_Dir = "UGUI/Sprite/Dynamic/HeroHeadFrame2/";

		public static string s_Sprite_Dynamic_TeamIcon_Dir = "UGUI/Sprite/Dynamic/TeamIcon_Big/";

		public static string s_Sprite_Dynamic_TeamIconBg_Dir = "UGUI/Sprite/Dynamic/TeamIconBg/";

		public static string s_Sprite_Dynamic_TeamIcon2_Dir = "UGUI/Sprite/Dynamic/TeamIcon2/";

		public static string s_Sprite_System_Equip_Dir = "UGUI/Sprite/System/Equip/";

		public static string s_Sprite_System_BattleEquip_Dir = "UGUI/Sprite/System/BattleEquip/";

		public static string s_Sprite_System_Honor_Dir = "UGUI/Sprite/System/Honor/";

		public static string s_Sprite_System_HeroSelect_Dir = "UGUI/Sprite/System/HeroSelect/";

		public static string s_Sprite_System_Qualifying_Dir = "UGUI/Sprite/System/Qualifying/";

		public static string s_Sprite_System_Burn_Dir = "UGUI/Sprite/System/BurnExpedition/";

		public static string s_Sprite_System_Mall_Dir = "UGUI/Sprite/System/Mall/";

		public static string s_Sprite_System_ShareUI_Dir = "UGUI/Sprite/System/ShareUI/";

		public static string s_Sprite_System_Lobby_Dir = "UGUI/Sprite/System/LobbyDynamic/";

		public static string s_Sprite_System_Wifi_Dir = "UGUI/Sprite/System/Wifi/";

		public static string s_Sprite_System_Settlement_Dir = "UGUI/Sprite/System/Settlement/";

		public static string s_Sprite_System_GiftCenter_Dir = "UGUI/Sprite/System/GiftCenter/";

		public static string s_Sprite_System_Guild_RedEnvelope = "UGUI/Sprite/System/Guild/";

		public static string s_battleSignalPrefabDir = "UGUI/Sprite/Battle/Signal/";

		public static string s_Form_Part_GiftSkin_Dir = "UGUI/Form/Part/GiftSkin/";

		public static string s_voiceIcon_path = s_Sprite_System_HeroSelect_Dir + "HeroSelect_btn_voice.prefab";

		public static string s_no_voiceIcon_path = s_Sprite_System_HeroSelect_Dir + "HeroSelect_btn_No_voice.prefab";

		public static string s_microphone_path = s_Sprite_System_HeroSelect_Dir + "HeroSelect_btn_Microphone.prefab";

		public static string s_no_microphone_path = s_Sprite_System_HeroSelect_Dir + "HeroSelect_btn_No_Microphone.prefab";

		public static Color s_Color_White = new Color(1f, 1f, 1f);

		public static Color s_Color_White_HalfAlpha = new Color(1f, 1f, 1f, 0.490196079f);

		public static Color s_Color_White_FullAlpha = new Color(1f, 1f, 1f, 0f);

		public static Color s_Color_Grey = new Color(0.3137255f, 0.3137255f, 0.3137255f);

		public static Color s_Color_GrayShader = new Color(0f, 1f, 1f);

		public static Color s_Color_Full = new Color(1f, 1f, 1f, 1f);

		public static Color s_Color_DisableGray = new Color(0.392156869f, 0.392156869f, 0.392156869f, 1f);

		public static Color s_Color_InCD = new Color(0.4862745f, 0.4862745f, 0.4862745f);

		public static Color s_Text_Color_White = new Color(1f, 1f, 1f);

		public static Color s_Text_Color_Disable = new Color(0.6039216f, 0.6f, 0.6f);

		public static Color s_Text_Color_Vip_Chat_Self = new Color(1f, 0.894117653f, 0f);

		public static Color s_Text_Color_Vip_Chat_Other = new Color(0.7764706f, 0.6509804f, 0.3137255f);

		public static Color Intimacy_Full = new Color(1f, 0.09411765f, 0f);

		public static Color Intimacy_High = new Color(1f, 0.09411765f, 1f);

		public static Color Intimacy_Mid = new Color(1f, 0.521568656f, 0.13333334f);

		public static Color Intimacy_Low = new Color(1f, 0.8745098f, 0.180392161f);

		public static Color Intimacy_Freeze = new Color(0.807843149f, 0.8117647f, 0.882352948f);

		public static Color s_Text_Color_Self = new Color(0.9490196f, 0.7882353f, 0.3019608f);

		public static Color s_Text_Color_Camp_1 = new Color(0.403921574f, 0.6039216f, 0.968627453f);

		public static Color s_Text_Color_Camp_2 = new Color(0.858823538f, 0.180392161f, 0.282352954f);

		public static Color s_Text_Color_CommonGray = new Color(0.7019608f, 0.7058824f, 0.7137255f);

		public static Color s_Text_Color_MyHeroName = new Color(0.8784314f, 0.7294118f, 0.13333334f);

		public static Color s_Text_OutLineColor_MyHeroName = new Color(0.117647059f, 0.08627451f, 0.0235294122f);

		public static Color s_Text_Color_Self_WuJun = new Color(1f, 1f, 1f);

		public static Color s_Text_Color_Other_WuJun = new Color(0.333333343f, 0.482352942f, 0.6313726f);

		public static Color s_Text_Color_ListElement_Normal = new Color(0.494117647f, 0.533333361f, 0.635294139f);

		public static Color s_Text_Color_ListElement_Select = new Color(1f, 1f, 1f);

		public static Color s_Text_Color_Hero_Name_Active = new Color(0.9254902f, 0.8509804f, 0.6431373f);

		public static Color s_Text_Color_Hero_Name_DeActive = new Color(0.4f, 0.3647059f, 0.270588249f);

		public static Color s_Text_Color_Camp_Allies = new Color(0.3529412f, 0.549019635f, 0.8352941f);

		public static Color s_Text_Color_Camp_Enemy = new Color(0.6862745f, 0.160784319f, 0.235294119f);

		public static Color s_Text_Color_Blue = new Color(0.345098048f, 0.56078434f, 0.9019608f);

		public static Color s_Text_Color_Green = new Color(0.117647059f, 0.9137255f, 0.545098066f);

		public static Color s_Text_Color_Settle_FightScore_Up = new Color(0.2901961f, 0.9254902f, 0.396078438f);

		public static Color s_Text_Color_Settle_FightScore_Donw = new Color(0.945098042f, 0.258823544f, 0.258823544f);

		public static Color s_Color_Button_Disable = new Color(0.384313732f, 0.384313732f, 0.384313732f, 0.9019608f);

		public static Color s_Color_BraveScore_BaojiKedu_On = new Color(0f, 1f, 0.607843161f, 1f);

		public static Color s_Color_BraveScore_BaojiKedu_Off = new Color(0f, 0.7921569f, 1f, 1f);

		public static Color s_Color_EnemyHero_Button_PINK = new Color(1f, 0.5647059f, 0.5647059f);

		public static Color s_Color_Battery_Low = new Color(0.854901969f, 0.258823544f, 0.215686277f);

		public static Color s_Color_Battery_Mid = new Color(1f, 0.7372549f, 0.09019608f);

		public static Color s_Color_Battery_High = new Color(0.09803922f, 0.670588255f, 0.403921574f);

		public static Color[] s_Text_Color_Hero_Advance = new Color[5]
		{
			new Color(1f, 1f, 1f),
			new Color(0.3882353f, 0.9019608f, 0.239215687f),
			new Color(0.117647059f, 0.6431373f, 0.9137255f),
			new Color(0.7647059f, 0.3372549f, 0.8235294f),
			new Color(0.9490196f, 0.466666669f, 0.07058824f)
		};

		public static string s_iconRes_Camp1 = "UGUI/Sprite/Dynamic/TeamIcon/01";

		public static string s_iconRes_Camp2 = "UGUI/Sprite/Dynamic/TeamIcon/02";

		public static string s_iconRes_Camp3 = "UGUI/Sprite/Dynamic/TeamIcon/03";

		public static string s_iconRes_Camp4 = "UGUI/Sprite/Dynamic/TeamIcon/04";

		public static string s_iconRes_Camp5 = "UGUI/Sprite/Dynamic/TeamIcon/05";

		public static string[] s_WuJunSelf_CircleIcon = new string[5]
		{
			s_Sprite_Dynamic_BustCircleSmall_Dir + "Img_Map_WuJunSelf_CircleIcon01",
			s_Sprite_Dynamic_BustCircleSmall_Dir + "Img_Map_WuJunSelf_CircleIcon02",
			s_Sprite_Dynamic_BustCircleSmall_Dir + "Img_Map_WuJunSelf_CircleIcon03",
			s_Sprite_Dynamic_BustCircleSmall_Dir + "Img_Map_WuJunSelf_CircleIcon04",
			s_Sprite_Dynamic_BustCircleSmall_Dir + "Img_Map_WuJunSelf_CircleIcon05"
		};

		public static Color s_Text_Color_Can_Afford = Color.white;

		public static Color s_Text_Color_Can_Not_Afford = new Color(0.6784314f, 0.227450982f, 0.203921571f);

		private static readonly Regex s_regexEmoji = new Regex("[\\u0000-\\u0019]|\\ud83c[\\udf00-\\udfff]|\\ud83d[\\udc00-\\udeff]|\\ud83d[\\ude80-\\udeff]|\\u007c|<[^>]*>");

		public static List<char> s_replaceCharList = new List<char>();

		public static Font s_gameFontHaHei = null;

		public static string s_gameFontHaHeiPath = "UGUI/Font/gameFontYaHei";

		public static string s_ui_defaultShaderName = "Sprites/Default";

		public static string s_ui_graySpritePath = s_Sprite_Dynamic_BustHero_Dir + "gray";

		public static string[] s_materianlParsKey = new string[6]
		{
			"_StencilComp",
			"_Stencil",
			"_StencilOp",
			"_StencilWriteMask",
			"_StencilReadMask",
			"_ColorMask"
		};

		public static Vector3 s_vector3_One = new Vector3(1f, 1f, 1f);



		public static Color[] s_Text_Skill_HurtType_Color = new Color[4]
		{
			new Color(0.8156863f, 0.423529416f, 0.423529416f),
			new Color(0.5882353f, 0.596078455f, 0.9607843f),
			new Color(0.9372549f, 0.75686276f, 0.211764708f),
			new Color(0.5882353f, 0.9607843f, 0.8352941f)
		};

		public static Color[] s_Text_SkillName_And_HurtValue_Color = new Color[4]
		{
			new Color(0.831372559f, 0.623529434f, 0.623529434f),
			new Color(0.6039216f, 0.607843161f, 0.7882353f),
			new Color(0.796078444f, 0.7372549f, 0.5686275f),
			new Color(0.545098066f, 0.698039234f, 0.670588255f)
		};

		public static Color[] s_Text_Total_Damage_Text_Color = new Color[2]
		{
			new Color(0.933333337f, 0.8862745f, 0.6627451f),
			new Color(0.5058824f, 0.5647059f, 0.8862745f)
		};

		public static Color[] s_Text_Total_Damage_Value_Color = new Color[2]
		{
			new Color(0.933333337f, 0.8784314f, 0.7882353f),
			new Color(0.7882353f, 0.858823538f, 0.933333337f)
		};

		public static Color[] s_Text_Total_Damage_Text_Outline_Color = new Color[2]
		{
			new Color(0.227450982f, 0.109803922f, 0.07058824f),
			new Color(0.07058824f, 0.109803922f, 0.227450982f)
		};
        
        
		public static Vector2 GetFixedTextSize(Text text, string content, float fixedWidth)
		{
			return Vector2.zero;
		}

		public static T GetComponentInChildren<T>(GameObject go) where T : Component
		{
			if (go == null)
			{
				return (T)null;
			}
			T component = go.GetComponent<T>();
			if (component != null)
			{
				return component;
			}
			Transform transform = go.transform;
			int childCount = transform.childCount;
			for (int i = 0; i < childCount; i++)
			{
				component = GetComponentInChildren<T>(transform.GetChild(i).gameObject);
				if (component != null)
				{
					return component;
				}
			}
			return (T)null;
		}

		public static void GetComponentsInChildren<T>(GameObject go, T[] components, ref int count) where T : Component
		{
			T component = go.GetComponent<T>();
			if (component != null)
			{
				components[count] = component;
				count++;
			}
			for (int i = 0; i < go.transform.childCount; i++)
			{
				GetComponentsInChildren(go.transform.GetChild(i).gameObject, components, ref count);
			}
		}

		public static string StringReplace(string scrStr, params string[] values)
		{
			return string.Format(scrStr, values);
		}

		public static Vector3 ScreenToWorldPoint(Camera camera, Vector2 screenPoint, float z)
		{
			return (!(camera == null)) ? camera.ScreenToWorldPoint(new Vector3(screenPoint.x, screenPoint.y, z)) : new Vector3(screenPoint.x, screenPoint.y, z);
		}

		public static Vector2 WorldToScreenPoint(Camera camera, Vector3 worldPoint)
		{
			return (!(camera == null)) ? ((Vector2)camera.WorldToScreenPoint(worldPoint)) : new Vector2(worldPoint.x, worldPoint.y);
		}

		public static void SetGameObjectLayer(GameObject gameObject, int layer)
		{
			if (gameObject != null)
			{
				gameObject.layer = layer;
				for (int i = 0; i < gameObject.transform.childCount; i++)
				{
					SetGameObjectLayer(gameObject.transform.GetChild(i).gameObject, layer);
				}
			}
		}

		public static float ValueInRange(float value, float min, float max)
		{
			if (value < min)
			{
				return min;
			}
			if (value > max)
			{
				return max;
			}
			return value;
		}

		public static void ResetUIScale(GameObject target)
		{
			Vector3 localScale = target.transform.localScale;
			Transform parent = target.transform.parent;
			target.transform.SetParent(null);
			target.transform.localScale = localScale;
			target.transform.SetParent(parent);
		}

		public static string RemoveEmoji(string str)
		{
			return s_regexEmoji.Replace(str, string.Empty);
		}
        

		public static float[] GetMaterailMaskPars(Material tarMat)
		{
			float[] array = new float[s_materianlParsKey.Length];
			for (int i = 0; i < s_materianlParsKey.Length; i++)
			{
				array[i] = tarMat.GetFloat(s_materianlParsKey[i]);
			}
			return array;
		}

		public static void SetMaterailMaskPars(float[] pars, Material tarMat)
		{
			for (int i = 0; i < s_materianlParsKey.Length; i++)
			{
				tarMat.SetFloat(s_materianlParsKey[i], pars[i]);
			}
		}

		public static void SetImageSprite(Image image, GameObject prefab, bool isShowSpecMatrial = false)
		{
			if (!(image == null))
			{
				if (prefab == null)
				{
					image.sprite = null;
				}
				else
				{
					SpriteRenderer component = prefab.GetComponent<SpriteRenderer>();
					if (component != null)
					{
						image.sprite = component.sprite;
						if (isShowSpecMatrial && component.sharedMaterial != null && component.sharedMaterial.shader != null && !component.sharedMaterial.shader.name.Equals(s_ui_defaultShaderName))
						{
							float[] materailMaskPars = GetMaterailMaskPars(((MaskableGraphic)image).material);
							((MaskableGraphic)image).material = new Material(component.sharedMaterial) ;
							((MaskableGraphic)image).material.shaderKeywords = component.sharedMaterial.shaderKeywords;
							SetMaterailMaskPars(materailMaskPars, ((MaskableGraphic)image).material);
						}
						else if (isShowSpecMatrial)
						{
							((MaskableGraphic)image).material = (Material)null ;
						}
					}
					
				}
			}
		}

		public static void SetImageSprite(Image image, string prefabPath, CUIForm formScript, bool loadSync = true, bool needCached = false, bool unloadBelongedAssetBundleAfterLoaded = false, bool isShowSpecMatrial = false)
		{
			if (!(image == null))
			{
				if (loadSync)
				{
					//SetImageSprite(image, GetSpritePrefeb(prefabPath, needCached, unloadBelongedAssetBundleAfterLoaded), isShowSpecMatrial);
				}
				else if (formScript != null)
				{
					//formScript.AddASyncLoadedImage(image, prefabPath, needCached, unloadBelongedAssetBundleAfterLoaded, isShowSpecMatrial);
				}
			}
		}

		public static void SetGenderImageSprite(Image image, byte bGender)
		{
			switch (bGender)
			{
			case 1:
				SetImageSprite(image, string.Format("{0}icon/Ico_boy.prefab", "UGUI/Sprite/Dynamic/"), null, true, false, false, false);
				break;
			case 2:
				SetImageSprite(image, string.Format("{0}icon/Ico_girl.prefab", "UGUI/Sprite/Dynamic/"), null, true, false, false, false);
				break;
			}
		}
        

		public static void SetImageGrey(Graphic graphic, bool isSetGrey)
		{
			SetImageGrey(graphic, isSetGrey, Color.white);
		}

		private static void SetImageGrey(Graphic graphic, bool isSetGrey, Color defaultColor)
		{
			if (graphic != null)
			{
				graphic.color = ((!isSetGrey) ? defaultColor : s_Color_Grey);
			}
		}

		public static void SetImageGrayMatrial(Image image)
		{
            GameObject spritePrefeb = null;// GetSpritePrefeb(s_ui_graySpritePath, false, false);
			SpriteRenderer component = spritePrefeb.GetComponent<SpriteRenderer>();
			if (component != null)
			{
				float[] materailMaskPars = GetMaterailMaskPars(((MaskableGraphic)image).material);
				((MaskableGraphic)image).material = new Material(component.sharedMaterial) ;
				((MaskableGraphic)image).material.shaderKeywords = component.sharedMaterial.shaderKeywords;
				SetMaterailMaskPars(materailMaskPars, ((MaskableGraphic)image).material);
			}
		}

		public static CUIForm GetFormScript(Transform transform)
		{
			if (transform == null)
			{
				return null;
			}
			CUIForm component = transform.gameObject.GetComponent<CUIForm>();
			if (component != null)
			{
				return component;
			}
			return GetFormScript(transform.parent);
		}
        
        

		public static void SetIntValueToPlayerPrefeb(string key, int value)
		{
			if (GetIntValueToPlayerPrefeb(key) != value)
			{
				PlayerPrefs.SetInt(key, value);
				PlayerPrefs.Save();
			}
		}

		public static int GetIntValueToPlayerPrefeb(string key)
		{
			int result = 0;
			if (PlayerPrefs.HasKey(key))
			{
				result = PlayerPrefs.GetInt(key);
			}
			return result;
		}

		public static void SetStringValueToPlayerPrefeb(string key, string value)
		{
			if (GetStringValueToPlayerPrefeb(key) != value)
			{
				PlayerPrefs.SetString(key, value);
				PlayerPrefs.Save();
			}
		}

		public static string GetStringValueToPlayerPrefeb(string key)
		{
			string result = string.Empty;
			if (PlayerPrefs.HasKey(key))
			{
				result = PlayerPrefs.GetString(key);
			}
			return result;
		}

		public static bool Approximately(float a, float b)
		{
			return Math.Abs(a - b) < 0.001f;
		}

		public static void ActiveChilds(Transform fatherRoot, bool isActive)
		{
			if (!(fatherRoot == null))
			{
				for (int i = 0; i < fatherRoot.childCount; i++)
				{
					fatherRoot.GetChild(i).gameObject.SetActiveEx(isActive);
				}
			}
		}

		public static void SetObjImage(Transform targetTrans, string imgPath, CUIForm form)
		{
			if (!(targetTrans == null))
			{
				Image component = ((Component)targetTrans).GetComponent<Image>();
				if (component != null)
				{
					SetImageSprite(component, imgPath, form, true, false, false, false);
				}
			}
		}

        #region UGUI坐标转换
        public static Vector3 World_To_UGUI_WorldPoint(Camera mainCam, Camera uiCam, Vector3 worldPos, RectTransform canvasTrans)
        {
            if (mainCam == null) return worldPos;
            Vector3 screenPoint = mainCam.WorldToScreenPoint(worldPos);
            return Screen_To_UGUI_WorldPoint(uiCam, screenPoint, canvasTrans);
        }

        public static Vector3 Screen_To_UGUI_WorldPoint(Camera uiCam, Vector3 screenPoint, RectTransform canvasTrans)
        {
            //if (uiCam == null) return screenPoint;
            Vector3 worldPoint;
            if (!RectTransformUtility.ScreenPointToWorldPointInRectangle(canvasTrans, screenPoint, uiCam, out worldPoint))
            {
                worldPoint = Vector3.zero;
                LOG.Error("ScreenToUGUIWorldPoint转换失败");
            }
            return worldPoint;
        }

        public static Vector2 World_To_UGUI_LocalPoint(Camera mainCam, Camera uiCam, Vector3 worldPos, RectTransform canvasTrans)
        {
            if (mainCam == null) return worldPos;
            Vector2 screenPoint = mainCam.WorldToScreenPoint(worldPos);
            return Screen_To_UGUI_LocalPoint(uiCam, screenPoint, canvasTrans);
        }

        public static Vector2 Screen_To_UGUI_LocalPoint(Camera uiCam, Vector3 screenPoint, RectTransform canvasTrans)
        {
            //if (uiCam == null) return screenPoint;
            Vector2 pos;
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasTrans, screenPoint, uiCam, out pos))
            {
                pos = Vector2.zero;
                LOG.Error("ScreenToUGUILocalPoint转换失败");
            }
            return pos;
        }
        #endregion



        public static float GetPreferredHeight(this Text uiText, float width)
        {
            //uiText.UpdateTextProcessor();
            //Debug.LogError("------"+width);
            return uiText.cachedTextGeneratorForLayout.GetPreferredHeight(
                uiText.text,
                //GetGenerationSettings(new Vector2(this.rectTransform.rect.size.x, 0f))
                uiText.GetGenerationSettings(new Vector2(width, 0.0f))
            ) / uiText.pixelsPerUnit;
        }

    }
}
