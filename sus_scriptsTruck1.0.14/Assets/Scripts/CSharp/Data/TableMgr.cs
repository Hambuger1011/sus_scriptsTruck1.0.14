
namespace pb
{
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using AB;

    public partial class TableMgr
    {
        public static T Deserialize<T>(string strFileName)
        {
            Stopwatch st = new Stopwatch();
            st.Start();
            byte[] rawBytes = null;
            if (ABSystem.Instance.isUseAssetBundle)
            {
                var asset = ABSystem.ui.bundle.LoadImme(AbTag.Null, enResType.eText, strFileName);
                rawBytes = asset.resTextAsset.bytes;
            }
            else
            {
                rawBytes = File.ReadAllBytes(strFileName);
            }
            T obj = default(T);
            using (MemoryStream ms = new MemoryStream(rawBytes))
            {
                try
                {
                    obj = ProtoBuf.Serializer.Deserialize<T>(ms);
                }
                catch (System.Exception ex)
                {
                    LOG.Error("反序列化异常" + ex.Message);
                }
            }
            st.Stop();
            LOG.Info("pb解析耗时:" + st.ElapsedMilliseconds + "ms 读取耗时:" + st.ElapsedMilliseconds + "ms");
            return obj;
        }

    }

}
