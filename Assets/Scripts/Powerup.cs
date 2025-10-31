using UnityEngine;

public class Powerup : MonoBehaviour
{
    public enum PowerupType
    {
        SpeedBoost,
        Shield,
        ScoreMultiplier,
        Hinder
    }

    public PowerupType type = PowerupType.SpeedBoost;
    public float duration = 5f;
    public float multiplier = 1.5f; // default for non-speed boosts

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        var mover = other.GetComponent<Mover>();
        if (mover != null)
        {
            // Force SpeedBoost to 5x regardless of the inspector multiplier
            float appliedMultiplier = (type == PowerupType.SpeedBoost) ? 5f : multiplier;
            mover.ApplyPowerup(type, duration, appliedMultiplier);
        }

        Destroy(gameObject);
    }
}


