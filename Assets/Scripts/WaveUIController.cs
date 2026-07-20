using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WaveUIController : MonoBehaviour
{
    [Header("Refrences")]
    [SerializeField] private WaveController waveController;
    [SerializeField] private GameObject wavePanel;
    [SerializeField] private TMP_Text waveText;
    [SerializeField] private TMP_Text enemiesText;
    [SerializeField] private Image enemyBarFill;

    private void Start()
    {
        if (waveController == null)
            waveController = FindAnyObjectByType<WaveController>();
    }

    private void Update()
    {
        if (waveController == null)
            return;

        bool shouldShow = waveController.HasStarted;

        if (wavePanel != null)
            wavePanel.SetActive(shouldShow);

        if (!shouldShow)
            return;

        int enemiesTotal = Mathf.Max(1, waveController.TotalEnemies);
        int remainingEnemies = waveController.EnemRemaining;

        waveText.text = $"WAVE:  {waveController.CurrentWave}";
        enemiesText.text = $"Enemies   remaining:  {remainingEnemies}";

        enemyBarFill.fillAmount = Mathf.Clamp01((float)remainingEnemies / enemiesTotal);
    }
}
