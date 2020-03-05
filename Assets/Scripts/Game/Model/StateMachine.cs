using State;

namespace Model
{
    public abstract class StateMachine
    {
        protected PlayerStates P_PlayerState;

        public void SetPlayerState(PlayerStates playerState) => P_PlayerState = playerState;
    }
}