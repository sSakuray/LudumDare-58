using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Player Movement")]
    [SerializeField] private float minSpeed;
    [SerializeField] public float maxSpeed;
    [SerializeField] private float currentSpeed;
    [SerializeField] private float acceleration; 
    [SerializeField] private float boostDecay;
    public Transform cam;

    [Header("Jump")]
    [SerializeField] public float jumpForce;
    [SerializeField] private float jumpCooldown;
    [SerializeField] private float airMultiplier;
    [SerializeField] public float gravity;
    private bool readyToJump;
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
        startYScale = transform.localScale.y;
    }

    private void Update()
    {
        GroundCheck();
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
        if (Input.GetKeyDown(jumpKey) && readyToJump)
        {
            bool canJump = grounded || (sliding && slidingScript.IsGroundedForJump());
            if (canJump)
            {
                readyToJump = false;
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
                currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, boostDecay * Time.deltaTime);
            
            currentSpeed = moving ? (currentSpeed < minSpeed ? minSpeed : Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.deltaTime)) : 0f;
            horizontalVelocity = moving ? moveDirection.normalized * currentSpeed : Vector3.zero;
        }
        else if (moving)
        {
            horizontalVelocity = Vector3.Lerp(horizontalVelocity, moveDirection.normalized * currentSpeed * airMultiplier, airMultiplier * Time.deltaTime * 2f);
        }

        verticalVelocity = grounded && verticalVelocity < 0 ? -2f : verticalVelocity + gravity * Time.deltaTime;
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

    public float GetCurrentSpeed() => currentSpeed;
    
    public void ApplySpeedBoost(float speed)
    {
        if (speed > currentSpeed)
        {
            currentSpeed = speed;
        }
    }
}
