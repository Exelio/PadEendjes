using System;
using System.Collections;
using UnityEngine;

public class TrafficLightView : MonoBehaviour
{
    public event Action<bool> OnLightChange;
    [Header("Logic variables")]
    [SerializeField] private float _timeTillSwitch;
    [SerializeField] private float _orangeLightTime = 1f;

    [Header("Looks variables")]
    [SerializeField] private Renderer _renderer;
    [SerializeField] private Material _greenLight, _redLight, _orangeLight;
    [SerializeField] private Material _greenLightOff, _redLightOff, _orangeLightOff;

    private Material[] _lightMaterials; 

    private bool _isRedLight;
    public bool IsRedLight { get => _isRedLight; set { _isRedLight = value; } }

    public void Initialize()
    {
        _lightMaterials = _renderer.sharedMaterials;

        if(!_isRedLight) ChangeLightColor(_redLight, _greenLightOff, _orangeLightOff);
        else ChangeLightColor(_redLightOff, _greenLight, _orangeLightOff);
        StartCoroutine(ChangeLight());
    }

    private IEnumerator ChangeLight()
    {
        switch (_isRedLight)
        {
            case true:
                yield return new WaitForSeconds(_timeTillSwitch);
                _isRedLight = false;
                OnLightChange?.Invoke(_isRedLight);
                ChangeLightColor(_redLightOff, _greenLightOff, _orangeLight);
                yield return new WaitForSeconds(_orangeLightTime);
                ChangeLightColor(_redLight, _greenLightOff, _orangeLightOff);
                StartCoroutine(ChangeLight());
                break;
            case false:
                yield return new WaitForSeconds(_timeTillSwitch - _orangeLightTime);
                _isRedLight = true;
                OnLightChange?.Invoke(_isRedLight);
                ChangeLightColor(_redLightOff, _greenLight, _orangeLightOff);
                StartCoroutine(ChangeLight());
                break;
        }
    }

    private void ChangeLightColor(Material red, Material green, Material orange)
    {
        _lightMaterials[2] = red;
        _lightMaterials[3] = orange;
        _lightMaterials[4] = green;

        _renderer.sharedMaterials = _lightMaterials;
    }
}
