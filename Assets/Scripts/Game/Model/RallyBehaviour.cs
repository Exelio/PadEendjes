using System.Collections.Generic;
using UnityEngine;
using View;

namespace Model
{
    public class RallyBehaviour
    {
        public List<GameObject> RallyPoints { get => _rallyPoints; set => _rallyPoints = value; }

        private List<GameObject> _rallyPoints = new List<GameObject>();

        private bool _isToggled;

        private readonly RallyView _view;

        public RallyBehaviour(RallyView view)
        {
            _view = view;
        }

        public void PlaceRallyPoints(Transform spawnPosition, int amount)
        {
            foreach (GameObject go in _rallyPoints)
                _view.DestroyRallyPoint(go);

            _rallyPoints.Clear();

            for (int i = 0; i < amount; i++)
            {
                _view.Offset = new Vector3(GetRandomFloat(), 0f, GetRandomFloat());

                Vector3 position = (spawnPosition.position - spawnPosition.forward) + _view.Offset;

                _rallyPoints.Add(_view.InstantiateRallyPoint(position));
            }
        }

        private float GetRandomFloat()
        {
            return Random.Range(_view.OffsetRandomizer.x, _view.OffsetRandomizer.y);
        }
    }
}