using System;
using System.Collections;
using InputHandling;
using UnityEngine;
using Model;
using View;
using System.Collections.Generic;

namespace Game
{
    public class ApplicationBehaviour : SingletonMonoBehaviour<ApplicationBehaviour>
    {
        public event EventHandler Initialized;

        [SerializeField] private PlayerView _player;
        [SerializeField] private CameraView _camera;
        [SerializeField] private RewardView _reward;
        [SerializeField] private DuckView[] _ducklings;
        [SerializeField] private TrafficHub _trafficHUb;

        private PlayerEngine _playerEngine;
        private CameraEngine _cameraEngine;

        private PlayerStateMachine _playerStateMachine;

        private InputHandler _inputHandler;

        private RewardBehaviour _rewardBehaviour;
        private List<TrafficController> _vehicleBehaviours = new List<TrafficController>();
        private List<DuckBehaviour> _duckBehaviours = new List<DuckBehaviour>();

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            _playerEngine = new PlayerEngine(_player);
            _playerStateMachine = new PlayerStateMachine(_playerEngine);
            _cameraEngine = new CameraEngine(_camera, _player.transform);
            _inputHandler = new InputHandler();
            _inputHandler.LeftStickCommand = new MoveCommand(_playerStateMachine);
            _inputHandler.RightStickCommand = new RotateCameraCommand(_cameraEngine);
            _inputHandler.ACommand = new InteractCommand(_playerStateMachine);
            _inputHandler.YCommand = new CameraInteractCommand(_cameraEngine);
            _rewardBehaviour = new RewardBehaviour(_reward);
            CreateDucklingModels();

            StartCoroutine(LateInitialize());
        }

        private void CreateDucklingModels()
        {
            foreach (var view in _ducklings)
            {
                DuckBehaviour controller = new DuckBehaviour(view);
                _duckBehaviours.Add(controller);
                controller.OnCaught += DuckCaught;
                controller.OnScared += DuckScared;
            }
        }

        private void DuckScared()
        {
            _rewardBehaviour.LostDuck();
        }

        private void DuckCaught()
        {
            _rewardBehaviour.CaughtDuck();
        }

        private void Update()
        {
            _inputHandler.Update();

            UpdateDucks();
        }

        private void UpdateDucks()
        {
            foreach (var behaviour in _duckBehaviours)
                behaviour.Update();
        }

        private void FixedUpdateDucks()
        {
            foreach (var behaviour in _duckBehaviours)
                behaviour.FixedUpdate();
        }

        private void FixedUpdate()
        {
            _playerStateMachine.FixedUpdate();
            _cameraEngine.FixedCameraUpdate();

            FixedUpdateDucks();
        }

        private IEnumerator LateInitialize()
        {
            yield return new WaitForEndOfFrame();

            Initialized?.Invoke(this, EventArgs.Empty);
        }
    }
}