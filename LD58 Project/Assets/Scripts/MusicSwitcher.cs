using UnityEngine;

public class MusicSwitcher : MonoBehaviour
{
    private AudioSource[] musicTracks;

    private void Start()
    {
        // Автоматически находим все AudioSource на этом объекте
        musicTracks = GetComponents<AudioSource>();
        
        // Останавливаем все треки в начале
        StopAllMusic();
    }

    public void PlayTrack(int trackIndex)
    {
        if (trackIndex < 0 || trackIndex >= musicTracks.Length) return;

        // Останавливаем все треки
        StopAllMusic();

        // Запускаем нужный трек
        musicTracks[trackIndex].Play();
    }

    public void StopAllMusic()
    {
        foreach (AudioSource track in musicTracks)
        {
            if (track != null)
            {
                track.Stop();
            }
        }
    }
}
