using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManagement : MonoBehaviour
{
    bool isSettingsMenu = false;
    public GameObject settingsMenuObject;
    public GameObject GlobalMenuObject;

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit");
    }

    public void ToggleToSettings()
    {
        isSettingsMenu = !isSettingsMenu;

        // En fonction de la valeur de isSettingsMenu, activer ou désactiver le GameObject
        if (settingsMenuObject != null)
        {
            settingsMenuObject.SetActive(isSettingsMenu);
            GlobalMenuObject.SetActive(!isSettingsMenu);
        }
    }

    public void LoadActualScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
        Time.timeScale = 1;
    }

    public void LoadBeforeScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex - 1);
        Time.timeScale = 1;
    }

    public void LoadNextScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex + 1);
        Time.timeScale = 1;
    }

    public void LoadStartScene()
    {
        SceneManager.LoadScene(0);
        Time.timeScale = 1;
    }
}
