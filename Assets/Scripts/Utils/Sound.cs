using System;
using UnityEngine;


namespace Utils
{
    [Serializable]
    public class Sound
    {
        [Header("Audio Title")]
        public string ClipName;

        [Header("Audio Clip")]
        public AudioClip AudioClip;

        [Header("Audio Stats")]
        public float Volume;
        public float Pitch;
        
        [Space]
        public bool IsLooping;

        [Header("3D Sound Settings")]
        [Range(0, 1)]
        public float SpatialBlend;

        [Range(0, 5)]
        public float DopplerLevel;

        [Range(0, 360)]
        public float Spread;
        
        [Space]
        public float MinDistance;
        public float MaxDistance;

        [HideInInspector]
        public AudioSource AudioSource;
    }
}