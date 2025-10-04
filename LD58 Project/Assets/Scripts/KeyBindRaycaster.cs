using UnityEngine;

public class KeyBindRaycaster : MonoBehaviour
{
    public LayerMask layer;
    public Camera playerCamera;
    private byte Range = 3;
    private OpenDoor openDoor;
    private ShowHideKeybind showHideKeybind;




    private void Start()
    {

    }

    private void Update()
    {
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Range, layer))
        {

            showHideKeybind = hit.transform.gameObject.GetComponent<ShowHideKeybind>();
            showHideKeybind.ShowButton();
            Debug.Log("Ты какашка");

            if (Input.GetKeyDown(KeyCode.E))
            {
                openDoor = hit.transform.gameObject.GetComponent<OpenDoor>();
                openDoor.OpenDoor2();
            }
        }
        else if (showHideKeybind != null)
        {
            showHideKeybind.HideButton();   
        }
    }
}
