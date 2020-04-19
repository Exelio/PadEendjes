using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficHub : MonoBehaviour
{
    private List<TrafficController> _controllerList = new List<TrafficController>();

    [SerializeField] private VehicleView _view;
    [SerializeField] private Waypoint[] _startPoints;
    [SerializeField] private float _timeTillNextSpawn = 2f;

    private int spawnAmount;
    private List<int> numbersGotten = new List<int>();

    private void Start()
    {
        spawnAmount = GetRandom(1, _startPoints.Length);
        StartCoroutine(SpawnVehicles(spawnAmount));
    }

    private void Update()
    {
        UpdateVehicles();
    }

    private void FixedUpdate()
    {
        FixedUpdateVehicles();
    }

    private void UpdateVehicles()
    {
        foreach (var controller in _controllerList)
        {
            controller.Update();
        }
    }

    private void FixedUpdateVehicles()
    {
        foreach (var controller in _controllerList)
        {
            controller.FixedUpdate();
        }
    }

    private int GetRandom(int min, int max)
    {
        return UnityEngine.Random.Range(min, max + 1);
    }

    IEnumerator SpawnVehicles(int amount)
    {
        CreateVehicleModels(amount);
        yield return new WaitForSeconds(_timeTillNextSpawn);

        spawnAmount = GetRandom(1, _startPoints.Length);
        StartCoroutine(SpawnVehicles(amount));
    }

    private void CreateVehicleModels(int amount)
    {
        int number = GetNumber();
        GameObject obj = Instantiate(_view.gameObject, _startPoints[number - 1].transform);
        VehicleView view = obj.GetComponent<VehicleView>();
        view.StartWaypoint = _startPoints[number - 1];
        TrafficController controller = new TrafficController(view);
        _controllerList.Add(controller);

        controller.OnDestroy += Remove;
    }

    private int GetNumber()
    {
        int number = GetRandom(1, _startPoints.Length);
        return number;
    }

    private void Remove(TrafficController controller)
    {
        _controllerList.Remove(controller);
    }
}
