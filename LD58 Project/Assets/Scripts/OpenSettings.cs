using UnityEngine;
public class OpenSettings : MonoBehaviour
{
    public GameObject settingsScreen;
    public bool isSettingsOpen = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isSettingsOpen == false)
            {
                settingsScreen.SetActive(true);
                isSettingsOpen = true;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                Time.timeScale = 0f;
            }
            else
            {
                settingsScreen.SetActive(false);
                isSettingsOpen = false;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                Time.timeScale = 1f;
            }
        }
    }
}
