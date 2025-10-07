using UnityEngine;

public class HighscoreUpdate : MonoBehaviour
{
    void Start()
    {
        this.GetComponent<HighscoreCounter>().GetSavedHighscore();
    }
}
