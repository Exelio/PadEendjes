using State;

namespace Model
{
    public abstract class StateMachine
    {
        protected PlayerStates _playerState;

        public void SetPlayerState(PlayerStates playerState) => _playerState = playerState;
    }
}