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

        public MistakeManager(MistakeView view)
        {
            _view = view;
        }

        public void OnMistake(Mistakes mistake)
        {
            OnPopUp?.Invoke();
        }
    }
}
