using System;
using UnityEngine;

namespace Utils
{
    [Serializable]
    public struct S_PlayerStats
    {
        public Rigidbody Rigidbody;

        public CapsuleCollider Collider;

        public float Gravity;
        public float JumpHeight;
        public float JumpMovementScalar;
        public float Speed;
        public float RotationSpeed;

        [HideInInspector]
        public Transform Transform => Rigidbody.transform;
    }
}