using Utils;
using UnityEngine;

namespace View
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerView : MonoBehaviour
    {
        public EnvironmentQueryConfig QueryConfig
        {
            get => _queryConfig;
            set
            {
                _queryConfig = value;
            }
        }

        public S_PlayerStats Stats
        {
            get => _stats;
            set
            {
                _stats = value;
            }
        }

        [SerializeField]
        private EnvironmentQueryConfig _queryConfig;

        [SerializeField]
        private S_PlayerStats _stats;
    }
}