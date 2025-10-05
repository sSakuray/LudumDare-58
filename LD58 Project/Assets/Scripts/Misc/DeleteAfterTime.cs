using UnityEngine;
using System.Collections;

public class DeleteAfterTime : MonoBehaviour
{
    public int waitTime = 10;
    void Awake()
    {
        StartCoroutine(DeleteAfterDelay());
    }

    private IEnumerator DeleteAfterDelay()
    {
        yield return new WaitForSeconds(waitTime);
        Object.Destroy(this.gameObject);
    }
}
