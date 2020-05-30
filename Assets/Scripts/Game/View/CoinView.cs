using System;
using UnityEngine;

public class CoinView : MonoBehaviour
{
    public event Action<int> OnCoinTrigger;

    [SerializeField] private int _coinValue;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            OnCoinTrigger?.Invoke(_coinValue);
            Destroy(this.gameObject);
        }
    }
}
