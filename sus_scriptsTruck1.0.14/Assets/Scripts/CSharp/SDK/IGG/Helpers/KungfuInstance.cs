using IGG.SDK.Core.Configuration;
using IGG.SDK.Framework;
using IGG.SDK.Framework.Listener;

namespace Script.Game.Helpers
{
    public static class KungfuInstance
    {
        private static Kungfu instance;

        private static IGGConfigurationProvider provider;

        public static void SetConfigurationProvider(IGGConfigurationProvider configurationProvider)
        {
            provider = configurationProvider;
        }

        /// <summary>
        /// 复位
        /// </summary>
        public static void Reset(string newGameId)
        {
            if (instance != null)
            {
                instance.Uninitialize(newGameId);
                instance = null;
            }
        }

        public static Kungfu Get()
        {
            if (instance == null)
            {
                instance = new Kungfu(provider);
            }
            
            

            return instance;
        }
    }
}