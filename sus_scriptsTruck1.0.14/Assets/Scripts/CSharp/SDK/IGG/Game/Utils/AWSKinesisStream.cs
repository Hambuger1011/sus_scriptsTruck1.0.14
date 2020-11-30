//#define IGGSDK_EDITOR_ANDROID // 为了编码的方便在这边定义了该宏
using System;
using UnityEngine;
using System.Runtime.InteropServices;
using IGG.SDK.Modules.EventCollection;
using IGG.SDK.Modules.EventCollection.VO;

namespace IGGUtils
{
    /// <summary>
    /// 对各平台AWSKinesisStream的封装（Wrapper）从而实现我们对流服务接口的定义。
    /// </summary>
    public class AWSKinesisStream : IStreamService
    {
        public enum Type
        {
            GAME = 1,
            IGGSDK = 2
        }

        private string streamName;

#if UNITY_ANDROID
        private static AndroidJavaObject mAWSKinesisWrapper;
        public AWSKinesisStream(Type type, AWSKinesisStreamConfig config)
        {
            streamName = type == Type.GAME ? config.gameStreamName : config.sdkStreamName;
            mAWSKinesisWrapper = new AndroidJavaObject("com.igg.sdk.unity.AWSKinesisWrapper", new object[]
            {
                config.accessKeyId, config.secretKey, type == Type.GAME ? config.gameRegion : config.sdkRegion, streamName
                , config.cachePath
            });
        }

        public void Flush()
        {
           mAWSKinesisWrapper.Call("flush");
        }

        public void PurgeBuffer()
        {
            mAWSKinesisWrapper.Call("purgeBuffer");
        }

        public void Write(EventPacket packet)
        {
            mAWSKinesisWrapper.Call("write",new object[]{packet.Stringify(), streamName});
        }

        
        public int DiskBytesUsed()
        {
            return 0;
        }

#elif UNITY_IOS

        protected IntPtr mObject;
        [DllImport("__Internal")]
        private static extern IntPtr AWSKinessisWrapper_alloc(int type, string accessKeyId, string secretKey, string region, string streamName, string cacheDir);
        public AWSKinesisStream(Type type, AWSKinesisStreamConfig config)
        {
            streamName = type == Type.GAME ? config.gameStreamName : config.sdkStreamName;
            mObject = AWSKinessisWrapper_alloc((int)type, config.accessKeyId, config.secretKey, type == Type.GAME ? config.gameRegion : config.sdkRegion, streamName
                , config.cachePath);
        }

        [DllImport("__Internal")]
        private static extern void AWSKinessisWrapper_Flush(IntPtr nsObject);
        public void Flush()
        {
           AWSKinessisWrapper_Flush(mObject);
        }

        [DllImport("__Internal")]
        private static extern void AWSKinessisWrapper_PurgeBuffer(IntPtr nsObject);
        public void PurgeBuffer()
        {
            AWSKinessisWrapper_PurgeBuffer(mObject);
        }

        [DllImport("__Internal")]
        private static extern void AWSKinessisWrapper_Write(IntPtr nsObject, string data, string streamName);
        public void Write(EventPacket packet)
        {
             AWSKinessisWrapper_Write(mObject, packet.Stringify(), streamName);
        }

        [DllImport("__Internal")]
        private static extern int AWSKinessisWrapper_diskBytesUsed(IntPtr nsObject);
        public int DiskBytesUsed()
        {
            return AWSKinessisWrapper_diskBytesUsed(mObject);
        }
#else
        public AWSKinesisStream(Type type, AWSKinesisStreamConfig config)
        {
        }
        
        public void Flush()
        {

        }

        public void PurgeBuffer()
        {

        }

        public void Write(EventPacket packet)
        {

        }

        public int DiskBytesUsed()
        {
            return 0;
        }
#endif
    }
}