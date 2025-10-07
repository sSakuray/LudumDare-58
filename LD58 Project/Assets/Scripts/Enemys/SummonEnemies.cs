using UnityEngine;
using System.Collections;

public class SummonEnemies : MonoBehaviour
{
    public float timeMin = 5;
    public float timeMax = 40;
    public GameObject flicker;
    public GameObject rush;

    public void StartEnemySpawnTimer()
    {
        StartCoroutine(CallMethodsAtRandomIntervals());
    }

    private IEnumerator CallMethodsAtRandomIntervals()
    {
        while (true)
        {
            float waitTime1 = Random.Range(timeMin, timeMax);
            float waitTime2 = Random.Range(timeMin, timeMax);
            yield return new WaitForSeconds(waitTime1);
            flicker.GetComponent<FlickerSummon>().CallFLicker();
            yield return new WaitForSeconds(waitTime2);

        }
    }
}
