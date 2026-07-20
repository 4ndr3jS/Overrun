using System;
using TMPro;
using UnityEngine;

public class CurrencyController : MonoBehaviour
{
    public static CurrencyController Instance;

    [SerializeField] private int startingCoins = 0;
    private int playerCoins = 0;
    [SerializeField] private TMP_Text number;

    public event Action<int> OnCoinChange;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        else 
        {
            Instance = this;
            playerCoins = startingCoins;
        }
    }

    private void Start()
    {
        UpdateCoinDisplay();
    }

    public int GetCoins() => playerCoins;

    public bool SpendCoins(int amount)
    {
        if(playerCoins >= amount)
        {
            playerCoins -= amount;
            UpdateCoinDisplay();
            return true;
        }
        return false;
    }

    public void AddCoins(int amount)
    {
        playerCoins += amount;
        UpdateCoinDisplay();
    }

    public void SetCoins(int amount)
    {
        playerCoins = amount;
        UpdateCoinDisplay();
    }

    private void UpdateCoinDisplay()
    {
        if (number != null)
            number.text = playerCoins.ToString();
           
        OnCoinChange?.Invoke(playerCoins);
    }
}
