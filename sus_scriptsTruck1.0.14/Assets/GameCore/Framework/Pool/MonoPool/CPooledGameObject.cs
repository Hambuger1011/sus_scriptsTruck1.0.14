using Framework;
using Object = UnityEngine.Object;
using System;
using UnityEngine;
using AB;

public class CPooledGameObject
{

    #region Component
    [NonSerialized]
    public GameObject gameObject;

    [NonSerialized]
    public Transform transform;

    [NonSerialized]
    public ParticleSystem[] Particles;

    [NonSerialized]
    public MeshRenderer[] MeshRenderers;

    //[NonSerialized]
    //public ParticleScaler[] ParticleScalers;

    [NonSerialized]
    public Animation legacyAnim;

    [NonSerialized]
    public Animator mecAnim;

    [NonSerialized]
    public SkinnedMeshRenderer[] SkinnedRenderers;

    [NonSerialized]
    public LineRenderer[] LineRenderers;
    #endregion

    /// <summary>
    /// pool key
    /// </summary>
    [NonSerialized]
    public string m_prefabKey;

    [NonSerialized]
    public bool m_isInit;

    /// <summary>
    /// 缩放参数
    /// </summary>
    [NonSerialized]
    public Vector3 m_defaultScale;

    [NonSerialized]
    private IPooledMonoBehaviour[] m_cachedIPooledMonos;

    [NonSerialized]
    private bool m_inUse;

    [NonSerialized]
    public ParticleInitState[] ParticleInitStates;

    [NonSerialized]
    public CAsset bundle;

    public void Initialize(string prefabKey, GameObject go, CAsset asset)
    {
        #region 查找IPooledMonoBehaviour
        MonoBehaviour[] componentsInChildren = go.GetComponentsInChildren<MonoBehaviour>(true);
        if (componentsInChildren.Length > 0)
        {
            int num = 0;
            for (int i = 0; i < componentsInChildren.Length; i++)
            {
                if (componentsInChildren[i] is IPooledMonoBehaviour)
                {
                    num++;
                }
            }
            m_cachedIPooledMonos = new IPooledMonoBehaviour[num];
            int idx = 0;
            for (int i = 0; i < componentsInChildren.Length; i++)
            {
                if (componentsInChildren[i] is IPooledMonoBehaviour)
                {
                    m_cachedIPooledMonos[idx] = (componentsInChildren[i] as IPooledMonoBehaviour);
                    idx++;
                }
            }
        }
        else
        {
            m_cachedIPooledMonos = new IPooledMonoBehaviour[0];
        }
        #endregion

        this.bundle = asset;
        this.bundle.Retain(this);
        m_prefabKey = prefabKey;
        m_defaultScale = go.transform.localScale;
        m_isInit = true;
        m_inUse = false;
        gameObject = go;
        transform = go.transform;

        #region Components
        Particles = go.GetComponentsInChildren<ParticleSystem>(true);
        MeshRenderers = go.GetComponentsInChildren<MeshRenderer>(true);
        //ParticleScalers = go.GetComponentsInChildren<ParticleScaler>(true);
        legacyAnim = go.GetComponent<Animation>();
        mecAnim = go.GetComponent<Animator>();
        SkinnedRenderers = go.GetComponentsInChildren<SkinnedMeshRenderer>(true);
        LineRenderers = go.GetComponentsInChildren<LineRenderer>(true);
        #endregion

        ParticleInitStates = new ParticleInitState[Particles.Length];
        if (Particles.Length != 0)
        {
            for (int i = 0; i < Particles.Length; i++)
            {
                ParticleInitStates[i].EmmitState = Particles[i].enableEmission;
                ParticleInitStates[i].StartSize = Particles[i].startSize;
                ParticleInitStates[i].StartLifeTime = Particles[i].startLifetime;
                ParticleInitStates[i].StartSpeed = Particles[i].startSpeed;
                ParticleInitStates[i].Trans = Particles[i].transform;
                ParticleInitStates[i].LocalScale = ParticleInitStates[i].Trans.localScale;
            }
        }
    }

    public void UnInitialize()
    {
        if (gameObject != null)
        {
            GameObject.Destroy(gameObject);
            this.gameObject = null;
            this.transform = null;
        }

        if (bundle != null)
        {
            bundle.Release(this);
            bundle = null;
        }
    }

    #region 播放、停止特效
    public void PlayAllParticles()
    {
        if (Particles != null)
        {
            for (int i = 0; i < Particles.Length; i++)
            {
                Particles[i].Play(false);
            }
        }
    }

    public void StopAllParticles()
    {
        if (Particles != null)
        {
            for (int i = 0; i < Particles.Length; i++)
            {
                Particles[i].Stop(false);
            }
        }
    }
    #endregion

    /// <summary>
    /// 不是tag的gameObject全隐藏
    /// </summary>
    public void HideGameObjWithNoTag(string tag)
    {
        //if (gameObject != null)
        //{
        //    int layer = LayerMask.NameToLayer(TagManager.Layer.Hide);
        //    SetGameObjLayerRecursively(gameObject, layer, tag);
        //}
    }

    /// <summary>
    /// 设置层
    /// </summary>
    public void SetGameObjLayerRecursively(GameObject go, int layer, string tag)
    {
        if (!go.CompareTag(tag))
        {
            go.layer = layer;
        }
        Transform transform = go.transform;
        int childCount = transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            SetGameObjLayerRecursively(transform.GetChild(i).gameObject, layer, tag);
        }
    }


    public void AddCachedMono(MonoBehaviour mono, bool defaultEnabled)
    {
        if (mono != null && mono is IPooledMonoBehaviour)
        {
            IPooledMonoBehaviour[] array = new IPooledMonoBehaviour[m_cachedIPooledMonos.Length + 1];
            for (int i = 0; i < m_cachedIPooledMonos.Length; i++)
            {
                array[i] = m_cachedIPooledMonos[i];
            }
            array[m_cachedIPooledMonos.Length] = (mono as IPooledMonoBehaviour);
            m_cachedIPooledMonos = array;
        }
    }

    #region 复用回调
    public void OnCreate()
    {
        if (m_cachedIPooledMonos != null && m_cachedIPooledMonos.Length > 0)
        {
            for (int i = 0; i < m_cachedIPooledMonos.Length; i++)
            {
                if (m_cachedIPooledMonos[i] != null)
                {
                    m_cachedIPooledMonos[i].OnCreate();
                }
            }
        }
    }

    public void OnGet()
    {
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }
        if (m_cachedIPooledMonos != null && m_cachedIPooledMonos.Length > 0)
        {
            for (int i = 0; i < m_cachedIPooledMonos.Length; i++)
            {
                if (m_cachedIPooledMonos[i] != null)
                {
                    m_cachedIPooledMonos[i].OnGet();
                }
            }
        }
        m_inUse = true;
    }

    public void OnRecycle()
    {
        if (m_cachedIPooledMonos != null && m_cachedIPooledMonos.Length > 0)
        {
            for (int i = 0; i < m_cachedIPooledMonos.Length; i++)
            {
                if (m_cachedIPooledMonos[i] != null)
                {
                    m_cachedIPooledMonos[i].OnRecycle();
                }
            }
        }
        if (gameObject != null)
        {
            gameObject.SetActive(false);
        }
        m_inUse = false;
    }
    #endregion

    public void OnPrepare()
    {
        gameObject.SetActive(false);
    }

    public IPooledMonoBehaviour GetCachedMonobehaviourByType(Type type, bool canBeSubClass = true)
    {
        for (int i = 0; i < m_cachedIPooledMonos.Length; i++)
        {
            if (m_cachedIPooledMonos[i] != null && (m_cachedIPooledMonos[i].GetType() == type || (canBeSubClass && m_cachedIPooledMonos[i].GetType().IsSubclassOf(type))))
            {
                return m_cachedIPooledMonos[i];
            }
        }
        return null;
    }

    /// <summary>
    /// 特效参数
    /// </summary>
    public struct ParticleInitState
    {
        public bool EmmitState;

        public float StartSize;

        public float StartLifeTime;

        public float StartSpeed;

        public Vector3 LocalScale;

        public Transform Trans;
    }
}
