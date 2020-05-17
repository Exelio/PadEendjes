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
        private Sound[] _clips;

        public AudioManager(AudioView view)
        {
            _view = view;
            _sounds = _view.Sounds;
            _clips = _view.Clips;
        }

        public void Play(string clipName, AudioSource source)
        {
            Sound sound = Array.Find(_sounds, AudioStats => AudioStats.ClipName == clipName);
            SetAudioSource(ref source, sound);

            source.Play();
        }

        public void PlayRandomClip(AudioSource source)
        {
            Sound sound = _clips[UnityEngine.Random.Range(0, _clips.Length)];
            SetAudioSource(ref source, sound);
            source.PlayOneShot(sound.AudioClip);
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