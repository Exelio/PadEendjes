using System;
using View;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace Model
{
    public class TrafficHubBehaviour
    {
        private TrafficHubView _view;
        private TrafficHubVariables _variables;
        private List<TrafficController> _controllerList = new List<TrafficController>();

        private bool _canSpawnCars;
        private bool _spawnCars;

        public TrafficHubBehaviour(TrafficHubView view, AudioManager audioManager)
        {
            _view = view;
            _variables = _view.Variables;
            _variables.AudioManager = audioManager;

            _canSpawnCars = true;
            _spawnCars = true;

            _view.StartCoroutine(SpawnVehicles());
        }

        public void FixedUpdateHub()
        {
            FixedUpdateVehicles();
        }

        private void FixedUpdateVehicles()
        {
            if (!_spawnCars) return;

            foreach (var controller in _controllerList)
            {
                controller?.FixedUpdate();
            }
        }

        public void PauzeCars()
        {
            _spawnCars = false;
            foreach (var controller in _controllerList)
            {
                if (controller == null) return;
                controller?.OnPauze();
            }
        }

        public void Resume()
        {
            Debug.Log("Spawn cars");
            _spawnCars = true;

            foreach (var controller in _controllerList)
            {
                if (controller == null) return;
                controller?.OnResume();
            }
        }

        public void AddController()
        {
            int number = GetNumber();
            VehicleView view = _view.CreateVehicleView(_variables.View.gameObject, _variables.StartPoints[number - 1].transform);

            view.StartWaypoint = _variables.StartPoints[number - 1];
            view.transform.parent = null;
            TrafficController controller = new TrafficController(view, _variables.AudioManager);

            _controllerList.Add(controller);
            _view.StartCoroutine(controller.FindTargetWithDelay(.2f));
            controller.OnDestroy += Remove;
            CheckCanSpawnCars();
        }

        private void CheckCanSpawnCars()
        {
            _canSpawnCars = _controllerList.Count < _variables.MaxCarsInScene;
        }

        public void SetForwardChecking(bool value)
        {
            foreach (var controller in _controllerList)
            {
                if (controller == null) return;
                controller?.ToggleForwardChecking(value);
            }
        }

        private int GetNumber()
        {
            int number = GetRandom(1, _variables.StartPoints.Length);
            return number;
        }

        private int GetRandom(int min, int max)
        {
            return UnityEngine.Random.Range(min, max + 1);
        }

        private void Remove(TrafficController controller)
        {
            _controllerList.Remove(controller);
            CheckCanSpawnCars();
        }

        private IEnumerator SpawnVehicles()
        {
            if (_spawnCars && _canSpawnCars)
                AddController();
            yield return new WaitForSeconds(_variables.TimeTillNextSpawn);

            _view.StartCoroutine(SpawnVehicles());
        }
    }
}