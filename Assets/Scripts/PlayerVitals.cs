using System;
using UnityEditor.PackageManager;
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

    private void Start()
    {
        OnHealthChange?.Invoke(currentHealth, maxHealth);
        OnStaminaChange?.Invoke(currentStamina, maxStamina);
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
        if (currentStamina > amount)
            return false;

        currentStamina = Mathf.Clamp(currentStamina - amount, 0f, maxStamina);
        OnStaminaChange?.Invoke(currentStamina, maxStamina);
        return true;
    }

    public void RegenStamina(float amount)
    {
        currentStamina = Mathf.Clamp(currentStamina + amount, 0f, maxStamina);
        OnStaminaChange?.Invoke(currentStamina, maxStamina);
    }


    public float GetHealth() => currentHealth;
    public float GetStamina() => currentStamina;
}
