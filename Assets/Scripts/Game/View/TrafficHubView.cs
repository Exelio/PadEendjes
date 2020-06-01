using Model;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace View
{
    [Serializable]
    public struct TrafficHubVariables
    {
        public VehicleView View;
        public Waypoint[] StartPoints;

        public float TimeTillNextSpawn;
        public int MaxCarsInScene;

        public AudioManager AudioManager;
    }

    public class TrafficHubView : MonoBehaviour
    {
        public event Action OnAddVehicle;

        public TrafficHubVariables Variables => _variables;
        [SerializeField] private TrafficHubVariables _variables;

        public VehicleView CreateVehicleView(GameObject obj, Transform startPoint)
        {
            GameObject go = Instantiate(obj, startPoint);
            VehicleView view = go.GetComponent<VehicleView>();

            return view;
        }
    }
}