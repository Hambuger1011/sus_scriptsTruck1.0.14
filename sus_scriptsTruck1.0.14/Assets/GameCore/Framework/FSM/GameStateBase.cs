using Framework;

using System;
namespace FSM
{
    public abstract class GameStateBase : IState
    {
        protected string _name;
        public virtual string name
        {
            get
            {
                if(_name == null)
                {
                    _name = base.GetType().Name;
                }
                return _name;
            }
        }

        public bool isLoading { get; protected set; }

        public virtual void OnStateEnter()
        {
        }

        public virtual void OnStateLeave()
        {
        }

        public virtual void OnStateOverride()
        {
        }

        public virtual void OnStateResume()
        {
        }
    }
}
