﻿using Utils;
using UnityEngine;
using System;
using System.Collections;

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

        public Animator Animator { get => _animator; set => _animator = value; }

        public event Action<GameObject> OnInteract;

        [SerializeField] private Animator _animator;
        [SerializeField] private PlayerStats _stats;

        public event Action OnExitRoad;
        public event Action OnUnsaveSpot;

        public bool IsOnSafeRoad;

        private LayerMask _interactLayer;
        private LayerMask _streetLayer;
        private LayerMask _crossingRoadLayer;
        private LayerMask _unsaveLayer;
        private LayerMask _areaToWalkLayer;
        private LayerMask _safeRoadLayer;

        public void Initialize()
        {
            _stats.Rigidbody = gameObject.GetComponent<Rigidbody>();
            _stats.Collider = gameObject.GetComponent<CapsuleCollider>();
            _stats.AudioSource = gameObject.GetComponent<AudioSource>();

            _stats.InteractableObject = null;

            _interactLayer = LayerMask.NameToLayer("Interactable");
            _streetLayer = LayerMask.NameToLayer("Street");
            _crossingRoadLayer = LayerMask.NameToLayer("CrossingRoad");
            _unsaveLayer = LayerMask.NameToLayer("UnsaveSpots");
            _areaToWalkLayer = LayerMask.NameToLayer("WalkingArea");
            _safeRoadLayer = LayerMask.NameToLayer("SafeRoad");
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
            {
                _stats.InteractableObject = other.transform.parent.gameObject;
                OnInteract?.Invoke(other.transform.parent.gameObject);
            }

            if (other.gameObject.layer == _safeRoadLayer)
                IsOnSafeRoad = true;
        }

        private GameObject unsaveObj;
        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.layer == _crossingRoadLayer) _stats.IsOnCrossingRoad = true;

            if (other.gameObject.layer == _areaToWalkLayer) _stats.IsOnWalkingArea = true;

            if (other.gameObject.layer == _unsaveLayer && !_stats.IsOnStreet) { OnUnsaveSpot?.Invoke(); }

            if (other.gameObject.layer == _streetLayer || 
                other.gameObject.layer == _crossingRoadLayer || 
                other.gameObject.layer == _unsaveLayer || 
                other.gameObject.layer == _areaToWalkLayer ||
                IsOnSafeRoad)
            {
                _stats.IsOnStreet = true;
            }
            else
                _stats.IsOnStreet = false;


        }

        private void OnTriggerExit()
        {
            _stats.InteractableObject = null;
            _stats.IsOnStreet = false;
            _stats.IsOnCrossingRoad = false;
            _stats.IsOnWalkingArea = false;
            IsOnSafeRoad = false;

            OnExitRoad?.Invoke();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, _stats.MaxRadius);
        }
    }
}