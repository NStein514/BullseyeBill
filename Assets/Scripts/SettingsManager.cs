using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance;

    [Header("UI Sliders")]
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        LoadSettingsUI();
    }

    public void OnMusicVolumeChanged(float value)
    {
        AudioManager.Instance.SetMusicVolume(value);
    }

    public void OnSFXVolumeChanged(float value)
    {
        AudioManager.Instance.SetSFXVolume(value);
    }

    public void LoadSettingsUI()
    {
        // Load saved settings
        SaveData data = SaveSystem.LoadGame();

        // Apply to audio manager 
        AudioManager.Instance.SetMusicVolume(data.musicVolume);
        AudioManager.Instance.SetSFXVolume(data.sfxVolume);

        // Update sliders 
        if (musicVolumeSlider != null)
            musicVolumeSlider.value = data.musicVolume;

        if (sfxVolumeSlider != null)
            sfxVolumeSlider.value = data.sfxVolume;
    }

    public void SaveSettings()
    {
        // Load existing data 
        SaveData data = SaveSystem.LoadGame();

        data.musicVolume = musicVolumeSlider.value;
        data.sfxVolume = sfxVolumeSlider.value;

        SaveSystem.SaveGame(data); 
    }
}
