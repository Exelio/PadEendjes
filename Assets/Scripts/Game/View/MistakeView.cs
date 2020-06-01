using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace View
{
    [Serializable]
    public class MistakeText
    {
        public Mistakes MistakeKind;
        [TextArea(1, 2)] public string TitleText;
        [TextArea(3, 5)] public string MistakeTextDescription;
    }

    public class MistakeView : MonoBehaviour
    {
        public GameObject PopUpPanel { get => _popUpPanel; set => _popUpPanel = value; }
        public GameObject LookedWellPanel { get => _lookedWellPanel; set { _lookedWellPanel = value; } }
        public Text LookedWellText { get => _lookedWellText; set { _lookedWellText = value; } }
        public Text Title { get => _title; set => _title = value; }
        public Text Text { get => _text; set => _text = value; }
        public AudioSource AudioSource;

        public Dictionary<Mistakes, MistakeText> Mistakes = new Dictionary<Mistakes, MistakeText>();

        [SerializeField] private GameObject _popUpPanel;
        [SerializeField] private GameObject _lookedWellPanel;
        [SerializeField] private Text _lookedWellText;
        [SerializeField] private Text _title;
        [SerializeField] private Text _text;
        [SerializeField] private MistakeText[] _mistakes;

        private void Start()
        {
            foreach (var mistake in _mistakes)
            {
                Mistakes.Add(mistake.MistakeKind, mistake);
            }
        }
    }
}
