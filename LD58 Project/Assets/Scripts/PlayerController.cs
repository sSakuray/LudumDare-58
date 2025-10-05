using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Player Movement")]
    [SerializeField] private float minSpeed; // Минимальная скорость ходьбы
    [SerializeField] public float maxSpeed; // Максимальная скорость бега
    [SerializeField] public float currentSpeed; // Текущая скорость персонажа
    [SerializeField] private float acceleration = 10f; // Скорость разгона (единиц/сек) - чем больше, тем быстрее набор скорости
    [SerializeField] private float boostDecay; // Скорость замедления после буста
    public Transform cam;

    [Header("Jump")]
    [SerializeField] public float jumpForce; // Высота прыжка
    [SerializeField] private float jumpCooldown; // Задержка между прыжками
    [SerializeField] private float airMultiplier; // Множитель управления в воздухе
    [SerializeField] private float airControlStrength; // Сила управления в воздухе
    [SerializeField] private float jumpMomentumMultiplier; // Бонус дальности прыжка от скорости (0.5 = +50% при макс скорости)
    [SerializeField] private float jumpBoostDuration; // Время плавного набора скорости прыжка в секундах
    [SerializeField] public float gravity; // Гравитация
    [SerializeField] private float doubleJumpDelay; // Задержка перед двойным прыжком
    private bool readyToJump;
    private bool hasDoubleJump;
    private bool hasJumpedFromGround;
    private float timeSinceFirstJump;
    private float jumpMomentumTimer;
    private float targetJumpVelocity;
    private KeyCode jumpKey = KeyCode.Space;

    [Header("Crouching")]
    [SerializeField] private float crouchSpeed; // Скорость приседа
    [SerializeField] private float crouchYScale; // Масштаб Y при приседе
    [SerializeField] private float startYScale; // Начальный масштаб Y
    private bool crouching;
    private KeyCode crouchKey = KeyCode.LeftControl;

    [Header("Ground Check")]
    [SerializeField] private LayerMask whatIsGround; // Слой земли
    [SerializeField] private float groundCheckDistance; // Дистанция проверки земли
    public Transform player;
    public bool grounded;

    [Header("Hook")]
    public bool freeze;
    public bool activeGrappling;

    [Header("Audio")]
    [SerializeField] private SoundPlayer soundPlayer;
    [SerializeField] private float minStepInterval = 0.6f;
    [SerializeField] private float maxStepInterval = 0.3f;
    [SerializeField] private float minPitch = 0.8f;
    [SerializeField] private float maxPitch = 1.3f;
    private float stepTimer;

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
        
        if (soundPlayer == null)
        {
            soundPlayer = FindObjectOfType<SoundPlayer>();
        }
    }

    private void Update()
    {
        GroundCheck();
        
        if (!grounded && hasJumpedFromGround)
        {
            timeSinceFirstJump += Time.deltaTime;
        }
        
        if (freeze)
        {
            horizontalVelocity = Vector3.zero;
            verticalVelocity = 0f;
            return; 
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
            HandleJumpInput();
        }
        if (Input.GetKeyDown(crouchKey) && !sliding)
        {
            Crouch();
        }
    }

    private void HandleJumpInput()
    {
        bool canGroundJump = readyToJump && (grounded || (sliding && slidingScript.IsGroundedForJump()));
        bool canDoubleJump = !grounded && hasDoubleJump && timeSinceFirstJump >= doubleJumpDelay;
        bool canFirstAirJump = !grounded && !hasJumpedFromGround && readyToJump;

        if (canGroundJump)
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
        else if (canDoubleJump)
        {
            hasDoubleJump = false;
            Jump();
        }
        else if (canFirstAirJump)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void MovePlayer()
    {
        moveDirection = transform.forward * Input.GetAxisRaw("Vertical") + transform.right * Input.GetAxisRaw("Horizontal");
        bool moving = moveDirection.magnitude > 0.1f;

        if (grounded)
        {
            HandleGroundMovement(moving);
        }
        else
        {
            HandleAirMovement(moving);
        }
        ApplyGravity();
        controller.Move((horizontalVelocity + Vector3.up * verticalVelocity) * Time.deltaTime);
    }

    private void HandleGroundMovement(bool moving)
    {
        float targetSpeed = crouching ? crouchSpeed : maxSpeed;

        if (currentSpeed > targetSpeed)
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, boostDecay * Time.deltaTime);
        }
        
        currentSpeed = moving ? Mathf.Max(minSpeed, Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.deltaTime)) : 0f;
        
        if (verticalVelocity <= 0)
        {
            horizontalVelocity = moving ? moveDirection.normalized * currentSpeed : Vector3.zero;
        }
        
        if (verticalVelocity < 0)
        {
            verticalVelocity = -2f;
            
            if (!readyToJump)
            {
                hasJumpedFromGround = false;
                timeSinceFirstJump = 0f;
                hasDoubleJump = false;
            }
        }
        
        HandleFootsteps(moving);
    }

    private void HandleFootsteps(bool moving)
    {
        if (moving && soundPlayer != null)
        {
            stepTimer += Time.deltaTime;
            
            float speedRatio = currentSpeed / maxSpeed;
            float interval = Mathf.Lerp(minStepInterval, maxStepInterval, speedRatio);
            
            if (stepTimer >= interval)
            {
                float pitch = Mathf.Lerp(minPitch, maxPitch, speedRatio);
                soundPlayer.PlaySoundWithPitch(0, pitch);
                stepTimer = 0f;
            }
        }
        else
        {
            stepTimer = 0f;
        }
    }

    private void HandleAirMovement(bool moving)
    {
        if (jumpMomentumTimer > 0)
        {
            jumpMomentumTimer -= Time.deltaTime;
            float progress = 1f - (jumpMomentumTimer / jumpBoostDuration);
            float currentTarget = Mathf.Lerp(horizontalVelocity.magnitude, targetJumpVelocity, progress);
            
            if (horizontalVelocity.magnitude > 0.1f)
            {
                horizontalVelocity = horizontalVelocity.normalized * currentTarget;
            }
        }
        
        if (moving)
        {
            float currentMagnitude = horizontalVelocity.magnitude;
            float minTarget = currentMagnitude * 0.95f;
            float airTarget = currentSpeed * airMultiplier;
            Vector3 targetVelocity = moveDirection.normalized * Mathf.Max(minTarget, airTarget);
            horizontalVelocity = Vector3.Lerp(horizontalVelocity, targetVelocity, airControlStrength * Time.deltaTime);
        }
    }

    private void ApplyGravity()
    {
        if (!activeGrappling && !wallRunning && !grounded)
        {
            verticalVelocity += gravity * Time.deltaTime;
        }
    }

    private void Jump()
    {
        verticalVelocity = jumpForce;
        
        if (horizontalVelocity.magnitude > 0.1f && currentSpeed > 0)
        {
            float speedRatio = currentSpeed / maxSpeed;
            float boost = 1f + (speedRatio * jumpMomentumMultiplier);
            targetJumpVelocity = horizontalVelocity.magnitude * boost;
            jumpMomentumTimer = jumpBoostDuration;
        }
        else
        {
            targetJumpVelocity = 0f;
            jumpMomentumTimer = 0f;
        }
    }

    private void ResetJump()
    {
        readyToJump = true;
    }

    private void Crouch()
    {
        crouching = !crouching;
        float yScale = crouching ? crouchYScale : startYScale;
        transform.localScale = new Vector3(transform.localScale.x, yScale, transform.localScale.z);
    }

    public void SetSliding(bool isSliding)
    {
        sliding = isSliding;
    }
    
    public void SetWallRunning(bool isWallRunning)
    {
        wallRunning = isWallRunning;
    }
    
    public void DisableDoubleJump()
    {
        hasDoubleJump = false;
    }
    
    public void EnableDoubleJump()
    {
        hasDoubleJump = true;
        hasJumpedFromGround = true;
        timeSinceFirstJump = doubleJumpDelay;
    }

    public float GetCurrentSpeed()
    {
        return currentSpeed;
    }
    
    public void ApplySpeedBoost(float speed)
    {
        if (speed > currentSpeed)
        {
            currentSpeed = speed;
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (Vector3.Dot(hit.normal, Vector3.up) < 0.7f)
        {
            bool hasNoTag = hit.collider.tag == "Untagged";
            bool isDefaultLayer = hit.collider.gameObject.layer == 0;
            
            if (hasNoTag && isDefaultLayer)
            {
                currentSpeed = minSpeed;
                horizontalVelocity = Vector3.zero;
            }
        }
        
        if (Vector3.Dot(hit.normal, Vector3.down) > 0.7f && verticalVelocity > 0)
        {
            verticalVelocity = 0f; 
        }
    }
}