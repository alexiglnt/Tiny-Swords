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
    public int colorBlindIndex;
}

public class SettingsMenu : MonoBehaviour
{
    public AudioMixer audioMixer;
    public TMPro.TMP_Dropdown resolutionDropdown;
    public TMPro.TMP_Dropdown colorBlindDropdown;
    public Slider volumeSlider;
    public Toggle fullscreenToggle;

    Resolution[] resolutions;
    GameSettings gameSettings = new GameSettings();

    private void Awake()
    {
        LoadSettingsFromJson();  // Charger les paramètres avant le Start des autres scripts

        //Debug.Log("AWAKE - Volume: " + gameSettings.volume + ", Resolution Index: " + gameSettings.resolutionIndex + ", Quality Index: " + gameSettings.qualityIndex + ", Fullscreen: " + gameSettings.isFullScreen);
        Debug.Log("AWAKE - COLORBLIND : " + gameSettings.colorBlindIndex);
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


        // Créer une liste de noms de types de daltonisme dans le même ordre que l'énumération d'origine
        List<string> colorBlindModeNames = new List<string>
        {
            "Normal",
            "Protanopia",
            "Protanomaly",
            "Deuteranopia",
            "Deuteranomaly",
            "Tritanopia",
            "Tritanomaly",
            "Achromatopsia",
            "Achromatomaly"
        };

        // Ajouter ces noms en tant qu'options au dropdown
        colorBlindDropdown.AddOptions(colorBlindModeNames);

        UpdateUI();
    }

    public void ApplySettings()
    {
        float volumeValue;
        audioMixer.GetFloat("volume", out volumeValue);

        // Mettre à jour les paramètres de gameSettings
        gameSettings.volume = volumeValue;
        gameSettings.resolutionIndex = resolutionDropdown.value;
        gameSettings.qualityIndex = QualitySettings.GetQualityLevel();
        gameSettings.isFullScreen = Screen.fullScreen;
        gameSettings.colorBlindIndex = colorBlindDropdown.value;

        // Sauvegarder les paramètres dans le fichier JSON
        SaveIntoJson();
    }

    void UpdateUI()
    {
        // Variable temporaire car ca ne fonctionne pas directement avec gameSettings.resolutionIndex (jsp pk)
        int resolutionIndexValue = gameSettings.resolutionIndex;
        int colorBlindIndexValue = gameSettings.colorBlindIndex;

        Debug.Log("ICIIII" + colorBlindIndexValue);

        volumeSlider.value = gameSettings.volume;
        resolutionDropdown.value = resolutionIndexValue;
        fullscreenToggle.isOn = gameSettings.isFullScreen;
        colorBlindDropdown.value = colorBlindIndexValue;
    }

    public void SaveIntoJson()
    {
        // Affichez les valeurs de gameSettings dans la console
        Debug.Log("Before Save - gameSettings: " + JsonUtility.ToJson(gameSettings));

        // Créer un objet GameSettings et le convertir en JSON
        string gameSettingsVar = JsonUtility.ToJson(gameSettings);

        // Écrire le JSON dans un fichier
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
            // Debug.Log("JSON loaded - Volume: " + gameSettings.volume + ", Resolution Index: " + gameSettings.resolutionIndex + ", Quality Index: " + gameSettings.qualityIndex + ", Fullscreen: " + gameSettings.isFullScreen);
            Debug.Log("COLORBLIND : " + gameSettings.colorBlindIndex);

        }
        else
        {
            Debug.Log("JSON VIIIIIIIIIDE");

            // Si le fichier JSON est vide, utiliser des paramètres par défaut
            gameSettings = new GameSettings
            {
                volume = 1f,  // Volume maximal par défaut
                resolutionIndex = 0,  // Première résolution par défaut
                qualityIndex = 2,  // Qualité moyenne par défaut
                isFullScreen = true,  // Mode plein écran par défaut
                colorBlindIndex = 0  // Mode normal par défaut
            };
        }
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        // SaveIntoJson();  // Sauvegarder les paramètres après modification
        ApplySettings();
    }

    public void SetColorBlind(int colorBlindIndex)
    {
        // Mettre à jour la valeur de colorBlindIndex dans gameSettings avec la nouvelle valeur
        gameSettings.colorBlindIndex = colorBlindIndex;

        // Sauvegarder les paramètres dans le fichier JSON
        SaveIntoJson();
    }


    public void SetResolutionDropdownValue(int dropdownIndex)
    {
        // Récupérez la valeur du dropdown (par exemple, "1920x1080")
        string selectedResolution = resolutionDropdown.options[dropdownIndex].text;

        // Utilisez la nouvelle méthode pour définir la résolution
        SetResolutionByText(selectedResolution);
    }

    public void SetResolutionByText(string resolutionText)
    {
        // Parsez la résolution depuis le format "1920x1080"
        string[] dimensions = resolutionText.Split('x');
        if (dimensions.Length == 2)
        {
            int width = int.Parse(dimensions[0]);
            int height = int.Parse(dimensions[1]);

            // Trouvez l'index de la résolution correspondante dans la liste
            int resolutionIndex = System.Array.FindIndex(resolutions, r => r.width == width && r.height == height);
            if (resolutionIndex != -1)
            {
                // Définir la résolution et sauvegarder les paramètres
                SetResolution(resolutionIndex);
            }
        }
    }

    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("volume", volume);
        //SaveIntoJson();  // Sauvegarder les paramètres après modification
        ApplySettings();
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        //SaveIntoJson();  // Sauvegarder les paramètres après modification
        ApplySettings();
    }

    public void SetFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
        //SaveIntoJson();  // Sauvegarder les paramètres après modification
        ApplySettings();
    }
}
