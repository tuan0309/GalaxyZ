using UnityEngine;

public class EnemyAnimation : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private EnemyNormal enemyNormal;

    private static readonly int SpeedHash =
        Animator.StringToHash("Speed");

    private static readonly int AttackHash =
        Animator.StringToHash("Attack");

    private static readonly int HurtHash =
        Animator.StringToHash("Hurt");

    private static readonly int DieHash =
        Animator.StringToHash("Die");

    private bool isDead;

    private void Awake()
    {
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        if (enemyNormal == null)
        {
            enemyNormal = GetComponent<EnemyNormal>();
        }

        if (enemyNormal == null)
        {
            enemyNormal = GetComponentInParent<EnemyNormal>();
        }
    }

    public void SetMovement(float speed)
    {
        if (isDead || animator == null)
            return;

        animator.SetFloat(
            SpeedHash,
            speed
        );
    }

    public void PlayAttack()
    {
        if (isDead || animator == null)
            return;

        animator.SetTrigger(
            AttackHash
        );
    }

    public void PlayHurt()
    {
        if (isDead || animator == null)
            return;

        animator.SetTrigger(
            HurtHash
        );
    }

    public void PlayDeath()
{
    if (isDead || animator == null)
        return;

    isDead = true;

    // Dừng movement
    animator.SetFloat(
        SpeedHash,
        0f
    );

    // Xóa trigger hành động cũ
    animator.ResetTrigger(AttackHash);
    animator.ResetTrigger(HurtHash);

    // Chạy Die
    animator.SetTrigger(DieHash);
}
    // ===============================
    // ANIMATION EVENT
    // ===============================

    public void DealDamage()
    {
        if (enemyNormal != null)
        {
            enemyNormal.DealDamage();
        }
    }
}