using System.Numerics;

namespace Model
{
    public class PlayerBehaviour : StateMachine
    {
        public Vector2 Direction { get; set; }

        public bool IsTriggered { get; set; }

        public Idle Idle { get; }
        public Move Move { get; }
        public Jump Jump { get; }
        public Fall Fall { get; }

        public PlayerBehaviour(PlayerEngine engine)
        {
            Idle = new Idle(this, engine);
            Move = new Move(this, engine);
            Jump = new Jump(this, engine);
            Fall = new Fall(this, engine);

            SetPlayerState(Idle);
        }

        public void FixedUpdate() => P_PlayerState?.FixedUpdate();
    }
}