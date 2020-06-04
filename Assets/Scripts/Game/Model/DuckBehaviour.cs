using System;
using System.Collections;
using UnityEngine;
using View;

namespace Model
{
    public class DuckBehaviour
    {
        public event Action OnCaught;
        public event Action OnScared;

        private DuckView _view;
        private DuckVariables _variables;
        private Vector3 _lastPosition = Vector3.zero;

        private float _time;
        private float _timeTillIdleChange;
        private float _distance = 0;
        private float _animationTimer;

        private float _timeBetweenTargetChanges = 1.25f;
        private float _targetTimer;

        private bool _canTargetChange;

        private readonly AudioManager _audioManager;

        public DuckBehaviour(DuckView view, AudioManager audioManager)
        {
            _view = view;
            _variables = _view.Variables;
            _audioManager = audioManager;
            _view.OnCaught += DuckCaught;
            _view.OnScared += DuckScared;

            GetRandomTime();
        }

        private void DuckScared(Transform trans)
        {
            OnTargetChange(trans);

            _view.StartCoroutine(DuckPanic(3));
            OnScared?.Invoke();
        }

        private void DuckCaught(Transform trans)
        {
            if (!_canTargetChange) return;

            OnTargetChange(trans);
            OnCaught?.Invoke();

            _variables.Trigger.enabled = false;
        }

        public void Update(bool gamePauzed)
        {
            if (_distance <= _variables.MaxDistance)
                CheckIdleChange();

            CheckSpeed();
            PlaySoundAtRandom();

            if (gamePauzed) return;
            TargetChangeTimer();
        }

        private void TargetChangeTimer()
        {
            if(_targetTimer >= _timeBetweenTargetChanges)
            {
                _canTargetChange = true;
                _targetTimer = 0;

                if(!_view.IsCaught && !_variables.Trigger.enabled)
                    _variables.Trigger.enabled = true;
            }

            if (!_canTargetChange) _targetTimer += Time.deltaTime;
        }

        private void CheckSpeed()
        {
            _variables.Animator.SetFloat("DuckDistance", _distance);
        }

        public void OnTargetChange(Transform target)
        {
            _variables.FollowTarget = target;
            _canTargetChange = false;
        }

        public void FixedUpdate()
        {
            if (_variables.FollowTarget != null)
            {
                FollowTarget();
                LookAtTarget();
            }
        }

        private void CheckIdleChange()
        {
            if (_time >= _timeTillIdleChange)
            {
                _variables.Animator.SetTrigger("OnIdle2");
                GetRandomTime();
                _time = 0;
            }

            _time += Time.deltaTime;
        }

        private void GetRandomTime()
        {
            _timeTillIdleChange = UnityEngine.Random.Range(_variables.TimeUntilIdleChange.x, _variables.TimeUntilIdleChange.y);
        }

        private void LookAtTarget()
        {
            _view.transform.LookAt(_variables.FollowTarget.position);
        }

        private void FollowTarget()
        {
            _distance = Vector3.Distance(_view.transform.position, _variables.FollowTarget.position);
            if (_distance >= _variables.MaxDistance)
            {
                _view.transform.position = Vector3.Lerp(_view.transform.position,
                    _variables.FollowTarget.position - (_variables.FollowTarget.forward * _variables.TargetOffset),
                    _variables.FollowSpeed);
            }
            else
            {
                _lastPosition = _view.transform.position;
                _distance = 0;
            }
        }

        private void PlaySoundAtRandom()
        {
            if (_animationTimer <= 0f && !_view.IsCaught)
            {
                _animationTimer = UnityEngine.Random.Range(_variables.TimeBetweenAudio.x, _variables.TimeBetweenAudio.y);

                if (!_variables.Source.isPlaying)
                {
                    _audioManager.Play("DuckQuack", _variables.Source);
                    _variables.ParticleSystem.Play();
                }
            }
            else
                _animationTimer -= Time.deltaTime;
        }

        private IEnumerator DuckPanic(int amount)
        {
            int panicCount = 0;
            while(panicCount < amount)
            {
                if (!_variables.Source.isPlaying)
                {
                    yield return new WaitForSeconds(.1f);
                    panicCount++;
                    _audioManager.Play("DuckPanic", _variables.Source);
                    yield return null;
                }
                yield return null;
            }
        }
    }
}