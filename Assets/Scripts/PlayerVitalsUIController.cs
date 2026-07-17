using UnityEngine;
using UnityEngine.UI;

public class PlayerVitalsUIController : MonoBehaviour
{
    [Header("Health Bar")]
    public Image healthFill;

    [Header("Stamina Stamina")]
    public Image staminaFill;

    private void OnEnable()
    {
        if(PlayerVitals.Instance != null)
        {
            PlayerVitals.Instance.OnHealthChange += HandleHealthChange;
            PlayerVitals.Instance.OnStaminaChange += HandleStaminaChange;

            HandleHealthChange(PlayerVitals.Instance.GetHealth(), 100f);
            HandleStaminaChange(PlayerVitals.Instance.GetStamina(), 100f);
        }
    }

    private void OnDisable()
    {
        if (PlayerVitals.Instance != null)
        {
            PlayerVitals.Instance.OnHealthChange -= HandleHealthChange;
            PlayerVitals.Instance.OnStaminaChange -= HandleStaminaChange;
        }
    }

    private void HandleHealthChange(float current, float max)
    {
        if (healthFill != null)
            healthFill.fillAmount = max > 0f ? current / max : 0f;
    }

    private void HandleStaminaChange(float current, float max)
    {
        if (staminaFill != null)
            staminaFill.fillAmount = max > 0f ? current / max : 0f;
    }
}
