using TMPro;
using UnityEngine;
using System.Collections;

public class HighscoreCounter : MonoBehaviour
{
    private const string HighscoreKey = "Highscore";
    public TMP_Text timerText;
    private float elapsedTime = 0f;
    private Coroutine timerCoroutine;

    public void StartHighscoreTimer()
    {
        if (timerCoroutine == null)
        {
            timerCoroutine = StartCoroutine(CountTime());
        }
    }

    public void StopHighscoreTimer()
    {
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
            timerCoroutine = null;
            SaveTimer();
        }
    }

    private IEnumerator CountTime()
    {
        while (true)
        {
            elapsedTime += Time.deltaTime;
            UpdateTimerText(elapsedTime);
            yield return null;
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
        PlayerPrefs.SetString(HighscoreKey, timerText.text);
        PlayerPrefs.Save();
    }

    public void GetSavedHighscore()
    {
        string savedHighscore = PlayerPrefs.GetString(HighscoreKey, "not found");
        timerText.text = savedHighscore;
    }
}