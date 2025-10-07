using UnityEngine;
using System.Collections;

public class FlickerSummon : MonoBehaviour
{
    public GameObject flicker;
    public GameObject player;
    private float currentSpeed;
    public void CallFLicker()
    {
        flicker.SetActive(true);
        StartCoroutine(DeleteAfterDelay());
    }
    
    private IEnumerator DeleteAfterDelay()
    {
        yield return new WaitForSeconds(3);
        currentSpeed = player.GetComponent<PlayerController>().currentSpeed;
        if (currentSpeed > 1)
        {
            GetComponent<LoadScene>().LoadSceneSpecific();
        }
        yield return new WaitForSeconds(1);
        flicker.SetActive(false);
    }


}
