using UnityEngine;

public class Grappling : MonoBehaviour
{
    [Header("Grapple Settings")]
    [SerializeField] private Transform grappleGunTip;   
    [SerializeField] private Transform cam;           
    [SerializeField] private Transform player;        
    [SerializeField] public LayerMask whatIsGrappleable; // Слой объектов к которым можно цепляться
    [SerializeField] public LineRenderer lr;           
    [SerializeField] private KeyCode grappleKey = KeyCode.Mouse0; // Клавиша для граплинга (по умолчанию ЛКМ)
    [SerializeField] private float maxGrappleDistance; // Максимальная дистанция граплинга
    [SerializeField] private float grappleSpeed;       // Скорость подтягивания к точке
    [SerializeField] private float grapplingCd;       // Кулдаун между выстрелами (в секундах)
    
    [Header("Rope Animation")]
    [SerializeField] private int ropeSegments; // Количество сегментов веревки для анимации
    [SerializeField] private float ropeWaveSpeed; // Скорость волн на веревке
    [SerializeField] private float ropeWaveHeight; // Высота волн на веревке
    
    private Vector3 currentGrapplePosition;
    private Vector3 grapplePoint;
    private bool grappling;
    private bool pulling;
    private float grapplingCdTimer;
    private PlayerController pc;
    private bool hookFlying;
    private float hookFlyProgress;
    private Soul targetSoul; 

    void Start()
    {
        if (grappleGunTip == null)
        {
            grappleGunTip = transform;
        }
        if (lr == null)
        {
            lr = GetComponent<LineRenderer>();
        }
        if (cam == null)
        {
            cam = Camera.main?.transform;
        }
        
        if (player != null)
        {
            pc = player.GetComponent<PlayerController>();
        }
        else
        {
            pc = GetComponentInParent<PlayerController>();
            if (pc == null) 
            {
                pc = GetComponent<PlayerController>();
            }
            if (pc != null) 
            {
                player = pc.transform;
            }
        }
    }

    bool IsValidVector(Vector3 vector)
    {
        return !float.IsNaN(vector.x) && !float.IsNaN(vector.y) && !float.IsNaN(vector.z) &&
        !float.IsInfinity(vector.x) && !float.IsInfinity(vector.y) && !float.IsInfinity(vector.z);
    }

    void Update()
    {
        if (Input.GetKeyDown(grappleKey))
        {
            StartGrapple();
        }
        if (grapplingCdTimer > 0)
        {
            grapplingCdTimer -= Time.deltaTime;
        }
        if (pulling)
        {
            UpdateGrapplePull();
        }
    }

    void UpdateGrapplePull()
    {
        float dist = Vector3.Distance(transform.position, grapplePoint);
        if (dist < 1f) 
        {
            StopGrapple(); 
            return; 
        }
        
        Vector3 dir = (grapplePoint - transform.position).normalized;
        float speed = grappleSpeed;
        
        float gravityCompensation = Mathf.Abs(pc.gravity);
        
        pc.horizontalVelocity = new Vector3(dir.x, 0f, dir.z) * speed;
        pc.verticalVelocity = dir.y * speed + gravityCompensation * Time.deltaTime;
    }

    void LateUpdate()
    {
        DrawRope();
    }

    void StartGrapple()
    {
        if (cam == null || grapplingCdTimer > 0 || grappling) 
        {
            return;
        }
        grappling = true;
        bool hit = Physics.Raycast(cam.position, cam.forward, out RaycastHit hitInfo, maxGrappleDistance);
        grapplePoint = hit ? hitInfo.point : cam.position + cam.forward * maxGrappleDistance;
        
        targetSoul = null;
        if (hit)
        {
            Soul soul = hitInfo.collider.GetComponent<Soul>();
            if (soul != null)
            {
                targetSoul = soul;
            }
        }
        
        lr.enabled = true; lr.positionCount = ropeSegments;
        currentGrapplePosition = grappleGunTip.position;
        hookFlying = true;
        hookFlyProgress = 0f;
        
        if (targetSoul != null)
        {
            Invoke(nameof(StopGrapple), 0.5f);
        }
        else if (hit && ((1 << hitInfo.collider.gameObject.layer) & whatIsGrappleable) != 0)
        {
            pc.horizontalVelocity = Vector3.zero; pc.verticalVelocity = 0f;
            pc.activeGrappling = true; pulling = true;
        }
        else
        {
            Invoke(nameof(StopGrapple), 0.5f);
        }
    }

    void StopGrapple()
    {
        if (!grappling) 
        {
            return;
        }
        CancelInvoke();
        pulling = false; pc.freeze = false; pc.activeGrappling = false;
        grappling = false; grapplingCdTimer = grapplingCd;
        hookFlying = false;
        lr.enabled = false; lr.positionCount = 0;
    }

    void DrawRope()
    {
        if (!grappling || lr.positionCount < 2) 
        {
            return;
        }

        Vector3 startPos = grappleGunTip.position;
        Vector3 endPos = grapplePoint;

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
                
                if (targetSoul != null)
                {
                    targetSoul.Collect();
                    targetSoul = null;
                }
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
}
