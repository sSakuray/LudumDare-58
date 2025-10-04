using UnityEngine;

public class Sliding : MonoBehaviour
{
    [SerializeField] private float maxSlideSpeed;
    [SerializeField] private float slideAcceleration = 40f; // набор скорости на горке
    [SerializeField] private float slideYScale = 0.5f;
    [SerializeField] private float slopeGravityForce = -5f;
    [SerializeField] private float groundCheckDistance = 1.2f;
    [SerializeField] private float maxSlopeAngle = 45f;
    [SerializeField] private LayerMask slopeLayer;
    [SerializeField] private KeyCode slideKey;

    private CharacterController controller;
    private PlayerController pc;
    private Vector3 slideVelocity;
    private float startYScale;
    private bool isSliding;
    private RaycastHit slopeHit;
    private float verticalVelocity;
    private float maxSpeedReached;
    private bool wasOnSlope;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        pc = GetComponent<PlayerController>();
        startYScale = transform.localScale.y;
    }

    private void Update()
    {
        if (!isSliding && Input.GetKey(slideKey) && CanSlide())
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

    private bool CanSlide() => OnSlope() || pc.GetCurrentSpeed() >= pc.maxSpeed;

    private bool OnSlope()
    {
        Vector3 pos = transform.position + Vector3.up * 0.1f;
        LayerMask layer = slopeLayer.value != 0 ? slopeLayer : ~0;
        
        if (!Physics.Raycast(pos, Vector3.down, out slopeHit, groundCheckDistance + 0.5f, layer))
        {
            return false;
        }
        
        float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
        return angle >= 5f && angle < maxSlopeAngle;
    }

    private bool IsGrounded()
    {
        Vector3 pos = transform.position + Vector3.up * 0.1f;
        LayerMask layer = slopeLayer.value != 0 ? slopeLayer : ~0;
        return Physics.Raycast(pos, Vector3.down, groundCheckDistance, layer);
    }

    private void StartSlide()
    {
        isSliding = true;
        pc.SetSliding(true);
        transform.localScale = new Vector3(transform.localScale.x, slideYScale, transform.localScale.z);
        
        slideVelocity = controller.velocity;
        slideVelocity.y = 0;
        if (slideVelocity.magnitude < 2f) slideVelocity = Vector3.zero;
        
        maxSpeedReached = slideVelocity.magnitude;
        wasOnSlope = false;
    }

    private void HandleSliding()
    {
        bool grounded = IsGrounded();
        bool onSlope = OnSlope();

        if (grounded)
        {
            verticalVelocity = verticalVelocity < 0 ? -2f : verticalVelocity;
            
            if (onSlope)
            {
                verticalVelocity = verticalVelocity > 0 ? Mathf.MoveTowards(verticalVelocity, slopeGravityForce, 100f * Time.deltaTime) : slopeGravityForce;
                wasOnSlope = true;
                
                if (slideVelocity.magnitude > 0)
                {
                    slideVelocity = Vector3.ProjectOnPlane(slideVelocity, slopeHit.normal).normalized * slideVelocity.magnitude;
                }
                
                slideVelocity += Vector3.ProjectOnPlane(pc.cam.forward, slopeHit.normal).normalized * slideAcceleration * Time.deltaTime;
                slideVelocity = Vector3.ClampMagnitude(slideVelocity, maxSlideSpeed);
                
                if (slideVelocity.magnitude > maxSpeedReached)
                {
                    maxSpeedReached = slideVelocity.magnitude;
                }
            }
        }
        else
        {
            verticalVelocity += pc.gravity * Time.deltaTime;
        }
        
        controller.Move((slideVelocity + Vector3.up * verticalVelocity) * Time.deltaTime);
    }

    private void StopSlide()
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

    public bool IsSliding() => isSliding;
    public bool IsGroundedForJump() => IsGrounded();
    public void JumpWhileSliding(float force) => verticalVelocity = force;
}