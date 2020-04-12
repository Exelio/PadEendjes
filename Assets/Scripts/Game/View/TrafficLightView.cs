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
    [SerializeField] private Material _greenLight;
    [SerializeField] private Material _redlight;
    [SerializeField] private Material _orangeLight;

    private bool _isRedLight;
    public bool IsRedLight { get => _isRedLight; set { _isRedLight = value; } }

    public void Initialize()
    {
        StartCoroutine(ChangeLight());
    }

    private IEnumerator ChangeLight()
    {
        //Debug.Log($"Change light {name}");
        switch (_isRedLight)
        {
            case true:
                yield return new WaitForSeconds(_timeTillSwitch);
                _isRedLight = false;
                OnLightChange?.Invoke(_isRedLight);
                ChangeLightColor(_orangeLight);
                yield return new WaitForSeconds(_orangeLightTime);
                ChangeLightColor(_redlight);
                StartCoroutine(ChangeLight());
                break;
            case false:
                yield return new WaitForSeconds(_timeTillSwitch - _orangeLightTime);
                _isRedLight = true;
                OnLightChange?.Invoke(_isRedLight);
                ChangeLightColor(_greenLight);
                StartCoroutine(ChangeLight());
                break;
        }
    }

    private void ChangeLightColor(Material mat)
    {
        _greenLight.DisableKeyword("_EMISSION");
        _redlight.DisableKeyword("_EMISSION");
        _orangeLight.DisableKeyword("_EMISSION");

        mat.EnableKeyword("_EMISSION");
    }
}
