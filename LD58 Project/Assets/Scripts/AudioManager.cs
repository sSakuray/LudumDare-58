using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] private GameObject musicAudioObject;
    [SerializeField] private GameObject sfxAudioObject;
    public Slider musicVolumeSlider;
    [Range(0, 10)]
    public float musicSliderValue = 10f;
    
    public Slider sfxVolumeSlider;
    [Range(0, 10)]
    public float sfxSliderValue = 10f;
    
    public Slider masterVolumeSlider;
    [Range(0, 10)]
    public float masterSliderValue = 10f;

    private AudioSource[] musicAudioSources;
    private AudioSource[] sfxAudioSources;
    
    private float currentMasterVolume = 10f;
    private float currentMusicVolume = 10f;
    private float currentSFXVolume = 10f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (musicAudioObject == null)
        {
            GameObject found = GameObject.Find("MusicAudioObject");
            if (found != null) musicAudioObject = found;
        }
        
        if (sfxAudioObject == null)
        {
            GameObject found = GameObject.Find("SFXAudioObject");
            if (found != null) sfxAudioObject = found;
        }
        
        CacheAudioSources();
        ApplyMusicVolume();
        ApplySFXVolume();
    }

    private void Start()
    {
        CacheAudioSources();

        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.minValue = 0;
            musicVolumeSlider.maxValue = 10;
            musicVolumeSlider.value = 10f;
            musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
        }
        
        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.minValue = 0;
            sfxVolumeSlider.maxValue = 10;
            sfxVolumeSlider.value = 10f;
            sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);
        }
        
        if (masterVolumeSlider != null)
        {
            masterVolumeSlider.minValue = 0;
            masterVolumeSlider.maxValue = 10;
            masterVolumeSlider.value = 10f;
            masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        }

        SetMusicVolume(musicVolumeSlider != null ? musicVolumeSlider.value : 10f);
        SetSFXVolume(sfxVolumeSlider != null ? sfxVolumeSlider.value : 10f);
    }


    private void CacheAudioSources()
    {
        if (musicAudioObject != null)
        {
            musicAudioSources = musicAudioObject.GetComponentsInChildren<AudioSource>();
        }
        
        if (sfxAudioObject != null)
        {
            sfxAudioSources = sfxAudioObject.GetComponentsInChildren<AudioSource>();
        }
    }

    public void SetMusicVolume(float volume)
    {
        currentMusicVolume = volume;
        ApplyMusicVolume();
    }

    public void SetSFXVolume(float volume)
    {
        currentSFXVolume = volume;
        ApplySFXVolume();
    }

    public void SetMasterVolume(float volume)
    {
        currentMasterVolume = volume;
        ApplyMusicVolume();
        ApplySFXVolume();
    }

    private void ApplyMusicVolume()
    {
        if (musicAudioSources != null)
        {
            float finalVolume = (currentMusicVolume / 10f) * (currentMasterVolume / 10f) * 0.5f;
            foreach (AudioSource audioSource in musicAudioSources)
            {
                if (audioSource != null)
                {
                    audioSource.volume = finalVolume;
                }
            }
        }
    }

    private void ApplySFXVolume()
    {
        if (sfxAudioSources != null)
        {
            float finalVolume = (currentSFXVolume / 10f) * (currentMasterVolume / 10f) * 0.5f;
            foreach (AudioSource audioSource in sfxAudioSources)
            {
                if (audioSource != null)
                {
                    audioSource.volume = finalVolume;
                }
            }
        }
    }
}
