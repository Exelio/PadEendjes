using System;
using UnityEngine;

public class DuckView : MonoBehaviour
{
    public event EventHandler OnCaught;

    public void OnInteract()
    {
        Debug.Log("Quack");
        OnCaught?.Invoke(this, EventArgs.Empty);
    }
}