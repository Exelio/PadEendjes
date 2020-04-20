using Utils;
using UnityEngine;

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

        private LayerMask _interactLayer;
        
        public void Initialize()
        {
            _stats.Rigidbody = gameObject.GetComponent<Rigidbody>();
            _stats.Collider = gameObject.GetComponent<CapsuleCollider>();
            _stats.InteractableObject = null;

            _interactLayer = LayerMask.NameToLayer("Interactable");
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

        private void OnTriggerExit()
        {
            _stats.InteractableObject = null;
        }
    }
}