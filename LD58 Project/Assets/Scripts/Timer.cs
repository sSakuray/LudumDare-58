using UnityEngine;
using TMPro;
using System.Collections;

public class CountdownTimer : MonoBehaviour
{
    public TMP_Text timerText; // Reference to the UI Text component
    public Animator animator; // Reference to the Animator component
    public float countdownTime = 120f; // Countdown time in seconds (e.g., 120 seconds = 2 minutes)
    public bool isPaused = false;

    private void Start()
    {
        StartCoroutine(Countdown());
    }

    private IEnumerator Countdown()
    {
        float timeRemaining = countdownTime;
        float elapsedTime = 0f;

        //нужно сделать паузу таймера!!!!!!!!!!!!!!!!!!!!!!
        
        while (timeRemaining > 0)
        {
            // Update the timer text
            UpdateTimerText(timeRemaining);

            // Wait for a frame
            yield return null;

            // Decrease the time remaining
            timeRemaining -= Time.deltaTime;
            elapsedTime += Time.deltaTime;

            // Check if a second has passed
            if (elapsedTime >= 1f)
            {
                // Call the animator to play an animation
                PlayAnimation();

                // Reset elapsed time
                elapsedTime = 0f;
            }
            if (timeRemaining < 30)
            {
                timerText.color = Color.red;
            }
        }

        // Ensure the timer shows 00:00:00 when finished
            UpdateTimerText(0);
    }

    private void UpdateTimerText(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        int milliseconds = Mathf.FloorToInt((time - Mathf.FloorToInt(time)) * 100);

        // Format the time string as 00:00:00
        timerText.text = string.Format("{0:D2}:{1:D2}:{2:D2}", minutes, seconds, milliseconds);
    }

    private void PlayAnimation()
    {
      animator.SetTrigger("PlayAnimation"); // Make sure you have a trigger parameter named "PlayAnimation" in your Animator
    }
}