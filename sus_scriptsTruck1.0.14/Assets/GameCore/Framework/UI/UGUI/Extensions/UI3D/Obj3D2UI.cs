using Framework;

using Object= UnityEngine.Object;

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public enum enUI3DType
{
    eNone = 0,
    eParticle = 1,
}
public class Obj3D2UI : MonoBehaviour
{
    public const string layer = "3DUI";
    Camera mCam;
    GameObject mRootObj;
    GameObject mRenderObj;
    RawImage mRawImage;
    RenderTexture m_Texture;
    Material m_mat;
    public Vector2 pixel = new Vector2(512,512);

    public RawImage uiImage
    {
        get
        {
            return mRawImage;
        }
    }

    public GameObject renderObj
    {
        get {
            return mRenderObj;
        }
    }

    void OnEnable() 
    {

        if (mRootObj != null)
        {
            mRootObj.SetActiveEx(true);
        }
    }

    void OnDisable() 
    {

        if (mRootObj != null)
        {
            mRootObj.SetActiveEx(false);
        }
    }

    void OnDestroy()
    {
        if (mRenderObj != null)
        {
            GameObject.Destroy(mRenderObj);
            mRenderObj = null;
        }
        if (mRootObj != null)
        {
            GameObject.Destroy(mRootObj);
            mRootObj = null;
        }

#if POOL
        if (m_Texture != null)
        {
            RenderTexture.ReleaseTemporary(m_Texture);
            m_Texture = null;
        }
#else
        if (m_Texture != null)
        {
            m_Texture.Release();
            GameObject.Destroy(m_Texture);
            m_Texture = null;
        }
#endif
        if (m_mat != null)
        {
            GameObject.Destroy(m_mat);
            m_mat = null;
        }
        //LOG.Info("OnDestroy:" + this.name);
    }

    void Init()
    {
        if(mRawImage != null)
        {
            return;
        }

        mRawImage = this.GetComponent<RawImage>();

#if POOL
        m_Texture = RenderTexture.GetTemporary((int)pixel.x, (int)pixel.y, 24, RenderTextureFormat.Default, RenderTextureReadWrite.Default);
#else
        m_Texture = new RenderTexture((int)pixel.x, (int)pixel.y, 24, RenderTextureFormat.Default, RenderTextureReadWrite.Default);
#endif
        m_Texture.useMipMap = false;
        mRawImage.texture = m_Texture;

        Transform parent = Get3DUIRoot().transform;
        mRootObj = new GameObject("GameObject3d2UI");
        mRootObj.transform.SetParent(parent);
        mRootObj.transform.position = Get3d2UIObjPos();

        mCam = mRootObj.AddComponent<Camera>();
        mCam.clearFlags = CameraClearFlags.SolidColor;
        mCam.orthographic = true;
        //mCam.farClipPlane = 5;
        mCam.targetTexture = m_Texture;
        mCam.backgroundColor = Color.clear;
        mCam.cullingMask = LayerMask.GetMask(layer);
    }

    public void SetData(GameObject prefab,float ui3dCamSize, float ui3dCamFar, Vector3 pos,enUI3DType type = enUI3DType.eNone)
    {
        Init();
        switch (type)
        {
            case enUI3DType.eParticle:
                //this.mRawImage.defaultMaterial.shader = Shader.Find("UI/Default");
                m_mat = new Material(Shader.Find("Framework/UGUI/Obj3D2UI"));
                mRawImage.material = m_mat;
                //mRawImage.material.SetFloat("_UseUIAlphaClip", 1);
                break;
            default:
                mRawImage.material = null;
                break;
        }

        if (mRenderObj != null)
        {
            Destroy(mRenderObj);
        }
        mRenderObj = CreateRenderObj(prefab,ui3dCamSize, ui3dCamFar, pos);
        //mRawImage.name = mRootObj.name;
    }

    GameObject CreateRenderObj(GameObject prefab, float ui3dCamSize, float ui3dCamFar, Vector3 pos)
    {
        GameObject go = null;
        if(prefab != null)
        {
            int layerType = LayerMask.NameToLayer(layer);
            go = GameObject.Instantiate(prefab);
            go.transform.SetParent(mRootObj.transform);
            //go.transform.localScale = scale;
            mRootObj.name = go.name;
            go.transform.localPosition = pos;
            mCam.orthographicSize = ui3dCamSize;
            mCam.farClipPlane = ui3dCamFar;

            var trans = go.GetComponentsInChildren<Transform>(true);
            for(int i=0;i<trans.Length;++i) {
                trans[i].gameObject.layer = layerType;
            }
            go.SetActiveEx(true);
        }
        return go;
    }
    
    public static GameObject Get3DUIRoot() 
    {
        GameObject objRoot = GameObject.Find("3DUIRoot");
        if(objRoot == null) {
            objRoot = new GameObject("3DUIRoot");
        }
        return objRoot;
    }

    static int seqID = 0;
    public static Vector3 Get3d2UIObjPos() {
        seqID++;
        Vector3 pos = Vector3.zero;
        pos.x = 20000 + seqID * 1000;
        pos.y = 20000;
        return pos;
    }
}
