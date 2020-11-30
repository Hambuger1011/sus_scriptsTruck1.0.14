using Framework;

using UnityEngine;
using GameLogic;

namespace UGUI
{
	public class CUIParticleScript : CUIComponent
	{
		public string m_resPath = string.Empty;

		public bool m_isFixScaleToForm;

		public bool m_isFixScaleToParticleSystem;

		public bool m_isEndlessPlay = true;

		private Renderer[] m_renderers;

		private int m_rendererCount;

		public override void Initialize(CUIForm formScript)
		{
			if (!base.m_isInitialized)
			{
				LoadRes();
				InitializeRenderers();
				base.Initialize(formScript);
				if (m_isFixScaleToForm)
				{
					ResetScale();
				}
				if (m_isFixScaleToParticleSystem)
				{
					ResetParticleScale();
				}
				if (base.myForm.IsHided())
				{
					OnHide();
				}
				base.m_isInitialized = true;
			}
		}

		public override void UnInitialize()
		{
			if (base.m_isInitialized)
			{
				m_resPath = null;
				m_renderers = null;
				base.UnInitialize();
				base.m_isInitialized = false;
			}
		}

		protected override void OnDestroy()
		{
			UnInitialize();
			base.OnDestroy();
		}

		public override void OnHide()
		{
			base.OnHide();
			if (m_isEndlessPlay)
			{
				CUIUtility.SetGameObjectLayer(base.gameObject, 31);
			}
			else
			{
				base.gameObject.SetActiveEx(false);
			}
		}

		public override void OnAppear()
		{
			base.OnAppear();
			if (m_isEndlessPlay)
			{
				CUIUtility.SetGameObjectLayer(base.gameObject, 5);
			}
			else
			{
				base.gameObject.SetActiveEx(true);
			}
		}

		public override void SetSortingOrder(int sortingOrder)
		{
			base.SetSortingOrder(sortingOrder);
			if (m_renderers != null)
			{
				for (int i = 0; i < m_rendererCount; i++)
				{
					m_renderers[i].sortingOrder = sortingOrder;
				}
			}
		}

		private void InitializeRenderers()
		{
			m_renderers = new Renderer[100];
			m_rendererCount = 0;
			CUIUtility.GetComponentsInChildren(base.gameObject, m_renderers, ref m_rendererCount);
		}

		private void ResetScale()
		{
			Vector3 localScale = base.myForm.gameObject.transform.localScale;
			float num = 1f / localScale.x;
			base.gameObject.transform.localScale = new Vector3(num, num, 0f);
		}

		private void ResetParticleScale()
		{
			float scale;
			if (!((Object)base.myForm == (Object)null))
			{
				scale = 1f;
				RectTransform component = ((Component)base.myForm).GetComponent<RectTransform>();
				if (base.myForm.canvasScaler.matchWidthOrHeight == 0f)
				{
					float num = component.rect.width / component.rect.height;
					Vector2 referenceResolution = base.myForm.canvasScaler.referenceResolution;
					float x = referenceResolution.x;
					Vector2 referenceResolution2 = base.myForm.canvasScaler.referenceResolution;
					scale = num / (x / referenceResolution2.y);
				}
				else if (base.myForm.canvasScaler.matchWidthOrHeight != 1f)
				{
					goto IL_00af;
				}
				goto IL_00af;
			}
			return;
			IL_00af:
			InitializeParticleScaler(base.gameObject, scale);
		}

		private void InitializeParticleScaler(GameObject gameObject, float scale)
		{
			//ParticleScaler particleScaler = gameObject.GetComponent<ParticleScaler>();
			//if ((Object)particleScaler == (Object)null)
			//{
			//	particleScaler = gameObject.AddComponent<ParticleScaler>();
			//}
			//if (particleScaler.particleScale != scale)
			//{
			//	particleScaler.particleScale = scale;
			//	particleScaler.alsoScaleGameobject = false;
			//	particleScaler.CheckAndApplyScale();
			//}
		}

		private void ClearRes()
		{
			m_renderers = null;
			m_rendererCount = 0;
			if (base.gameObject.transform.childCount > 0)
			{
				Transform child = base.gameObject.transform.GetChild(0);
				if ((Object)child != (Object)null)
				{
					child.SetParent(null);
					Object.Destroy(child.gameObject);
				}
			}
		}

		private void LoadRes()
		{
			string resPath = m_resPath;
			//if (!string.IsNullOrEmpty(resPath))
			//{
			//	resPath = ((GameSettings.ParticleQuality != SGameRenderQuality.Low) ? ((GameSettings.ParticleQuality != SGameRenderQuality.Medium) ? (CUIUtility.s_Particle_Dir + m_resPath + "/" + m_resPath + ".prefeb") : (CUIUtility.s_Particle_Dir + m_resPath + "/" + m_resPath + "_mid.prefeb")) : (CUIUtility.s_Particle_Dir + m_resPath + "/" + m_resPath + "_low.prefeb"));
			//	GameObject gameObject = Singleton<CResourceManager>.GetInstance().GetResource(resPath, typeof(GameObject), enResourceType.UIPrefab, false, false).m_content as GameObject;
			//	if ((Object)gameObject != (Object)null && base.gameObject.transform.childCount == 0)
			//	{
			//		GameObject gameObject2 = Object.Instantiate((Object)gameObject) as GameObject;
			//		gameObject2.transform.SetParent(base.gameObject.transform);
			//		gameObject2.transform.localPosition = Vector3.zero;
			//		gameObject2.transform.localRotation = Quaternion.identity;
			//		gameObject2.transform.localScale = Vector3.one;
			//	}
			//}
		}

		public void LoadRes(string resName)
		{
			if (base.m_isInitialized)
			{
				m_resPath = resName;
				ClearRes();
				LoadRes();
				InitializeRenderers();
				SetSortingOrder(base.myForm.GetSortingOrder());
				if (m_isFixScaleToForm)
				{
					ResetScale();
				}
				if (m_isFixScaleToParticleSystem)
				{
					ResetParticleScale();
				}
				if (base.myForm.IsHided())
				{
					OnHide();
				}
			}
		}
	}
}
