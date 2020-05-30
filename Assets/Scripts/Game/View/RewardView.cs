using System;
using UnityEngine;
using UnityEngine.UI;

namespace View
{
    [Serializable]
    public struct RewardVariables
    {
        [Header("GUI")]
        public int DucksNeeded;
        public int MaxMistakes;
        public Text DuckCounter;
        public Text CoinCounter;

        [Header("EndPanel")]
        public GameObject EndPanel;
        public Image[] TopPanelStars;
        [Space]
        public Sprite NoStar;
        public Sprite Star;
        [Space]
        public Text LevelComplete;
        public Image StarLevelComplete;
        [Space]
        public Text FoundAllDucks;
        public Image StarFoundAllDucks;
        [Space]
        public Text Mistakes;
        public Image StarMistakes;

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