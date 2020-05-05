using Game;
using View;
using Utils;
using UnityEngine;
using System.Collections.Generic;
using System;

namespace Model
{
    public class PlayerEngine
    {
        public event Action<bool> OnStreetInFront;

        public bool IsGrounded { get; private set; }

        public PlayerStats Stats => _stats;

        private PlayerStats _stats;

        private Vector3 _velocity;

        private readonly PlayerView _view;
        private readonly EnvironmentQuery _query;

        private List<GameObject> _duckList = new List<GameObject>();

        private float _boredTimer;

        private bool _isStreetInFront;
        private bool _isCloseToCrossingRoad;

        public PlayerEngine(PlayerView view)
        {
            view.Initialize();

            _view = view;
            _stats = _view.Stats;
            _query = new EnvironmentQuery();
            _boredTimer = _stats.TimeTillBored;

            ApplicationBehaviour.Instance.Initialized += (obj, args) => AssignPlayerStats();
        }

        public void FixedPlayerUpdate()
        {
            _stats = _view.Stats;

            IsGrounded = _query.IsGrounded(_stats.Transform.position, _stats.Collider.radius * 0.9f, _stats.WalkableLayer);
            _isStreetInFront = _query.IsStreetInFront(_stats.Transform.position, _stats.DistanceToStreet, _stats.Transform.forward, _stats.StreetLayer);
            CheckChangeIsStreetInFront();
            _isCloseToCrossingRoad = _query.IsCloseToCrossingRoad(_stats.Transform.position, 10f, _stats.CrossingRoadLayer);

            Commit();
            ApplyAnimation();
        }

        private void CheckChangeIsStreetInFront()
        {
            bool value = false;
            if (_isStreetInFront || _stats.IsOnStreet)
            {
                value = true;
            }
            else
            {
                value = false;
            }

            OnStreetInFront?.Invoke(value);
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

        public void ApplyRotation(float horizontal, float vertical)
        {
            if (horizontal != 0f || vertical != 0f)
            {
                Vector3 cameraForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
                Vector3 cameraRight = Vector3.Scale(Camera.main.transform.right, new Vector3(1, 0, 1)).normalized;
                Vector3 forwardDirection = cameraRight * horizontal + cameraForward * vertical;

                _stats.Transform.rotation = Quaternion.Lerp(_stats.Transform.localRotation, Quaternion.LookRotation(forwardDirection), _stats.RotationLerpSpeed);
            }
        }

        public void ApplyInteraction()
        {
            DuckView view = _stats.InteractableObject?.GetComponent<DuckView>();

            if (view != null && view.FollowTarget == null)
            {
                _duckList.Add(view.gameObject);
                CheckDuckList(view);
                _stats.InteractableObject = null;
            }
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