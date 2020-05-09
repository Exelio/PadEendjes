using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonsBehaviour : MonoBehaviour
{
    public string StartingPanel;
    public GameObject[] Panels;

    public event Action OnPauze;
    public event Action OnResume;
    private void Start()
    {
        ChangePanel(StartingPanel);
    }

    
    public void LoadScene(string SceneToLoad)
    {
        SceneManager.LoadScene(SceneToLoad);
    }

    public void ChangePanel(string name)
    {
        foreach (var item in Panels)
        {
            if(item.name==name)
            {
                item.SetActive(true);
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
