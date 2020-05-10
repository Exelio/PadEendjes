using System;
using UnityEngine;

public class PondView : MonoBehaviour
{
    public event Action<Transform> OnLevelEnd;
    public event Action OnTrigger;

    [SerializeField] private LayerMask _checkLayer;

    private bool _isInteractable;
    private bool _isInTrigger;

    public void ChangeInteractable(bool value)
    {
        _isInteractable = value;

        if(value)
            OnLevelEnd?.Invoke(this.transform);
    }

    //public void OnButtonPress()
    //{
    //    if (_isInTrigger && _isInteractable)
    //}

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player") 
        {
            if(!_isInTrigger)
                OnTrigger?.Invoke();

            _isInTrigger = true;
        }   
    }

    private void OnTriggerExit()
    {
        _isInTrigger = false;
    }
}
