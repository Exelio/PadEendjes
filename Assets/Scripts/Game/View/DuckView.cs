using System;
using UnityEngine;

namespace View
{
    [Serializable]
    public struct DuckVariables
    {

    }

    [RequireComponent(typeof(AudioSource))]
    public class DuckView : MonoBehaviour
    {
        public event Action<Transform> OnCaught;
        public event Action<Transform> OnScared;

        public Transform FollowTarget { get => _followTarget; set { _followTarget = value; } }
        public float FollowSpeed => _followSpeed;
        public float TargetOffset => _targetOffset;
        public float MaxDistance => _maxDistance;
        public Animator Animator { get => _animator; set { _animator = value; } }
        public Vector2 TimeUntilIdleChange => _timeUntilIdleChange;
        public Rigidbody Rigidbody { get => _rigidbody; set { _rigidbody = value; } }

        public Transform Transform { get => transform; set { transform.position = value.position; transform.rotation = value.rotation; } }

        public Vector2 TimeBetweenAudio { get => _timeBetweenAudio; set => _timeBetweenAudio = value; }
        public AudioSource Source { get => _source; set => _source = value; }
        public Collider Trigger { get => _trigger; set => _trigger = value; }

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

        [Header("Audio variables")]
        [Tooltip("Max/Min time between audio clip played")]
        [SerializeField] private Vector2 _timeBetweenAudio;
        [SerializeField] private AudioSource _source; 

        private Transform _followTarget;

        public void OnInteract(Transform target)
        {
            if (_followTarget != target)
            {
                OnCaught?.Invoke(target);
            }
        }

        public void OnGettingScared(Transform target)
        {
            OnScared?.Invoke(target);
        }

        public void ResetAnimTrigger(string triggerName)
        {
            _animator.ResetTrigger(triggerName);
        }
    }
}