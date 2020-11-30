/*
 * https://www.cnblogs.com/tiancaiwrk/p/10900173.html
 */
using UnityEngine;
using UnityEditor;
using System.Diagnostics;
using System.Collections.Generic;

namespace MyEditor
{
    public class UnitySVN
    {
        private const string Add_CMD = "add";
        private const string COMMIT_CMD = "commit";
        private const string UPDATE_CMD = "update";
        private const string REVERT_CMD = "revert";

        private static System.Text.StringBuilder ms_sb = new System.Text.StringBuilder();

        #region MenuItem Funcs
        [MenuItem("Assets/SVN/更新工程", false, 1001)]
        public static void SVN_UpdateAll()
        {
            UnityEngine.Debug.Log("更新工程:"+ System.Environment.CurrentDirectory);
            Update(System.Environment.CurrentDirectory);
            UnityEngine.Debug.Log("更新工程End");
        }

        [MenuItem("Assets/SVN/Update", false, 1001)]
        public static void SVN_Update()
        {
            var paths = GetAssetPathList();
            if (paths.Count > 0)
            {
                Update(paths: paths.ToArray());
            }
        }
        [MenuItem("Assets/SVN/Revert", false, 1002)]
        public static void SVN_Revert()
        {
            var paths = GetAssetPathList();
            if (paths.Count > 0)
            {
                Revert(paths.ToArray());
            }
        }
        [MenuItem("Assets/SVN/Commit", false, 1003)]
        public static void SVN_Commit()
        {
            var paths = GetAssetPathList();
            if (paths.Count > 0)
            {
                Commit("UnitySVN Upload", true, paths.ToArray());
            }
        }
        #endregion

        #region Wrapped Funcs
        // add
        public static void Add(params string[] paths)
        {
            WrappedCommadn(Add_CMD, paths, false);
        }
        // update
        public static void Update(params string[] paths)
        {
            WrappedCommadn(UPDATE_CMD, paths, false);
            SaveAndRefresh();
        }
        // revert
        public static void Revert(params string[] paths)
        {
            WrappedCommadn(REVERT_CMD, paths, false);
            SaveAndRefresh();
        }
        // add->update->commit
        public static void Commit(string log, bool add = true, params string[] paths)
        {
            if (add)
            {
                Add(paths);
            }
            Update(paths);
            string extMsg = log ?? string.Empty;
            WrappedCommadn(command: COMMIT_CMD, paths: paths, newThread: true, extCommand: "/logmsg:\"Auto Upload : " + (extMsg) + "\"");
        }

        /// <summary>
        /// Wrap SVN Command
        /// </summary>
        /// <param name="command"></param>
        /// <param name="path"></param>
        /// <param name="extCommand"></param>
        public static void WrappedCommadn(string command, string[] paths, bool newThread = false, string extCommand = null)
        {
            if (paths == null || paths.Length == 0)
            {
                return;
            }

            ms_sb.Append(paths[0]);
            for (int i = 1; i < paths.Length; i++)
            {
                ms_sb.Append("/*");
                ms_sb.Append(paths[i]);
            }

            string cmd = "/c tortoiseproc.exe /command:{0} /path:\"{1}\" {2} /closeonend 2";
            string pathString = ms_sb.ToString();
            var commandString = string.Format(cmd, command, pathString, extCommand ?? string.Empty);

            ProcessStartInfo info = new ProcessStartInfo("cmd.exe", commandString);
            info.WindowStyle = ProcessWindowStyle.Hidden;
            if (newThread)
            {
                System.Threading.ThreadPool.QueueUserWorkItem((_obj) =>
                {
                    RunProcess(info);
                });
            }
            else
            {
                RunProcess(info);
            }
        }
        #endregion

        #region Help Funcs
        public static HashSet<string> GetAssets()
        {
            HashSet<string> allAssets = new HashSet<string>();
            const string BaseFolder = "Assets";
            foreach (var obj in Selection.objects)
            {
                var assetPath = AssetDatabase.GetAssetPath(obj);

                List<string> fullDirs = FullDirectories(assetPath, BaseFolder);
                allAssets.UnionWith(fullDirs);

                var dps = AssetDatabase.GetDependencies(assetPath, true);
                foreach (var dp in dps)
                {
                    if (dp != assetPath)
                    {
                        List<string> dpsDirs = FullDirectories(dp, BaseFolder);
                        allAssets.UnionWith(dpsDirs);
                    }
                }
            }
            return allAssets;
        }
        public static List<string> GetAssetPathList()
        {
            var path = new List<string>(GetAssets());
            path.Sort((_l, _r) =>
            {
                if (_l.Length > _r.Length)
                {
                    return 1;
                }
                if (_l.Length < _r.Length)
                {
                    return -1;
                }
                return 0;
            });
            return path;
        }
        public static void SaveAndRefresh()
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        public static List<string> FullDirectories(string path, string baseFolder)
        {
            List<string> retVal = new List<string>();
            retVal.Add(path);
            retVal.Add(path + ".meta");
            baseFolder = baseFolder.Replace("\\", "/");
            var dir = System.IO.Path.GetDirectoryName(path).Replace("\\", "/");
            while (string.IsNullOrEmpty(dir) == false && dir != baseFolder)
            {
                retVal.Add(dir);
                retVal.Add(dir + ".meta");
                dir = System.IO.Path.GetDirectoryName(dir).Replace("\\", "/");
            }
            return retVal;
        }
        private static void RunProcess(ProcessStartInfo info)
        {
            Process p = null;
            try
            {
                using (p = Process.Start(info))
                {
                    p.WaitForExit();
                }
            }
            catch (System.Exception ex)
            {
                UnityEngine.Debug.LogError(@ex.ToString());
                if (p != null)
                {
                    p.Kill();
                }
            }
        }
        #endregion

    }

    /*
    / closeonend：0不自动关闭对话框
 
    / closeonend：1会自动关闭，如果没有错误
 
    / closeonend：2会自动关闭，如果没有发生错误和冲突
 
    / closeonend：3会自动关闭，如果没有错误，冲突和合并
 
    / closeonend：4会自动关闭，如果没有错误，冲突和合并
     */
}