using Framework;

///*
//项目启动时检测必须的宏
//*/
//using UnityEditor;
//using System.Collections.Generic;
//using System.Linq;
//using System;

//namespace Framework
//{
//	/// <summary>
//	/// A class to register define symbols for Unity features.
//	/// </summary>
//	[InitializeOnLoad]
//	public sealed class UnityFeatureDefineSymbols
//	{
//        /// <summary>
//        /// For each feature availability symbol, an array of class names whose presence indicates the feature.
//        /// </summary>
//        private static readonly List<string> s_FeatureAvailabilityClasses = new List<string>()
//        {
//            "ENABLE_DEBUG",
//            "ASTAR_DEBUG"
//        };

//        /// <summary>
//        /// Initializes the <see cref="UnityFeatureDefineSymbols"/> class.
//        /// </summary>
//        static UnityFeatureDefineSymbols()
//        {
//            SetSymbolForAllBuildTargets(s_FeatureAvailabilityClasses);
//		}

//		/// <summary>
//		/// Sets the symbol for all build targets.
//		/// </summary>
//		public static void SetSymbolForAllBuildTargets(List<string> requestSymbols)
//		{
//			foreach (BuildTargetGroup target in System.Enum.GetValues(typeof(BuildTargetGroup)))
//			{
//				// prevent editor spam in Unity 5.x
//				if (target == BuildTargetGroup.Unknown)
//				{
//					continue;
//				}
//#if UNITY_5_3_0
//				// prevent editor spam in 5.3.0
//				if ((int)target == 25) // tvOS throwing out error
//				{
//					continue;
//				}
//#endif
//#if UNITY_5_4_OR_NEWER
//				// prevent editor spam in 5.4.0 beta
//				if ((int)target == 15) // WP8 not yet marked as obsolete
//				{
//					continue;
//				}
//#endif

//                System.Reflection.MemberInfo member = typeof(BuildTargetGroup).GetMember(target.ToString())[0];
//                if (member.IsDefined(typeof(ObsoleteAttribute),true))
//                {
//                    continue;
//                }

//                HashSet<string> symbols = new HashSet<string>(PlayerSettings.GetScriptingDefineSymbolsForGroup(target).Split(';'));
//                foreach(var s in requestSymbols)
//                {
//                    if(symbols.Add(s))
//                    {
//                        //TODDO: ....
//                    }
//                }
//				PlayerSettings.SetScriptingDefineSymbolsForGroup(target, string.Join(";", symbols.ToArray()));
//			}
//		}
//	}
//}
