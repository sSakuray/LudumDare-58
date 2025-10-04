using UnityEngine;

public class ShowHideKeybind : MonoBehaviour
{
    [SerializeField] private GameObject keybind;
    public bool hideKeybind = true;

    public void ShowButton()
    {
        if (hideKeybind) keybind.SetActive(true);
        else return;
    }

    public void HideButton()
    {
        keybind.SetActive(false);
    }
}
