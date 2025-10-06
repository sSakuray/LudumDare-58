using UnityEngine;

public class WallRunning : MonoBehaviour
{
    [SerializeField] private float wallRunSpeed;
    [SerializeField] private float wallStickForce;
    [SerializeField] private float wallJumpHeight;
    [SerializeField] private float wallJumpDistance;
    [SerializeField] private float maxWallRunTime;
    [SerializeField] private float wallTiltAngle = 15f;
    [SerializeField] private float tiltSpeed = 5f;
    [SerializeField] private Transform cam; 

    private CharacterController controller;
    private PlayerController pc;
    private bool onWall;
    private Vector3 wallNormal;
    private float wallRunTimer;
    private GameObject lastWall;
    private bool hasWallDoubleJump;
    private float wallJumpCooldown;
    private Vector3 originalRotation;
    private float currentTilt;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        pc = GetComponent<PlayerController>();
        originalRotation = transform.eulerAngles;
    }

    private void Update()
    {
        if (wallJumpCooldown > 0)
        {
            wallJumpCooldown -= Time.deltaTime;
        }
        
        bool canWallRun = onWall && !pc.grounded && wallRunTimer > 0 && wallJumpCooldown <= 0;
        
        if (canWallRun)
        {
            pc.SetWallRunning(true);
            WallRun();
            ApplyWallTilt();
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
                pc.horizontalVelocity *= 0.8f;
            }
            pc.SetWallRunning(false);
            ResetTilt();
            
            if (hasWallDoubleJump && !pc.grounded && Input.GetKeyDown(KeyCode.Space))
            {
                WallDoubleJump();
            }
            
            if (pc.grounded)
            {
                wallRunTimer = maxWallRunTime;
                hasWallDoubleJump = false;
            }
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (pc.grounded)
        {
            onWall = false;
            lastWall = null;
            pc.SetWallRunning(false);
            hasWallDoubleJump = false;
            wallJumpCooldown = 0f;
            return;
        }
        
        if (wallJumpCooldown > 0)
        {
            return;
        }
        
        if (hit.gameObject.CompareTag("Wall"))
        {
            float dotUp = Vector3.Dot(hit.normal, Vector3.up);
            float dotDown = Vector3.Dot(hit.normal, Vector3.down);
            
            if (dotUp > 0.7f)
            {
                onWall = false;
                return;
            }
            
            if (dotDown > 0.7f)
            {
                return;
            }
            
            if (lastWall != hit.gameObject)
            {
                lastWall = hit.gameObject;
                wallRunTimer = maxWallRunTime;
            }
            
            onWall = true;
            wallNormal = hit.normal;
        }
    }

    private void WallRun()
    {
        
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
        Vector3 movement = -wallNormal * wallStickForce + moveDir.normalized * wallRunSpeed;
        controller.Move(movement * Time.deltaTime);
    }

    private void WallJump()
    {
        Vector3 camDirection = cam.forward;
        camDirection.y = 0;
        
        float speedRatio = pc.currentSpeed / pc.maxSpeed;
        float finalJumpDistance = wallJumpDistance * Mathf.Lerp(0.5f, 5.0f, speedRatio);
        
        pc.horizontalVelocity = camDirection.normalized * finalJumpDistance;
        pc.verticalVelocity = wallJumpHeight;
        hasWallDoubleJump = true;
        wallJumpCooldown = 0.5f;
        pc.SetWallRunning(false);
        onWall = false;
    }
    
    private void WallDoubleJump()
    {
        
        pc.verticalVelocity = wallJumpHeight * 1.3f;
        
        Vector3 forwardDir = cam.forward;
        forwardDir.y = 0;
        forwardDir.Normalize();
        pc.horizontalVelocity += forwardDir * (pc.currentSpeed * 0.5f);
        
        hasWallDoubleJump = false;
    }
    
    private void ApplyWallTilt()
    {
        
        float wallSide = Vector3.Dot(transform.right, wallNormal);
        float targetTilt = wallSide > 0 ? -wallTiltAngle : wallTiltAngle;
        
        currentTilt = Mathf.Lerp(currentTilt, targetTilt, tiltSpeed * Time.deltaTime);
        
        Vector3 rotation = transform.eulerAngles;
        rotation.z = currentTilt;
        transform.eulerAngles = rotation;
    }
    
    private void ResetTilt()
    {
        currentTilt = Mathf.Lerp(currentTilt, 0f, tiltSpeed * Time.deltaTime);
        
        Vector3 rotation = transform.eulerAngles;
        rotation.z = currentTilt;
        transform.eulerAngles = rotation;
    }
}