using UnityEngine;
using System.Collections;

public class RushSummon : MonoBehaviour
{
    public Transform spawnPoint;
    public GameObject rushPrefab;
    public GameObject rushNotifUI;
    public void RushSummonVoid()
    {
        rushNotifUI.SetActive(true);
        Instantiate(rushPrefab);
        StartCoroutine(SpawnDelay());

    }
    private IEnumerator SpawnDelay()
    {
        yield return new WaitForSeconds(10);
        rushNotifUI.SetActive(false);
    }
}