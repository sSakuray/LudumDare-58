using UnityEngine;

public class OpenDoor : MonoBehaviour
{
    private Animator animator;
    public ShowHideKeybind showHideKeybind;
    private bool isDoorOpen = true;
    void Start()
    {
        showHideKeybind = GetComponent<ShowHideKeybind>();
        animator = GetComponent<Animator>();
    }
    public void OpenDoor2()
    {
        if (isDoorOpen)
        {
            animator.SetTrigger("Open");
            isDoorOpen = !isDoorOpen;
            showHideKeybind.hideKeybind = false;
        }
        else
        {
            return;
        }
        

    }

}
