using Object= UnityEngine.Object;



namespace Framework
{
    using Framework;
    using Framework;
    using System;
    using UnityEngine;
    using Object = UnityEngine.Object;
    using Common;
    using UGUI;
    using AB;

    public class CUIParticleScript : CUIComponent
	{
        [Header("1.特效prefab（可选）")]
        public GameObject prefabEffect;

        [Header("2.特效路径（可选）")]
		public string m_resPath = string.Empty;

		public bool m_isFixScaleToForm;

		public bool m_isFixScaleToParticleSystem;

        CAsset asset;

        public int addSortingOrder = 0;



        //public override void Initialize(CUIFormScript formScript)
        //{
        //	if (this.m_isInitialized)
        //	{
        //		return;
        //	}
        //	this.LoadRes();
        //	this.InitializeRenderers();
        //	base.Initialize(formScript);
        //	if (this.m_isFixScaleToForm)
        //	{
        //		this.ResetScale();
        //	}
        //	if (this.m_isFixScaleToParticleSystem)
        //	{
        //		this.ResetParticleScale();
        //	}
        //	if (this.myFormScript != null && this.myFormScript.IsHided())
        //	{
        //		this.OnHide();
        //	}
        //}

        //protected override void OnDestroy()
        //{
        //	this.m_renderers = null;
        //          if(asset.handle != null)
        //          {
        //              asset.handle.Release(this);
        //          }
        //	base.OnDestroy();
        //      }
        //      public override void OnAppear()
        //      {
        //          base.OnAppear();
        //          //CUIUtility.SetGameObjectLayer(base.gameObject, LayerMask.NameToLayer("UI"));
        //      }

        //      public override void OnHide()
        //{
        //          if(this.IsDestroyed())
        //          {
        //              return;
        //          }
        //	base.OnHide();
        //	//CUIUtility.SetGameObjectLayer(base.gameObject, LayerMask.NameToLayer("Hidden"));
        //}


        //public override void SetSortingOrder(int sortingOrder)
        //{
        //          LOG.Info("gameobject.name:"+gameObject.name+"    sortOrder:"+sortingOrder);
        //	base.SetSortingOrder(sortingOrder);
        //	for (int i = 0; i < this.m_rendererCount; i++)
        //	{
        //              if (this.m_renderers[i] != null)
        //              {
        //                  this.m_renderers[i].sortingOrder = sortingOrder + addSortingOrder;
        //              }
        //	}
        //}

        //private void InitializeRenderers()
        //{
        //	this.m_renderers = new Renderer[100];
        //	this.m_rendererCount = 0;
        //	CUIUtility.GetComponentsInChildren<Renderer>(base.gameObject, this.m_renderers, ref this.m_rendererCount);
        //}

        //private void ResetScale()
        //{
        //	float num = 1f / this.myFormScript.t.localScale.x;
        //	base.t.localScale = (new Vector3(num, num, 0f));
        //}

        //private void ResetParticleScale()
        //{
        //	if (this.myFormScript == null)
        //	{
        //		return;
        //	}
        //	float scale = 1f;
        //	RectTransform component = this.myFormScript.GetComponent<RectTransform>();
        //	if (this.myFormScript.canvasScaler.matchWidthOrHeight == 0f)
        //	{
        //		scale = component.rect.width / component.rect.height / (this.myFormScript.canvasScaler.referenceResolution.x / this.myFormScript.canvasScaler.referenceResolution.y);
        //	}
        //	else if (this.myFormScript.canvasScaler.matchWidthOrHeight == 1f)
        //	{
        //	}
        //	this.InitializeParticleScaler(base.gameObject, scale);
        //}

        //private void InitializeParticleScaler(GameObject gameObject, float scale)
        //{
        //	ParticleScaler particleScaler = gameObject.GetComponent<ParticleScaler>();
        //	if (particleScaler == null)
        //	{
        //		particleScaler = gameObject.AddComponent<ParticleScaler>();
        //	}
        //	if (particleScaler.particleScale != scale)
        //	{
        //		particleScaler.particleScale = scale;
        //		particleScaler.alsoScaleGameobject = false;
        //		particleScaler.CheckAndApplyScale();
        //	}
        //}

        public override void OnOpen()
        {
            base.OnOpen();
            LoadRes();
        }

        GameObject m_fxObj;
        private void LoadRes()
        {
            if(m_fxObj != null)
            {
                return;
            }
            string text = this.m_resPath;
            if (prefabEffect != null)
            {
                m_fxObj = GameObject.Instantiate<GameObject>(prefabEffect);
                if (m_fxObj != null)// && base.t.childCount == 0)
                {
                    //uiParticleSystem = m_fxObj.GetComponent<ParticleSystem>();
                    var t = m_fxObj.transform;
                    t.SetParent(this.transform);
                    t.localPosition = Vector3.zero;
                    t.localRotation = Quaternion.identity;
                    t.localScale = Vector3.one;
                }
            }
            else if (!string.IsNullOrEmpty(text))
            {
                //if (GameSettings.ParticleQuality == SGameRenderQuality.Low)
                //{
                //	text = string.Concat(new string[]
                //	{
                //		CUIUtility.s_Particle_Dir,
                //		this.m_resPath,
                //		"/",
                //		this.m_resPath,
                //		"_low.prefeb"
                //	});
                //}
                //else if (GameSettings.ParticleQuality == SGameRenderQuality.Medium)
                //{
                //	text = string.Concat(new string[]
                //	{
                //		CUIUtility.s_Particle_Dir,
                //		this.m_resPath,
                //		"/",
                //		this.m_resPath,
                //		"_mid.prefeb"
                //	});
                //}
                //else
                //{
                //	text = string.Concat(new string[]
                //	{
                //		CUIUtility.s_Particle_Dir,
                //		this.m_resPath,
                //		"/",
                //		this.m_resPath,
                //		".prefeb"
                //	});
                //}
                asset = ABSystem.ui.bundle(AbResBundle_DataTable.IsDataTableAsset(text)).LoadImme(AbTag.Null, enResType.ePrefab,text);
                if (asset != null)
                {
                    asset.Retain(this);
                    m_fxObj = asset.Instantiate();
                    if (gameObject != null)// && base.t.childCount == 0)
                    {
                        //uiParticleSystem = gameObject.GetComponent<ParticleSystem>();
                        var t = m_fxObj.transform;
                        t.SetParent(this.transform);
                        t.localPosition = Vector3.zero;
                        t.localRotation = Quaternion.identity;
                        t.localScale = Vector3.one;
                    }
                }
            }

            if(m_fxObj != null)
            {
                var uiDepth = this.GetComponent<UIDepth>();
                if(uiDepth == null)
                {
                    uiDepth = this.gameObject.AddComponent<UIDepth>();
                }
                uiDepth.order = this.addSortingOrder;
                uiDepth.ResetOrder();
            }
        }

        //void Start()
        //{
        //    IniteData();
        //}

        //public void IniteData()
        //{
        //    if (beloneCanvas != string.Empty)
        //    {
        //        CUIFormScript form = CUIManager.Instance.GetForm(beloneCanvas);
        //        CUIParticleScript particleScript = GetComponent<CUIParticleScript>();
        //        if (particleScript != null && form!= null)
        //        {
        //            particleScript.Initialize(form);
        //            particleScript.SetSortingOrder(form.GetSortingOrder() + particleScript.addSortingOrder);
        //        }
        //    }
        //}


        public void LoadRes(string resName)
        {
            if (!this.m_isInitialized)
            {
                return;
            }
            this.m_resPath = resName;
            this.LoadRes();
            //this.InitializeRenderers();
            this.SetSortingOrder(this.myForm.GetSortingOrder());
            if (this.m_isFixScaleToForm)
            {
                //this.ResetScale();
            }
            if (this.m_isFixScaleToParticleSystem)
            {
                //this.ResetParticleScale();
            }
            if (this.myForm.IsHided())
            {
                this.OnHide();
            }
        }

        [ContextMenu("初始化特效")]
        public void InitPS()
        {
            this.m_isInitialized = false;
            myForm = this.GetComponentInParent<CUIForm>();
            Initialize(this.myForm);
        }



        [ContextMenu("SetSortingOrder")]
        public void SetSortingOrder()
        {
            //InitializeRenderers();
            Canvas canvas = this.GetComponentInParent<Canvas>();
            int sortingOrder = canvas.sortingOrder;
            SetSortingOrder(sortingOrder);
        }
    }
}
