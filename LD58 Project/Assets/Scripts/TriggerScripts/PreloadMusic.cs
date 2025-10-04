using UnityEngine;

public class PreloadMusic : MonoBehaviour
{
    private AudioSource audioSource;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.Stop();
    }
}
