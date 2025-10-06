using UnityEngine;

public class AnimTester : MonoBehaviour
{
    public Animator animator;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            animator.SetTrigger("Open");
        }
        
    }
}
