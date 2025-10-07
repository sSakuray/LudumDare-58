using UnityEngine;

public class Sliding : MonoBehaviour
{
    [SerializeField] private KeyCode slideKey;
    [SerializeField] private float maxSlideSpeed;
    [SerializeField] private float slideAcceleration;
    [SerializeField] private float slideYScale;
    [SerializeField] private float slopeGravityForce;
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask slopeLayer;

    private CharacterController controller;
    private PlayerController pc;
    private Vector3 slideVelocity;
    private float startYScale;
    private bool isSliding;
    private RaycastHit hit;
    private float verticalVelocity;
    private float maxSpeedReached;
    private bool wasOnSlope;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        pc = GetComponent<PlayerController>();
        startYScale = transform.localScale.y;
    }

    void Update()
    {
        if  (!isSliding && Input.GetKey(slideKey) && CanSlide())
        {
            StartSlide();
        }
        if (Input.GetKeyUp(slideKey) && isSliding)
        {
            StopSlide();
        }
        if (isSliding)
        {
            HandleSliding();
        }
    }

    bool CanSlide()
    {
        if (OnSlope()) 
        {
            return true;
        }
        if (OnGround() && pc.GetCurrentSpeed() >= pc.maxSpeed) 
        {
            return true;
        }
        return false;
    }

    bool OnSlope()
    {
        return Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, out hit, groundCheckDistance, slopeLayer);
    }

    bool OnGround()
    {
        return Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, out hit, groundCheckDistance, groundLayer);
    }

    bool IsGrounded()
    {
        return OnGround() || OnSlope();
    }

    void StartSlide()
    {
        isSliding = true;
        pc.SetSliding(true);
        transform.localScale = new Vector3(transform.localScale.x, slideYScale, transform.localScale.z);
        
        slideVelocity = controller.velocity;
        slideVelocity.y = 0;
        
        if (slideVelocity.magnitude < 2f)
        {
            slideVelocity = Vector3.zero;
        }
        
        maxSpeedReached = slideVelocity.magnitude;
        wasOnSlope = false;
    }

    void HandleSliding()
    {
        bool onGround = OnGround();
        bool onSlope = OnSlope();
        
        if (!onGround && !onSlope)
        {
            verticalVelocity += pc.gravity * Time.deltaTime;
            controller.Move((slideVelocity + Vector3.up * verticalVelocity) * Time.deltaTime);
            return;
        }
        
        verticalVelocity = verticalVelocity < 0 ? -2f : verticalVelocity;
        
        if (onSlope)
        {
            HandleSlopeSliding();
        }
        else
        {
            HandleGroundSliding();
        }
        
        controller.Move((slideVelocity + Vector3.up * verticalVelocity) * Time.deltaTime);
    }

    void HandleSlopeSliding()
    {
        verticalVelocity = verticalVelocity > 0 ? Mathf.MoveTowards(verticalVelocity, slopeGravityForce, 100f * Time.deltaTime) : slopeGravityForce;
        wasOnSlope = true;

        Vector3 camDir = pc.cam.forward;
        camDir.y = 0;
        camDir.Normalize();
        
        Vector3 slopeDir = Vector3.ProjectOnPlane(camDir, hit.normal).normalized;
        
        if (slideVelocity.magnitude > 0.1f)
        {
            Vector3 currentDir = Vector3.ProjectOnPlane(slideVelocity.normalized, hit.normal);
            Vector3 targetDir = Vector3.Slerp(currentDir, slopeDir, 8f * Time.deltaTime);
            slideVelocity = targetDir * slideVelocity.magnitude;
        }
        else
        {
            slideVelocity = slopeDir * slideAcceleration;
        }
        
        slideVelocity += slideVelocity.normalized * slideAcceleration * Time.deltaTime;
        slideVelocity = Vector3.ClampMagnitude(slideVelocity, maxSlideSpeed);

        if (slideVelocity.magnitude > maxSpeedReached)
        {
            maxSpeedReached = slideVelocity.magnitude;
        }
    }

    void HandleGroundSliding()
    {
        verticalVelocity = verticalVelocity > 0 ? Mathf.MoveTowards(verticalVelocity, slopeGravityForce, 100f * Time.deltaTime) : slopeGravityForce;
        
        if (hit.normal != Vector3.zero)
        {
            slideVelocity = Vector3.ProjectOnPlane(slideVelocity, hit.normal);
        }
        
        Vector3 camDir = pc.cam.forward;
        camDir.y = 0;
        camDir.Normalize();
        
        Vector3 targetDir = Vector3.Slerp(slideVelocity.normalized, camDir, 3f * Time.deltaTime);
        slideVelocity = targetDir * slideVelocity.magnitude;

        if (slideVelocity.magnitude < pc.maxSpeed)
        {
            slideVelocity = camDir * pc.maxSpeed;
        }
    }

    void StopSlide()
    {
        isSliding = false;
        pc.SetSliding(false);
        transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        
        if (wasOnSlope && maxSpeedReached > 5f)
        {
            pc.ApplySpeedBoost(maxSpeedReached);
        }
        
        slideVelocity = Vector3.zero;
    }

    public bool IsSliding()
    {
        return isSliding;
    }

    public bool IsGroundedForJump()
    {
        return IsGrounded();
    }

    public void JumpWhileSliding(float force)
    {
        verticalVelocity = force;
    }
    
}