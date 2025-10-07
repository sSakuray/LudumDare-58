using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartScene : MonoBehaviour
{
    public void RestartSceneSpecific()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
