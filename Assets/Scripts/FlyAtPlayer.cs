using UnityEngine;

public class FlyAtPlayer : MonoBehaviour
{
    [SerializeField] float speed = 1.0f;
    [SerializeField] Transform player;
    private Vector3 playerPosition;
    private bool hasReachedTarget = false;
    
    private void Awake() {
        gameObject.SetActive(false);
    }

    void Start()
    {
        if (player != null)
        {
            playerPosition = player.position;
        }
    }

    void Update()
    {
        if (hasReachedTarget) return;
        
        transform.position = Vector3.MoveTowards(transform.position, playerPosition, Time.deltaTime * speed);
        
        // More efficient distance check
        if (Vector3.SqrMagnitude(transform.position - playerPosition) < 0.01f)
        {
            hasReachedTarget = true;
            Destroy(gameObject);
        }
    }
}
