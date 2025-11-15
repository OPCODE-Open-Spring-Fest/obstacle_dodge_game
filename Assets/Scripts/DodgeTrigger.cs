using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DodgeTrigger : MonoBehaviour
{
    private const string PLAYER_TAG = "Player";
    
    private bool hasBeenTriggered = false;

    void OnTriggerEnter(Collider other)
    {
        if (hasBeenTriggered) return;

        if (other.gameObject.CompareTag(PLAYER_TAG))
        {
            if (StreakManager.Instance != null)
            {
                StreakManager.Instance.IncreaseStreak();
            }
            
            hasBeenTriggered = true;
        }
    }
}