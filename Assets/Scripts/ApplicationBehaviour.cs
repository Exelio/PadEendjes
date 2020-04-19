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
        [SerializeField] private VehicleView[] _vehicles;

        private PlayerEngine _playerEngine;
        private CameraModel _cameraModel;

        private PlayerStateMachine _playerStateMachine;

        private InputHandler _inputHandler;

        private RewardBehaviour _rewardBehaviour;
        private List<TrafficController> _vehicleBehaviours = new List<TrafficController>();
        private List<DuckBehaviour> _duckBehaviours = new List<DuckBehaviour>();

        private void Start()
        {
            _playerEngine = new PlayerEngine(_player);
            _playerStateMachine = new PlayerStateMachine(_playerEngine);
            _cameraModel = new CameraModel(_camera);
            _inputHandler = new InputHandler();
            _inputHandler.LeftStickCommand = new MoveCommand(_playerStateMachine);
            _inputHandler.ACommand = new InteractCommand(_playerStateMachine);
            _rewardBehaviour = new RewardBehaviour(_reward);
            CreateVehicleModels();
            CreateDucklingModels();

            CheckDucks();

            StartCoroutine(LateInitialize());
        }

        private void CheckDucks()
        {
            foreach (var duckling in _duckBehaviours)
            {
                duckling.OnCaught += DuckCaught;
                duckling.OnScared += DuckScared;
            }
        }

        private void CreateDucklingModels()
        {
            foreach (var view in _ducklings)
            {
                DuckBehaviour controller = new DuckBehaviour(view);
                _duckBehaviours.Add(controller);
            }
        }

        private void CreateVehicleModels()
        {
            foreach (var view in _vehicles)
            {
                TrafficController controller = new TrafficController(view);
                _vehicleBehaviours.Add(controller);
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
            InputCamera();

            UpdateVehicles();
            UpdateDucks();
        }

        private void UpdateDucks()
        {
            foreach (var behaviour in _duckBehaviours)
            {
                behaviour.Update();
            }
        }

        private void InputCamera()
        {
            if (Input.GetKeyDown(KeyCode.C))
                _cameraModel.HasChanged = true;

            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            _cameraModel.MouseX = mouseX;
            _cameraModel.MouseY = mouseY;
        }

        private void UpdateVehicles()
        {
            foreach (var behaviour in _vehicleBehaviours)
            {
                behaviour.Update();
            }
        }

        private void FixedUpdateDucks()
        {
            foreach (var behaviour in _duckBehaviours)
            {
                behaviour.FixedUpdate();
            }
        }

        private void FixedUpdate()
        {
            _playerStateMachine.FixedUpdate();
            FixedUpdateVehicles();

            _cameraModel.FixedUpdate();
            FixedUpdateDucks();
        }

        private void FixedUpdateVehicles()
        {
            foreach (var behaviour in _vehicleBehaviours)
            {
                behaviour.FixedUpdate();
            }
        }

        private IEnumerator LateInitialize()
        {
            yield return new WaitForEndOfFrame();

            Initialized?.Invoke(this, EventArgs.Empty);
        }
    }
}