using Utils;
using UnityEngine;

namespace View
{
    public class AudioView : MonoBehaviour
    {
        public Sound[] Sounds { get => _sounds; set => _sounds = value; }

        [SerializeField]
        private Sound[] _sounds;
    }
}