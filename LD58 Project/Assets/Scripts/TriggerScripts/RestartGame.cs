using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartGame : MonoBehaviour
{
    public string levelName = "Level1";
    public void RestartTheGame()
    {
        SceneManager.LoadScene(levelName);
    }
}
