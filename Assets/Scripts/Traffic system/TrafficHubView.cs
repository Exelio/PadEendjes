using Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficHubView : MonoBehaviour
{
    private List<TrafficController> _controllerList = new List<TrafficController>();

    [SerializeField] private VehicleView _view;
    public AudioManager AudioManager;
    [SerializeField] private Waypoint[] _startPoints;
    [SerializeField] private float _timeTillNextSpawn = 2f;
    [SerializeField] private int _maxCarsInScene;

    private int spawnAmount;
    private List<int> numbersGotten = new List<int>();

    private bool _spawnCars = true;

    private void Start()
    {
        spawnAmount = GetRandom(1, _startPoints.Length);
        StartCoroutine(SpawnVehicles(spawnAmount));
    }

    public void FixedUpdateHub()
    {
        FixedUpdateVehicles();
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
        _spawnCars = true;
    }

    private void FixedUpdateVehicles()
    {
        foreach (var controller in _controllerList)
        {
            if (controller == null) return;
            controller?.FixedUpdate();
        }
    }

    private int GetRandom(int min, int max)
    {
        return UnityEngine.Random.Range(min, max + 1);
    }

    IEnumerator SpawnVehicles(int amount)
    {
        if(_spawnCars && _controllerList.Count < _maxCarsInScene)
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
        view.transform.parent = null;
        TrafficController controller = new TrafficController(view, AudioManager);
        StartCoroutine(controller.FindTargetWithDelay(.2f));
        _controllerList.Add(controller);

        controller.OnDestroy += Remove;
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
        int number = GetRandom(1, _startPoints.Length);
        return number;
    }

    private void Remove(TrafficController controller)
    {
        _controllerList.Remove(controller);
    }
}
