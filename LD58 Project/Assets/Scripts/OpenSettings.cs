using Unity.VisualScripting;
using UnityEngine;

public class OpenSettings : MonoBehaviour
{
    public GameObject settingsScreen;
    public bool isSettingsOpen = false;
    public GameObject player;
    public GameObject cam;
    private PlayerController playerController;
    private CameraController cameraController;

    void Start()
    {
        playerController = player.GetComponent<PlayerController>();
        cameraController = cam.GetComponent<CameraController>();
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isSettingsOpen == false)
            {
                settingsScreen.SetActive(true);
                playerController.enabled = false;
                cameraController.enabled = false;
                isSettingsOpen = true;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                Time.timeScale = 0f;
            }
            else
            {
                settingsScreen.SetActive(false);
                playerController.enabled = true;
                cameraController.enabled = true;
                isSettingsOpen = false;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                Time.timeScale = 1f;
            }
        }
    }
}
