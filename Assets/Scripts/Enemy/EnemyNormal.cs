using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class EnemyNormal : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform player;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float rotationSpeed = 10f;

    [Header("Attack")]
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackDamage = 15f;
    [SerializeField] private float attackCooldown = 1.2f;

    [Tooltip("Thời gian animation Attack chạy trước khi enemy được phép hành động lại")]
    [SerializeField] private float attackDuration = 0.8f;

    [Header("References")]
    [SerializeField] private EnemyAnimation enemyAnimation;

    private CharacterController controller;
    private PlayerHealth playerHealth;

    private bool isDead;
    private bool isAttacking;

    private float nextAttackTime;

    public bool IsDead => isDead;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        if (enemyAnimation == null)
        {
            enemyAnimation =
                GetComponentInChildren<EnemyAnimation>();
        }

        if (enemyAnimation == null)
        {
            Debug.LogError(
                gameObject.name +
                ": Không tìm thấy EnemyAnimation!"
            );
        }
    }

    private void Start()
    {
        FindPlayer();
    }

    private void Update()
    {
        if (isDead)
            return;

        if (player == null)
        {
            FindPlayer();
            return;
        }

        HandleAI();
    }

    // =====================================================
    // FIND PLAYER
    // =====================================================

    private void FindPlayer()
    {
        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject == null)
            return;

        player = playerObject.transform;

        playerHealth =
            playerObject.GetComponent<PlayerHealth>();
    }

    // =====================================================
    // AI
    // =====================================================

    private void HandleAI()
    {
        Vector3 direction =
            player.position - transform.position;

        direction.y = 0f;

        float distanceToPlayer =
            direction.magnitude;

        // =========================================
        // ĐANG ATTACK
        // =========================================

        if (isAttacking)
        {
            StopMovement();

            FacePlayer(direction);

            return;
        }

        // =========================================
        // PLAYER TRONG TẦM ĐÁNH
        // =========================================

        if (distanceToPlayer <= attackRange)
        {
            StopMovement();

            FacePlayer(direction);

            if (Time.time >= nextAttackTime)
            {
                StartCoroutine(
                    AttackRoutine()
                );
            }

            return;
        }

        // =========================================
        // CHASE PLAYER
        // =========================================

        MoveToPlayer(direction);
    }

    // =====================================================
    // MOVEMENT
    // =====================================================

    private void MoveToPlayer(Vector3 direction)
    {
        if (direction.sqrMagnitude < 0.001f)
        {
            StopMovement();
            return;
        }

        direction.Normalize();

        controller.Move(
            direction *
            moveSpeed *
            Time.deltaTime
        );

        if (enemyAnimation != null)
        {
            enemyAnimation.SetMovement(1f);
        }

        FacePlayer(direction);
    }

    private void StopMovement()
    {
        if (enemyAnimation != null)
        {
            enemyAnimation.SetMovement(0f);
        }
    }

    // =====================================================
    // ROTATION
    // =====================================================

    private void FacePlayer(Vector3 direction)
    {
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f)
            return;

        Quaternion targetRotation =
            Quaternion.LookRotation(
                direction.normalized,
                Vector3.up
            );

        transform.rotation =
            Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
    }

    // =====================================================
    // ATTACK
    // =====================================================

    private IEnumerator AttackRoutine()
    {
        if (isDead || isAttacking)
            yield break;

        isAttacking = true;

        StopMovement();

        Debug.Log(
            gameObject.name +
            " START ATTACK"
        );

        if (enemyAnimation != null)
        {
            enemyAnimation.PlayAttack();
        }

        // Chờ animation Attack chạy
        yield return new WaitForSeconds(
            attackDuration
        );

        isAttacking = false;

        nextAttackTime =
            Time.time + attackCooldown;

        Debug.Log(
            gameObject.name +
            " FINISH ATTACK"
        );
    }

    // =====================================================
    // ANIMATION EVENT
    // GIỮ EVENT NÀY
    // =====================================================

    public void DealDamage()
    {
        if (isDead || player == null)
            return;

        Vector3 enemyPos =
            transform.position;

        Vector3 playerPos =
            player.position;

        enemyPos.y = 0f;
        playerPos.y = 0f;

        float distance =
            Vector3.Distance(
                enemyPos,
                playerPos
            );

        // Player đã chạy khỏi tầm kiếm
        if (distance > attackRange + 0.4f)
        {
            Debug.Log("ATTACK MISS");
            return;
        }

        if (playerHealth != null)
        {
            playerHealth.TakeDamage(
                attackDamage
            );

            Debug.Log(
                "Enemy gây " +
                attackDamage +
                " damage!"
            );
        }
    }

    // =====================================================
    // DEATH
    // =====================================================

    public void Die()
    {
        if (isDead)
            return;

        isDead = true;
        isAttacking = false;

        StopMovement();

        StopAllCoroutines();

        if (enemyAnimation != null)
        {
            enemyAnimation.PlayDeath();
        }
    }

    // =====================================================
    // DEBUG
    // =====================================================

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(
            transform.position,
            attackRange
        );
    }
}