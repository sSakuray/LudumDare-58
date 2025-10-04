using UnityEngine;

public class WallRunning : MonoBehaviour
{
    [SerializeField] private float wallRunSpeed = 6f;
    [SerializeField] private float wallStickForce = 50f;
    [SerializeField] private float wallJumpHeight = 8f;
    [SerializeField] private float wallJumpDistance = 6f;
    [SerializeField] private float maxWallRunTime = 2f;
    [SerializeField] private Transform cam;

    private CharacterController controller;
    private PlayerController pc;
    
    private bool onWall;
    private Vector3 wallNormal;
    private float wallRunTimer;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        pc = GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (onWall && !pc.grounded && wallRunTimer > 0)
        {
            pc.SetWallRunning(true);
            WallRun();
            
            wallRunTimer -= Time.deltaTime;
            
            if (Input.GetKeyDown(KeyCode.Space))
            {
                WallJump();
            }
        }
        else
        {
            if (pc.wallRunning)
            {
                pc.horizontalVelocity = Vector3.zero;
            }
            
            pc.SetWallRunning(false);
            
            if (pc.grounded)
            {
                wallRunTimer = maxWallRunTime;
            }
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Wall") && !pc.grounded)
        {
            onWall = true;
            wallNormal = hit.normal;
        }
        else if (pc.grounded)
        {
            onWall = false;
        }
    }

    private void WallRun()
    {
        Vector3 stickDir = -wallNormal * wallStickForce;
        
        float vertical = Input.GetAxisRaw("Vertical");
        float horizontal = Input.GetAxisRaw("Horizontal");
        
        Vector3 moveDir = Vector3.zero;
        if (Mathf.Abs(vertical) > 0.1f)
        {
            moveDir += cam.forward * vertical;
        }
        if (Mathf.Abs(horizontal) > 0.1f)
        {
            moveDir += cam.right * horizontal;
        }
        
        moveDir.y = 0;
        moveDir = moveDir.normalized * wallRunSpeed;
        
        controller.Move((stickDir + moveDir) * Time.deltaTime);
    }

    private void WallJump()
    {
        pc.horizontalVelocity = Vector3.zero;
        
        Vector3 camDirection = cam.forward;
        camDirection.y = 0;
        camDirection = camDirection.normalized;
        
        pc.horizontalVelocity = camDirection * wallJumpDistance;
        
        pc.verticalVelocity = wallJumpHeight;
        
        pc.SetWallRunning(false);
        onWall = false;
    }
}