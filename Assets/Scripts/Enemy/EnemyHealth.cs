using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;

    [Header("References")]
    [SerializeField] private EnemyNormal enemyNormal;
    [SerializeField] private EnemyAnimation enemyAnimation;

    [Header("Reward")]
    [SerializeField] private int xpReward = 10;
    [SerializeField] private int scoreReward = 1;

    private bool isDead;

    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;
    public bool IsDead => isDead;

    private void Awake()
    {
        currentHealth = maxHealth;

        if (enemyNormal == null)
        {
            enemyNormal =
                GetComponent<EnemyNormal>();
        }

        if (enemyAnimation == null)
        {
            enemyAnimation =
                GetComponentInChildren<EnemyAnimation>();
        }
    }

    // =====================================================
    // TAKE DAMAGE
    // =====================================================

    public void TakeDamage(float damage)
    {
        if (isDead)
            return;

        if (damage <= 0f)
            return;

        currentHealth -= damage;

        currentHealth = Mathf.Clamp(
            currentHealth,
            0f,
            maxHealth
        );

        Debug.Log(
            $"{gameObject.name} HP: " +
            $"{currentHealth}/{maxHealth}"
        );

        // =========================================
        // DEAD
        // =========================================

        if (currentHealth <= 0f)
        {
            Die();
            return;
        }

        // =========================================
        // HURT
        // =========================================

        if (enemyAnimation != null)
        {
            enemyAnimation.PlayHurt();
        }
    }

    // =====================================================
    // DIE
    // =====================================================

    private void Die()
    {
        if (isDead)
            return;

        isDead = true;

        currentHealth = 0f;

        Debug.Log(
            gameObject.name +
            " DEAD"
        );

        // =========================================
        // STOP AI + PLAY DIE
        // =========================================

        if (enemyNormal != null)
        {
            enemyNormal.Die();
        }
        else if (enemyAnimation != null)
        {
            enemyAnimation.PlayDeath();
        }

        // =========================================
        // GIVE PLAYER REWARD
        // =========================================

        GiveReward();

        // =========================================
        // DISABLE COLLISION
        // =========================================

        DisableCollision();

        // KHÔNG Destroy enemy ở đây.
        // Xác sẽ nằm lại trong scene.
    }

    // =====================================================
    // REWARD
    // =====================================================

    private void GiveReward()
    {
        PlayerHUD playerHUD =
            FindFirstObjectByType<PlayerHUD>();

        if (playerHUD == null)
            return;

        playerHUD.AddXP(
            xpReward
        );

        playerHUD.AddScore(
            scoreReward
        );
    }

    // =====================================================
    // DISABLE COLLISION
    // =====================================================

private void DisableCollision()
{
    // Tắt tất cả CharacterController
    CharacterController[] controllers =
        GetComponentsInChildren<CharacterController>(true);

    foreach (CharacterController controller in controllers)
    {
        if (controller != null)
        {
            controller.enabled = false;
        }
    }

    // Tắt tất cả Collider thường
    Collider[] colliders =
        GetComponentsInChildren<Collider>(true);

    foreach (Collider col in colliders)
    {
        if (col != null)
        {
            col.enabled = false;
        }
    }

    // Tắt Rigidbody collision nếu có
    Rigidbody[] rigidbodies =
        GetComponentsInChildren<Rigidbody>(true);

    foreach (Rigidbody rb in rigidbodies)
    {
        if (rb == null)
            continue;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.isKinematic = true;
        rb.detectCollisions = false;
    }
}

    // =====================================================
    // HEAL - OPTIONAL
    // =====================================================

    public void Heal(float amount)
    {
        if (isDead)
            return;

        if (amount <= 0f)
            return;

        currentHealth += amount;

        currentHealth = Mathf.Clamp(
            currentHealth,
            0f,
            maxHealth
        );
    }
}