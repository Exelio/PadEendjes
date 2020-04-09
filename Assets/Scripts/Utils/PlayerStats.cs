using System;
using UnityEngine;

namespace Utils
{
    [Serializable]
    public struct PlayerStats
    {
        [Header("Layer Properties")]
        [Tooltip("Assign the walkable layer")]
        public LayerMask WalkableLayer;

        [Header("Movement Parameters")]
        [Tooltip("Set the player's movement speed")]
        public float Speed;

        [Range(0f, 0.3f)]
        [Tooltip("Linear interpolation of the player's rotation")]
        public float RotationLerpSpeed;

        [Header("Gravity Parameters")]
        [Tooltip("Set the player's gravity")]
        public float Gravity;

        [HideInInspector]
        public Rigidbody Rigidbody;

        [HideInInspector]
        public CapsuleCollider Collider;

        [HideInInspector]
        public Transform Transform => Rigidbody.transform;

        [HideInInspector]
        public GameObject InteractableObject;
    }
}