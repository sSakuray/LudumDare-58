using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Player Movement")]
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    private KeyCode runKey = KeyCode.LeftShift;
    public Transform cam;

    [Header("Jump")]
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpCooldown;
    [SerializeField] private float airMultiplier;
    [SerializeField] private float gravity;
    private bool readyToJump;
    private KeyCode jumpKey = KeyCode.Space;

    [Header("Crouching")]
    [SerializeField] private float crouchSpeed;
    [SerializeField] private float crouchYScale;
    [SerializeField] private float startYScale;
    private bool crouching;
    private KeyCode crouchKey = KeyCode.LeftControl;

    [Header("Ground Check")]
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private float groundCheckDistance = 1.08f;
    public Transform player;
    public bool grounded;

    [Header("Other")]
    private CharacterController controller;
    private Vector3 moveDirection;
    private bool wantsToRun;
    private float currentSpeed;
    private float verticalVelocity;
    private Vector3 horizontalVelocity;
    private bool sliding = false;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        readyToJump = true;
        startYScale = transform.localScale.y;
    }

    private void Update()
    {
        GroundCheck();
        HandleInput();
        
        if (!sliding)
            MovePlayer();
    }

    private void GroundCheck()
    {
        grounded = Physics.Raycast(player.position, Vector3.down, groundCheckDistance, whatIsGround);
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(jumpKey) && grounded && readyToJump)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }

        if (Input.GetKeyDown(crouchKey) && !sliding)
        {
            Crouch();
        }
    }

    private void MovePlayer()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");
        wantsToRun = Input.GetKey(runKey);

        moveDirection = transform.forward * verticalInput + transform.right * horizontalInput;

        if (crouching)
        {
            currentSpeed = crouchSpeed;
        }
        else
        {
            currentSpeed = wantsToRun ? runSpeed : walkSpeed;
        }

        if (grounded)
        {
            horizontalVelocity = moveDirection.normalized * currentSpeed;
        }
        else
        {
            if (moveDirection.magnitude > 0.1f)
            {
                Vector3 airControl = moveDirection.normalized * currentSpeed * airMultiplier;
                horizontalVelocity = Vector3.Lerp(horizontalVelocity, airControl, airMultiplier * Time.deltaTime * 10f);
            }
        }

        if (grounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f;
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        Vector3 finalMove = horizontalVelocity * Time.deltaTime + new Vector3(0, verticalVelocity * Time.deltaTime, 0);
        
        controller.Move(finalMove);
    }

    private void Jump()
    {
        verticalVelocity = jumpForce;
    }

    private void ResetJump()
    {
        readyToJump = true;
    }

    private void Crouch()
    {
        if (crouching)
        {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
            crouching = false;
        }
        else
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            crouching = true;
        }
    }

    public void SetSliding(bool isSliding)
    {
        sliding = isSliding;
    }
}
