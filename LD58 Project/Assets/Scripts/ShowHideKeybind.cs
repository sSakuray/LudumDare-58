using UnityEngine;

public class ShowHideKeybind : MonoBehaviour
{
    [SerializeField] private GameObject keybind;

    public void ShowButton()
    {
        keybind.SetActive(true);
    }

    public void HideButton()
    {
        keybind.SetActive(false);
    }
}
