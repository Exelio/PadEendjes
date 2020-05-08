using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonsBehaviour : MonoBehaviour
{
    public string StartingPanel;
    public GameObject[] Panels;
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
}
