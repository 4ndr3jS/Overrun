using System;
using System.ComponentModel.Design;
using UnityEngine;

public class EnemyHelath : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 50f;
    private float currentHealth;

    [Header("Hit flash")]
    public Color flashColor = new Color(1f, 0.3f, 0.3f, 1f);
    public float flashDuration = 0.15f;

    [Header("Death flash")]
    public int deathFlickerCount = 3;
    public float deathFlickerInterval = 0.08f;

    public event Action<float, float> OnHealthChange;
    public event Action OnDeath;

    private SpriteFlash sf;
    private bool isDead;

    private void Awake()
    {
        currentHealth = maxHealth;
        sf = GetComponent<SpriteFlash>();
    }

    public void TakeDamage(float amount)
    {
        if (isDead || amount <= 0f)
            return;

        currentHealth = Mathf.Clamp(currentHealth -amount, 0f, maxHealth);
        OnHealthChange?.Invoke(currentHealth, maxHealth);

        if (sf != null)
            sf.Flash(flashColor, flashDuration);

        if (currentHealth <= 0f)
        {
            Die();
            return;
        }

        if (sf != null)
            sf.Flash(flashColor, flashDuration);
    }

    private void Die()
    {
        if (isDead)
            return;

        isDead = true;
        OnDeath?.Invoke();

        if (sf != null)
            sf.DeathFlicker(deathFlickerCount, deathFlickerInterval, () => Destroy(gameObject));
        else
            Destroy(gameObject);
    }

    public void SetMaxHealth(float newMaxHealth)
    {
        maxHealth = Mathf.Max(1f, newMaxHealth);
        currentHealth = maxHealth;
        OnHealthChange?.Invoke(currentHealth, maxHealth);
    }

    public float GetHealth() => currentHealth;
    public float GetMaxHealth() => maxHealth;
}
