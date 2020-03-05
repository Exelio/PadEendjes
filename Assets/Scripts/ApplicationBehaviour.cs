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

        private PlayerBehaviour _playerBehaviour;

        private InputHandler _inputHandler;

        private void Start()
        {
            _playerEngine = new PlayerEngine(_player);
            _playerBehaviour = new PlayerBehaviour(_playerEngine);
            _inputHandler = new InputHandler();
            _inputHandler.LeftStickCommand = new MoveCommand(_playerBehaviour);
            _inputHandler.ACommand = new JumpCommand(_playerBehaviour);

            StartCoroutine(LateInitialize());
        }

        private void Update()
        {
            _inputHandler.Update();
        }

        private void FixedUpdate()
        {
            _playerBehaviour.FixedUpdate();
        }

        private IEnumerator LateInitialize()
        {
            yield return new WaitForEndOfFrame();

            Initialized?.Invoke(this, EventArgs.Empty);
        }
    }
}