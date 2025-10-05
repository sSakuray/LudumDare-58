using UnityEngine;

public class Swinging : MonoBehaviour
{
    [SerializeField] private Transform swingGunTip;     
    [SerializeField] private Transform cam;       
    [SerializeField] private Transform player;          
    [SerializeField] private LayerMask whatIsGrappleable; // Слой объектов к которым можно цепляться
    [SerializeField] private LineRenderer lr;          
    [SerializeField] private KeyCode swingKey = KeyCode.Mouse1; 
    [SerializeField] private float maxSwingDistance; // Максимальная дистанция крюка 
    [SerializeField] private float swingForce = 15f; // Базовая сила раскачки
    [SerializeField] private float swingSpeedBonus = 25f; // Бонус силы от скорости персонажа
    private int ropeSegments = 25;     
    private float ropeWaveSpeed = 10f;
    private float ropeWaveHeight = 0.5f; 
    
    public float currentSwingSpeed; // Текущая скорость качания (для отладки)
    public float swingAngle; // Угол качания относительно вертикали (для отладки)
    public float currentRopeLengthDebug; // Текущая длина веревки (для отладки)
    
    private Vector3 swingPoint;
    private Vector3 currentGrapplePosition;
    private bool swinging;
    private float currentRopeLength;
    private PlayerController pc;
    private bool hookFlying; 
    private float hookFlyProgress;

    void Start()
    {
        if (swingGunTip == null) swingGunTip = transform;
        if (lr == null) lr = GetComponent<LineRenderer>();
        if (cam == null) cam = Camera.main?.transform;
        
        if (player != null)
        {
            pc = player.GetComponent<PlayerController>();
        }
        else
        {
            pc = GetComponentInParent<PlayerController>();
            if (pc == null) pc = GetComponent<PlayerController>();
            if (pc != null) player = pc.transform;
        }
    }

    bool IsValidVector(Vector3 vector)
    {
        return !float.IsNaN(vector.x) && !float.IsNaN(vector.y) && !float.IsNaN(vector.z) &&
        !float.IsInfinity(vector.x) && !float.IsInfinity(vector.y) && !float.IsInfinity(vector.z);
    }

    void Update()
    {
        if (Input.GetKeyDown(swingKey))
        {
            StartSwing();
        }
        if (Input.GetKeyUp(swingKey))
        {
            StopSwing();
        }
        if (swinging)
        {
            UpdateSwing();
        }
    }
    
    void UpdateSwing()
    {
        float currentDist = Vector3.Distance(player.position, swingPoint);
        if (currentDist > currentRopeLength)
        {
            player.position = swingPoint + (player.position - swingPoint).normalized * currentRopeLength;
        }
        
        Vector3 ropeDir = (swingPoint - player.position).normalized;
        Vector3 vel = pc.horizontalVelocity + Vector3.up * pc.verticalVelocity;
        
        vel += Vector3.ProjectOnPlane(Vector3.down * Mathf.Abs(pc.gravity), ropeDir) * Time.deltaTime;
        
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (input.magnitude > 0.1f)
        {
            float force = swingForce + (pc.currentSpeed / pc.maxSpeed * swingSpeedBonus);
            Vector3 inputDir = (cam.right * input.x + cam.forward * input.y); inputDir.y = 0; inputDir.Normalize();
            Vector3 tangInput = Vector3.ProjectOnPlane(inputDir, ropeDir);
            Vector3 tangVel = Vector3.ProjectOnPlane(vel, ropeDir);
            float bonus = tangVel.magnitude > 0.5f && Vector3.Dot(tangInput.normalized, tangVel.normalized) > 0 ? 2f : 1f;
            vel += tangInput * force * bonus * Time.deltaTime;
        }
        
        vel = Vector3.ProjectOnPlane(vel, ropeDir);
        
        pc.horizontalVelocity = new Vector3(vel.x, 0f, vel.z);
        pc.verticalVelocity = vel.y;
        UpdateDebugInfo();
    }
    
    void UpdateDebugInfo()
    {
        Vector3 vel = pc.horizontalVelocity; vel.y = pc.verticalVelocity;
        currentSwingSpeed = vel.magnitude;
        swingAngle = Vector3.Angle(Vector3.down, player.position - swingPoint);
        currentRopeLengthDebug = currentRopeLength;
    }

    void LateUpdate()
    {
        DrawRope();
    }

    void StartSwing()
    {
        if (cam == null || swinging || !Physics.Raycast(cam.position, cam.forward, out RaycastHit hit, maxSwingDistance, whatIsGrappleable)) 
        {
            return;
        }
        swingPoint = hit.point; swinging = true; pc.activeGrappling = true;
        currentRopeLength = Vector3.Distance(player.position, swingPoint);
        lr.positionCount = ropeSegments; lr.enabled = true; currentGrapplePosition = swingGunTip.position;
        hookFlying = true;
        hookFlyProgress = 0f;
    }
    
    void StopSwing()
    {
        if (!swinging) 
        {
            return;
        }
        float boost = 0.2f + (pc.currentSpeed / pc.maxSpeed * 0.6f);
        pc.horizontalVelocity += pc.horizontalVelocity.normalized * pc.horizontalVelocity.magnitude * boost;
        if (pc.verticalVelocity < 0) 
        {
            pc.verticalVelocity *= (1f + boost * 0.5f);
        }
        swinging = false; pc.activeGrappling = false;
        hookFlying = false;
        lr.positionCount = 0; lr.enabled = false;
    }

    void DrawRope()
    {
        if (!swinging || lr == null || lr.positionCount < 2) 
        {
            return;
        }

        Vector3 startPos = swingGunTip.position;
        Vector3 endPos = swingPoint;

        if (!IsValidVector(startPos) || !IsValidVector(endPos))
        {
            return;
        }
        if (hookFlying)
        {
            hookFlyProgress += Time.deltaTime * 8f;
            if (hookFlyProgress >= 1f)
            {
                hookFlyProgress = 1f;
                hookFlying = false;
            }
            currentGrapplePosition = Vector3.Lerp(startPos, endPos, hookFlyProgress);
        }
        else
        {
            currentGrapplePosition = endPos;
        }

        if (!IsValidVector(currentGrapplePosition))
        {
            currentGrapplePosition = endPos;
        }

        int segments = lr.positionCount;
        for (int i = 0; i < segments; i++)
        {
            float t = segments > 1 ? (float)i / (segments - 1) : 0f;
            Vector3 point = Vector3.Lerp(startPos, currentGrapplePosition, t);
            
            if (hookFlying && segments > 2)
            {
                Vector3 ropeDirection = (currentGrapplePosition - startPos);
                if (ropeDirection.magnitude > 0.01f)
                {
                    ropeDirection = ropeDirection.normalized;
                    Vector3 perpendicular = Vector3.Cross(ropeDirection, Vector3.up);
                    
                    if (perpendicular.magnitude < 0.1f)
                        perpendicular = Vector3.Cross(ropeDirection, Vector3.right);
                    
                    if (perpendicular.magnitude > 0.01f)
                    {
                        perpendicular = perpendicular.normalized;
                        
                        float wave1 = Mathf.Sin(t * Mathf.PI * 2f + Time.time * ropeWaveSpeed) * ropeWaveHeight;
                        float wave2 = Mathf.Sin(t * Mathf.PI * 3f + Time.time * ropeWaveSpeed * 1.5f) * ropeWaveHeight * 0.3f;
                        float combinedWave = wave1 + wave2;
                        
                        float fadeOut = Mathf.Pow(Mathf.Sin(t * Mathf.PI), 0.5f);
                        combinedWave *= fadeOut;
                        
                        float intensity = Mathf.Clamp01(hookFlyProgress * 2f);
                        combinedWave *= intensity;
                        
                        Vector3 waveOffset = perpendicular * combinedWave;
                        if (IsValidVector(waveOffset))
                        {
                            point += waveOffset;
                        }
                    }
                }
            }
            
            lr.SetPosition(i, point);
        }
    }
    
    public bool IsSwinging() => swinging;
}
