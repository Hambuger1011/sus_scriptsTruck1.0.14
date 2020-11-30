namespace AB
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;
    using System.IO;
    using System;
    using System.Reflection;
    using System.Text;

    public static partial class ABEditor
    {

        //[MenuItem("GameTools/AssetBundle/一键生成ab", false, MenuPriority.AB + 100)]
        //public static void OnKeyGen()
        //{
        //    Analyze();
        //    Build();
        //}

        //[MenuItem("GameTools/AssetBundle/分析资源", false, MenuPriority.AB + 101)]
        //static void Analyze()
        //{
        //    try
        //    {
        //        var options = new AbOptions_UI();
        //        AbBuilder uiBuilder = new AbBuilder(options);
        //        uiBuilder.Begin();
        //        uiBuilder.Analyze();

        //        var shaders = uiBuilder.analyzer.config.abMap[AbAnalyze.shader_ab];
        //        AddShaderVariants(shaders);
        //        uiBuilder.End();
        //    }
        //    finally
        //    {
        //        EditorUtility.ClearProgressBar();
        //    }
        //}

        //[MenuItem("GameTools/AssetBundle/生成ab", false, MenuPriority.AB + 100)]
        //public static void Build()
        //{
        //    try
        //    {
        //        AbBuilder uiBuilder = new AbBuilder(new AbOptions_UI());
        //        uiBuilder.Begin();
        //        //PackageAtlasRes(uiBuilder);

        //        //Debug.Log("打AB的路径：" + AbUtility.AbBuildPath + "UI/");
        //        uiBuilder.Build();
        //        uiBuilder.End();
        //    }
        //    finally
        //    {
        //        EditorUtility.ClearProgressBar();
        //    }
        //}


        //[MenuItem("GameTools/AssetBundle/提取ab", false, MenuPriority.AB + 100)]
        //public static void CopyAB()
        //{
        //    try
        //    {
        //        AbBuilder uiBuilder = new AbBuilder(new AbOptions_UI());
        //        //uiBuilder.Begin();
        //        uiBuilder.abOptions.CopyAbFiles();
        //        //uiBuilder.End();
        //    }
        //    finally
        //    {
        //        EditorUtility.ClearProgressBar();
        //    }
        //}



        [MenuItem("GameTools/AssetBundle/清除缓存", false, MenuPriority.AB + 1000)]
        static void ClearCache()
        {
#if UNITY_2017_1_OR_NEWER
            Caching.ClearCache();
#else
            Caching.CleanCache();
#endif
            PlayerPrefs.DeleteAll();
        }



        public static void AddShaderVariants(List<string> useShader)
        {
            HashSet<Shader> shaders = new HashSet<Shader>();
            foreach (var itr in useShader)
            {
                shaders.Add(AssetDatabase.LoadAssetAtPath<Shader>(itr));
            }
            AddShaderVariants(shaders);
        }

        static void AddShaderVariants(HashSet<Shader> useShader)
        {
            var t = Type.GetType("UnityEditor.ShaderUtil,UnityEditor.dll");
            var method = t.GetMethod("AddNewShaderToCollection", BindingFlags.NonPublic | BindingFlags.Static);
            if (method == null)
            {
                Debug.LogError("找不到方法");
                foreach (var m in t.GetMethods(BindingFlags.NonPublic | BindingFlags.Static))
                {
                    Debug.Log(m.Name);
                }
                return;
            }
            //method.Invoke(null, new object[] { mat.shader, true });//Shader shader, ShaderVariantCollection collection

            Directory.CreateDirectory("Assets/Bundle/Shaders/");
            var path = "Assets/Bundle/Shaders/ShaderVariants.shaderVariants";
            var shaderVariant = AssetDatabase.LoadAssetAtPath<ShaderVariantCollection>(path);
            if (shaderVariant == null)
            {
                shaderVariant = new ShaderVariantCollection();
                AssetDatabase.CreateAsset(shaderVariant, path);
                //shaderVariant = AssetDatabase.LoadAssetAtPath<ShaderVariantCollection>(path);
            }

            //HashSet<Shader> useShader = new HashSet<Shader>();
            #region 打印shader
            StringBuilder sb = new StringBuilder();
            SerializedObject shaderVariantSerializedObj = new SerializedObject(shaderVariant);
            var m_Shaders = shaderVariantSerializedObj.FindProperty("m_Shaders");
            Debug.Log("shader count = " + m_Shaders.arraySize + "/" + shaderVariant.shaderCount);
            for (int i = 0; i < m_Shaders.arraySize; i++)
            {
                SerializedProperty arrayElementAtIndex = m_Shaders.GetArrayElementAtIndex(i);
                Shader shader = (Shader)arrayElementAtIndex.FindPropertyRelative("first").objectReferenceValue;
                if (shader == null)
                {
                    LOG.Error("shader已经不存在:" + arrayElementAtIndex.FindPropertyRelative("first").displayName);
                    continue;
                }
                //SerializedProperty serializedProperty = arrayElementAtIndex.FindPropertyRelative("second.variants");
                //Debug.Log(shader);
                sb.AppendLine(string.Format("\t\t\t\t\"{0}\",", shader.name));
                useShader.Add(shader);
            }
            Debug.Log(sb);
            #endregion

            //t = AbstractABBuild.GetBuildType();
            //var shaderMethod = t.GetMethod("GetShaderVariantCollection", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
            //HashSet<string> addShaderSet = (HashSet<string>)shaderMethod.Invoke(null, null);//获取MyABBuilder的shader
            foreach (var shader in useShader)
            {
                if (shader.name == "Standard")
                {
                    Debug.LogError("跳过Standard");
                    continue;
                }
                method.Invoke(null, new object[] { shader, shaderVariant });//Shader shader, ShaderVariantCollection collection
            }
            Debug.Log("shader count = " + shaderVariant.shaderCount);
            SerializedObject graphicsSettings = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/GraphicsSettings.asset")[0]);
            var m_PreloadedShaders = graphicsSettings.FindProperty("m_PreloadedShaders");
            //for(int i=0,iMax = m_PreloadedShaders.arraySize; i < iMax; ++i)
            //{
            //    var item = m_PreloadedShaders.GetArrayElementAtIndex(i);
            //    LOG.Error(item.objectReferenceValue);
            //}
            if (m_PreloadedShaders.arraySize == 0)
            {
                m_PreloadedShaders.arraySize = 1;
            }
            m_PreloadedShaders.GetArrayElementAtIndex(0).objectReferenceValue = shaderVariant;
#if false
            shaderVariant.Clear();
            SerializedProperty it = graphicsSettings.GetIterator();
            SerializedProperty dataPoint;
            while (it.NextVisible(true))
            {
                if (it.name == "m_AlwaysIncludedShaders")
                {
                    //it.ClearArray();
                    for(int i=0;i<it.arraySize;++i)
                    {
                        dataPoint = it.GetArrayElementAtIndex(i);
                        Shader shader = (Shader)dataPoint.objectReferenceValue;
#if false
                        ShaderVariantCollection.ShaderVariant variant = new ShaderVariantCollection.ShaderVariant(shader, PassType.Deferred);
                        shaderVariant.Add(variant);
#else
                        method.Invoke(null, new object[] { shader , shaderVariant});//Shader shader, ShaderVariantCollection collection
#endif
                    }
                }
            }
            EditorUtility.SetDirty(shaderVariant);
#endif
        }


#if false
        [MenuItem("GameTools/AbManager/Command/设置AlwaysIncludedShaders", false, MenuPriority.AB + 600)]
        static void AlwaysIncludedShaders()
        {
            List<string> myShaders = new List<string>
            {
                
        "UGUI/Obj3D2UI",
        "Hidden/CubeBlur",
        "Hidden/CubeCopy",
        "Hidden/CubeBlend",
        "UI/Default",
        "UI/Default Font",
        "UI/DefaultSeprateAlpha_Grey",
        "UI/UIGrey",


//1.自定义shader:assets/shaders/sepratealpha/ui-defaultsepratealpha.shader
"UI/DefaultSeprateAlpha",
//2.自定义shader:assets/bundle/shaders/builtinshaders/defaultresourcesextra/particle add.shader
"Particles/Additive",
//3.自定义shader:assets/bundle/shaders/builtinshaders/defaultresourcesextra/particle alpha blend.shader
"Particles/Alpha Blended",
//4.自定义shader:assets/bundle/shaders/builtinshaders/defaultresourcesextra/flare.shader
"FX/Flare",
//5.自定义shader:assets/bundle/shaders/builtinshaders/defaultresourcesextra/mobile/mobile-particle-add.shader
"Mobile/Particles/Additive",
//6.自定义shader:assets/shaders/environment/madfinger-unlit-scroll2layers-mul-sine.shader
"MADFINGER/Environment/Scroll 2 Layers Multiplicative No Lightmap Sine)",
//7.自定义shader:assets/shaders/rfx4/shaders/rfx4_particle.shader
"KriptoFX/RFX4/Particle",
//8.自定义shader:assets/shaders/rfx4/shaders/rfx4_particle_ztest.shader
"KriptoFX/RFX4/Particle_ZTest",
//9.自定义shader:assets/bundle/shaders/builtinshaders/defaultresourcesextra/normal-vertexlit.shader
"Legacy Shaders/VertexLit",
//10.自定义shader:assets/shaders/tgp_basic_rim.shader
"Toony Colors Pro/TGP_Basic_Rim",
//11.自定义shader:assets/bundle/shaders/builtinshaders/defaultresourcesextra/alpha-parallax.shader
"Legacy Shaders/Transparent/Parallax Diffuse",
//12.自定义shader:assets/bundle/shaders/builtinshaders/defaultresourcesextra/alpha-bumped.shader
"Legacy Shaders/Transparent/Bumped Diffuse",
//13.自定义shader:assets/bundle/shaders/builtinshaders/defaultresourcesextra/alpha-diffuse.shader
"Legacy Shaders/Transparent/Diffuse",
//14.自定义shader:assets/bundle/shaders/builtinshaders/defaultresourcesextra/alpha-vertexlit.shader
"Legacy Shaders/Transparent/VertexLit",
//15.自定义shader:assets/z_temp/shaders/environment/madfinger-unlit-scroll2layers-mul-skybox.shader
"MADFINGER/Environment/Scroll 2 Layers Multiplicative - Skybox",
//16.自定义shader:assets/bundle/shaders/builtinshaders/defaultresourcesextra/mobile/mobile-particle-alpha.shader
"Mobile/Particles/Alpha Blended",
//17.自定义shader:assets/resources/prefabs/greyscale.shader
"RGB -> Grey Texture/Alpha Blended",
//18.自定义shader:assets/shaders/tgp_outline.shader
"Hidden/ToonyColors-Outline",
//19.自定义shader:assets/shaders/tgp_outline_basic_onelight.shader
"Toony Colors Pro/Outline/OneDirLight/Basic",
//20.自定义shader:assets/shaders/tgp_outline_basic_rim_onelight.shader
"Toony Colors Pro/Outline/OneDirLight/Basic Rim",
//21.自定义shader:assets/bundle/shaders/builtinshaders/defaultresourcesextra/unlit/unlit-normal.shader
"Unlit/Texture",
//22.自定义shader:assets/bundle/shaders/builtinshaders/defaultresourcesextra/mobile/mobile-diffuse.shader
"Mobile/Diffuse",
//23.自定义shader:assets/bundle/shaders/builtinshaders/defaultresourcesextra/standard.shader
//"Standard",
//24.自定义shader:assets/bundle/shaders/builtinshaders/defaultresources/internal-errorshader.shader
//"Hidden/InternalErrorShader",
//25.自定义shader:assets/bundle/shaders/builtinshaders/defaultresourcesextra/normal-diffuse.shader
"Legacy Shaders/Diffuse",
//26.自定义shader:assets/jmo assets/toony colors pro/shaders 2.0/include/tcp2_outline_sm2.shader
"Hidden/Toony Colors Pro 2/Outline Only (Shader Model 2)",
//27.自定义shader:assets/jmo assets/toony colors pro/shaders 2.0/variants/tcp2_mobile_specular_rimoutline_outline.shader
"Hidden/Toony Colors Pro 2/Variants/Mobile Specular RimOutline Outline",
//28.自定义shader:assets/jmo assets/toony colors pro/shaders 2.0/variants/tcp2_mobile_rim_outline.shader
"Hidden/Toony Colors Pro 2/Variants/Mobile Rim Outline",
//29.自定义shader:assets/jmo assets/toony colors pro/shaders 2.0/variants base/tcp2_mobile_specular_outline.shader
"Hidden/Toony Colors Pro 2/Variants/Mobile Specular Outline",
//30.自定义shader:assets/bundle/shaders/builtinshaders/defaultresourcesextra/alphatest-softedgeunlit.shader
"Legacy Shaders/Transparent/Cutout/Soft Edge Unlit",
//31.自定义shader:assets/jmo assets/toony colors pro/shaders 2.0/variants/tcp2_desktop_rim_outline.shader
"Hidden/Toony Colors Pro 2/Variants/Desktop Rim Outline",
//32.自定义shader:assets/jmo assets/toony colors pro/shaders 2.0/include/tcp2_outline.shader
"Hidden/Toony Colors Pro 2/Outline Only",
//33.自定义shader:assets/jmo assets/toony colors pro/shaders 2.0/variants base/tcp2_mobile_outline.shader
"Hidden/Toony Colors Pro 2/Variants/Mobile Outline",
//34.自定义shader:assets/shaders/backgroundandcharacters/interlacepatternadditive.shader
"Self-Illumin/AngryBots/InterlacePatternAdditive",
//35.自定义shader:assets/shaders/projectormultiply.shader
"Projector/Multiply",
//36.自定义shader:assets/scripts/battle/actor/weapon/new/cweapon_207/skillhintbg.shader
"Custom/SkillHintBg",
//37.自定义shader:assets/bundle/shaders/builtinshaders/defaultresourcesextra/particle addsmooth.shader
"Particles/Additive (Soft)",
//38.自定义shader:assets/shaders/backgroundandcharacters/realtimereflectioninwaterflow.shader
"AngryBots/RealtimeReflectionInWaterFlow",
//39.自定义shader:assets/shaders/backgroundandcharacters/fallback.shader
"AngryBots/Fallback",
//40.自定义shader:assets/shaders/alphablendicon.shader
"Icon/Alpha Blended",
//41.自定义shader:assets/shaders/drdcfireball.shader
"Custom/DRDCFIREBALL",
//42.自定义shader:assets/shaders/rfx4/shaders/rfx4_mobilediffuse.shader
"KriptoFX/RFX4/DiffuseMobile",
//43.自定义shader:assets/bundle/shaders/builtinshaders/defaultresourcesextra/mobile/mobile-vertexlit.shader
"Mobile/VertexLit",
//44.自定义shader:assets/shaders/environment/madfinger-animtexture.shader
"MADFINGER/FX/Anim texture",
//45.自定义shader:assets/shaders/drdc.shader
"Custom/DRDC",
//46.自定义shader:assets/shaders/uiouline.shader
"Custom/UIOULINE",
//47.自定义shader:assets/bundle/shaders/builtinshaders/defaultresourcesextra/mobile/mobile-particle-multiply.shader
"Mobile/Particles/Multiply",
//48.自定义shader:assets/jmo assets/toony colors pro/shaders 2.0/variants/tcp2_desktop_specular_rimoutline_outline.shader
"Hidden/Toony Colors Pro 2/Variants/Desktop Specular RimOutline Outline",
//49.自定义shader:assets/standard assets/effects/toonshading/shaders/toonbasic.shader
"Toon/Basic",
//50.自定义shader:assets/bundle/shaders/builtinshaders/defaultresourcesextra/alphatest-diffuse.shader
"Legacy Shaders/Transparent/Cutout/Diffuse",
//51.自定义shader:assets/bundle/shaders/builtinshaders/defaultresourcesextra/alphatest-vertexlit.shader
"Legacy Shaders/Transparent/Cutout/VertexLit",
//52.自定义shader:assets/z_temp/shaders/environment/madfinger-skybox.shader
"MADFINGER/Environment/Skybox - opaque - no fog",
//53.自定义shader:assets/bundle/shaders/builtinshaders/defaultresourcesextra/unlit/unlit-color.shader
"Unlit/Color",
//54.自定义shader:assets/vuforia/shaders/videobackground.shader
"Custom/VideoBackground",
//============================内置shader============================
//1.内置shader:library/unity default resources
"GUI/Text Shader",
//2.内置shader:resources/unity_builtin_extra
"Particles/Alpha Blended Premultiply",
//3.内置shader:resources/unity_builtin_extra
"Legacy Shaders/Diffuse",
//4.内置shader:resources/unity_builtin_extra
"Sprites/Default",
//5.内置shader:resources/unity_builtin_extra
//"Standard",


        };

            for (int i = myShaders.Count - 1; i >= 0; --i)
            {
                var shaderName = myShaders[i];
                if (Shader.Find(shaderName) == null)
                {
                    myShaders.RemoveAt(i);
                }
            }

            SerializedObject graphicsSettings = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/GraphicsSettings.asset")[0]);
            SerializedProperty it = graphicsSettings.GetIterator();
            SerializedProperty dataPoint;
            while (it.NextVisible(true))
            {
                if (it.name == "m_AlwaysIncludedShaders")
                {
                    it.ClearArray();

                    for (int i = 0; i < myShaders.Count; i++)
                    {
                        it.InsertArrayElementAtIndex(i);
                        dataPoint = it.GetArrayElementAtIndex(i);
                        dataPoint.objectReferenceValue = Shader.Find(myShaders[i]);
                    }

                    graphicsSettings.ApplyModifiedProperties();
                }
            }
        }
#endif



        //static void PackageAtlasRes(AbBuilder uiBuilder)
        //{
        //    try
        //    {
        //        var atlasDirs = Directory.GetDirectories("assets/UIAtlsaCache", "*.*", SearchOption.TopDirectoryOnly);
        //        foreach (var path in atlasDirs)
        //        {
        //            AbPackage package = new AbPackage();
        //            package.abType = enResType.eAtlas;
        //            var fileGUIDs = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(Texture2D).Name), new[] { path });
        //            //Debug.LogError(fileGUIDs.Length);
        //            int p = 0;
        //            foreach (string guid in fileGUIDs)
        //            {
        //                var file = AssetDatabase.GUIDToAssetPath(guid);
        //                ++p;
        //                string assetName = AbUtility.NormalizerDir(file.ToLower().Replace(Application.dataPath.ToLower(), "assets"));
        //                if (EditorUtility.DisplayCancelableProgressBar(string.Format("扫描{0}({1}/{2})", path, p, fileGUIDs.Length), assetName, (float)p / fileGUIDs.Length))
        //                {
        //                    throw new Exception("用户停止");
        //                }
        //                package.objs.Add(assetName);
        //            }
        //            //string output = AbUtility.AbBuildPath + "Atlas/";
        //            package.SetAbFileName(string.Concat("Atlas/", Path.GetDirectoryName(path), "/", Path.GetFileName(path), ABMgr.const_extension));//输出文件名
        //        }
        //    }
        //    finally
        //    {
        //        EditorUtility.ClearProgressBar();
        //    }
        //}

        //[MenuItem("Tools/AB/打包章节资源")]
        //static void PackageChapterRes()
        //{
        //    int[] chapterIDs = new int[] { 1 };
        //    foreach(var chapterID in chapterIDs)
        //    {
        //        string abConfigEditorPath = string.Format("Assets/Bundle/Config/Chapter/{0:D3}.asset", chapterID);
        //        AbResConfig config = AbResConfig.LoadInEditor(abConfigEditorPath);
        //        config.SaveInEditor();
        //    }
        //}
    }
}