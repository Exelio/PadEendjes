using View;
using Utils;
using UnityEngine;
using System;

namespace Model
{
    public class AudioManager
    {
        private AudioView _view;

        private Sound[] _sounds;

        public AudioManager(AudioView view)
        {
            _view = view;
            _sounds = _view.Sounds;
        }

        public void Play(string clipName, AudioSource source)
        {
            Sound sound = Array.Find(_sounds, AudioStats => AudioStats.ClipName == clipName);
            SetAudioSource(ref source, sound);

            source.Play();
        }

        private void SetAudioSource(ref AudioSource source, Sound audio)
        {
            source.clip = audio.AudioClip;
            source.volume = audio.Volume;
            source.pitch = audio.Pitch;
            source.loop = audio.IsLooping;
            source.spatialBlend = audio.SpatialBlend;
            source.dopplerLevel = audio.DopplerLevel;
            source.spread = audio.Spread;
            source.minDistance = audio.MinDistance;
            source.maxDistance = audio.MaxDistance;
        }
    }
}