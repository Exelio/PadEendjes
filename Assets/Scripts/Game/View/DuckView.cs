using System;
using UnityEngine;

namespace View
{
    [Serializable]
    public struct DuckVariables
    {
        public Transform FollowTarget;

        [Header("Movement variables")]
        public float FollowSpeed;
        public float TargetOffset;
        public float MaxDistance;

        [Header("Animation variables")]
        public Animator Animator;
        public Vector2 TimeUntilIdleChange;

        [Header("Required components")]
        public Rigidbody Rigidbody;
        public Collider Trigger;

        [Header("Audio variables")]
        public AudioSource Source;
        public Vector2 TimeBetweenAudio;

        [Header("Particle variables")]
        public ParticleSystem ParticleSystem;
    }

    [RequireComponent(typeof(AudioSource))]
    public class DuckView : MonoBehaviour
    {
        public bool IsCaught;
        public event Action<Transform> OnCaught;
        public event Action<Transform> OnScared;

        public DuckVariables Variables => _variables;
        [SerializeField] private DuckVariables _variables;

        public void OnInteract(Transform target)
        {
            if (_variables.FollowTarget != target)
            {
                IsCaught = true;
                OnCaught?.Invoke(target);
            }
        }

        public void OnGettingScared(Transform target)
        {
            IsCaught = false;
            OnScared?.Invoke(target);
        }

        public void ResetAnimTrigger(string triggerName)
        {
            _variables.Animator.ResetTrigger(triggerName);
        }
    }
}