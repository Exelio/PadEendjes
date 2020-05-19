using System;
using System.Numerics;

namespace Model
{
    public class PlayerStateMachine : StateMachine
    {
        public event Action OnStateSwitch;
        public Vector2 Direction { get; set; }

        public Idle Idle { get; }
        public Move Move { get; }
        public Fall Fall { get; }

        public PlayerStateMachine(PlayerEngine engine)
        {
            Idle = new Idle(this, engine);
            Move = new Move(this, engine);
            Fall = new Fall(this, engine);

            SetPlayerState(Idle);
        }

        public void FixedUpdate() => _playerState?.FixedUpdate();
        public void RequestInteraction() => _playerState?.Interact();
        public void SetPlayerStateToIdle() => OnStateSwitch?.Invoke();
    }
}