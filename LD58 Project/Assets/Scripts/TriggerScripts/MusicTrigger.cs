using UnityEngine;

public class MusicTrigger : MonoBehaviour
{
    [SerializeField] private MusicSwitcher musicSwitcher;
    [SerializeField] private int trackIndex;
    [SerializeField] private string tagFilter = "Player";

    private void Start()
    {
        if (musicSwitcher == null)
        {
            musicSwitcher = FindObjectOfType<MusicSwitcher>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!string.IsNullOrEmpty(tagFilter) && !other.CompareTag(tagFilter)) return;

        if (musicSwitcher != null)
        {
            musicSwitcher.PlayTrack(trackIndex);
        }
    }
}
