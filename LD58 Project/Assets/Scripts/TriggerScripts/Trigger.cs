using UnityEngine;

public class Trigger : MonoBehaviour
{
    [SerializeField] string tagFilter;
    [SerializeField] int musicTrackIndex;

    void OnTriggerEnter(Collider other)
    {
        if (!string.IsNullOrEmpty(tagFilter) && !other.gameObject.CompareTag(tagFilter)) return;
        
        MusicSwitcher musicSwitcher = FindObjectOfType<MusicSwitcher>();
        if (musicSwitcher != null)
        {
            musicSwitcher.PlayTrack(musicTrackIndex);
        }
    }
}