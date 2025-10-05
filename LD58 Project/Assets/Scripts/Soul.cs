using UnityEngine;

public class Soul : MonoBehaviour
{
    [SerializeField] private float timeToAdd = 10f;
    [SerializeField] private GameObject collectEffect;
    [SerializeField] private AudioClip collectSound;
    
    private bool collected = false;
    
    public void Collect()
    {
        if (collected) return;
        collected = true;
        
        Timer timer = FindObjectOfType<Timer>();
        if (timer != null)
        {
            timer.AddTime(timeToAdd);
        }
        
        if (collectEffect != null)
        {
            Instantiate(collectEffect, transform.position, Quaternion.identity);
        }
        
        if (collectSound != null)
        {
            AudioSource.PlayClipAtPoint(collectSound, transform.position);
        }
        
        Destroy(gameObject);
    }
}
