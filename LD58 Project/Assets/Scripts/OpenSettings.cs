using Unity.VisualScripting;
using UnityEngine;

public class OpenSettings : MonoBehaviour
{
    public GameObject settingsScreen;
    public bool isSettingsOpen = false;
    public GameObject player;
    public GameObject cam;
    private CameraController cameraController;

    void Start()
    {
        cameraController = cam.GetComponent<CameraController>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isSettingsOpen == false)
            {
                //open settings
                settingsScreen.SetActive(true);
                cameraController.enabled = false;
                isSettingsOpen = true;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                //close settings
                settingsScreen.SetActive(false);
                cameraController.enabled = true;
                isSettingsOpen = false;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }
}
