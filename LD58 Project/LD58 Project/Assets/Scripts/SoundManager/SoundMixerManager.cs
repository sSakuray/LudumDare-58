using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundMixerManager : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider sfxSlider;

    private void Start()
    {
        // Настройка диапазонов слайдеров для децибел (-80 до 0)
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
    }

    public void SetMasterVolume()
    {
        float volume = masterSlider.value;
        audioMixer.SetFloat("MasterVolume", volume);
    }

    public void SetMusicVolume()
    {
        float volume = musicSlider.value;
        audioMixer.SetFloat("MusicVolume", volume);
    }

    public void SetSFXVolume()
    {
        float volume = sfxSlider.value;
        audioMixer.SetFloat("SFXVolume", volume);
    }
}
