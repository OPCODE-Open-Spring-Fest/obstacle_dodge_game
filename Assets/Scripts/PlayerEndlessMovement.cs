using UnityEngine;

public class PlayerEndlessMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("Forward movement speed")]
    public float speed = 5f;
    
    [Tooltip("Lateral movement speed (left/right)")]
    public float lateralSpeed = 5f;
    
    [Tooltip("Maximum lateral movement distance from center")]
    public float maxLateralDistance = 5f;
    
    [Header("Input Settings")]
    [Tooltip("Use horizontal input for lateral movement")]
    public bool useLateralMovement = true;
    
    [Tooltip("Input smoothing")]
    public float inputSmoothing = 10f;
    
    private Rigidbody rb;
    private Vector2 movementInput;
    private Vector2 smoothedInput;
    private float currentLateralPosition = 0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("PlayerEndlessMovement: Rigidbody component not found!");
        }
    }

    void Update()
    {
        // Get input
        movementInput.x = Input.GetAxis("Horizontal");
        movementInput.y = Input.GetAxis("Vertical");
        
        // Smooth input
        smoothedInput = Vector2.Lerp(smoothedInput, movementInput, Time.deltaTime * inputSmoothing);
    }

    void FixedUpdate()
    {
        if (rb == null) return;

        // Always move forward
        Vector3 forwardMove = Vector3.forward * speed * Time.fixedDeltaTime;
        
        // Lateral movement (left/right)
        if (useLateralMovement)
        {
            float lateralInput = smoothedInput.x;
            float targetLateralPosition = Mathf.Clamp(
                currentLateralPosition + (lateralInput * lateralSpeed * Time.fixedDeltaTime),
                -maxLateralDistance,
                maxLateralDistance
            );
            currentLateralPosition = targetLateralPosition;
            
            Vector3 lateralMove = Vector3.right * (targetLateralPosition - transform.position.x);
            rb.MovePosition(rb.position + forwardMove + lateralMove);
        }
        else
        {
            // Original rotation-based movement
            rb.MovePosition(rb.position + forwardMove);
            
            if (movementInput != Vector2.zero)
            {
                float angle = Mathf.Atan2(movementInput.y, movementInput.x) * Mathf.Rad2Deg;
                Quaternion targetRotation = Quaternion.Euler(0f, angle, 0f);
                rb.MoveRotation(Quaternion.Lerp(rb.rotation, targetRotation, Time.fixedDeltaTime * 5f));
            }
        }
    }

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    public float GetSpeed()
    {
        return speed;
    }
}