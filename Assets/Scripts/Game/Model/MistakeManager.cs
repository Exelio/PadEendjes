using System;
using System.Collections;
using UnityEngine;
using Utils;
using View;

namespace Model
{
    public class MistakeManager
    {
        public event Action OnPopUp;
        public event Action OnPopUpOver;

        private MistakeView _view;
        private readonly AudioManager _audioManager;

        public MistakeManager(MistakeView view, AudioManager audioManager)
        {
            _view = view;
            _audioManager = audioManager;
        }

        public void OnMistake(Mistakes mistake)
        {
            OnPopUp?.Invoke();

            ChangeText(mistake);

            _view.PopUpPanel.SetActive(true);

            _audioManager.Play("Mistake", _view.AudioSource);
        }

        public void ChangeText(Mistakes mistake)
        {
            _view.Title.text = _view.Mistakes[mistake].TitleText;
            _view.Text.text = _view.Mistakes[mistake].MistakeTextDescription;
        }

        public void LookCheck(string text)
        {
            _view.LookedWellPanel.SetActive(true);
            _view.LookedWellText.text = text;

            _view.StartCoroutine(SetPanel());
        }

        private IEnumerator SetPanel()
        {
            yield return new WaitForSeconds(0.4f);
            _view.LookedWellPanel.SetActive(false);
        }
    }
}
