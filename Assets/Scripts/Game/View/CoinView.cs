using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinView : MonoBehaviour
{
    public event Action<int> OnCoinTrigger;

    private int _coinValue;
    // Start is called before the first frame update
    void Start()
    {
        _coinValue = 1;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            OnCoinTrigger?.Invoke(_coinValue);
            Destroy(this.gameObject);
        }
    }
}
