using Utils;
using UnityEngine;

namespace View
{
    public class AudioView : MonoBehaviour
    {
        public Sound[] Sounds { get => _sounds; set => _sounds = value; }
        public Sound[] Clips { get => _clips; set => _clips = value; }

        [SerializeField]
        private Sound[] _sounds;
        [SerializeField]
        private Sound[] _clips;
    }
}