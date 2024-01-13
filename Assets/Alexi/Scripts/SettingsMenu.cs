using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using System.IO;  // Ajout de cette ligne pour utiliser la classe File

[System.Serializable]
public class GameSettings
{
    public float volume;
    public int resolutionIndex;
    public int qualityIndex;
    public bool isFullScreen;
}

public class SettingsMenu : MonoBehaviour
{
    public AudioMixer audioMixer;
    public TMPro.TMP_Dropdown resolutionDropdown;
    public Slider volumeSlider;
    public Toggle fullscreenToggle;

    Resolution[] resolutions;
    GameSettings gameSettings = new GameSettings();

    private void Awake()
    {
        LoadSettingsFromJson();  // Charger les param�tres avant le Start des autres scripts

        Debug.Log("AWAKE - Volume: " + gameSettings.volume + ", Resolution Index: " + gameSettings.resolutionIndex + ", Quality Index: " + gameSettings.qualityIndex + ", Fullscreen: " + gameSettings.isFullScreen);
    }

    private void Start()
    {
        resolutions = Screen.resolutions;

        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();

        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + "x" + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
                currentResolutionIndex = i;
        }

        resolutionDropdown.AddOptions(options);

        Debug.Log("START - Volume: " + gameSettings.volume + ", Resolution Index: " + gameSettings.resolutionIndex + ", Quality Index: " + gameSettings.qualityIndex + ", Fullscreen: " + gameSettings.isFullScreen);
        UpdateUI();
    }

    public void ApplySettings()
    {
        float volumeValue;
        audioMixer.GetFloat("volume", out volumeValue);

        // Mettre � jour les param�tres de gameSettings
        gameSettings.volume = volumeValue;
        gameSettings.resolutionIndex = resolutionDropdown.value;
        gameSettings.qualityIndex = QualitySettings.GetQualityLevel();
        gameSettings.isFullScreen = Screen.fullScreen;

        // Sauvegarder les param�tres dans le fichier JSON
        SaveIntoJson();
    }

    void UpdateUI()
    {
        // Variable temporaire car ca ne fonctionne pas directement avec gameSettings.resolutionIndex (jsp pk)
        int resolutionIndexValue = gameSettings.resolutionIndex;

        volumeSlider.value = gameSettings.volume;
        resolutionDropdown.value = resolutionIndexValue;
        fullscreenToggle.isOn = gameSettings.isFullScreen;
    }

    public void SaveIntoJson()
    {
        // Affichez les valeurs de gameSettings dans la console
        Debug.Log("Before Save - gameSettings: " + JsonUtility.ToJson(gameSettings));

        // Cr�er un objet GameSettings et le convertir en JSON
        string gameSettingsVar = JsonUtility.ToJson(gameSettings);

        // �crire le JSON dans un fichier
        File.WriteAllText(Application.persistentDataPath + "/SettingsData.json", gameSettingsVar);

        Debug.Log("Settings saved into JSON file : " + gameSettingsVar);
    }


    void LoadSettingsFromJson()
    {
        // Lire le JSON depuis un fichier
        string json = File.ReadAllText(Application.persistentDataPath + "/SettingsData.json");

        // Si le fichier JSON n'est pas vide, convertir le JSON en GameSettings
        if (!string.IsNullOrEmpty(json))
        {
            gameSettings = JsonUtility.FromJson<GameSettings>(json);
            Debug.Log("JSON loaded - Volume: " + gameSettings.volume + ", Resolution Index: " + gameSettings.resolutionIndex + ", Quality Index: " + gameSettings.qualityIndex + ", Fullscreen: " + gameSettings.isFullScreen);

        }
        else
        {
            Debug.Log("JSON VIIIIIIIIIDE");

            // Si le fichier JSON est vide, utiliser des param�tres par d�faut
            gameSettings = new GameSettings
            {
                volume = 1f,  // Volume maximal par d�faut
                resolutionIndex = 0,  // Premi�re r�solution par d�faut
                qualityIndex = 2,  // Qualit� moyenne par d�faut
                isFullScreen = true  // Mode plein �cran par d�faut
            };
        }
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        // SaveIntoJson();  // Sauvegarder les param�tres apr�s modification
        ApplySettings();
    }

    public void SetResolutionDropdownValue(int dropdownIndex)
    {
        // R�cup�rez la valeur du dropdown (par exemple, "1920x1080")
        string selectedResolution = resolutionDropdown.options[dropdownIndex].text;

        // Utilisez la nouvelle m�thode pour d�finir la r�solution
        SetResolutionByText(selectedResolution);
    }

    public void SetResolutionByText(string resolutionText)
    {
        // Parsez la r�solution depuis le format "1920x1080"
        string[] dimensions = resolutionText.Split('x');
        if (dimensions.Length == 2)
        {
            int width = int.Parse(dimensions[0]);
            int height = int.Parse(dimensions[1]);

            // Trouvez l'index de la r�solution correspondante dans la liste
            int resolutionIndex = System.Array.FindIndex(resolutions, r => r.width == width && r.height == height);
            if (resolutionIndex != -1)
            {
                // D�finir la r�solution et sauvegarder les param�tres
                SetResolution(resolutionIndex);
            }
        }
    }

    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("volume", volume);
        //SaveIntoJson();  // Sauvegarder les param�tres apr�s modification
        ApplySettings();
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        //SaveIntoJson();  // Sauvegarder les param�tres apr�s modification
        ApplySettings();
    }

    public void SetFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
        //SaveIntoJson();  // Sauvegarder les param�tres apr�s modification
        ApplySettings();
    }
}
