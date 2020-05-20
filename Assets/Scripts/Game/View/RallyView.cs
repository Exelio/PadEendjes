using UnityEngine;

namespace View
{
    public class RallyView : MonoBehaviour
    {
        public Vector2 OffsetRandomizer { get => _offsetRandomizer; set => _offsetRandomizer = value; }

        public Vector3 Offset { get; set; }

        [SerializeField]
        private GameObject _rallyPoint;

        [SerializeField]
        private Vector2 _offsetRandomizer;

        public GameObject InstantiateRallyPoint(Vector3 position)
        {
            GameObject gameObject = Instantiate(_rallyPoint, position, Quaternion.identity);
            
            gameObject.transform.parent = transform;

            return gameObject;
        }

        public void DestroyRallyPoint(GameObject go)
        {
            Destroy(go);
        }
    }
}