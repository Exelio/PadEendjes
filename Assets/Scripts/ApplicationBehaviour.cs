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
    public class ApplicationBehaviour : MonoBehaviour /*SingletonMonoBehaviour<ApplicationBehaviour>*/
    {
        public event EventHandler Initialized;

        #region Editor changeable variables

        [SerializeField] private AudioView _audio;
        [SerializeField] private PlayerView _player;
        [SerializeField] private CameraView _camera;
        [SerializeField] private RewardView _reward;
        [SerializeField] private DuckView[] _ducklings;
        [SerializeField] private TrafficHubView _trafficHUb;
        [SerializeField] private MistakeView _mistake;
        [SerializeField] private ButtonsBehaviour _button;
        [SerializeField] private PondView _pond;
        [SerializeField] private RallyView _rally;
        [SerializeField] private CoinView[] _coins;

        #endregion

        #region Models

        private AudioManager _audioManager;
        private PlayerEngine _playerEngine;
        private CameraEngine _cameraEngine;
        private PlayerStateMachine _playerStateMachine;
        private InputHandler _inputHandler;
        private RewardBehaviour _rewardBehaviour;
        private List<DuckBehaviour> _duckBehaviours = new List<DuckBehaviour>();
        private MistakeManager _mistakeManager;
        private RallyBehaviour _rallyBehaviour;
        private TrafficHubBehaviour _trafficHubBehaviour;

        #endregion

        private bool _pauzed;
        private bool _levelComplete;

        private void Start()
        {
            LockCursor(false,CursorLockMode.Locked);

            #region Model Creation

            _audioManager = new AudioManager(_audio);
            _playerEngine = new PlayerEngine(_player,_audioManager);
            _playerStateMachine = new PlayerStateMachine(_playerEngine);
            _cameraEngine = new CameraEngine(_camera, _player.transform);
            _mistakeManager = new MistakeManager(_mistake, _audioManager);
            _rewardBehaviour = new RewardBehaviour(_reward);
            _rallyBehaviour = new RallyBehaviour(_rally);
            _inputHandler = new InputHandler();
            _trafficHubBehaviour = new TrafficHubBehaviour(_trafficHUb, _audioManager);
            CreateDucklingModels();

            #endregion

            #region Command assigning

            _inputHandler.LeftStickCommand = new MoveCommand(_playerStateMachine);
            _inputHandler.RightStickCommand = new RotateCameraCommand(_cameraEngine);
            _inputHandler.YCommand = new CameraInteractCommand(_cameraEngine);

            #endregion

            #region Audio assigning

            _button.AudioManager = _audioManager;
            _pond.AudioManager = _audioManager;

            #endregion

            #region Reward assigning

            _reward.MaxDuckAmount = _ducklings.Length;
            _rewardBehaviour.ChangeText();

            #endregion

            #region Events

            _playerEngine.OnStreetInFront += ChangeCameraView;
            _playerEngine.OnMistake += AddMistake;
            _playerEngine.OnUnsaveSpotSet += _cameraEngine.SetCrossUnsaveSpot;

            _cameraEngine.OnMistake += AddMistake;
            _cameraEngine.OnLookedWell += LookedWell;
            _cameraEngine.OnCameraRotateToForward += _playerEngine.ToggleRotation;

            _mistakeManager.OnPopUp += PauzeGame;
            _mistakeManager.OnPopUpOver += ResumeGame;

            _button.OnPauze += PauzeGame;
            _button.OnResume += ResumeGame;

            _pond.OnTrigger += CheckEnoughDucks;
            _pond.OnLevelEnd += LevelComplete;

            CheckCoins();

            #endregion

            StartCoroutine(LateInitialize());
        }

        private void LookedWell(string obj)
        {
            _mistakeManager.LookCheck(obj);
        }

        #region Coin related

        private void CheckCoins()
        {
            foreach (var coin in _coins)
            {
                coin.OnCoinTrigger += AddCoin;
            }
        }

        private void AddCoin(int obj)
        {
            _rewardBehaviour.AddCoin(obj);
        }

        #endregion

        private void LevelComplete(Transform obj)
        {
            LockCursor(true, CursorLockMode.None);
            foreach (var duck in _duckBehaviours)
            {
                duck.OnTargetChange(obj);
            }

            _rewardBehaviour.CompletedLevel(); 
            _playerStateMachine.SetPlayerStateToIdle();
            _trafficHubBehaviour.PauzeCars();

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
            _trafficHubBehaviour.Resume();
            LockCursor(false, CursorLockMode.Locked);
        }

        private void PauzeGame()
        {
            _pauzed = true;
            _playerStateMachine.SetPlayerStateToIdle();
            _trafficHubBehaviour.PauzeCars();
            LockCursor(true,CursorLockMode.None);
        }

        private void AddMistake(Mistakes mistake)
        {
            _mistakeManager.OnMistake(mistake);
            _rewardBehaviour.AddMistake();
            _rallyBehaviour.PlaceRallyPoints(_player.transform, _playerEngine.DuckList.Count);

            Debug.Log("Triggered");

            for (int i = 0; i < _playerEngine.DuckList.Count; i++)
                _playerEngine.DuckList[i].GetComponent<DuckView>().OnGettingScared(_rallyBehaviour.RallyPoints[i].transform);

            _playerEngine.RemoveDucks();
        }

        private void ChangeCameraView(bool value)
        {
            _cameraEngine.ToggleAnchorPoint(value);
            _trafficHubBehaviour.SetForwardChecking(value);
        }

        #region Duck related

        private void CreateDucklingModels()
        {
            if (_ducklings.Length == 0) return;

            foreach (var view in _ducklings)
            {
                DuckBehaviour controller = new DuckBehaviour(view, _audioManager);
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
            _rewardBehaviour.CaughtDuck(_playerEngine.DuckList.Count);
        }

        private void UpdateDucks()
        {
            if (_ducklings.Length == 0) return;
            foreach (var behaviour in _duckBehaviours)
                behaviour.Update(_pauzed);
        }

        private void FixedUpdateDucks()
        {
            if (_ducklings.Length == 0) return;
            foreach (var behaviour in _duckBehaviours)
                behaviour.FixedUpdate();
        }

        #endregion

        private void Update()
        {
            UpdateDucks();
            if (_pauzed) return;

            if (_levelComplete) return;
            _inputHandler.Update();
            _button.OnGamePauze();
        }

        private void FixedUpdate()
        {
            FixedUpdateDucks();
            _cameraEngine.FixedCameraUpdate(_pauzed);
            _playerStateMachine.FixedUpdate();
            if (_pauzed || _levelComplete) 
            {
                _playerStateMachine.Direction = MathB.Vector2Conversion(0, 0);
                return; 
            }
            _trafficHubBehaviour.FixedUpdateHub();
        }

        private IEnumerator LateInitialize()
        {
            yield return new WaitForEndOfFrame();

            Initialized?.Invoke(this, EventArgs.Empty);
        }
    }
}