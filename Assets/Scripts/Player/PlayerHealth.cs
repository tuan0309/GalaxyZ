using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;

    [Header("References")]
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerAim playerAim;
    [SerializeField] private PlayerWeapon playerWeapon;
    [SerializeField] private PlayerAnimation playerAnimation;

    private bool isDead;

    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;
    public bool IsDead => isDead;

    private void Awake()
    {
        currentHealth = maxHealth;

        if (playerMovement == null)
            playerMovement = GetComponent<PlayerMovement>();

        if (playerAim == null)
            playerAim = GetComponent<PlayerAim>();

        if (playerWeapon == null)
            playerWeapon = GetComponent<PlayerWeapon>();

        if (playerAnimation == null)
            playerAnimation = GetComponent<PlayerAnimation>();
    }

    public void TakeDamage(float damage)
    {
        if (isDead)
            return;

        currentHealth -= damage;

        currentHealth = Mathf.Clamp(
            currentHealth,
            0f,
            maxHealth
        );

        Debug.Log(
            $"Player HP: {currentHealth}/{maxHealth}"
        );

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        if (isDead)
            return;

        currentHealth += amount;

        currentHealth = Mathf.Clamp(
            currentHealth,
            0f,
            maxHealth
        );

        Debug.Log(
            $"Player Heal: {currentHealth}/{maxHealth}"
        );
    }

    private void Die()
    {
        if (isDead)
            return;

        isDead = true;

        Debug.Log("PLAYER DEAD");

        // Chạy animation chết
        if (playerAnimation != null)
        {
            playerAnimation.PlayDeath();
        }

        // Tắt điều khiển
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }

        if (playerAim != null)
        {
            playerAim.enabled = false;
        }

        if (playerWeapon != null)
        {
            playerWeapon.enabled = false;
        }
    }

    // Dùng để test nhanh HP
    private void Update()
    {
        if (isDead)
            return;

        // Nhấn H để mất 20 máu
        if (Input.GetKeyDown(KeyCode.H))
        {
            TakeDamage(20f);
        }

        // Nhấn G để hồi 20 máu
        if (Input.GetKeyDown(KeyCode.G))
        {
            Heal(20f);
        }
    }
}