using TMPro;
using UnityEngine;

public class PlayerHUD : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private PlayerHealth playerHealth;

    [Header("HP UI")]
    [SerializeField] private RectTransform healthFill;
    [SerializeField] private TMP_Text healthText;

    [Header("XP UI")]
    [SerializeField] private RectTransform xpFill;
    [SerializeField] private TMP_Text levelText;

    [Header("XP Data")]
    [SerializeField] private int currentXP = 0;
    [SerializeField] private int xpToNextLevel = 100;
    [SerializeField] private int currentLevel = 1;
    [SerializeField] private int xpIncreasePerLevel = 25;

    [Header("Wave UI")]
    [SerializeField] private TMP_Text waveText;
    [SerializeField] private TMP_Text enemyText;
    [SerializeField] private TMP_Text scoreText;

    [Header("Wave Data")]
    [SerializeField] private int currentWave = 1;
    [SerializeField] private int maxWave = 3;
    [SerializeField] private int enemyCount = 10;
    [SerializeField] private int score = 0;

    public int CurrentXP => currentXP;
    public int XPToNextLevel => xpToNextLevel;
    public int CurrentLevel => currentLevel;

    private void Start()
    {
        UpdateAllUI();
    }

    private void Update()
    {
        UpdateHealth();
        UpdateXP();

        // TEST XP
        // Nhấn X để cộng 10 XP
        if (Input.GetKeyDown(KeyCode.X))
        {
            AddXP(10);
        }
    }

    private void UpdateAllUI()
    {
        UpdateHealth();
        UpdateXP();
        UpdateWaveUI();
    }

    private void UpdateHealth()
    {
        if (playerHealth == null)
            return;

        float hpPercent =
            playerHealth.CurrentHealth /
            playerHealth.MaxHealth;

        hpPercent = Mathf.Clamp01(hpPercent);

        if (healthFill != null)
        {
            Vector2 anchorMax = healthFill.anchorMax;

            anchorMax.x = hpPercent;

            healthFill.anchorMax = anchorMax;
        }

        if (healthText != null)
        {
            healthText.text =
                Mathf.CeilToInt(
                    playerHealth.CurrentHealth
                ).ToString();
        }
    }

    private void UpdateXP()
    {
        float xpPercent =
            (float)currentXP /
            xpToNextLevel;

        xpPercent = Mathf.Clamp01(xpPercent);

        if (xpFill != null)
        {
            Vector2 anchorMax = xpFill.anchorMax;

            anchorMax.x = xpPercent;

            xpFill.anchorMax = anchorMax;
        }

        if (levelText != null)
        {
            levelText.text =
                $"LV {currentLevel}";
        }
    }

    public void AddXP(int amount)
    {
        if (amount <= 0)
            return;

        currentXP += amount;

        while (currentXP >= xpToNextLevel)
        {
            currentXP -= xpToNextLevel;

            LevelUp();
        }

        UpdateXP();
    }

    private void LevelUp()
    {
        currentLevel++;

        xpToNextLevel += xpIncreasePerLevel;

        Debug.Log(
            $"LEVEL UP! Level: {currentLevel}"
        );
    }

    private void UpdateWaveUI()
    {
        if (waveText != null)
        {
            waveText.text =
                $"WAVE {currentWave}/{maxWave}";
        }

        if (enemyText != null)
        {
            enemyText.text =
                $"Quái {enemyCount}";
        }

        if (scoreText != null)
        {
            scoreText.text =
                $"★ {score}";
        }
    }

    public void SetWave(int wave, int totalWaves)
    {
        currentWave = wave;
        maxWave = totalWaves;

        UpdateWaveUI();
    }

    public void SetEnemyCount(int count)
    {
        enemyCount = count;

        UpdateWaveUI();
    }

    public void AddScore(int amount)
    {
        score += amount;

        UpdateWaveUI();
    }

    public void SetScore(int newScore)
    {
        score = newScore;

        UpdateWaveUI();
    }
}