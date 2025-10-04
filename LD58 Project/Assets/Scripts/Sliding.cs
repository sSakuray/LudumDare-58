using UnityEngine;

public class Sliding : MonoBehaviour
{
    [Header("Sliding Settings")]
    [SerializeField] private float maxSlideSpeed = 25f;
    [SerializeField] private float slideAcceleration = 40f;
    [SerializeField] private float slideYScale = 0.5f;
    [SerializeField] private KeyCode slideKey = KeyCode.LeftControl;

    [Header("Slope Detection")]
    [SerializeField] private float groundCheckDistance = 1.08f;
    [SerializeField] private float maxSlopeAngle = 45f;

    private CharacterController controller;
    private PlayerController pc;
    private Vector3 currentVelocity;
    private float startYScale;
    private bool sliding = false;
    private RaycastHit slopeHit;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        pc = GetComponent<PlayerController>();
        startYScale = transform.localScale.y;
    }

    private void Update()
    {
        if (Input.GetKeyDown(slideKey) && OnSlope() && !sliding)
        {
            StartSlide();
        }

        if (Input.GetKeyUp(slideKey) && sliding)
        {
            StopSlide();
        }

        if (sliding)
        {
            HandleSliding();
        }
    }

    private bool OnSlope()
    {
        Vector3 rayStart = transform.position + Vector3.up * 0.1f;
        
        if (Physics.Raycast(rayStart, Vector3.down, out slopeHit, groundCheckDistance + 0.5f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle >= 5f && angle < maxSlopeAngle + 0.5f;
        }
        
        return false;
    }

    private void StartSlide()
    {
        sliding = true;
        pc.SetSliding(true);
        transform.localScale = new Vector3(transform.localScale.x, slideYScale, transform.localScale.z);
        currentVelocity = Vector3.zero;
    }

    private void HandleSliding()
    {
        if (!OnSlope())
        {
            StopSlide();
            return;
        }

        Vector3 slopeDirection = Vector3.ProjectOnPlane(Vector3.down, slopeHit.normal).normalized;

        currentVelocity += slopeDirection * slideAcceleration * Time.deltaTime;

        if (currentVelocity.magnitude > maxSlideSpeed)
        {
            currentVelocity = currentVelocity.normalized * maxSlideSpeed;
        }

        controller.Move(currentVelocity * Time.deltaTime);
    }

    private void StopSlide()
    {
        sliding = false;
        pc.SetSliding(false);
        transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        currentVelocity = Vector3.zero;
    }

    public bool IsSliding()
    {
        return sliding;
    }
}