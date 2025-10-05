using UnityEngine;

public class WallRunning : MonoBehaviour
{
    [SerializeField] private float wallRunSpeed; // Скорость бега по стене
    [SerializeField] private float wallStickForce; // Сила прилипания к стене
    [SerializeField] private float wallJumpHeight; // Высота прыжка от стены
    [SerializeField] private float wallJumpDistance; // Дальность прыжка от стены
    [SerializeField] private float maxWallRunTime; // Максимальное время бега по стене в секундах
    [SerializeField] private Transform cam; 

    private CharacterController controller;
    private PlayerController pc;
    private bool onWall;
    private Vector3 wallNormal;
    private float wallRunTimer;
    private GameObject lastWall;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        pc = GetComponent<PlayerController>();
    }

    private void Update()
    {
        bool canWallRun = onWall && !pc.grounded && wallRunTimer > 0;
        
        if (canWallRun)
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
        if (pc.grounded)
        {
            onWall = false;
            lastWall = null;
            pc.SetWallRunning(false);
            return;
        }
        
        if (hit.gameObject.CompareTag("Wall"))
        {
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
        pc.EnableDoubleJump();
        pc.SetWallRunning(false);
        onWall = false;
    }
}