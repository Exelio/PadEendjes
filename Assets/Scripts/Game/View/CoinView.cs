using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinView : MonoBehaviour
{
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

        }
    }
}
