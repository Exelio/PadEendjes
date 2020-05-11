using System;
using UnityEngine;

namespace View
{
    public class DuckView : MonoBehaviour
    {
        public event EventHandler OnCaught;
        public event EventHandler OnScared;

        public Transform FollowTarget { get => _followTarget; set { _followTarget = value; } }
        public float FollowSpeed => _followSpeed;
        public float TargetOffset => _targetOffset;
        public float MaxDistance => _maxDistance;
        public Animator Animator { get => _animator; set { _animator = value; } }
        public Vector2 TimeUntilIdleChange => _timeUntilIdleChange;
        public Rigidbody Rigidbody { get => _rigidbody; set { _rigidbody = value; } }

        public Transform Transform { get => transform; set { transform.position = value.position; transform.rotation = value.rotation; } }

        [Header("Movement variables")]
        [SerializeField] private float _followSpeed;
        [SerializeField] private float _targetOffset;
        [Tooltip("Max distance between this and the target")] [SerializeField] private float _maxDistance = 0.5f;

        [Header("Animation variables")]
        [Tooltip("min max time")] [SerializeField] private Vector2 _timeUntilIdleChange;

        [Header("Component variables")]
        [SerializeField] private Animator _animator;
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private Collider _trigger;

        private Transform _followTarget;

        public void OnInteract(Transform target)
        {
            if (_followTarget == null)
            {
                _trigger.enabled = false;
                _followTarget = target;
                OnCaught?.Invoke(this, EventArgs.Empty);
            }
        }

        public void OnGettingScared()
        {
            _followTarget = null;
            OnScared?.Invoke(this, EventArgs.Empty);
        }

        public void ResetAnimTrigger(string triggerName)
        {
            _animator.ResetTrigger(triggerName);
        }
    }
}