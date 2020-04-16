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
        [SerializeField] private RewardView _reward;
        [SerializeField] private DuckView _duckling;

        private PlayerEngine _playerEngine;

        private PlayerStateMachine _playerStateMachine;

        private InputHandler _inputHandler;

        private RewardBehaviour _rewardBehaviour;

        private void Start()
        {
            _playerEngine = new PlayerEngine(_player);
            _playerStateMachine = new PlayerStateMachine(_playerEngine);
            _inputHandler = new InputHandler();
            _inputHandler.LeftStickCommand = new MoveCommand(_playerStateMachine);
            _inputHandler.ACommand = new InteractCommand(_playerStateMachine);
            _rewardBehaviour = new RewardBehaviour(_reward);

            _duckling.OnCaught += DuckCaught;
            _duckling.OnScared += DuckScared;

            StartCoroutine(LateInitialize());
        }

        private void DuckScared(object sender, EventArgs e)
        {
            _rewardBehaviour.LostDuck();
        }

        private void DuckCaught(object sender, EventArgs e)
        {
            _rewardBehaviour.CaughtDuck();
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