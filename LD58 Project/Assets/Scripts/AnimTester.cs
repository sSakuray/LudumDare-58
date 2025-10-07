using UnityEngine;

public class AnimTester : MonoBehaviour
{
    public Animator animator;
    void Start()
    {

    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            animator.SetTrigger("Open");
        }
        
    }
}
