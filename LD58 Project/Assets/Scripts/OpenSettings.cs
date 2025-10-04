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
    private Grappling grappling;

    void Start()
    {
        playerController = player.GetComponent<PlayerController>();
        grappling = player.GetComponent<Grappling>();
        cameraController = cam.GetComponent<CameraController>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            if (isSettingsOpen == false)
            {
                //open settings
                settingsScreen.SetActive(true);
                playerController.enabled = false;
                cameraController.enabled = false;
                grappling.enabled = false;
                isSettingsOpen = true;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                //close settings
                settingsScreen.SetActive(false);
                playerController.enabled = true;
                cameraController.enabled = true;
                grappling.enabled = true;
                isSettingsOpen = false;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }
}
