using System;
using TMPro;
using UnityEngine;

public class PlayerVitals : MonoBehaviour
{
    public static PlayerVitals Instance;

    [Header("Player")]
    [SerializeField]
    private float maxHealth = 100f;
    private float currentHealth;

    [Header("Stamina")]
    [SerializeField]
    private float maxStamina = 100f;
    private float currentStamina;

    [SerializeField] private float staminaRegen = 1f;
    [SerializeField] private float staminaRegenInterval = 0.2f;
    [SerializeField] private float healthRegenSec = 2f;

    [Header("Player stats")]
    [SerializeField] private int monstersKilledCount;
    [SerializeField] private int highestWaveAcheived;

    [SerializeField] private TMP_Text monstersKilled;
    [SerializeField] private TMP_Text highestWave;

    private float staminaRegenTimer;

    public event Action<float, float> OnHealthChange;
    public event Action<float, float> OnStaminaChange;
    public event Action OnDeath;

    private bool isDead = false;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        currentHealth = maxHealth;
        currentStamina = maxStamina;
    }

    private void Update()
    {
        if(!isDead && currentHealth < maxHealth)
        {
            currentHealth = Mathf.Min(currentHealth + healthRegenSec * Time.deltaTime, maxHealth);
        }

        OnHealthChange?.Invoke(currentHealth, maxHealth);


        if (currentStamina >= maxStamina)
        {
            staminaRegenTimer = 0f;
            return;
        }

        staminaRegenTimer += Time.deltaTime;

        while(staminaRegenTimer >= staminaRegenInterval && currentStamina < maxStamina)
        {
            RegenStamina(staminaRegen);
            staminaRegenTimer -= staminaRegenInterval;
        }
    }

    private void Start()
    {
        OnHealthChange?.Invoke(currentHealth, maxHealth);
        OnStaminaChange?.Invoke(currentStamina, maxStamina);
        UpdateStatsUI();
    }

    public void TakeDamage(float amount)
    {
        if (isDead || amount <= 0f)
            return;

        currentHealth = Mathf.Clamp(currentHealth - amount, 0f, maxHealth);
        OnHealthChange?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0f)
        {
            isDead = true;
            OnDeath?.Invoke();
        }
            
    }

    public void Revive(float reviveHealth = -1f)
    {
        isDead = false;
        currentHealth = reviveHealth > 0f ? Mathf.Clamp(reviveHealth, 0f, maxHealth) : maxHealth;
        OnHealthChange?.Invoke(currentHealth, maxHealth);
    }

    public bool IsDead() => isDead;

    public bool UseStamina(float amount)
    {
        amount = Mathf.Max(0f, amount);

        if (currentStamina < amount)
            return false;

        currentStamina -= amount;

        OnStaminaChange?.Invoke(currentStamina, maxStamina);
        return true;
    }

    public void RegenStamina(float amount)
    {
        currentStamina = Mathf.Clamp(currentStamina + amount, 0f, maxStamina);
        OnStaminaChange?.Invoke(currentStamina, maxStamina);
    }

    public void RecordMonstersKill()
    {
        monstersKilledCount++;
        UpdateStatsUI();
    }

    public void RecordHighestWave(int wave)
    {
        if (wave <= highestWaveAcheived)
            return;

        highestWaveAcheived = wave;
        UpdateStatsUI();
    }

    public int GetMonsterKills()
    {
        return monstersKilledCount;
    }

    public int GetHighestWave()
    {
        return highestWaveAcheived;
    }

    public void SetPlayerStats(int kills, int highestWave)
    {
        monstersKilledCount = Mathf.Max(0, kills);
        highestWaveAcheived = Mathf.Max(0, highestWave);

        UpdateStatsUI();
    }

    private void UpdateStatsUI()
    {
        if (monstersKilled != null)
            monstersKilled.text = $"MONSTERS  KILLED:  {monstersKilledCount}";

        if (highestWave != null)
            highestWave.text = $"HIGHEST  WAVE:  {highestWaveAcheived}";
    }


    public float GetHealth() => currentHealth;
    public float GetStamina() => currentStamina;

    public void SetVitals(float health, float stamina)
    {
        currentHealth = Mathf.Clamp(health, 0f, maxHealth);
        currentStamina = Mathf.Clamp(stamina, 0f, maxStamina);

        isDead = currentHealth <= 0f;

        OnHealthChange?.Invoke(currentHealth, maxHealth);
        OnStaminaChange?.Invoke(currentStamina, maxStamina);

        if (isDead)
            OnDeath?.Invoke();
    }

    public bool Heal(float amount)
    {
        if (isDead || amount <= 0f || currentHealth >= maxHealth)
            return false;

        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        OnHealthChange?.Invoke(currentHealth, maxHealth);

        return true;
    }

    public bool Refreshen(float amount)
    {
        if (isDead || amount <= 0f || currentStamina >= maxStamina)
            return false;

        currentStamina = Mathf.Min(currentStamina + amount, maxStamina);
        OnStaminaChange?.Invoke(currentStamina, maxStamina);

        return true;
    }
}
