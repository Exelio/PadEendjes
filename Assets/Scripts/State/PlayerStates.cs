using Model;

namespace State
{
    public abstract class PlayerStates
    {
        protected PlayerStateMachine _playerStateMachine;

        public PlayerStates(PlayerStateMachine playerStateMachine) => _playerStateMachine = playerStateMachine;

        public virtual void FixedUpdate() { }
        public virtual void Interact() { }
    }
}