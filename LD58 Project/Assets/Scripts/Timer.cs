using UnityEngine;
using TMPro;
using System.Collections;

public class Timer : MonoBehaviour
{
    public TMP_Text timerText; 
    public Animator animator; 
    public float countdownTime = 120f; 
    public bool isPaused = false;

    private float timeRemaining;
    private float elapsedTime = 0f;

    private void Start()
    {
        timeRemaining = countdownTime;
        StartCoroutine(Countdown());
    }

    public void AddTime(float seconds)
    {
        timeRemaining += seconds;
    }

    private IEnumerator Countdown()
    {
        while (timeRemaining > 0)
        {
            UpdateTimerText(timeRemaining);

            yield return null;

            timeRemaining -= Time.deltaTime;
            elapsedTime += Time.deltaTime;

            if (elapsedTime >= 1f)
            {
                PlayAnimation();

                elapsedTime = 0f;
            }
            if (timeRemaining < 30)
            {
                timerText.color = Color.red;
            }
        }

            UpdateTimerText(0);
    }

    private void UpdateTimerText(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        int milliseconds = Mathf.FloorToInt((time - Mathf.FloorToInt(time)) * 100);

        timerText.text = string.Format("{0:D2}:{1:D2}:{2:D2}", minutes, seconds, milliseconds);
    }

    private void PlayAnimation()
    {
      animator.SetTrigger("PlayAnimation");
    }
}