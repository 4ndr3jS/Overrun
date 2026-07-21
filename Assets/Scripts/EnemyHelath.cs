using System;
using UnityEngine;

public class EnemyHelath : MonoBehaviour
{
    [Header("Coin drop")]
    [SerializeField] GameObject coinPrefab;
    [SerializeField] private int coinsPerDrop = 1;
    [SerializeField] private float coinScatterRdius;

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
    }

    private void Die()
    {
        if (isDead)
            return;

        isDead = true;

        PlayerVitals.Instance?.RecordMonstersKill();
        DropCoins();

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

    private void DropCoins()
    {
        if (coinPrefab == null)
            return;

        Vector2 offset = UnityEngine.Random.insideUnitCircle * coinScatterRdius;
        GameObject coin = Instantiate(coinPrefab, (Vector2)transform.position + offset, Quaternion.identity);

        CoinPickup pickup = coin.GetComponent<CoinPickup>();

        if (pickup != null)
            pickup.SetCoinValue(coinsPerDrop);
    }

    public float GetHealth() => currentHealth;
    public float GetMaxHealth() => maxHealth;
}
