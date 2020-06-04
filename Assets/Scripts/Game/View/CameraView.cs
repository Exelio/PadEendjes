using Utils;
using UnityEngine;

namespace View
{
    public class CameraView : MonoBehaviour
    {
        public GameObject LeftPoint;
        public GameObject RightPoint;

        public CameraStats Stats { get => _stats; set => _stats = value; }

        [SerializeField]
        private CameraStats _stats;
    }
}