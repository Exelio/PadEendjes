using Utils;
using UnityEngine;

namespace View
{
    public class CameraView : MonoBehaviour
    {
        public CameraStats Stats { get => _stats; set => _stats = value; }

        [SerializeField]
        private CameraStats _stats;
    }
}