using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyFollower : MonoBehaviour
{
    [Header("Targeting")]
    [SerializeField] private Transform player;
    [SerializeField] private float followSpeed = 5f;
    [SerializeField] private float stoppingDistance = 0.5f;
    [SerializeField] private bool lockYPosition = true;

    [Header("Damage")]
    [SerializeField] private int heartDamage = 1;
    [SerializeField] private float damageCooldown = 1.5f;

    [Header("FX")]
    [SerializeField] private GameObject hitEffectPrefab;

    [Header("Obstacle Avoidance")]
    [SerializeField] private float obstacleCheckRadius = 0.5f;
    [SerializeField] private LayerMask obstacleLayers = Physics.DefaultRaycastLayers;

    private float lastDamageTime = -Mathf.Infinity;
    private Vector3 initialYPosition;
    private Rigidbody rb;

    private void Awake()
    {
        if (player == null)
        {
            var playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                player = playerObject.transform;
            }
        }

        if (lockYPosition)
        {
            initialYPosition = transform.position;
        }

        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }

        rb.constraints = lockYPosition
            ? RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY
            : RigidbodyConstraints.FreezeRotation;
        rb.useGravity = !lockYPosition;
        rb.isKinematic = false;
    }

    private void FixedUpdate()
    {
        if (player == null) return;

        Vector3 targetPosition = player.position;
        if (lockYPosition)
        {
            targetPosition.y = initialYPosition.y;
        }

        Vector3 currentPosition = rb.position;
        if (lockYPosition)
        {
            currentPosition.y = initialYPosition.y;
        }

        Vector3 direction = targetPosition - currentPosition;
        float distance = direction.magnitude;

        if (distance <= stoppingDistance)
        {
            StopMovement();
            UpdateRotation(direction);
            MaintainPlanePosition();
            return;
        }

        Vector3 moveDirection = direction.normalized;
        Vector3 desiredMove = moveDirection * followSpeed * Time.fixedDeltaTime;
        if (lockYPosition)
        {
            desiredMove.y = 0f;
        }

        float moveDistance = desiredMove.magnitude;
        if (moveDistance > 0f)
        {
            float radius = Mathf.Max(0.01f, obstacleCheckRadius);
            bool blocked = false;
            RaycastHit primaryHit;

            if (Physics.SphereCast(currentPosition, radius, moveDirection, out primaryHit, moveDistance, obstacleLayers, QueryTriggerInteraction.Ignore))
            {
                if (!primaryHit.collider.CompareTag("Player"))
                {
                    float allowedDistance = Mathf.Max(0f, primaryHit.distance - 0.05f);
                    if (allowedDistance <= 0.001f)
                    {
                        blocked = true;
                    }
                    desiredMove = moveDirection * Mathf.Min(moveDistance, allowedDistance);
                }
            }

            if (blocked)
            {
                Vector3 slideDirection = Vector3.ProjectOnPlane(moveDirection, primaryHit.normal);
                if (slideDirection.sqrMagnitude > 0.0001f)
                {
                    slideDirection.Normalize();
                    if (lockYPosition)
                    {
                        slideDirection.y = 0f;
                        slideDirection = slideDirection.normalized;
                    }

                    float slideDistance = followSpeed * Time.fixedDeltaTime;
                    if (Physics.SphereCast(currentPosition, radius, slideDirection, out RaycastHit slideHit, slideDistance, obstacleLayers, QueryTriggerInteraction.Ignore))
                    {
                        if (!slideHit.collider.CompareTag("Player"))
                        {
                            slideDistance = Mathf.Max(0f, slideHit.distance - 0.05f);
                        }
                    }

                    desiredMove = slideDirection * slideDistance;
                }
                else
                {
                    desiredMove = Vector3.zero;
                }
            }

            if (desiredMove.sqrMagnitude > 0f)
            {
                rb.MovePosition(rb.position + desiredMove);
            }
        }

        UpdateRotation(direction);
        MaintainPlanePosition();
    }

    private void OnCollisionEnter(Collision collision)
    {
        TryDamage(collision.collider, ExtractHitPoint(collision));
    }

    private void OnCollisionStay(Collision collision)
    {
        TryDamage(collision.collider, ExtractHitPoint(collision));
    }

    private void OnTriggerEnter(Collider other)
    {
        TryDamage(other, other.ClosestPoint(transform.position));
    }

    private void OnTriggerStay(Collider other)
    {
        TryDamage(other, other.ClosestPoint(transform.position));
    }

    private void TryDamage(Collider other, Vector3 hitPoint)
    {
        if (!other.CompareTag("Player")) return;
        if (Time.time - lastDamageTime < damageCooldown) return;

        if (heartDamage == 0) return;

        var collectibles = other.GetComponent<PlayerCollectibles>();
        if (collectibles == null) return;
        if (collectibles.IsInvincible) return;

        collectibles.AddLife(-Mathf.Abs(heartDamage));
        if (collectibles.GetLives() <= 0)
        {
            Debug.Log("EnemyFollower: player hearts depleted. Loading Game Over.");
            DeathHelper.TriggerDeath(other.gameObject);
        }

        SpawnHitEffect(hitPoint);
        lastDamageTime = Time.time;
    }

    private void StopMovement()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    private Vector3 ExtractHitPoint(Collision collision)
    {
        if (collision.contactCount > 0)
        {
            return collision.GetContact(0).point;
        }

        return collision.collider != null
            ? collision.collider.ClosestPoint(transform.position)
            : transform.position;
    }

    private void SpawnHitEffect(Vector3 position)
    {
        if (hitEffectPrefab == null) return;
        Instantiate(hitEffectPrefab, position, Quaternion.identity);
    }

    private void UpdateRotation(Vector3 direction)
    {
        Vector3 lookDirection = direction;
        if (lockYPosition)
        {
            lookDirection.y = 0f;
        }

        if (lookDirection.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection.normalized, Vector3.up);
            rb.MoveRotation(targetRotation);
        }
    }

    private void MaintainPlanePosition()
    {
        if (!lockYPosition) return;

        Vector3 pos = rb.position;
        pos.y = initialYPosition.y;
        rb.position = pos;
    }
}

