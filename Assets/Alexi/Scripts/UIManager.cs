using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    
    public GameObject globalMenu;
    public GameObject settingsMenu;
    // public GameObject cardDisplayUI;

    void Start()
    {
        
    }

    
    void Update()
    {
        
    }

    public void OpenSettings()
    {
        globalMenu.SetActive(false);
        settingsMenu.SetActive(true);
    }

    public void CloseSettings()
    {
        globalMenu.SetActive(true);
        settingsMenu.SetActive(false);
    }

    public void OpenGlobalMenu()
    {
        globalMenu.SetActive(true);
        settingsMenu.SetActive(false);
    }

    public void CloseGlobalMenu()
    {
        globalMenu.SetActive(false);
        settingsMenu.SetActive(false);
    }
}
