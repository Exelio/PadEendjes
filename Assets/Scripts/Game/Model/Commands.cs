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
        void Execute();
    }
}

namespace Model
{
    public class MoveCommand : IDirectionalCommand
    {
        private readonly PlayerStateMachine _playerStateMachine;

        public MoveCommand(PlayerStateMachine playerBehaviour)
        {
            _playerStateMachine = playerBehaviour;
        }

        public void Execute(Vector2 direction)
        {
            _playerStateMachine.Direction = direction;
        }
    }

    public class InteractCommand : IImpulseCommand
    {
        private readonly PlayerStateMachine _playerStateMachine;

        public InteractCommand(PlayerStateMachine playerStateMachine)
        {
            _playerStateMachine = playerStateMachine;
        }

        public void Execute()
        {
            _playerStateMachine.RequestInteraction();
        }
    }
}