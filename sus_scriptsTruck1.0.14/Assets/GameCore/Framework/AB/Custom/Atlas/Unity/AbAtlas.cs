#if !TP
namespace AB
{
#if UNITY_EDITOR
    using UnityEditor;
    using UnityEditor.Experimental.SceneManagement;
    using UnityEditor.SceneManagement;
#endif
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using System.Linq;
    using System.IO;

    [DisallowMultipleComponent]
    public class AbAtlas : MonoBehaviour
    {
        [Header("Sheet图片"), SerializeField]
        public List<Sprite> spriteSheet = new List<Sprite>();

        Dictionary<string, Sprite> mSheetDict = null;
#if false
        [SerializeField]
        public Texture2D _mainTex;
        [SerializeField]
        public Texture2D _alphaTex;

        [NonSerialized]
        private Material m_material;

        [NonSerialized]
        private Material m_greyMaterial;

        public Material specialMaterial;


        public static Shader shader
        {
            get
            {
                return Shader.Find("Framework/UGUI/Image");
            }
        }

#region 材质
        public Material material
        {
            get
            {
                if (this.m_material == null)
                {
                    if (this.specialMaterial != null)
                    {
                        this.m_material = this.specialMaterial;
                    }

                    if (this._alphaTex != null)//有alpha分离
                    {
                        if (this.m_material == null)
                        {
                            this.m_material = new Material(shader);
                            this.m_material.SetTexture("_MainTex", this._mainTex);
                            this.m_material.SetTexture("_AlphaTex", this._alphaTex);
                            this.m_material.EnableKeyword("_SEPERATE_ALPHA_TEX_ON");
                        }
                    }
                    else//没有alpha分离
                    {
                        m_material = null;
                    }
                    //if (AbAtlasMgr.HasInstance())
                    {
                        AbAtlasMgr.Instance.Add(_mainTex, this);
                    }
                }
                return this.m_material;
            }
        }

        public Material greyMaterial
        {
            get
            {
                if (this.m_greyMaterial == null)
                {
                    if (this.m_material != null)
                    {
                        this.m_greyMaterial = new Material(m_material);
                        this.m_greyMaterial.SetFloat("_IsGrey", 1);
                    }
                    else//没有alpha分离
                    {
                        m_greyMaterial = AbAtlasMgr.Instance.defaultGreyMat;
                    }
                }
                return this.m_greyMaterial;
            }
        }


        Material m_sprite3DMaterial;
        public Material sprite3DMaterial
        {
            get
            {

                if (m_sprite3DMaterial == null)
                {
                    this.m_sprite3DMaterial = new Material(Shader.Find("Framework/UI3D"/*"Sprites/Default"*/));
                    this.m_sprite3DMaterial.SetTexture("_MainTex", this._mainTex);
                    if (this._alphaTex != null)
                    {
                        this.m_sprite3DMaterial.SetTexture("_AlphaTex", this._alphaTex);
                        this.m_sprite3DMaterial.EnableKeyword("ETC1_EXTERNAL_ALPHA");
                    }
                }
                return m_sprite3DMaterial;
            }
        }
#endregion
#else
        //public Texture2D _mainTex
        //{
        //    get
        //    {
        //        if(spriteSheet.Count == 0)
        //        {
        //            return null;
        //        }
        //        return spriteSheet[0].texture;
        //    }
        //}
#endif


        public Sprite this[string sheetName]
        {
            get
            {
                Sprite spt = GetSprite(sheetName);
                if (spt == null)
                {
                    LOG.Error("找不到图片:" + sheetName + ",图集:" + this.name);
                }
                return spt;
            }
        }

        public Sprite GetSprite(string sheetName)
        {

            if (mSheetDict == null)
            {
                mSheetDict = new Dictionary<string, Sprite>();
                foreach (var s in spriteSheet)
                {
                    if (s == null)
                    {
                        LOG.Error("图集里有sprite丢失，请更新：" + this.name);
                        continue;
                    }
                    string sptName = s.name; ;
                    if (!mSheetDict.ContainsKey(sptName))
                    {
                        mSheetDict.Add(sptName, s);
                    }
                }
            }
            Sprite spt;
            if (!mSheetDict.TryGetValue(sheetName, out spt))
            {
                //Debug.LogError("找不到图片:" + sheetName);
            }
            return spt;
        }

        public void SetSprite(Image uiImage, string spriteName, bool applyMaterial = true, bool NativeSize = false)
        {
            uiImage.sprite = this[spriteName];
#if false
            if (uiImage.sprite != null)
            {
                if (applyMaterial)
                {
                    uiImage.material = this.material;
                }
            }
            else
            {
                uiImage.material = null;
            }
#endif
            if (NativeSize)
            {
                uiImage.SetNativeSize();
            }
        }


        public void SetSprite(SpriteRenderer sptRenderer, string sptName, bool applyMaterial = true)
        {
            sptRenderer.sprite = this[sptName];
#if false
            sptRenderer.material = this.sprite3DMaterial;
#endif
        }


        public void Release()
        {
#if false
            if (m_sprite3DMaterial != null)
            {
                UnityEngine.Object.Destroy(m_sprite3DMaterial);
                m_sprite3DMaterial = null;
            }

            if (m_greyMaterial != null && m_greyMaterial != AbAtlasMgr.Instance.defaultGreyMat)
            {
                UnityEngine.Object.Destroy(m_greyMaterial);
                m_greyMaterial = null;
            }

            if (this.m_material != null && m_material != this.specialMaterial)
            {
                UnityEngine.Object.Destroy(m_material);
                m_material = null;
            }
#endif
        }

        public UVDetail GetUV(string atlasName)
        {
            return UVDetail.Create(this[atlasName]);
        }

        #region 图集制作

#if UNITY_EDITOR



        [MenuItem("Assets/GameTools/Atlas/设置图集tag")]
        static void SetAtlasTag()
        {
            try
            {
                int p = 0;
                var objs = Selection.GetFiltered(typeof(UnityEditor.DefaultAsset), SelectionMode.Assets);
                foreach (var obj in objs)
                {
                    //Debug.LogError(obj.name + " " + obj.GetType());
                    var assetPath = AssetDatabase.GetAssetPath(obj);
                    ++p;
                    if (EditorUtility.DisplayCancelableProgressBar(string.Format("扫描{0}({1}/{2})", obj.name, p, objs.Length), assetPath, (float)p / objs.Length))
                    {
                        throw new Exception("用户停止");
                    }

                    var fileGUIDs = UnityEditor.AssetDatabase.FindAssets(string.Format("t:{0}", typeof(Sprite).Name), new[] { assetPath });
                    Debug.LogError(fileGUIDs.Length);
                    foreach (string guid in fileGUIDs)
                    {
                        var file = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                        TextureImporter tp = (TextureImporter)TextureImporter.GetAtPath(file);
                        tp.spritePackingTag = obj.name;
                        AssetDatabase.WriteImportSettingsIfDirty(file);
                    }
                }
            }
            finally
            {
                EditorUtility.ClearProgressBar();
                //AssetDatabase.Refresh();
                //AssetDatabase.SaveAssets();
            }
        }

        //[ContextMenu("更新sprites")]
        //public void UpdateSprites()
        //{
        //    var file = AssetDatabase.GetAssetPath(_mainTex);
        //    Sprite[] sprites = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(file).OfType<Sprite>().ToArray();
        //    this.spriteSheet.Clear();
        //    foreach (var spt in sprites)
        //    {
        //        this.spriteSheet.Add(spt);
        //    }

        //    EditorUtility.SetDirty(this.gameObject);
        //}

        [ContextMenu("更新sprites")]
        public void UpdateSprites()
        {
            var prefabPath = AssetDatabase.GetAssetPath(this.gameObject);
            var newPrefab = PrefabUtility.InstantiatePrefab(this.gameObject) as GameObject;
            Sprite oneOfAtlas = null;
            foreach (var spt in spriteSheet)
            {
                if (spt == null)
                {
                    continue;
                }
                oneOfAtlas = spt;
                break;
            }
            var file = AssetDatabase.GetAssetPath(oneOfAtlas);
            var dir = Path.GetDirectoryName(file);
            var fileGUIDs = UnityEditor.AssetDatabase.FindAssets(string.Format("t:{0}", typeof(Sprite).Name), new[] { dir });
            if (fileGUIDs.Length == 0)
            {
                return;
            }
            Debug.LogError(fileGUIDs.Length);
            this.spriteSheet.Clear();
            foreach (string guid in fileGUIDs)
            {
                var filename = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                var spt = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>(filename);
                this.spriteSheet.Add(spt);
            }


            bool isSuccess;
            PrefabUtility.SaveAsPrefabAsset(newPrefab, prefabPath, out isSuccess);
            var prefabStage = PrefabStageUtility.GetPrefabStage(newPrefab);
            if (prefabStage != null)
            {
                EditorSceneManager.MarkSceneDirty(prefabStage.scene);
            }
            GameObject.DestroyImmediate(newPrefab);
        }

        /// <summary>
        /// 创建
        /// </summary>
        [MenuItem("Assets/GameTools/Atlas/制作Atlas")]
        static void CreateAtlas()
        {
            try
            {
                int p = 0;
                var objs = Selection.GetFiltered(typeof(UnityEditor.DefaultAsset), SelectionMode.Assets);
                foreach (var obj in objs)
                {
                    var assetPath = AssetDatabase.GetAssetPath(obj);
                    ++p;
                    if (EditorUtility.DisplayCancelableProgressBar(string.Format("扫描{0}({1}/{2})", obj.name, p, objs.Length), assetPath, (float)p / objs.Length))
                    {
                        throw new Exception("用户停止");
                    }
                    //Debug.LogError(obj.name + " " + obj.GetType());
                    MakeAtlas(obj.name, assetPath, "Assets/Resources/UI/Atlas/");
                }
            }
            finally
            {
                EditorUtility.ClearProgressBar();
                //AssetDatabase.Refresh();
                //AssetDatabase.SaveAssets();
            }
        }

        static void MakeAtlas(string atlasName, string assetPath, string outpath)
        {
            Directory.CreateDirectory(outpath);
            string prefabPath = string.Format("{0}{1}.prefab", outpath, atlasName);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            if (prefab == null)
            {
#if true
                GameObject tmpGo = new GameObject(atlasName);
                tmpGo.AddComponent<AbAtlas>();
                prefab = PrefabUtility.CreatePrefab(prefabPath, tmpGo, ReplacePrefabOptions.Default);
                GameObject.DestroyImmediate(tmpGo);
#else
                PrefabUtility.CreateEmptyPrefab(prefabPath);
#endif
                AssetDatabase.Refresh();
                prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            }
            var newPrefab = PrefabUtility.InstantiatePrefab(prefab) as GameObject;


            var data = newPrefab.GetComponent<AbAtlas>();
            data.spriteSheet.Clear();
            //data._mainTex = AssetDatabase.LoadAssetAtPath<Texture2D>(tools.mainTexAssetPath);
            //if (tools.enalbeAlpha)
            //{
            //    data._alphaTex = AssetDatabase.LoadAssetAtPath<Texture2D>(tools.alphaTexAssetPath);
            //    data.specialMaterial = AssetDatabase.LoadAssetAtPath<Material>(tools.materialAssetPath);
            //}
            //else
            //{
            //    data._alphaTex = null;
            //    data.specialMaterial = null;
            //}

            //if (!data.md5.Equals(texMd5))
            {
#if false
                Sprite[] sprites = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(assetPath).OfType<Sprite>().ToArray();
                foreach (var spt in sprites)
                {
                    data.spriteSheet.Add(spt);
                }
#else

                var fileGUIDs = UnityEditor.AssetDatabase.FindAssets(string.Format("t:{0}", typeof(Sprite).Name), new[] { assetPath });
                Debug.LogError(fileGUIDs.Length);
                foreach (string guid in fileGUIDs)
                {
                    var file = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                    var spt = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>(file);
                    data.spriteSheet.Add(spt);
                }
#endif
                //EditorUtility.SetDirty(go);
                bool isSuccess;
                PrefabUtility.SaveAsPrefabAsset(newPrefab, prefabPath, out isSuccess);
                var prefabStage = PrefabStageUtility.GetPrefabStage(newPrefab);
                if (prefabStage != null)
                {
                    EditorSceneManager.MarkSceneDirty(prefabStage.scene);
                }
                GameObject.DestroyImmediate(newPrefab);
            }
            //Sprite[] sprites = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(assetPath).OfType<Sprite>().ToArray();
            //Debug.LogError(sprites.Length);
        }
#endif
#endregion
            }

#if UNITY_EDITOR

    [CustomEditor(typeof(AbAtlas), true)]
    public class AbAtlas_Editor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            GUILayout.Space(50);
            if (GUILayout.Button("更新图集", GUILayout.Height(25)))
            {
                var s = (AbAtlas)this.target;
                //s.UpdateSprites();
            }
        }
    }
#endif
}



#endif