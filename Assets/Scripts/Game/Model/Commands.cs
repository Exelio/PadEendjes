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

    public class RotateCameraCommand : IDirectionalCommand
    {
        private readonly CameraEngine _cameraEngine;

        public RotateCameraCommand(CameraEngine cameraEngine)
        {
            _cameraEngine = cameraEngine;
        }

        public void Execute(Vector2 direction)
        {
            _cameraEngine.ApplyRotation(direction.X, direction.Y);
        }
    }

    public class CameraInteractCommand : IImpulseCommand
    {
        private readonly CameraEngine _cameraEngine;

        public CameraInteractCommand(CameraEngine cameraEngine)
        {
            _cameraEngine = cameraEngine;
        }

        public void Execute()
        {
            //_cameraEngine.ToggleAnchorPoint();
        }
    }
}