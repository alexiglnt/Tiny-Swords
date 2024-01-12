using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public AudioMixer audioMixer;
    public GameObject pauseMenuUI;
    private float previousVolume;
    bool isMuted = false;

    public GameObject retractableBanner;
    bool isBannerOpened = false;



    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    void Resume()
    {
        Time.timeScale = 1f;
        pauseMenuUI.SetActive(false);
        GameIsPaused = false;
    }

    void Pause()
    {
        Time.timeScale = 0f;
        pauseMenuUI.SetActive(true);
        GameIsPaused = true;
    }

    void TurnOffVolume()
    {
        // Enregistrez le volume actuel
        audioMixer.GetFloat("volume", out previousVolume);

        // Baissez le volume
        audioMixer.SetFloat("volume", -40);
        isMuted = true;
    }

    void RestoreVolume()
    {
        // Rétablissez le volume précédent
        audioMixer.SetFloat("volume", previousVolume);
        isMuted = false;
    }

    public void SwitchVolume()
    {
        if (isMuted)
        {
            RestoreVolume();
        }
        else
        {
            TurnOffVolume();
        }
    }

    public void SwitchBanner()
    {
        Debug.Log("SwitchBanner()");

        if (isBannerOpened)
        {
            Vector3 currentPosition = retractableBanner.transform.position;
            float translationAmount = 130f; // Ajustez la valeur selon vos besoins
            retractableBanner.transform.position = new Vector3(currentPosition.x + translationAmount, currentPosition.y, currentPosition.z);
            
            isBannerOpened = false;
        }
        else
        {
            isBannerOpened = true;

            // Faites la translation vers la gauche de quelques pixels
            Vector3 currentPosition = retractableBanner.transform.position;
            float translationAmount = -130f; // Ajustez la valeur selon vos besoins
            retractableBanner.transform.position = new Vector3(currentPosition.x + translationAmount, currentPosition.y, currentPosition.z);
        }
    }


}
