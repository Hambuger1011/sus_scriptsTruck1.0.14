using IGG.SDK.Core.Error;
using IGG.SDK.Framework;
using IGG.SDK.Modules.EventCollection;
using IGG.SDK.Modules.EventCollection.VO;
using IGGUtils;
using Script.Game.Helpers;

namespace Helper.EventCollection
{
    /// <summary>
    /// 封装了一些事件与日志（EventCollection）上传的业务逻辑，简化研发接入EventCollection。
    /// </summary>
    public class EventCollectionHelper
    {
        private static EventCollectionHelper sInstance;

        private bool hasPrepared = false;
        
        public static EventCollectionHelper SharedInstance()
        {
            if (sInstance == null)
            {
                sInstance = new EventCollectionHelper();
            }
            return sInstance;
        }

        /// <summary>
        /// 初始化，关注一下调用时机（在appconfig加载完成之后调用）。
        /// </summary>
        public void Init()
        {
            KungfuInstance.Get().PrepareEventCollection(SDKtreamService, GameStreamService, OnPrepared);
        }

        private void OnPrepared(IGGError error)
        {
            hasPrepared = error.IsNone();
        }

        /// <summary>
        /// 实例化GameStreamService
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        private IStreamService GameStreamService(AWSKinesisStreamConfig config)
        {
            var game = new AWSKinesisStream(AWSKinesisStream.Type.GAME, config); // 实例化游戏层面需要用到的AWSKinesisStream

            return game;
        }
        
        /// <summary>
        /// 实例化SDKtreamService
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        private IStreamService SDKtreamService(AWSKinesisStreamConfig config)
        {
            var sdk = new AWSKinesisStream(AWSKinesisStream.Type.IGGSDK, config); // 实例化游戏层面需要用到的AWSKinesisStream

            return sdk;
        }

        /// <summary>
        /// 上传事件。
        /// </summary>
        /// <param name="eventValue">事件</param>
        /// <param name="escalation">事件的重要程度</param>
        public void Push(IGGEvent eventValue, IGGEventEscalation escalation)
        {
            if (hasPrepared)
            {
                KungfuInstance.Get().GetPreparedEventCollection().Push(eventValue, escalation);
            }
        }
    }
}