using System;
using UnityEngine;

namespace View
{
    [Serializable]
    public struct RewardVariables
    {
        public int DucksNeeded;
        public int MaxDucks;
        public int MaxMistakes;

        [HideInInspector] public bool IsLevelComplete;
        [HideInInspector] public int DuckCount;
        [HideInInspector] public int MistakeCount;
    }

    public class RewardView : MonoBehaviour
    {
        [SerializeField] private RewardVariables _variables;
        public RewardVariables Variables { get => _variables; }
    }
}