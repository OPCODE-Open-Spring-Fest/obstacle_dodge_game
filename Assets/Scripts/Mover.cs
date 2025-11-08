using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class Mover : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float dashSpeed = 30f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 5f;
    [SerializeField] private float postDashInvincibility = 1f; // 1-second buffer

    private bool isDashing = false;
    private bool isInvincible = false;
    public bool IsInvincible => isInvincible;

    private float nextDashTime = 0f;

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
        HandleDashInput();
    }

    void FixedUpdate()
    {
        if (isKnockedBack) return;
        if (isDashing) return;

        rb.MovePosition(rb.position + movement * Time.fixedDeltaTime);
    }

    private void HandleMovementInput()
    {
        if (isDashing) return; 
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        movement = new Vector3(horizontal * moveSpeed, 0f, vertical * moveSpeed);
    }

    private void HandleDashInput()
    {
        if (Time.time < nextDashTime) return;
        if (!Input.GetKeyDown(KeyCode.J)) return;

        Vector3 dashDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        if (dashDir == Vector3.zero) return;

        dashDir.Normalize();
        StartCoroutine(DashRoutine(dashDir));
    }

    private IEnumerator DashRoutine(Vector3 dashDirection)
    {
        isDashing = true;
        PlayerCollectibles pc = GetComponent<PlayerCollectibles>();
        if (pc != null)
        {
            pc.StartInvincibility(dashDuration + 1f);
        }
        rb.linearVelocity = dashDirection * dashSpeed;

        yield return new WaitForSeconds(dashDuration);

        rb.linearVelocity = Vector3.zero;
        isDashing = false;
        nextDashTime = Time.time + dashCooldown;
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

    // ---------- Powerups ----------
    public void ApplyPowerup(Powerup.PowerupType type, float duration, float multiplier)
    {
        if (type == Powerup.PowerupType.Hinder)
            StartCoroutine(HinderCoroutine(duration, multiplier));
        else if (type == Powerup.PowerupType.SpeedBoost)
            StartCoroutine(SpeedBoostCoroutine(duration, multiplier));
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

    public void ChangeSpeed(float multiplier) => moveSpeed *= multiplier;
    public void ResetSpeed() => moveSpeed = originalSpeed;

    private string FormatCountdown(string label, float seconds)
    {
        if (seconds < 0f) seconds = 0f;
        return label + ": " + seconds.ToString("0.0") + "s";
    }
}
