using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    private AudioSource[] sounds;

    private void Start()
    {
        sounds = GetComponents<AudioSource>();
    }

    public void PlaySound(int index)
    {
        if (index >= 0 && index < sounds.Length && sounds[index] != null)
        {
            sounds[index].Play();
        }
    }

    public void PlaySoundWithPitch(int index, float pitch)
    {
        if (index >= 0 && index < sounds.Length && sounds[index] != null)
        {
            sounds[index].pitch = pitch;
            sounds[index].Play();
        }
    }
}
