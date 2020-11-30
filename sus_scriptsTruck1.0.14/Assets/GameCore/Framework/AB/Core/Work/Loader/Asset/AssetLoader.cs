using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AB
{
    public enum enResType : int
    {
        eInvalid = 0,
        eObject,
        ePrefab,
        eAudio,
        eText,
        eTexture2D,
        eSprite,
        eFont,
        eScriptableObject,
        eAtlas,
        eScene,
        eShader,
        eMaterial,
        eMax,
    }

    public abstract class AssetLoader
    {
        public readonly static Type[] resType = new Type[]
        {
            typeof(Object),
            typeof(GameObject),
            typeof(AudioClip),
            typeof(TextAsset),
            typeof(Texture2D),
            typeof(Sprite),
            typeof(Font),
            typeof(ScriptableObject),
            typeof(Object),
            typeof(Object),
            typeof(Shader),
            typeof(Material),
        };

        protected CAsset asset;
        public AssetLoader(CAsset asset) { this.asset = asset; }
        public abstract int GetAllSize();
        public abstract int GetCurSize();
        public abstract float GetProgress();
        public abstract bool Load(enResType resType, string assetName, bool isAsync,ref Object obj);
    }
}
