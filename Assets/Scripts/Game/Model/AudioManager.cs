﻿using View;
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

            //Initialize();
        }

        //private void Initialize()
        //{
        //    foreach(Sound audio in _sounds)
        //    {
        //        audio.AudioSource = _view.gameObject.AddComponent<AudioSource>();

        //        audio.AudioSource.clip = audio.AudioClip;
        //        audio.AudioSource.volume = audio.Volume;
        //        audio.AudioSource.pitch = audio.Pitch;
        //        audio.AudioSource.loop = audio.IsLooping;
        //        audio.AudioSource.spatialBlend = audio.SpatialBlend;
        //        audio.AudioSource.dopplerLevel = audio.DopplerLevel;
        //        audio.AudioSource.spread = audio.Spread;
        //        audio.AudioSource.minDistance = audio.MinDistance;
        //        audio.AudioSource.maxDistance = audio.MaxDistance;
        //    }
        //}

        //public void Play(string clipName)
        //{
        //    Sound sound = Array.Find(_sounds, AudioStats => AudioStats.ClipName == clipName);

        //    sound.AudioSource.Play();
        //}

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