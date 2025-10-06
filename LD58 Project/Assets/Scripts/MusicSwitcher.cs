using UnityEngine;

public class MusicSwitcher : MonoBehaviour
{
    private AudioSource[] musicTracks;

    private void Start()
    {
        musicTracks = GetComponents<AudioSource>();
        
        StopAllMusic();
    }

    public void PlayTrack(int trackIndex)
    {
        if (trackIndex < 0 || trackIndex >= musicTracks.Length) return;

        StopAllMusic();

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
