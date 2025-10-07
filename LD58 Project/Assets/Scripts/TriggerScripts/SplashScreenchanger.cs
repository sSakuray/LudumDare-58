using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SplashScreenchanger : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(CallMethodAfterSeconds(3f));
    }

    private IEnumerator CallMethodAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        
        MyMethod();
    }

    private void MyMethod()
    {
        SceneManager.LoadScene("StartScreen");
    }
}
