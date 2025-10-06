using TMPro;
using UnityEngine;
using System.Collections;

public class HighscoreCounter : MonoBehaviour
{
    private const string HighscoreKey = "Highscore"; // Key for PlayerPrefs
    public TMP_Text timerText;
    private float elapsedTime = 0f; // Time elapsed in seconds
    private Coroutine timerCoroutine; // Store reference to the coroutine

    public void StartHighscoreTimer()
    {
        if (timerCoroutine == null) // Prevent starting multiple timers
        {
            timerCoroutine = StartCoroutine(CountTime());
        }
    }

    public void StopHighscoreTimer()
    {
        if (timerCoroutine != null) // Check if the coroutine is running
        {
            StopCoroutine(timerCoroutine); // Stop the coroutine
            timerCoroutine = null; // Clear the reference
            SaveTimer(); // Save the timer when stopped
        }
    }

    private IEnumerator CountTime()
    {
        while (true)
        {
            elapsedTime += Time.deltaTime; // Increment elapsed time
            UpdateTimerText(elapsedTime); // Update the timer text
            yield return null; // Wait for the next frame
        }
    }

    private void UpdateTimerText(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        int milliseconds = Mathf.FloorToInt((time - Mathf.FloorToInt(time)) * 100);

        timerText.text = string.Format("{0:D2}:{1:D2}:{2:D2}", minutes, seconds, milliseconds);
    }

    public void SaveTimer()
    {
        // Save the elapsed time as a string in PlayerPrefs
        PlayerPrefs.SetString(HighscoreKey, timerText.text);
        PlayerPrefs.Save(); // Ensure data is saved immediately
    }

    public void GetSavedHighscore()
    {
        string savedHighscore = PlayerPrefs.GetString(HighscoreKey, "not found");
        timerText.text = savedHighscore;
    }
}