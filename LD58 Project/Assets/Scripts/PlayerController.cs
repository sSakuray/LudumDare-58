using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Player Movement")]
    [SerializeField] private float minSpeed;
    [SerializeField] public float maxSpeed;
    [SerializeField] public float currentSpeed;
    [SerializeField] private float acceleration; 
    [SerializeField] private float boostDecay;
    public Transform cam;

    [Header("Jump")]
    [SerializeField] public float jumpForce;
    [SerializeField] private float jumpCooldown;
    [SerializeField] private float airMultiplier;
    [SerializeField] public float gravity;
    [SerializeField] private float doubleJumpDelay = 0.2f;
    private bool readyToJump;
    private bool hasDoubleJump;
    private bool hasJumpedFromGround;
    private float timeSinceFirstJump;
    private KeyCode jumpKey = KeyCode.Space;

    [Header("Crouching")]
    [SerializeField] private float crouchSpeed;
    [SerializeField] private float crouchYScale;
    [SerializeField] private float startYScale;
    private bool crouching;
    private KeyCode crouchKey = KeyCode.LeftControl;

    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private float groundCheckDistance = 1.08f;
    public Transform player;
    public bool grounded;

    [Header("Other")]
    private CharacterController controller;
    private Vector3 moveDirection;
    public float verticalVelocity;
    public Vector3 horizontalVelocity;
    private bool sliding;
    public bool wallRunning;
    private Sliding slidingScript;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        slidingScript = GetComponent<Sliding>();
        readyToJump = true;
        hasDoubleJump = false;
        hasJumpedFromGround = false;
        startYScale = transform.localScale.y;
    }

    private void Update()
    {
        GroundCheck();
        
        if (!grounded && hasJumpedFromGround)
        {
            timeSinceFirstJump += Time.deltaTime;
        }
        
        HandleInput();
        
        if (!sliding && !wallRunning)
        {
            MovePlayer();
        }
    }

    private void GroundCheck()
    {
        grounded = Physics.Raycast(player.position, Vector3.down, groundCheckDistance, whatIsGround);
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(jumpKey))
        {
            if (readyToJump && (grounded || (sliding && slidingScript.IsGroundedForJump())))
            {
                readyToJump = false;
                hasJumpedFromGround = true;
                timeSinceFirstJump = 0f;
                hasDoubleJump = true;
                
                if (sliding)
                {
                    slidingScript.JumpWhileSliding(jumpForce);
                }
                else
                {
                    Jump();
                }
                Invoke(nameof(ResetJump), jumpCooldown);
            }
            else if (!grounded && hasDoubleJump && timeSinceFirstJump >= doubleJumpDelay)
            {
                hasDoubleJump = false;
                Jump();
            }
            else if (!grounded && !hasJumpedFromGround && !hasDoubleJump && readyToJump)
            {
                readyToJump = false;
                Jump();
            }
        }

        if (Input.GetKeyDown(crouchKey) && !sliding)
        {
            Crouch();
        }
    }

    private void MovePlayer()
    {
        moveDirection = transform.forward * Input.GetAxisRaw("Vertical") + transform.right * Input.GetAxisRaw("Horizontal");
        float targetSpeed = crouching ? crouchSpeed : maxSpeed;
        bool moving = moveDirection.magnitude > 0.1f;

        if (grounded)
        {
            if (currentSpeed > targetSpeed)
            {
                currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, boostDecay * Time.deltaTime);
            }
            currentSpeed = moving ? (currentSpeed < minSpeed ? minSpeed : Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.deltaTime)) : 0f;
            horizontalVelocity = moving ? moveDirection.normalized * currentSpeed : Vector3.zero;
        }
        else if (moving)
        {
            horizontalVelocity = Vector3.Lerp(horizontalVelocity, moveDirection.normalized * currentSpeed * airMultiplier, airMultiplier * Time.deltaTime * 2f);
        }
        else if (!grounded)
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            if (Mathf.Abs(horizontal) > 0.1f)
            {
                Vector3 strafeDir = transform.right * horizontal * currentSpeed * airMultiplier * 0.5f;
                horizontalVelocity += strafeDir * Time.deltaTime * 10f;
            }
        }

        if (grounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f;
            if (!readyToJump)
            {
                hasJumpedFromGround = false;
                timeSinceFirstJump = 0f;
                hasDoubleJump = false;
            }
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }
        
        controller.Move((horizontalVelocity + Vector3.up * verticalVelocity) * Time.deltaTime);
    }

    private void Jump() => verticalVelocity = jumpForce;

    private void ResetJump() => readyToJump = true;

    private void Crouch()
    {
        crouching = !crouching;
        float yScale = crouching ? crouchYScale : startYScale;
        transform.localScale = new Vector3(transform.localScale.x, yScale, transform.localScale.z);
    }

    public void SetSliding(bool isSliding) => sliding = isSliding;
    
    public void SetWallRunning(bool isWallRunning) => wallRunning = isWallRunning;
    
    public void DisableDoubleJump() => hasDoubleJump = false;
    
    public void EnableDoubleJump()
    {
        hasDoubleJump = true;
        hasJumpedFromGround = true;
        timeSinceFirstJump = doubleJumpDelay;
    }

    public float GetCurrentSpeed() => currentSpeed;
    
    public void ApplySpeedBoost(float speed)
    {
        if (speed > currentSpeed)
        {
            currentSpeed = speed;
        }
    }
}
