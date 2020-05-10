using System;
using System.Collections;
using InputHandling;
using UnityEngine;
using Model;
using View;
using System.Collections.Generic;
using Utils;
using Boxsun.Math;

namespace Game
{
    public class ApplicationBehaviour : SingletonMonoBehaviour<ApplicationBehaviour>
    {
        public event EventHandler Initialized;

        [SerializeField] private PlayerView _player;
        [SerializeField] private CameraView _camera;
        [SerializeField] private RewardView _reward;
        [SerializeField] private DuckView[] _ducklings;
        [SerializeField] private TrafficHubView _trafficHUb;
        [SerializeField] private MistakeView _mistake;
        [SerializeField] private ButtonsBehaviour _button;
        [SerializeField] private PondView _pond;

        private PlayerEngine _playerEngine;
        private CameraEngine _cameraEngine;

        private PlayerStateMachine _playerStateMachine;

        private InputHandler _inputHandler;

        private RewardBehaviour _rewardBehaviour;
        private List<DuckBehaviour> _duckBehaviours = new List<DuckBehaviour>();

        private MistakeManager _mistakeManager;

        private bool _pauzed;
        private bool _levelComplete;

        private void Start()
        {
            LockCursor(false,CursorLockMode.Locked);
            
            _playerEngine = new PlayerEngine(_player);
            _playerStateMachine = new PlayerStateMachine(_playerEngine);
            _cameraEngine = new CameraEngine(_camera, _player.transform);
            _mistakeManager = new MistakeManager(_mistake);

            _inputHandler = new InputHandler();
            _inputHandler.LeftStickCommand = new MoveCommand(_playerStateMachine);
            _inputHandler.RightStickCommand = new RotateCameraCommand(_cameraEngine);
            _inputHandler.ACommand = new InteractCommand(_playerStateMachine);
            _inputHandler.YCommand = new CameraInteractCommand(_cameraEngine);

            _reward.MaxDuckAmount = _ducklings.Length;
            Debug.Log(_reward.MaxDuckAmount);
            _rewardBehaviour = new RewardBehaviour(_reward);
            CreateDucklingModels();

            _playerEngine.OnStreetInFront += ChangeCameraView;
            _playerEngine.OnMistake += AddMistake;

            _mistakeManager.OnPopUp += PauzeGame;
            _mistakeManager.OnPopUpOver += ResumeGame;

            _button.OnPauze += PauzeGame;
            _button.OnResume += ResumeGame;

            _pond.OnTrigger += CheckEnoughDucks;
            _pond.OnLevelEnd += LevelComplete;

            StartCoroutine(LateInitialize());
        }

        private void LevelComplete(Transform obj)
        {
            LockCursor(true, CursorLockMode.None);
            foreach (var duck in _duckBehaviours)
            {
                duck.OnTargetChange(obj);
            }

            _rewardBehaviour.CompletedLevel(); 
            _playerStateMachine.SetPlayerStateToIdle();
            _trafficHUb.PauzeCars();

            _levelComplete = true;
        }

        private void CheckEnoughDucks()
        {
            _pond.ChangeInteractable(_rewardBehaviour.CheckEnoughDucks());
        }

        private void LockCursor(bool value,CursorLockMode cursorMode)
        {
            if (Cursor.visible == value) return;
            Cursor.lockState = cursorMode;
            Cursor.visible = value;
        }

        private void ResumeGame()
        {
            _pauzed = false;
            _trafficHUb.Resume();
            LockCursor(false, CursorLockMode.Locked);
        }

        private void PauzeGame()
        {
            _pauzed = true;
            _playerStateMachine.SetPlayerStateToIdle();
            _trafficHUb.PauzeCars();
            LockCursor(true,CursorLockMode.None);
        }

        private void AddMistake(Mistakes mistake)
        {
            _mistakeManager.OnMistake(mistake);
            _rewardBehaviour.AddMistake();
        }

        private void ChangeCameraView(bool value)
        {
            _cameraEngine.ToggleAnchorPoint(value);
        }

        private void CreateDucklingModels()
        {
            if (_ducklings.Length == 0) return;

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
            if (_pauzed) return;
            UpdateDucks();

            if (_levelComplete) return;
            _inputHandler.Update();
            _button.OnGamePauze();
        }

        private void UpdateDucks()
        {
            if (_ducklings.Length == 0) return;
            foreach (var behaviour in _duckBehaviours)
                behaviour.Update();
        }

        private void FixedUpdateDucks()
        {
            if (_ducklings.Length == 0) return;
            foreach (var behaviour in _duckBehaviours)
                behaviour.FixedUpdate();
        }

        private void FixedUpdate()
        {
            _playerStateMachine.FixedUpdate();
            if (_pauzed || _levelComplete) 
            {
                _playerStateMachine.Direction = MathB.Vector2Conversion(0, 0);
                return; 
            }
            _cameraEngine.FixedCameraUpdate();
            _trafficHUb.FixedUpdateHub();

            FixedUpdateDucks();
        }

        private IEnumerator LateInitialize()
        {
            yield return new WaitForEndOfFrame();

            Initialized?.Invoke(this, EventArgs.Empty);
        }
    }
}