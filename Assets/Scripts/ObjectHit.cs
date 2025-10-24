using UnityEngine;

public class ObjectHit : MonoBehaviour
{
    private MeshRenderer meshRenderer;
    
    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }
    
    private void OnCollisionEnter(Collision other) {
        if(other.gameObject.CompareTag("Player")) {
            meshRenderer.material.color = Color.red;
            gameObject.tag = "Hit";
        }
    }
}
