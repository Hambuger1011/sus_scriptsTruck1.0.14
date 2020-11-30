//using Framework;

//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using System;
//#if UNITY_EDITOR
//using UnityEditor.Callbacks;
//using System.Linq;
//using UnityEditor;
//#endif

//public class TagManager
//{

//    public class Tag
//    {
//        //builtin
//        public static readonly string Untagged = "Untagged";
//        public static readonly string Respawn = "Respawn";
//        public static readonly string Finish = "Finish";
//        public static readonly string EditorOnly = "EditorOnly";
//        public static readonly string MainCamera = "MainCamera";
//        public static readonly string Player = "Player";
//        public static readonly string GameController = "GameController";

//        //custom
//        //public static readonly string BattleSceneWall = "BattleSceneWall";
//        //public static readonly string soldier = "soldier";
//        //public static readonly string InteractiveObj = "InteractiveObj";//互动物
        
//    }

//    public class Layer
//    {
//        //builtin
//        public static readonly string Default = "Default";
//        public static readonly string TransparentFX = "TransparentFX";
//        public static readonly string IgnoreRaycast = "Ignore Raycast";
//        public static readonly string Default_3 = "";
//        public static readonly string Water = "Water";
//        public static readonly string UI = "UI";
//        public static readonly string Default_6 = "";
//        public static readonly string Default_7 = "";


//        //custom begin 8
//        public static readonly string _3DUI = "3DUI";
//        public static readonly string Hide = "Hide";
//        public static readonly string UIRaw = "UIRaw";
//        public static readonly string Particles = "Particles";
//        //public static readonly string Actor = "Actor";
//    }


//#if UNITY_EDITOR

//    #region 检查tag和layer

//    static string[] requestSymbols = new string[]
//    {
//            //"ENABLE_DEBUG",
//            //"ASTAR_DEBUG",
//            //"UNITY_PURCHASING",
//            "USE_SERVER_DATA",
//    };


//    [DidReloadScripts]
//    static void CheckTagAndLayer()
//    {
//        if(EditorApplication.isPlayingOrWillChangePlaymode)
//        {
//            return;
//        }
//        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
//        AddTag(tagManager);
//        AddLayer(tagManager);
//        AssetDatabase.SaveAssets();
//        SetSymbolForAllBuildTargets();
//    }
//    static void AddTag(SerializedObject tagManager)
//    {
//        SerializedProperty it = tagManager.GetIterator();
//        while (it.NextVisible(true))
//        {
//            if (it.name != "tags")
//            {
//                continue;
//            }

//            //bool addFlag = false;
//            var t = typeof(TagManager.Tag);
//            var fields = t.GetFields(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
//            //if(it.arraySize < fields.Length)
//            //{
//            //    it.arraySize = fields.Length;
//            //    addFlag = true;
//            //}
//            for(int i=7;i< fields.Length;++i)
//            {
//                var f = fields[i];
//                var tag = (string)f.GetValue(null);
//                if(isHasTag(tag))
//                {
//                    continue;
//                }
//                //SerializedProperty dataPoint = it.GetArrayElementAtIndex(i);
//                ////Debug.LogError(i + "." + tag + " " + dataPoint.stringValue);
//                //if(dataPoint.stringValue != tag)
//                //{
//                //    addFlag = true;
//                //    dataPoint.stringValue = tag;
//                //    //tagManager.ApplyModifiedProperties();
//                //}

//                bool addFlag = false;
//                for (int j = 0; j < it.arraySize; j++)
//                {
//                    SerializedProperty dataPoint = it.GetArrayElementAtIndex(j);
//                    if (string.IsNullOrEmpty(dataPoint.stringValue))
//                    {
//                        addFlag = true;
//                        dataPoint.stringValue = tag;
//                        tagManager.ApplyModifiedProperties();
//                        break;
//                    }
//                }
//                if (!addFlag)
//                {
//                    it.arraySize += 1;
//                    SerializedProperty dataPoint = it.GetArrayElementAtIndex(it.arraySize - 1);
//                    dataPoint.stringValue = tag;
//                    tagManager.ApplyModifiedProperties();
//                }
//            }//end foreach
//            //if (addFlag)
//            //{
//            //    tagManager.ApplyModifiedProperties();
//            //}
//            break;
//        }//end while
//    }

//    static void AddLayer(SerializedObject tagManager)
//    {
//        SerializedProperty it = tagManager.GetIterator();
//        while (it.NextVisible(true))
//        {
//            //LOG.Error(it.name + " " + it.type);
//            if (it.name == "layers")
//            {
//                bool addFlag = false;
//                var t = typeof(TagManager.Layer);
//                var fields = t.GetFields(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
//                if (it.arraySize < fields.Length)
//                {
//                    it.arraySize = fields.Length;
//                    addFlag = true;
//                }
//                for (int i = 8; i < fields.Length; ++i)
//                {
//                    var f = fields[i];
//                    var layer = (string)f.GetValue(null);
//                    SerializedProperty dataPoint = it.GetArrayElementAtIndex(i);
//                    //Debug.LogError(i + "." + layer + " " + dataPoint.stringValue);
//                    if (dataPoint.stringValue != layer)
//                    {
//                        addFlag = true;
//                        dataPoint.stringValue = layer;
//                        //tagManager.ApplyModifiedProperties();
//                    }
//                }//end add tag
//                if (addFlag)
//                {
//                    tagManager.ApplyModifiedProperties();
//                }
//                break;
//            }//end if "tag"
//        }//end while
//    }
//    static bool isHasTag(string tag)
//    {
//        for (int i = 0; i < UnityEditorInternal.InternalEditorUtility.tags.Length; i++)
//        {
//            if (UnityEditorInternal.InternalEditorUtility.tags[i].Contains(tag))
//                return true;
//        }
//        return false;
//    }

//    static bool isHasLayer(string layer)
//    {
//        for (int i = 0; i < UnityEditorInternal.InternalEditorUtility.layers.Length; i++)
//        {
//            if (UnityEditorInternal.InternalEditorUtility.layers[i].Contains(layer))
//                return true;
//        }
//        return false;
//    }

//    public static void SetSymbolForAllBuildTargets()
//    {
//        foreach (BuildTargetGroup target in System.Enum.GetValues(typeof(BuildTargetGroup)))
//        {
//            // prevent editor spam in Unity 5.x
//            if (target == BuildTargetGroup.Unknown)
//            {
//                continue;
//            }
//#if UNITY_5_3_0
//				// prevent editor spam in 5.3.0
//				if ((int)target == 25) // tvOS throwing out error
//				{
//					continue;
//				}
//#endif
//#if UNITY_5_4_OR_NEWER
//            // prevent editor spam in 5.4.0 beta
//            if ((int)target == 15) // WP8 not yet marked as obsolete
//            {
//                continue;
//            }
//#endif

//            System.Reflection.MemberInfo member = typeof(BuildTargetGroup).GetMember(target.ToString())[0];
//            if (member.IsDefined(typeof(ObsoleteAttribute), true))
//            {
//                continue;
//            }

//            HashSet<string> symbols = new HashSet<string>(PlayerSettings.GetScriptingDefineSymbolsForGroup(target).Split(';'));
//            foreach (var s in requestSymbols)
//            {
//                if (symbols.Add(s))
//                {
//                    //TODDO: ....
//                }
//            }
//            PlayerSettings.SetScriptingDefineSymbolsForGroup(target, string.Join(";", symbols.ToArray()));
//        }
//    }
//    #endregion
//#endif
//}
