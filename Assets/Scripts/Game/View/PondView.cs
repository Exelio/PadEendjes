using Model;
using System;
using UnityEngine;

public class PondView : MonoBehaviour
{
    public event Action<Transform> OnLevelEnd;
    public event Action OnTrigger;

    public AudioManager AudioManager;

    [SerializeField]
    private string _checkingTag = "Player";

    [SerializeField]
    private AudioSource _audioSource;

    private bool _isInteractable;
    private bool _isInTrigger;

    private void Start()
    {
        AudioManager.Play("PondAmbient", _audioSource);
    }

    public void ChangeInteractable(bool value)
    {
        _isInteractable = value;

        if(value)
            OnLevelEnd?.Invoke(this.transform);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == _checkingTag) 
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
