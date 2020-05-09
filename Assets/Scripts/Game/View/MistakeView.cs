using UnityEngine;
using UnityEngine.UI;

namespace View
{
    public class MistakeView : MonoBehaviour
    {
        [SerializeField] GameObject _popUpPanel;
        [SerializeField] Text _title;
        [SerializeField] Text _text;
        
        public GameObject PopUpPanel { get => _popUpPanel; set => _popUpPanel = value; }
        public Text Title { get => _title; set => _title = value; }
        public Text Text { get => _text; set => _text = value; }
    }
}
