using UnityEngine;

public class Grappling : MonoBehaviour
{
    private PlayerController pc;
    [SerializeField] private Transform cam;
    [SerializeField] private Transform gunTip;
    [SerializeField] public LayerMask whatIsGrappleable;
    [SerializeField] public LineRenderer lr;
    [SerializeField] public float maxGrappleDistance;
    [SerializeField] public float grappleDelayTime;
    private Vector3 grapplePoint;
    [SerializeField] private float grapplingCd;
    [SerializeField] private float grapplingCdTimer;

    public KeyCode grappleKey = KeyCode.Mouse0;
    private bool grappling;

    private void Start()
    {
        pc = GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(grappleKey))
        {
            StartGrapple();
        }

        if (grapplingCdTimer > 0)
        {
            grapplingCdTimer -= Time.deltaTime;
        }
    }

    private void LateUpdate()
    {
        if (grappling)
        {
            lr.SetPosition(0, gunTip.position);
        }
    }

    private void StartGrapple()
    {
        if (grapplingCdTimer > 0)
        {
            return;
        }
        grappling = true;

        RaycastHit hit;
        if (Physics.Raycast(cam.position, cam.forward, out hit, maxGrappleDistance, whatIsGrappleable))
        {
            grapplePoint = hit.point;
            Invoke(nameof(ExecuteGrapple), grappleDelayTime);
        }
        else
        {
            grapplePoint = cam.position + cam.forward * maxGrappleDistance;
            Invoke(nameof(StopGrapple), grappleDelayTime);
        }

        lr.enabled = true;
        lr.SetPosition(1, grapplePoint);
    }

    private void ExecuteGrapple()
    {
        grapplingCdTimer = grapplingCd;
    }
    
    private void StopGrapple()
    {
        grappling = false;
        grapplingCdTimer = grapplingCd;
        lr.enabled = false;
    }
}
