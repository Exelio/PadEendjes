﻿using Game;
using View;
using Utils;
using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;

namespace Model
{
    public class PlayerEngine
    {
        public event Action<bool> OnStreetInFront;
        public event Action<Mistakes> OnMistake;
        public event Action<bool> OnUnsaveSpotSet;

        public List<GameObject> DuckList { get => _duckList; set => _duckList = value; }

        public bool IsGrounded { get; private set; }

        public PlayerStats Stats => _stats;

        private PlayerStats _stats;

        private Vector3 _velocity;

        private RoadBehaviour _roadBehaviour;

        private readonly PlayerView _view;
        private readonly AudioManager _audioManager;
        private readonly EnvironmentQuery _query;

        private List<GameObject> _duckList = new List<GameObject>();

        private float _boredTimer;

        private bool _isStreetInFront;
        private bool _isCloseToCrossingRoad;
        private bool _isCrossingRoadInFront;

        private bool _mistakeCrossingRoad;
        private bool _mistakeStraightCross;

        private Vector3 _previousForwardPosition;
        private RaycastHit _hit;

        public PlayerEngine(PlayerView view,AudioManager audioManager)
        {
            view.Initialize();

            _view = view;
            _audioManager = audioManager;
            _stats = _view.Stats;
            _query = new EnvironmentQuery();
            _boredTimer = _stats.TimeTillBored;

            _view.OnExitRoad += ResetMistakeBools;
            _view.OnInteract += ApplyInteraction;
            _view.OnUnsaveSpot += OnUnsaveSpot;

            //ApplicationBehaviour.Instance.Initialized += (obj, args) => AssignPlayerStats();
            AssignPlayerStats();
        }

        private void ResetMistakeBools()
        {
            _mistakeCrossingRoad = false;
            _mistakeStraightCross = false;
            _mistakeUnsaveSpot = false;
        }

        public void FixedPlayerUpdate()
        {
            _stats = _view.Stats;

            IsGrounded = _query.IsGrounded(_stats.Transform.position, _stats.Collider.radius * 0.9f, _stats.WalkableLayer);
            
            _isCloseToCrossingRoad = _query.CastSphere(_stats.Transform.position, _stats.MaxRadius, _stats.CrossingRoadLayer);

            if (!_stats.IsOnCrossingRoad && !_stats.IsOnStreet)
            {
                _isCrossingRoadInFront = _query.ShootRay(ref _hit, _stats.Transform.position, _stats.DistanceToStreet / 2, _stats.Transform.forward, _stats.CrossingRoadLayer);
                _isStreetInFront = _query.ShootRay(ref _hit, _stats.Transform.position, _stats.DistanceToStreet, _stats.Transform.forward, _stats.StreetLayer);
            }

            CheckChangeIsStreetInFront();
            CheckCorrectStreetCross();
            PlayFootStepSounds();

            Commit();
            ApplyAnimation();
        }

        private void PlayFootStepSounds()
        {
            if(_view.Animator.GetFloat("FootCurve")>0.9)
            {
                _audioManager.PlayRandomClip(_stats.AudioSource);
            }
        }

        private void AssignRoad(RoadBehaviour roadBehaviour)
        {
            _roadBehaviour = roadBehaviour;
        }

        private void CheckCorrectStreetCross()
        {
            if (_stats.IsOnWalkingArea) return;

            if (_isCrossingRoadInFront) _stats.IsOnCrossingRoad = true;

            if (_mistakeCrossingRoad) return;

            if (_stats.IsOnStreet && _isCloseToCrossingRoad && !_stats.IsOnCrossingRoad && !_view.IsOnSafeRoad)
            {
                float distanceWalked = Vector3.Distance(_startPosition, _view.transform.position);

                if (distanceWalked > 1.5f)
                {
                    _mistakeCrossingRoad = true;
                    CountMistake(Mistakes.NotUsingCrossingRoad);
                }
            }

            if (_stats.IsOnStreet || _stats.IsOnCrossingRoad)
            {
                //CheckWalkInStraightLine();
            }
            else
            {
                _startPosition = _view.transform.position;
            }
        }

        private bool _mistakeUnsaveSpot;
        private void OnUnsaveSpot()
        {
            if(!_mistakeUnsaveSpot)
            {
                _mistakeUnsaveSpot = true;
                CountMistake(Mistakes.CrossingAtAUnsaveSpot);
                OnUnsaveSpotSet?.Invoke(_mistakeUnsaveSpot);
            }
        }

        private void CountMistake(Mistakes mistake)
        {
            OnMistake?.Invoke(mistake);
        }

        private Vector3 _startPosition;
        private void CheckWalkInStraightLine()
        {
            if (_mistakeStraightCross) return;

            float distanceWalked = Vector3.Distance(_startPosition, _view.transform.position);
            float difference = _query.CheckAngle(_view.transform.position, _startPosition);

            if ((difference > _stats.MaxAngleDifference || difference < -_stats.MaxAngleDifference) && distanceWalked > 1f)
            {
                _mistakeStraightCross = true;
                CountMistake(Mistakes.NotCrossingStraight);
            }
            _previousForwardPosition = _stats.Transform.forward;
        }

        private bool _isMeshActive = true;
        private void CheckChangeIsStreetInFront()
        {
            bool value = false;
            if (_isStreetInFront || _stats.IsOnStreet)
            {
                value = true;
                if (_isMeshActive) { _isMeshActive = false; _view.StartCoroutine(SetMesh(1f)); }
            }
            else
            {
                value = false;
                if (!_isMeshActive) { _isMeshActive = true; _stats.MeshObj.SetActive(true); }
            }
                       
            OnStreetInFront?.Invoke(value);
        }

        private IEnumerator SetMesh(float time)
        {
            yield return new WaitForSeconds(time);
            _stats.MeshObj.SetActive(false);
        }

        public void ApplyGravity()
        {
            _velocity.y += -_stats.Gravity * Time.deltaTime;
        }

        public void ApplyMovement(float horizontal, float vertical)
        {
            Vector3 cameraForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
            Vector3 cameraRight = Vector3.Scale(Camera.main.transform.right, new Vector3(1, 0, 1)).normalized;

            _velocity = cameraRight * horizontal + cameraForward * vertical;
        }

        private void ApplyAnimation()
        {
            _view.MotionAnimation(_velocity.normalized.magnitude);

            if(Mathf.Approximately(_velocity.normalized.magnitude, 0))
            {
                _boredTimer -= Time.deltaTime;
                if(_boredTimer <= 0f)
                {
                    _view.SetBoredAnimation(true);
                }
            }
            else
            {
                _boredTimer = _stats.TimeTillBored;
                _view.SetBoredAnimation(false);
            }
        }

        public void ApplyIdle()
        {
            _velocity = Vector3.zero;
        }

        private bool _canRotate = true;
        public void ToggleRotation(bool value)
        {
            if (_canRotate == value) _canRotate = !value;
        }

        public void ApplyRotation(float horizontal, float vertical)
        {
            //if (!_canRotate) return;
            if (horizontal != 0f || vertical != 0f)
            {
                Vector3 cameraForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
                Vector3 cameraRight = Vector3.Scale(Camera.main.transform.right, new Vector3(1, 0, 1)).normalized;
                Vector3 forwardDirection = cameraRight * horizontal + cameraForward * vertical;

                _stats.Transform.rotation = Quaternion.Lerp(_stats.Transform.localRotation, Quaternion.LookRotation(forwardDirection), _stats.RotationLerpSpeed);
            }
        }

        public void ApplyInteraction(GameObject obj)
        {
            DuckView view = obj?.GetComponent<DuckView>();

            if (view != null)
            {
                _duckList.Add(view.gameObject);
                CheckDuckList(view);
                _stats.InteractableObject = null;
            }
        }

        public void RemoveDucks()
        {
            _duckList.Clear();
        }

        private void CheckDuckList(DuckView view)
        {
            if (_duckList.Count <= 1)
            {
                view.OnInteract(_view.transform);
            }
            else
                view.OnInteract(_duckList[_duckList.Count - 2].transform);
        }

        private void AssignPlayerStats()
        {
            _stats.Rigidbody.useGravity = false;
            _stats.Rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
            _stats.Collider.center = new Vector3(0f, (_stats.Collider.height / 2) + 0.03f, 0f);
        }

        private void Commit() => _stats.Rigidbody.velocity = _velocity * _stats.Speed;
    }
}