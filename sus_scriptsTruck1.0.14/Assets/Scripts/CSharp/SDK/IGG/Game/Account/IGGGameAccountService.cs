using IGG.SDK.Modules.Account.Service;
using Script.Game.Helpers;

namespace IGG.SDK.Service
{
    /// <summary>
    /// 用于模拟向游戏服务端验证当前session是否有效与模拟游戏账号被踢的接口(主要USDKDemo用，研发不要复制这些接口调用的逻辑。)。
    ///
    /// 游戏客户端要想实现向游戏服务端验证当前session是否有效与游戏账号被踢提醒，请参考相应账号文档中的流程图，不清除的可以联系技术部
    /// </summary>
    public class IGGGameAccountService
    {
        private const string TAG = "[IGGGameAccountService]";

        /// <summary>
        /// 模拟登录（将本地的session注册到服务端）。
        /// </summary>
        /// <param name="iggid"></param>
        /// <param name="accessToken"></param>
        /// <param name="listener"></param>
        public void SimulatorLogin(string iggid, string accessToken, AccountSimulatorService.SimulatorLoginResultListener listener)
        {
            AccountSimulatorService service = new AccountSimulatorService(KungfuInstance.Get().Configuration);
            service.Login(iggid, accessToken, listener);
        }

        /// <summary>
        /// 检测当前的session是否有效。
        /// </summary>
        /// <param name="iggid"></param>
        /// <param name="accessToken"></param>
        /// <param name="listener"></param>
        public void SimulatorCheck(string iggid, string accessToken, AccountSimulatorService.SimulatorCheckResultListener listener)
        {
            AccountSimulatorService service = new AccountSimulatorService(KungfuInstance.Get().Configuration);
            service.Check(iggid, accessToken, listener);
        }
    }
}
