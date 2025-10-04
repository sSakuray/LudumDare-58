using UnityEngine;

public class KeyBindRaycaster : MonoBehaviour
{
    public LayerMask layer;
    private Camera playerCamera;
    private byte Range = 3;
    private ShowHideKeybind keybind;



    private void Start()
    {
        
    }

    private void Update()
    {
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Range, layer))
        {
            //показать кейбинд
            keybind.GetComponent<ShowHideKeybind>();
            keybind.ShowButton();
        }
        else
        {
            keybind.GetComponent<ShowHideKeybind>();
            keybind.HideButton();
            //скрыть
        }
    }
}
