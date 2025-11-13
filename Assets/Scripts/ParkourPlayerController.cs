using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ParkourPlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("Movement speed for horizontal movement")]
    [SerializeField] private float moveSpeed = 10f;
    
    [Tooltip("Jump force applied when jumping")]
    [SerializeField] private float jumpForce = 12f;
    
    [Tooltip("Ground check distance (how far down to check for ground)")]
    [SerializeField] private float groundCheckDistance = 0.5f;
    
    [Tooltip("Ground check radius (for sphere cast)")]
    [SerializeField] private float groundCheckRadius = 0.4f;
    
    [Tooltip("Layer mask for what counts as ground. Set to 'Everything' if not working.")]
    [SerializeField] private LayerMask groundLayerMask = -1; // Everything by default
    
    [Tooltip("Optional: Transform representing ground check point (if not set, uses player position)")]
    [SerializeField] private Transform groundCheckPoint;
    
    [Tooltip("Disable the Mover script if it exists on this GameObject")]
    [SerializeField] private bool disableMoverScript = true;
    
    [Header("Air Control")]
    [Tooltip("How much control player has while in the air (0 = no control, 1 = full control)")]
    [SerializeField] [Range(0f, 1f)] private float airControl = 0.5f;
    
    [Header("Input Settings")]
    [Tooltip("Use Space key for jumping (can also use W/Up arrow)")]
    [SerializeField] private bool useSpaceForJump = true;
    
    private Rigidbody rb;
    private bool isGrounded;
    private bool wasGroundedLastFrame;
    private bool justJumped;
    private float jumpCooldown = 0.1f;
    private float lastJumpTime;
    private Vector3 movement;
    private float originalMoveSpeed;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("[ParkourPlayerController] Rigidbody component required!");
        }
        
        rb.freezeRotation = true;
        originalMoveSpeed = moveSpeed;
    }
    
    private void Start()
    {
        PrintInstructions();
        
        if (groundCheckPoint == null)
        {
            groundCheckPoint = transform;
        }
        
        if (disableMoverScript)
        {
            Mover mover = GetComponent<Mover>();
            if (mover != null)
            {
                mover.enabled = false;
                Debug.Log("[ParkourPlayerController] Disabled Mover script to avoid conflicts.");
            }
        }
        
        if (rb != null)
        {
            rb.useGravity = true;
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            
            rb.isKinematic = false;
        }
        
        Collider playerCollider = GetComponent<Collider>();
        if (playerCollider == null)
        {
            Debug.LogError("[ParkourPlayerController] Player needs a Collider component!");
        }
        else if (playerCollider.isTrigger)
        {
            Debug.LogWarning("[ParkourPlayerController] Player collider is set as Trigger! This will prevent physics collisions. Setting to non-trigger.");
            playerCollider.isTrigger = false;
        }
    }
    
    private void Update()
    {
        wasGroundedLastFrame = isGrounded;
        CheckGrounded();
        HandleInput();
        
        if (justJumped && (Time.time - lastJumpTime > jumpCooldown || (wasGroundedLastFrame && isGrounded)))
        {
            justJumped = false;
        }
    }
    
    private void FixedUpdate()
    {
        ApplyMovement();
    }
    
    private void PrintInstructions()
    {
        Debug.Log("PARKOUR LEVEL!");
        Debug.Log("WASD to move, Space to jump!");
    }
    
    private void CheckGrounded()
    {
        Collider col = GetComponent<Collider>();
        Vector3 checkPosition;
        
        if (col != null)
        {
            checkPosition = col.bounds.center;
            checkPosition.y = col.bounds.min.y;
        }
        else
        {
            checkPosition = groundCheckPoint != null ? groundCheckPoint.position : transform.position;
        }
        
        Vector3 sphereCheckPos = checkPosition + Vector3.up * groundCheckRadius;
        isGrounded = Physics.CheckSphere(sphereCheckPos, groundCheckRadius, groundLayerMask);
        
        if (!isGrounded)
        {
            RaycastHit hit;
            if (Physics.Raycast(checkPosition + Vector3.up * 0.1f, Vector3.down, out hit, groundCheckDistance + 0.1f, groundLayerMask))
            {
                isGrounded = true;
            }
        }
        
        if (!isGrounded && rb != null && Mathf.Abs(rb.linearVelocity.y) < 0.05f)
        {
            RaycastHit hit;
            if (Physics.Raycast(checkPosition, Vector3.down, out hit, groundCheckDistance * 1.5f, groundLayerMask))
            {
                isGrounded = true;
            }
        }
        
        if (rb != null && rb.linearVelocity.y > 0.1f)
        {
            isGrounded = false;
        }
        
        #if UNITY_EDITOR
        Debug.DrawRay(checkPosition, Vector3.down * groundCheckDistance, isGrounded ? Color.green : Color.red);
        #endif
    }
    
    private void HandleInput()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        
        movement = new Vector3(horizontal, 0f, vertical).normalized;
        
        bool jumpPressed = Input.GetKeyDown(KeyCode.Space) || 
                            (useSpaceForJump && Input.GetButtonDown("Jump"));
        
        if (jumpPressed)
        {
            bool canJump = isGrounded && !justJumped && (Time.time - lastJumpTime > jumpCooldown);
            
            if (canJump)
            {
                Jump();
            }
            else if (!isGrounded)
            {
                if (Time.time - lastJumpTime > 1f)
                {
                    Debug.Log($"[ParkourPlayerController] Cannot jump - not grounded. isGrounded: {isGrounded}, velocity.y: {rb.linearVelocity.y}");
                }
            }
        }
    }
    
    private void ApplyMovement()
    {
        if (rb == null) return;
        
        float effectiveSpeed = moveSpeed;
        if (!isGrounded)
        {
            effectiveSpeed *= airControl;
        }
        
        Vector3 horizontalVelocity = new Vector3(movement.x * effectiveSpeed, 0f, movement.z * effectiveSpeed);
        Vector3 currentVelocity = rb.linearVelocity;
        
        rb.linearVelocity = new Vector3(horizontalVelocity.x, currentVelocity.y, horizontalVelocity.z);
    }
    
    private void Jump()
    {
        if (rb == null)
        {
            Debug.LogError("[ParkourPlayerController] Cannot jump - Rigidbody is null!");
            return;
        }
        
        if (justJumped || !isGrounded)
        {
            return;
        }
        
        Vector3 velocity = rb.linearVelocity;
        velocity.y = 0f;
        rb.linearVelocity = velocity;
        
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        
        isGrounded = false;
        justJumped = true;
        lastJumpTime = Time.time;
        
        Debug.Log($"[ParkourPlayerController] Jumped! Force: {jumpForce}");
        
        if (AudioManager.Instance != null)
        {
        }
    }

    public bool IsGrounded()
    {
        return isGrounded;
    }
    
    public void SetMoveSpeed(float speed)
    {
        moveSpeed = speed;
    }
    
    public void ResetMoveSpeed()
    {
        moveSpeed = originalMoveSpeed;
    }

    public void SetJumpForce(float force)
    {
        jumpForce = force;
    }

    private void OnDrawGizmosSelected()
    {
        Collider col = GetComponent<Collider>();
        Vector3 checkPosition;
        
        if (col != null)
        {
            checkPosition = col.bounds.center;
            checkPosition.y = col.bounds.min.y;
        }
        else
        {
            checkPosition = groundCheckPoint != null ? groundCheckPoint.position : transform.position;
        }
        
        Gizmos.color = Color.yellow;
        Vector3 sphereCheckPos = checkPosition + Vector3.up * groundCheckRadius;
        Gizmos.DrawWireSphere(sphereCheckPos, groundCheckRadius);
        
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(checkPosition, checkPosition + Vector3.down * groundCheckDistance);
    }
}