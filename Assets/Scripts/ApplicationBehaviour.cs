using System;
using System.Collections;
using InputHandling;
using UnityEngine;
using Model;
using View;

namespace Game
{
    public class ApplicationBehaviour : SingletonMonoBehaviour<ApplicationBehaviour>
    {
        public event EventHandler Initialized;

        [SerializeField] private PlayerView _player;

        private PlayerEngine _playerEngine;

        private PlayerStateMachine _playerStateMachine;

        private InputHandler _inputHandler;

        private void Start()
        {
            _playerEngine = new PlayerEngine(_player);
            _playerStateMachine = new PlayerStateMachine(_playerEngine);
            _inputHandler = new InputHandler();
            _inputHandler.LeftStickCommand = new MoveCommand(_playerStateMachine);
            _inputHandler.ACommand = new InteractCommand(_playerStateMachine);

            StartCoroutine(LateInitialize());
        }

        private void Update()
        {
            _inputHandler.Update();
        }

        private void FixedUpdate()
        {
            _playerStateMachine.FixedUpdate();
        }

        private IEnumerator LateInitialize()
        {
            yield return new WaitForEndOfFrame();

            Initialized?.Invoke(this, EventArgs.Empty);
        }
    }
}