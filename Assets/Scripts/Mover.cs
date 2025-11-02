using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Mover : MonoBehaviour
{
    [SerializeField] float moveSpeed = 10f;
    private Vector3 movement;
    private float originalSpeed;
    private Vector3 originalScale;

    // Optional HUD icons (assign in Inspector if used)
    public Image speedBoostIcon;
    public Image shieldIcon;
    public Image hinderIcon;
    // Optional HUD countdown text for active powerup
    public Text powerupCountdownText;
    
    void Start()
    {
        PrintInstructions();
        originalSpeed = moveSpeed;
        originalScale = transform.localScale;

        if (speedBoostIcon != null) speedBoostIcon.enabled = false;
        if (shieldIcon != null) shieldIcon.enabled = false;
        if (hinderIcon != null) hinderIcon.enabled = false;
        if (powerupCountdownText != null) powerupCountdownText.enabled = false;
    }

    void Update()
    {
        MovePlayer();
    }

    void PrintInstructions() {
        Debug.Log("WELCOME TO THE GAME!");
        Debug.Log("W A S D!");
    }
    
    void MovePlayer() {
        // Cache input values to avoid multiple calls
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        
        // Only move if there's input
        if (horizontal != 0f || vertical != 0f)
        {
            movement.Set(horizontal * Time.deltaTime * moveSpeed, 0f, vertical * Time.deltaTime * moveSpeed);
            transform.Translate(movement);
        }
    }

    // Powerup entry point (called by Powerup.cs)
    public void ApplyPowerup(Powerup.PowerupType type, float duration, float multiplier)
    {
        if (type == Powerup.PowerupType.Hinder)
        {
            StartCoroutine(HinderCoroutine(duration, multiplier));
        }
        else if (type == Powerup.PowerupType.SpeedBoost)
        {
            StartCoroutine(SpeedBoostCoroutine(duration, multiplier));
        }
        else if (type == Powerup.PowerupType.Shield)
        {
            // Shield handled elsewhere in your project if present
            // Placeholder for compatibility
        }
        // ScoreMultiplier can be handled where scoring occurs
    }

    private IEnumerator HinderCoroutine(float duration, float multiplier)
    {
        moveSpeed *= multiplier;
        transform.localScale *= multiplier;
        if (hinderIcon != null) hinderIcon.enabled = true;
        if (powerupCountdownText != null)
        {
            powerupCountdownText.enabled = true;
            powerupCountdownText.text = FormatCountdown("Hinder", duration);
        }
        float endTime = Time.time + duration;
        while (Time.time < endTime)
        {
            if (powerupCountdownText != null)
            {
                powerupCountdownText.text = FormatCountdown("Hinder", endTime - Time.time);
            }
            yield return null;
        }

        moveSpeed = originalSpeed;
        transform.localScale = originalScale;
        if (hinderIcon != null) hinderIcon.enabled = false;
        if (powerupCountdownText != null) powerupCountdownText.enabled = false;
    }

    private IEnumerator SpeedBoostCoroutine(float duration, float multiplier)
    {
        float prev = moveSpeed;
        moveSpeed *= multiplier;
        if (speedBoostIcon != null) speedBoostIcon.enabled = true;
        if (powerupCountdownText != null)
        {
            powerupCountdownText.enabled = true;
            powerupCountdownText.text = FormatCountdown("Speed+", duration);
        }
        float endTime = Time.time + duration;
        while (Time.time < endTime)
        {
            if (powerupCountdownText != null)
            {
                powerupCountdownText.text = FormatCountdown("Speed+", endTime - Time.time);
            }
            yield return null;
        }

        moveSpeed = prev;
        if (speedBoostIcon != null) speedBoostIcon.enabled = false;
        if (powerupCountdownText != null) powerupCountdownText.enabled = false;
    }

    // Slow zone helpers
    public void ChangeSpeed(float multiplier)
    {
        moveSpeed *= multiplier;
    }

    public void ResetSpeed()
    {
        moveSpeed = originalSpeed;
    }

    private string FormatCountdown(string label, float seconds)
    {
        if (seconds < 0f) seconds = 0f;
        return label + ": " + seconds.ToString("0.0") + "s";
    }
}
