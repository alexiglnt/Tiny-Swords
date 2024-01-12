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

    public GameObject SoundOffBtn;
    public GameObject InvisibleButton;


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
        
        Vector3 currentPosition = retractableBanner.transform.position;
        Vector3 currentPositionInvisibleBtn = InvisibleButton.transform.position;
        float translationAmount = 130f; // Ajustez la valeur selon vos besoins
        float translationAmountNegative = -130f; // Ajustez la valeur selon vos besoins


        if (isBannerOpened)
        {
            isBannerOpened = false;
            
            retractableBanner.transform.position = new Vector3(currentPosition.x + translationAmount, currentPosition.y, currentPosition.z);
            InvisibleButton.transform.position = new Vector3(currentPositionInvisibleBtn.x + translationAmount, currentPositionInvisibleBtn.y, currentPositionInvisibleBtn.z);

        }
        else
        {
            isBannerOpened = true;
            SoundOffBtn.transform.SetAsLastSibling();
            
            retractableBanner.transform.position = new Vector3(currentPosition.x + translationAmountNegative, currentPosition.y, currentPosition.z);    
            InvisibleButton.transform.position = new Vector3(currentPositionInvisibleBtn.x + translationAmountNegative, currentPositionInvisibleBtn.y, currentPositionInvisibleBtn.z);
        }
    }


}
