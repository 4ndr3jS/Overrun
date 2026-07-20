using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    [SerializeField] private int coinVal = 1;
    private bool collected;

    public void SetCoinValue(int value)
    {
        coinVal = Mathf.Max(1, value);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collected || !collision.CompareTag("Player"))
            return;

        collected = true;

        if (CurrencyController.Instance != null)
            CurrencyController.Instance.AddCoins(coinVal);

        Destroy(gameObject);
    }
}
