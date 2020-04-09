using System;
using UnityEngine;

public class DuckView : MonoBehaviour
{
    public event EventHandler OnCaught;
    public event EventHandler OnScared;

    public void OnInteract()
    {
        Debug.Log("Quack");
        OnCaught?.Invoke(this, EventArgs.Empty);
    }

    public void OnGettingScared()
    {
        OnScared?.Invoke(this, EventArgs.Empty);
    }
}