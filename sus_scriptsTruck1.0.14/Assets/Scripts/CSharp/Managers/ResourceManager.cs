using AB;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : Singleton<ResourceManager>
{
    protected override void Init()
    {
    }

    #region Obsolete
    public GameObject Load(string path, Transform parents = null, bool needInstant = true)
    {
        GameObject go = Resources.Load<GameObject>(path);
        if (go == null) { LOG.Error(string.Format("Resources Load Error : path is '{0}'", path)); return go; }
        else
        {
            if (needInstant)
                go = GameObject.Instantiate<GameObject>(go, parents);
            return go;
        }
    }

    public const string UITexturePath = "Assets/Bundle/UI/UITexture/";
    public Sprite GetUITexture(string strSpriteName)
    {
        return ABSystem.ui.GetUITexture(AbTag.Global,string.Concat(UITexturePath, strSpriteName,".png"));
    }
    
    public Sprite GetUISprite(string strSpriteName)
    {
        return ABSystem.ui.GetAtlasSprite(strSpriteName);
    }



    private static string AudioTonesPath = "Assets/Bundle/Music/";
    public AudioClip GetAudioTones(string path)
    {
        var asset = ABSystem.ui.bundle(AbResBundle_DataTable.IsDataTableAsset(string.Concat(AudioTonesPath, path))).LoadImme(AbTag.Global, enResType.eAudio, string.Concat(AudioTonesPath, path));
        return asset.resAudioClip;
    }

    //private static string AudioBGMPath = "Music/BGM/";
    //public AudioClip GetAudioBGM(string path)
    //{
    //    return Resources.Load<AudioClip>(AudioBGMPath + path);
    //}


    #endregion

    public GameObject LoadResourcesUI(string path, ResourceLoadType resType = ResourceLoadType.AssetBundle)
    {
        GameObject prefab = Resources.Load<GameObject>(path);
        if(prefab == null)
        {
            LOG.Error(resType+",don't find prefab:" + path);
            return null;
        }
        GameObject go = GameObject.Instantiate(prefab);
        return go;
    }

    public GameObject LoadAssetBundleUI(string path, ResourceLoadType resType = ResourceLoadType.AssetBundle)
    {
        GameObject prefab = ABSystem.ui.GetGameObject(AbTag.Global, path);
        if (prefab == null)
        {
            LOG.Error(resType + ",don't find prefab:" + path);
            return null;
        }
        GameObject go = GameObject.Instantiate(prefab);
        return go;
    }
}

public enum ResourceLoadType
{
    RecesourceLoad,
    AssetBundle,
}