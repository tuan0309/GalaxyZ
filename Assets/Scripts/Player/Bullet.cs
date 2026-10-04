using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 35f;
    [SerializeField] private float lifeTime = 3f;

    [Header("Damage")]
    [SerializeField] private float damage = 25f;

    [Header("Effects")]
    [SerializeField] private GameObject impactEffect;

    private Rigidbody rb;
    private bool hasHit;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        rb.useGravity = false;
        rb.collisionDetectionMode =
            CollisionDetectionMode.ContinuousDynamic;
    }

    private void Start()
    {
        // Dùng velocity thay vì AddForce để tốc độ ổn định
        rb.linearVelocity =
            transform.forward * speed;

        Destroy(gameObject, lifeTime);
    }

    // =====================================================
    // COLLISION
    // =====================================================

    private void OnCollisionEnter(Collision collision)
    {
        if (hasHit)
            return;

        Debug.Log(
            "BULLET COLLISION WITH: " +
            collision.gameObject.name
        );

        Vector3 hitPoint = transform.position;
        Vector3 hitNormal = -transform.forward;

        if (collision.contactCount > 0)
        {
            hitPoint =
                collision.contacts[0].point;

            hitNormal =
                collision.contacts[0].normal;
        }

        ProcessHit(
            collision.collider,
            hitPoint,
            hitNormal
        );
    }

    // =====================================================
    // TRIGGER
    // =====================================================

    private void OnTriggerEnter(Collider other)
    {
        if (hasHit)
            return;

        Debug.Log(
            "BULLET TRIGGER WITH: " +
            other.gameObject.name
        );

        ProcessHit(
            other,
            transform.position,
            -transform.forward
        );
    }

    // =====================================================
    // PROCESS HIT
    // =====================================================

    private void ProcessHit(
        Collider hitCollider,
        Vector3 hitPoint,
        Vector3 hitNormal)
    {
        if (hasHit)
            return;

        // Không để bullet tự va vào Player
        if (hitCollider.CompareTag("Player"))
            return;

        EnemyHealth enemyHealth =
            hitCollider.GetComponentInParent<EnemyHealth>();

        if (enemyHealth != null)
        {
            hasHit = true;

            Debug.Log(
                "BULLET HIT ENEMY: " +
                enemyHealth.gameObject.name
            );

            enemyHealth.TakeDamage(damage);

            Debug.Log(
                "DAMAGE = " + damage
            );

            SpawnImpact(
                hitPoint,
                hitNormal
            );

            Destroy(gameObject);

            return;
        }

        // Trúng môi trường
        hasHit = true;

        SpawnImpact(
            hitPoint,
            hitNormal
        );

        Destroy(gameObject);
    }

    private void SpawnImpact(
        Vector3 position,
        Vector3 normal)
    {
        if (impactEffect == null)
            return;

        if (normal.sqrMagnitude < 0.001f)
            normal = Vector3.up;

        Instantiate(
            impactEffect,
            position,
            Quaternion.LookRotation(normal)
        );
    }
}