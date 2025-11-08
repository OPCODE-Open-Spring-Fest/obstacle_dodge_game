using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class Mover : MonoBehaviour
{
    [SerializeField] float moveSpeed = 10f;
    private Vector3 movement;
    private float originalSpeed;
    private Vector3 originalScale;

    private Rigidbody rb;
    private bool isKnockedBack = false;

    public Image speedBoostIcon;
    public Image shieldIcon;
    public Image hinderIcon;
    public Text powerupCountdownText;

    void Start()
    {
        PrintInstructions();
        originalSpeed = moveSpeed;
        originalScale = transform.localScale;

        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        if (speedBoostIcon != null) speedBoostIcon.enabled = false;
        if (shieldIcon != null) shieldIcon.enabled = false;
        if (hinderIcon != null) hinderIcon.enabled = false;
        if (powerupCountdownText != null) powerupCountdownText.enabled = false;
    }

    void Update()
    {
        HandleMovementInput();
    }

    void FixedUpdate()
    {
        if (!isKnockedBack)
        {
            rb.MovePosition(rb.position + movement * Time.fixedDeltaTime);
        }
    }
    void HandleMovementInput()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        movement = new Vector3(horizontal * moveSpeed, 0f, vertical * moveSpeed);
    }
    public void Knockback(Vector3 direction, float force)
    {
        if (isKnockedBack) return;
        StartCoroutine(KnockbackRoutine(direction, force));
    }

    private IEnumerator KnockbackRoutine(Vector3 direction, float force)
    {
        isKnockedBack = true;
        rb.AddForce(direction * force, ForceMode.Impulse);

        yield return new WaitForSeconds(0.15f);
        isKnockedBack = false;
    }

    void PrintInstructions()
    {
        Debug.Log("WELCOME TO THE GAME!");
        Debug.Log("W A S D!");
    }
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
            // shield
        }
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
        float end = Time.time + duration;
        while (Time.time < end)
        {
            if (powerupCountdownText != null)
                powerupCountdownText.text = FormatCountdown("Hinder", end - Time.time);
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
        float end = Time.time + duration;
        while (Time.time < end)
        {
            if (powerupCountdownText != null)
                powerupCountdownText.text = FormatCountdown("Speed+", end - Time.time);
            yield return null;
        }
        moveSpeed = prev;
        if (speedBoostIcon != null) speedBoostIcon.enabled = false;
        if (powerupCountdownText != null) powerupCountdownText.enabled = false;
    }

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