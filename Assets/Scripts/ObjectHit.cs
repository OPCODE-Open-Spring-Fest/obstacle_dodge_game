using UnityEngine;

public class ObjectHit : MonoBehaviour
{
    private MeshRenderer meshRenderer;
    
    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }
    
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            var collectibles = other.gameObject.GetComponent<PlayerCollectibles>();
            if (collectibles != null && collectibles.IsInvincible)
            {
                return;
            }

            meshRenderer.material.color = Color.red;
            gameObject.tag = "Hit";
        }
    }
}
