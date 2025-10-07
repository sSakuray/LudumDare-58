using UnityEngine;
using UnityEngine.VFX;

public class VFXAutoDestroy : MonoBehaviour
{
    [SerializeField] private float lifetime = 3f; 
    
    private VisualEffect vfx;
    
    private void Awake()
    {
        vfx = GetComponent<VisualEffect>();
    }
    
    private void Start()
    {
        Destroy(gameObject, lifetime);
    }
}
