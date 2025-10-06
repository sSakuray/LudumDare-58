using UnityEngine;
using System.Collections;

public class Soul : MonoBehaviour
{
    [SerializeField] private float timeToAdd = 10f;
    [SerializeField] private Renderer objectRenderer; // Renderer объекта с материалом
    [SerializeField] private float effectDuration = 1f; // Длительность эффекта перед удалением
    [SerializeField] private AudioClip collectSound;

    private Material material;
    private bool collected = false;
    private static readonly int ValueProperty = Shader.PropertyToID("_Value");
    
    private void Start()
    {
        // Получаем материал и устанавливаем Value в 0
        if (objectRenderer != null)
        {
            material = objectRenderer.material; // Создаём instance материала
            material.SetFloat(ValueProperty, 0f);
        }
    }
    
    public void Collect()
    {
        if (collected) return;
        collected = true;
        
        Timer timer = FindObjectOfType<Timer>();
        if (timer != null)
        {
            timer.AddTime(timeToAdd);
        }
        
        if (collectSound != null)
        {
            AudioSource.PlayClipAtPoint(collectSound, transform.position);
        }
        
        // Запускаем эффект растворения
        StartCoroutine(DissolveEffect());
    }
    
    private IEnumerator DissolveEffect()
    {
        if (material != null)
        {
            // Плавно меняем Value от 0 до 1
            float elapsed = 0f;
            while (elapsed < effectDuration)
            {
                elapsed += Time.deltaTime;
                float value = Mathf.Lerp(0f, 1f, elapsed / effectDuration);
                material.SetFloat(ValueProperty, value);
                yield return null;
            }
            
            material.SetFloat(ValueProperty, 1f);
        }
        
        // Удаляем объект (или родителя, если нужно)
        Destroy(gameObject);
    }
}
