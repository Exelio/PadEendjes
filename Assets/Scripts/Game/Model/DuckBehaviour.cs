using System;
using UnityEngine;
using View;

namespace Model
{
    public class DuckBehaviour
    {
        public event Action OnCaught;
        public event Action OnScared;

        private DuckView _view;
        private Vector3 _lastPosition = Vector3.zero;

        private float _time;
        private float _timeTillIdleChange;
        private float _distance = 0;
        private float _timer;

        private bool _isDuckCaught;

        private readonly AudioManager _audioManager;

        public DuckBehaviour(DuckView view, AudioManager audioManager)
        {
            _view = view;
            _audioManager = audioManager;
            _view.OnCaught += DuckCaught;
            _view.OnScared += DuckScared;

            GetRandomTime();
        }

        private void DuckScared(object sender, EventArgs e)
        {
            OnScared?.Invoke();
        }

        private void DuckCaught(object sender, EventArgs e)
        {
            OnCaught?.Invoke();

            _isDuckCaught = true;
        }

        public void Update()
        {
            if (_distance <= _view.MaxDistance)
                CheckIdleChange();

            CheckSpeed();
            PlaySoundAtRandom();
        }

        private void CheckSpeed()
        {
            _view.Animator.SetFloat("DuckDistance", _distance);
        }

        public void OnTargetChange(Transform target)
        {
            _view.FollowTarget = target;
        }

        public void FixedUpdate()
        {
            if (_view.FollowTarget != null)
            {
                FollowTarget();
                LookAtTarget();
            }
        }

        private void CheckIdleChange()
        {
            if (_time >= _timeTillIdleChange)
            {
                _view.Animator.SetTrigger("OnIdle2");
                GetRandomTime();
                _time = 0;
            }

            _time += Time.deltaTime;
        }

        private void GetRandomTime()
        {
            _timeTillIdleChange = UnityEngine.Random.Range(_view.TimeUntilIdleChange.x, _view.TimeUntilIdleChange.y);
        }

        private void LookAtTarget()
        {
            _view.Transform.LookAt(_view.FollowTarget.position);
        }

        private void FollowTarget()
        {
            _distance = Vector3.Distance(_view.transform.position, _view.FollowTarget.position);
            if (_distance >= _view.MaxDistance)
            {
                _view.transform.position = Vector3.Lerp(_view.Transform.position, _view.FollowTarget.position - (_view.FollowTarget.forward * _view.TargetOffset), _view.FollowSpeed);
            }
            else
            {
                _lastPosition = _view.transform.position;
                _distance = 0;
            }
        }

        private void PlaySoundAtRandom()
        {
            if (_timer <= 0f && !_isDuckCaught)
            {
                _timer = UnityEngine.Random.Range(_view.TimeBetweenAudio.x, _view.TimeBetweenAudio.y);

                _audioManager.Play("DuckQuack", _view.Source);
            }
            else
                _timer -= Time.deltaTime;
        }
    }
}