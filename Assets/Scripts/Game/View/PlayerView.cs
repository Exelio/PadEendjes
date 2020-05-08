using Utils;
using UnityEngine;
using System;

namespace View
{
    [RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(CapsuleCollider))]
    public class PlayerView : MonoBehaviour
    {
        public PlayerStats Stats
        {
            get => _stats;
            set
            {
                _stats = value;
            }
        }

        [SerializeField] private Animator _animator;
        [SerializeField] private PlayerStats _stats;

        public event Action OnExitRoad;

        private LayerMask _interactLayer;
        private LayerMask _streetLayer;
        private LayerMask _crossingRoadLayer;

        public void Initialize()
        {
            _stats.Rigidbody = gameObject.GetComponent<Rigidbody>();
            _stats.Collider = gameObject.GetComponent<CapsuleCollider>();
            _stats.InteractableObject = null;

            _interactLayer = LayerMask.NameToLayer("Interactable");
            _streetLayer = LayerMask.NameToLayer("Street");

            _crossingRoadLayer = LayerMask.NameToLayer("CrossingRoad");
        }

        public void MotionAnimation(float direction)
        {
            _animator.SetFloat("Direction", direction);
        }

        public void SetBoredAnimation(bool value)
        {
            if(_animator.GetBool("IsBored") != value)
                _animator.SetBool("IsBored", value);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == _interactLayer)
                _stats.InteractableObject = other.gameObject;
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.layer == _crossingRoadLayer) _stats.IsOnCrossingRoad = true;
            if (other.gameObject.layer == _streetLayer || other.gameObject.layer == _crossingRoadLayer)
                _stats.IsOnStreet = true;
            else
                _stats.IsOnStreet = false;

        }

        private void OnTriggerExit()
        {
            _stats.InteractableObject = null;
            _stats.IsOnStreet = false;
            _stats.IsOnCrossingRoad = false;

            OnExitRoad?.Invoke();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, _stats.MaxRadius);
        }
    }
}