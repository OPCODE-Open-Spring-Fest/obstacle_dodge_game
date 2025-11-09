using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EndlessRunnerMover : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float horizontalSpeed = 10f;
    
    [Header("Physics Settings")]
    [SerializeField] private bool useGravity = true;
    [SerializeField] private bool freezeRotation = true;
    
    private Rigidbody rb;
    private float baseMoveSpeed;
    private Vector3 movement;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("[EndlessRunnerMover] Rigidbody component required!");
            return;
        }
        
        baseMoveSpeed = moveSpeed;
        
        rb.useGravity = useGravity;
        if (freezeRotation)
        {
            rb.freezeRotation = true;
        }
        
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.isKinematic = false;
        
        Collider col = GetComponent<Collider>();
        if (col != null && col.isTrigger)
        {
            col.isTrigger = false;
            Debug.LogWarning("[EndlessRunnerMover] Player collider was set as trigger. Changed to non-trigger for physics.");
        }
    }
    
    void Update()
    {
        HandleInput();
    }
    
    void FixedUpdate()
    {
        ApplyMovement();
    }
    
    private void HandleInput()
    {
        float horizontal = Input.GetAxis("Horizontal");
        
        movement = new Vector3(horizontal * horizontalSpeed, 0f, moveSpeed);
    }
    
    private void ApplyMovement()
    {
        if (rb == null) return;
        
        Vector3 newPosition = rb.position + movement * Time.fixedDeltaTime;
        rb.MovePosition(newPosition);
    }
    
    public void SetSpeed(float newSpeed)
    {
        moveSpeed = newSpeed;
        movement.z = moveSpeed;
    }
    
    public float GetSpeed()
    {
        return moveSpeed;
    }
    
    public float GetBaseSpeed()
    {
        return baseMoveSpeed;
    }
    
    public void ResetSpeed()
    {
        moveSpeed = baseMoveSpeed;
        movement.z = moveSpeed;
    }
}

