using UnityEngine;

public class PlayerEndlessMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 5f;
    public float lateralSpeed = 5f;
    public float maxLateralDistance = 5f;
    
    [Header("Input Settings")]
    public bool useLateralMovement = true;
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

    /// <summary>
    /// NEW METHOD: Called by EndlessGameManager to set the player's start position.
    /// </summary>
    public void SetInitialPosition(float startDistance)
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }

        // Get current Y position to preserve it
        float currentY = transform.position.y; 

        // Set the Rigidbody's position
        Vector3 startPos = new Vector3(0f, currentY, startDistance);
        rb.position = startPos; 
        
        // Set the transform's position for good measure
        transform.position = startPos;
        
        // Reset lateral position
        currentLateralPosition = 0f;
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
        if (!EndlessGameManager.Instance.isGameRunning) return; // Don't move if game not running

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
            
            // This lerps the lateral position for smooth movement
            currentLateralPosition = Mathf.Lerp(currentLateralPosition, targetLateralPosition, Time.fixedDeltaTime * lateralSpeed);
            
            // Calculate the target X position based on currentLateralPosition
            Vector3 targetPosition = rb.position + forwardMove;
            targetPosition.x = currentLateralPosition;
            
            rb.MovePosition(targetPosition);
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