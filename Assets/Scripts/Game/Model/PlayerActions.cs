using State;
using System;

namespace Model
{
    public class Idle : PlayerStates
    {
        private readonly PlayerEngine _player;

        public Idle(PlayerStateMachine playerStateMachine, PlayerEngine player) : base(playerStateMachine) => _player = player;

        public override void FixedUpdate()
        {
            _player.ApplyIdle();
            _player.ApplyRotation(_playerStateMachine.Direction.X, _playerStateMachine.Direction.Y);
            _player.FixedPlayerUpdate();

            if (_playerStateMachine.Direction.Length() != 0)
                _playerStateMachine.SetPlayerState(_playerStateMachine.Move);

            if (!_player.IsGrounded)
                _playerStateMachine.SetPlayerState(_playerStateMachine.Fall);
        }
    }

    public class Move : PlayerStates
    {
        private readonly PlayerEngine _player;

        public Move(PlayerStateMachine playerStateMachine, PlayerEngine player) : base(playerStateMachine)
        { 
            _player = player;
            _playerStateMachine.OnStateSwitch += SwitchState;
        }

        private void SwitchState()
        {
            _playerStateMachine.SetPlayerState(_playerStateMachine.Idle);
        }

        public override void FixedUpdate()
        {
            _player.ApplyRotation(_playerStateMachine.Direction.X, _playerStateMachine.Direction.Y);
            _player.ApplyMovement(_playerStateMachine.Direction.X, _playerStateMachine.Direction.Y);
            _player.FixedPlayerUpdate();

            if (_playerStateMachine.Direction.Length() == 0)
                _playerStateMachine.SetPlayerState(_playerStateMachine.Idle);

            if (!_player.IsGrounded)
                _playerStateMachine.SetPlayerState(_playerStateMachine.Fall);
        }
    }

    public class Fall : PlayerStates
    {
        private readonly PlayerEngine _player;

        public Fall(PlayerStateMachine playerStateMachine, PlayerEngine player) : base(playerStateMachine) => _player = player;

        public override void FixedUpdate()
        {
            _player.ApplyGravity();
            _player.ApplyRotation(_playerStateMachine.Direction.X, _playerStateMachine.Direction.Y);
            _player.FixedPlayerUpdate();

            if (_player.IsGrounded)
                _playerStateMachine.SetPlayerState(_playerStateMachine.Idle);
        }
    }

//    public class Interact : PlayerStates
//    {
//        private readonly PlayerEngine _player;

//        public Interact(PlayerStateMachine playerStateMachine, PlayerEngine player) : base(playerStateMachine) => _player = player;

//        public override void FixedUpdate()
//        {
//            _player.ApplyGravity();
//            _player.ApplyRotation(_playerStateMachine.Direction.X, _playerStateMachine.Direction.Y);
//            _player.FixedPlayerUpdate();

//            if (_player.IsGrounded)
//                _playerStateMachine.SetPlayerState(_playerStateMachine.Idle);
//        }
//    }
}