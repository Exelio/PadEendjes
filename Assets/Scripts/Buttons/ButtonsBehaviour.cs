using Model;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonsBehaviour : MonoBehaviour
{
    public string StartingPanel;
    public GameObject[] Panels;

    public event Action OnPauze;
    public event Action OnResume;

    public AudioManager AudioManager;

    public AudioSource AudioSource;
    [SerializeField]
    private AudioClip _audioClip;

    private void Start()
    {
        ChangePanel(StartingPanel);
    }

    private void PlaySound()
    {
        if (AudioManager != null)
        {
            AudioManager.Play("Button", AudioSource);
        }
        else
        {
            AudioSource.clip = _audioClip;
            AudioSource.Play();
        }
            
    }
    
    public void LoadScene(string SceneToLoad)
    {
        PlaySound();
        SceneManager.LoadScene(SceneToLoad);
    }

    public void ChangePanel(string name)
    {
        foreach (var item in Panels)
        {
            if(item.name==name)
            {
                item.SetActive(true);
                PlaySound();
            }
            else
            {
                item.SetActive(false);
            }
        }
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void Resume()
    {
        PlaySound();
        OnResume?.Invoke();
        ChangePanel(null);
    }

    public void OnGamePauze()
    {
        if(Input.GetKeyDown(KeyCode.P)|| Input.GetKeyDown(KeyCode.Escape))
        {
            ChangePanel("Panel_Pauze");
            Pauze();
        }
    }

    private void Pauze()
    {
        OnPauze?.Invoke();
    }
}
