using AB;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class LuaBundle : ScriptableObject
{
//#if UNITY_EDITOR
    public const string PATH = "Assets/Bundle/lua/Lua.asset";
//#endif
    public List<string> keys = new List<string>();
    public List<TextAsset> scripts = new List<TextAsset>();

    Dictionary<string, byte[]> map;
    
    /// <summary>
    /// 加载lua脚本
    /// </summary>
    public byte[] Pop(string strVal)
    {
        if (map == null)
        {
            map = new Dictionary<string, byte[]>();
            for (int i = 0; i < keys.Count; ++i)
            {
                var s = scripts[i];
                if (s == null)
                {
                    LOG.Error("lua脚本丢失:"+keys[i]);
                    continue;
                }
                map.Add(keys[i], s.bytes);
            }
            keys = null;
            scripts = null;
        }
        byte[] obj;
        if (map.TryGetValue(strVal, out obj))
        {
            return obj;
        }
        return null;
    }












#if UNITY_EDITOR
    [MenuItem("XLua/更新Lua AB")]
    static void Update()
    {
        try
        {
            Directory.CreateDirectory("Assets/Bundle/Lua");
            var luaData = AssetDatabase.LoadAssetAtPath<LuaBundle>(LuaBundle.PATH);
            if (luaData == null)
            {
                luaData = ScriptableObject.CreateInstance<LuaBundle>();
                AssetDatabase.CreateAsset(luaData, LuaBundle.PATH);
                AssetDatabase.Refresh();
                //luaData = AssetDatabase.LoadAssetAtPath<LuaBundle>("Assets/Bundle/Lua.asset");
            }

            Dictionary<string, string> oldLua = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase);
            var fileGUIDs = UnityEditor.AssetDatabase.FindAssets(string.Format("t:{0}", typeof(TextAsset).Name), new[] { "Assets/Bundle/Lua" });
            int p = 0;
            foreach (string guid in fileGUIDs)
            {
                var file = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                ++p;
                if (EditorUtility.DisplayCancelableProgressBar(string.Format("扫描旧Lua({0}/{1})", p, fileGUIDs.Length), file, (float)p / fileGUIDs.Length))
                {
                    throw new Exception("用户停止");
                }
                oldLua.Add(file,guid);
            }


            var files = Directory.GetFiles("assets/scripts/lua", "*.lua", SearchOption.AllDirectories);
            int offset = "assets/scripts/lua/".Length;
            p = 0;
            foreach (var f in files)
            {
                ++p;
                if (EditorUtility.DisplayCancelableProgressBar(string.Format("复制{0}({1}/{2})", f, p, files.Length), f, (float)p / files.Length))
                {
                    throw new System.Exception("用户停止");
                }
                var key = AbUtility.NormalizerAbName(f.Substring(offset));
                var dst = "assets/bundle/lua/" + key + ".txt";
                Directory.CreateDirectory(Path.GetDirectoryName(dst));
                File.Copy(f, dst, true);
                oldLua.Remove(dst);
            }

            foreach(var itr in oldLua)
            {
                File.Delete(itr.Key);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);

            luaData = AssetDatabase.LoadAssetAtPath<LuaBundle>(LuaBundle.PATH);
            luaData.keys.Clear();
            luaData.scripts.Clear();
            p = 0;
            foreach (var f in files)
            {
                ++p;
                var key = AbUtility.NormalizerAbName(f.Substring(offset));
                if (EditorUtility.DisplayCancelableProgressBar(string.Format("添加列表{0}({1}/{2})", key, p, files.Length), key, (float)p / files.Length))
                {
                    throw new System.Exception("用户停止");
                }
                luaData.keys.Add(key);
                var dst = "assets/bundle/lua/" + key + ".txt";
                luaData.scripts.Add(AssetDatabase.LoadAssetAtPath<TextAsset>(dst));
            }
            EditorUtility.SetDirty(luaData);
        }
        finally
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            EditorUtility.ClearProgressBar();
        }
    }

#endif
}
