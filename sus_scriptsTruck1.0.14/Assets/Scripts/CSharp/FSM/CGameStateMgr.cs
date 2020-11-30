using FSM;
using System;
using System.Reflection;
using Framework;

public class CGameStateMgr : Framework.CSingleton<CGameStateMgr>
{
    public class GameStateAttribute : AutoRegisterAttribute
    {
    }

    private StateMachine gameState = new StateMachine();
    IState _preState;
    public IState preState
    {
        get
        {
            return _preState;
        }
    }

    public string currentStateName
    {
        get
        {
            IState currentState = this.GetCurrentState();
            return (currentState == null) ? "unkown state" : currentState.name;
        }
    }

    protected override void Init()
    {
        base.Init();
        Initialize();
    }

    public void Initialize()
    {
        this.gameState.RegisterStateByAttributes<GameStateAttribute>(Assembly.GetExecutingAssembly());
        /*
        this.gameState.RegisterState<BattleState>(new BattleState(), typeof(BattleState).Name);
        this.gameState.RegisterState<EmptyState>(new EmptyState(), typeof(EmptyState).Name);
        this.gameState.RegisterState<LaunchState>(new LaunchState(), typeof(LaunchState).Name);
        this.gameState.RegisterState<LoadingState>(new LoadingState(), typeof(LoadingState).Name);
        this.gameState.RegisterState<LobbyState>(new LobbyState(), typeof(LobbyState).Name);
        this.gameState.RegisterState<LoginState>(new LoginState(), typeof(LoginState).Name);
        this.gameState.RegisterState<MovieState>(new MovieState(), typeof(MovieState).Name);
        this.gameState.RegisterState<VersionUpdateState>(new VersionUpdateState(), typeof(VersionUpdateState).Name);
        */
    }

    public void Uninitialize()
    {
        this.gameState.Clear();
        this.gameState = null;
    }

    public void GotoState<T>() where T : IState
    {
        GotoState(typeof(T).Name);
    }


    public void GotoState(string name)
    {
        _preState = GetCurrentState();
        string str = string.Format("GameStateCtrl Goto State {0}", name);
        //LOG.Info(str);
        this.gameState.ChangeState(name);
    }

    public IState GetCurrentState()
    {
        return this.gameState.TopState();
    }
}
