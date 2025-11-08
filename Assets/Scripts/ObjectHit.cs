using UnityEngine;

public class ObjectHit : MonoBehaviour
{
    private MeshRenderer meshRenderer;
    private bool hasBeenHit = false;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        PlayerCollectibles collectibles = other.gameObject.GetComponent<PlayerCollectibles>();
        if (collectibles != null && collectibles.IsInvincible)
        {
            return;
        }
        if (hasBeenHit) return;
        hasBeenHit = true;

        meshRenderer.material.color = Color.red;
        gameObject.tag = "Hit";


        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayCollisionSFX();
        }
        other.gameObject.GetComponent<Mover>()?.Knockback(-other.contacts[0].normal, 5f);
    }
}
