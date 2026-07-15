using System;
using Unity.VisualScripting;
using UnityEngine;

public class CurrencyController : MonoBehaviour
{
    public static CurrencyController Instance;

    [SerializeField] private int startingCoins = 100;
    private int playerCoins = 100;
    public event Action<int> OnCoinChange;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else 
        {
            Instance = this;
            playerCoins = startingCoins;
        }
    }

    public int GetCoins() => playerCoins;

    public bool SpendCoins(int amount)
    {
        if(playerCoins >= amount)
        {
            playerCoins -= amount;
            OnCoinChange?.Invoke(playerCoins);
            return true;
        }
        return false;
    }

    public void AddCoins(int amount)
    {
        playerCoins += amount;
        OnCoinChange?.Invoke(playerCoins);
    }

    public void SetCoins(int amount)
    {
        playerCoins = amount;
        OnCoinChange?.Invoke(playerCoins);
    }
}
