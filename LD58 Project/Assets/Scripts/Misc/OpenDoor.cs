using UnityEngine;

public class OpenDoor : MonoBehaviour
{
    private Animator animator;
    public ShowHideKeybind showHideKeybind;
    private bool isDoorOpen = true;
    private SoundPlayer soundPlayer;
    void Start()
    {
        showHideKeybind = GetComponent<ShowHideKeybind>();
        animator = GetComponent<Animator>();
        soundPlayer = FindObjectOfType<SoundPlayer>();
    }
    public void OpenDoor2()
    {
        if (isDoorOpen)
        {
            soundPlayer.PlaySoundWithPitch(1, 1f);
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
