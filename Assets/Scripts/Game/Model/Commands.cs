using System.Numerics;
using InputHandling;

namespace InputHandling
{
    public interface IDirectionalCommand
    {
        void Execute(Vector2 direction);
    }

    public interface IImpulseCommand
    {
        void Execute(bool isTriggered);
    }
}

namespace Model
{
    public class MoveCommand : IDirectionalCommand
    {
        private readonly PlayerBehaviour _playerBehaviour;

        public MoveCommand(PlayerBehaviour playerBehaviour)
        {
            _playerBehaviour = playerBehaviour;
        }

        public void Execute(Vector2 direction)
        {
            _playerBehaviour.Direction = direction;
        }
    }

    public class JumpCommand : IImpulseCommand
    {
        private readonly PlayerBehaviour _playerBehaviour;

        public JumpCommand(PlayerBehaviour playerBehaviour)
        {
            _playerBehaviour = playerBehaviour;
        }

        public void Execute(bool isTriggered)
        {
            _playerBehaviour.IsTriggered = isTriggered;
        }
    }
}