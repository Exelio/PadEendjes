﻿using System;
using UnityEngine;
using UnityEngine.UI;

namespace View
{
    [Serializable]
    public struct RewardVariables
    {
        public int DucksNeeded;
        public int MaxMistakes;
        public Text DuckCounter;

        [HideInInspector] public int MaxDucks;
        [HideInInspector] public bool IsLevelComplete;
        [HideInInspector] public int DuckCount;
        [HideInInspector] public int MistakeCount;
    }

    public class RewardView : MonoBehaviour
    {
        [SerializeField] private RewardVariables _variables;
        public RewardVariables Variables { get => _variables;}

        [HideInInspector] public int MaxDuckAmount;
        private void Start()
        {
            _variables.MaxDucks = MaxDuckAmount;
        }
    }
}