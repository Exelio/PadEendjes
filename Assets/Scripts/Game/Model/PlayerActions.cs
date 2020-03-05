using State;

namespace Model
{
    public class Idle : PlayerStates
    {
        private readonly PlayerEngine _player;

        public Idle(PlayerBehaviour playerBehaviour, PlayerEngine player) : base(playerBehaviour)
        {
            _player = player;
        }

        public override void Idling()
        {
            _player.ApplyRotation(P_PlayerBehaviour.Direction.X, P_PlayerBehaviour.Direction.Y);
            _player.ApplyIdle();
            _player.FixedPlayerUpdate();
        }

        public override void Moving()
        {
            if (P_PlayerBehaviour.Direction.Length() != 0)
                P_PlayerBehaviour.SetPlayerState(P_PlayerBehaviour.Move);
        }

        public override void Jumping()
        {
            if (P_PlayerBehaviour.IsTriggered)
                P_PlayerBehaviour.SetPlayerState(P_PlayerBehaviour.Jump);
        }

        public override void Falling()
        {
            if (!_player.IsGrounded)
                P_PlayerBehaviour.SetPlayerState(P_PlayerBehaviour.Fall);
        }
    }

    public class Move : PlayerStates
    {
        private readonly PlayerEngine _player;

        public Move(PlayerBehaviour playerBehaviour, PlayerEngine player) : base(playerBehaviour)
        {
            _player = player;
        }

        public override void Idling()
        {
            if (P_PlayerBehaviour.Direction.Length() == 0)
                P_PlayerBehaviour.SetPlayerState(P_PlayerBehaviour.Idle);
        }

        public override void Moving()
        {
            _player.ApplyRotation(P_PlayerBehaviour.Direction.X, P_PlayerBehaviour.Direction.Y);
            _player.ApplyMovement(P_PlayerBehaviour.Direction.X, P_PlayerBehaviour.Direction.Y);
            _player.FixedPlayerUpdate();
        }

        public override void Jumping()
        {
            if (P_PlayerBehaviour.IsTriggered)
                P_PlayerBehaviour.SetPlayerState(P_PlayerBehaviour.Jump);
        }

        public override void Falling()
        {
            if (!_player.IsGrounded)
                P_PlayerBehaviour.SetPlayerState(P_PlayerBehaviour.Fall);
        }
    }

    public class Jump : PlayerStates
    {
        private readonly PlayerEngine _player;

        public Jump(PlayerBehaviour playerBehaviour, PlayerEngine player) : base(playerBehaviour)
        {
            _player = player;
        }

        public override void Idling() { }
        public override void Moving() { }

        public override void Jumping()
        {
            _player.ApplyRotation(P_PlayerBehaviour.Direction.X, P_PlayerBehaviour.Direction.Y);
            _player.ApplyJump();
            _player.FixedPlayerUpdate();
        }

        public override void Falling()
        {
            if (!_player.IsGrounded)
                P_PlayerBehaviour.SetPlayerState(P_PlayerBehaviour.Fall);
        }
    }

    public class Fall : PlayerStates
    {
        private readonly PlayerEngine _player;

        public Fall(PlayerBehaviour playerBehaviour, PlayerEngine player) : base(playerBehaviour)
        {
            _player = player;
        }

        public override void Idling()
        {
            if (_player.IsGrounded)
            {
                P_PlayerBehaviour.IsTriggered = false;

                P_PlayerBehaviour.SetPlayerState(P_PlayerBehaviour.Idle);
            }
        }

        public override void Moving() { }
        public override void Jumping() { }

        public override void Falling()
        {
            _player.ApplyGravity();
            _player.ApplyRotation(P_PlayerBehaviour.Direction.X, P_PlayerBehaviour.Direction.Y);
            _player.ApplyJumpMovement(P_PlayerBehaviour.Direction.X, P_PlayerBehaviour.Direction.Y);
            _player.FixedPlayerUpdate();
        }
    }
}