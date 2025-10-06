using UnityEngine;
using UnityEngine.VFX;

/// <summary>
/// Автоматически удаляет GameObject с VFX эффектом после его завершения
/// </summary>
public class VFXAutoDestroy : MonoBehaviour
{
    [SerializeField] private float lifetime = 3f; // Время жизни эффекта в секундах
    
    private VisualEffect vfx;
    
    private void Awake()
    {
        vfx = GetComponent<VisualEffect>();
    }
    
    private void Start()
    {
        // Удаляем объект через заданное время
        Destroy(gameObject, lifetime);
    }
}
