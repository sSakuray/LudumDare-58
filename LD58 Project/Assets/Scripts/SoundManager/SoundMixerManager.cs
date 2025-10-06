using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundMixerManager : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider sfxSlider;
    
    private const string MASTER_VOLUME_KEY = "MasterVolume";
    private const string MUSIC_VOLUME_KEY = "MusicVolume";
    private const string SFX_VOLUME_KEY = "SFXVolume";

    private void Start()
    {
        if (masterSlider != null)
        {
            masterSlider.minValue = -80f;
            masterSlider.maxValue = 0f;
        }
        
        if (musicSlider != null)
        {
            musicSlider.minValue = -80f;
            musicSlider.maxValue = 0f;
        }
        
        if (sfxSlider != null)
        {
            sfxSlider.minValue = -80f;
            sfxSlider.maxValue = 0f;
        }
        
        LoadVolumeSettings();
    }

    public void SetMasterVolume()
    {
        float volume = masterSlider.value;
        audioMixer.SetFloat("MasterVolume", volume);
        PlayerPrefs.SetFloat(MASTER_VOLUME_KEY, volume);
        PlayerPrefs.Save();
    }

    public void SetMusicVolume()
    {
        float volume = musicSlider.value;
        audioMixer.SetFloat("MusicVolume", volume);
        PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, volume);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume()
    {
        float volume = sfxSlider.value;
        audioMixer.SetFloat("SFXVolume", volume);
        PlayerPrefs.SetFloat(SFX_VOLUME_KEY, volume);
        PlayerPrefs.Save();
    }
    
    private void LoadVolumeSettings()
    {
        float masterVolume = PlayerPrefs.GetFloat(MASTER_VOLUME_KEY, -20f);
        float musicVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, -20f);
        float sfxVolume = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, -20f);
        
        // Устанавливаем значения слайдеров
        if (masterSlider != null)
        {
            masterSlider.value = masterVolume;
        }
        
        if (musicSlider != null)
        {
            musicSlider.value = musicVolume;
        }
        
        if (sfxSlider != null)
        {
            sfxSlider.value = sfxVolume;
        }
        
        audioMixer.SetFloat("MasterVolume", masterVolume);
        audioMixer.SetFloat("MusicVolume", musicVolume);
        audioMixer.SetFloat("SFXVolume", sfxVolume);
    }
}
