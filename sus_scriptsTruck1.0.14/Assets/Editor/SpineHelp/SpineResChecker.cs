using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using Spine.Unity;

public class SpineResChecker : MonoBehaviour
{
    
    [MenuItem("GameTools/SpineHelp/SpineRoleResChecker", false, 1000)]
    static void SpineRoleResChecker()
    {
        string rootDir = Path.Combine(Application.dataPath, "Bundle/Book");
        Debug.Log(rootDir);
        SpineRoleResCheck(rootDir);
    }

    [MenuItem("Assets/GameTools/SpineHelp/SpineRoleResChecker", false, 1000)]
    static void SpineRoleResChecker_InProjectRightClick()
    {
        string[] assetGUIDs = Selection.assetGUIDs;
        //Debug.Log($"assetGUIDs.Length={assetGUIDs.Length}");
        if (assetGUIDs.Length == 0 || assetGUIDs.Length > 1) return;
        string assetPath = AssetDatabase.GUIDToAssetPath(assetGUIDs[0]);
        //Debug.Log(assetPath);
        assetPath = assetPath.Substring("Assets/".Length);
        //Debug.Log(assetPath);
        string rootDir = Path.Combine(Application.dataPath, assetPath);
        //Debug.Log(rootDir);
        SpineRoleResCheck(rootDir);
    }

    /// <summary>1
    /// 检查角色的动画资源是否正确：
    ///     * 检查 *_SkeletonData.asset资源对应的Atlas是否正确,不正确给它赋上正确的。
    /// </summary>
    static void SpineRoleResCheck(string rootDir)
    {
        // 1.遍历所有的书籍目录
        string[] bookDirs = Directory.GetDirectories(rootDir, "*", SearchOption.TopDirectoryOnly);
        for (int i = 0; i < bookDirs.Length; i++)
        {
            string bookDir = bookDirs[i];
            string bookId = Path.GetFileName(bookDir);
            string roleDir = Path.Combine(bookDir, "Role").Replace('\\', '/');
            //Debug.Log($"bookId:{bookId} roleDir:{roleDir}");
            // 2.遍历一本书籍下的role资源中的动画数据，形如：1003010000_SkeletonData.asset
            string[] skeletonData_FilePaths = Directory.GetFiles(roleDir, "*SkeletonData.asset", SearchOption.TopDirectoryOnly);
            for (int j = 0; j < skeletonData_FilePaths.Length; j++)
            {
                string skeletonData_FilePath = skeletonData_FilePaths[j].Replace('\\', '/');
                string atlas_FilePath = skeletonData_FilePath.Replace("SkeletonData", "Atlas");
                //Debug.Log($"skeletonData_FilePath:{skeletonData_FilePath} skeletonData_FilePath:{skeletonData_FilePath}");
                string skeletonData_assetPath = skeletonData_FilePath.Substring(skeletonData_FilePath.IndexOf("Assets"));
                string atlas_assetPath = atlas_FilePath.Substring(atlas_FilePath.IndexOf("Assets"));
                //Debug.Log(skeletonData_assetPath);
                SkeletonDataAsset skeletonDataAsset = AssetDatabase.LoadAssetAtPath<SkeletonDataAsset>(skeletonData_assetPath);
                AtlasAssetBase atlasAsset = AssetDatabase.LoadAssetAtPath<AtlasAssetBase>(atlas_assetPath);
                if (atlasAsset == null)
                {
                    Debug.LogError($"请检查一下！！\n{skeletonData_assetPath} 找不到对应的 {atlas_assetPath} 资源！");
                    continue;
                }
                skeletonDataAsset.atlasAssets = new AtlasAssetBase[] { atlasAsset };
            }
        }
    }


}

//public class SpineResCheckerWindow : EditorWindow
//{

//}


