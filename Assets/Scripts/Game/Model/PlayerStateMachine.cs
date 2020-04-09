using System.Numerics;

namespace Model
{
    public class PlayerStateMachine : StateMachine
    {
        public Vector2 Direction { get; set; }

        public Idle Idle { get; }
        public Move Move { get; }
        public Fall Fall { get; }
        //public Interact Interact { get; }

        public PlayerStateMachine(PlayerEngine engine)
        {
            Idle = new Idle(this, engine);
            Move = new Move(this, engine);
            Fall = new Fall(this, engine);
            //Interact = new Interact(this, engine);

            SetPlayerState(Idle);
        }

        public void FixedUpdate() => _playerState?.FixedUpdate();
        public void RequestInteraction() => _playerState?.Interact();
    }
}