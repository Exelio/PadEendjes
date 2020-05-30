using System;
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

            switch (mistake)
            {
                case Mistakes.NotUsingCrossingRoad:
                    _view.Title.text = "gebruik zebrapaden";
                    _view.Text.text = "Wanneer er een zebrapad in de buurt is moet je hem gebruiken!";
                    break;
                case Mistakes.NotCrossingStraight:
                    _view.Title.text = "steek recht over";
                    _view.Text.text = "Steek loodrecht over ten opzichte van de straat zodat je snel aan de overkant bent.";
                    break;
                case Mistakes.NotLookingLeftAndRight:
                    _view.Title.text = "links en rechts kijken";
                    _view.Text.text = "Kijk links en recht voor je oversteekt zodat je eventueel aankomende voetuigen ziet";
                    break;
            }

            _view.PopUpPanel.SetActive(true);

            _audioManager.Play("Mistake", _view.AudioSource);
        }
    }
}
