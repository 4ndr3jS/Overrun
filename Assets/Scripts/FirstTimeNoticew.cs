using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class FirstTimeNoticeController : MonoBehaviour
{
    private const string SeenKey = "Overrun";

    [Header("UI")]
    [SerializeField] private GameObject noticePanel;
    [SerializeField] private Button beginButton;

    private PlayerInput playerInput;
    private PlayerController playerController;
    private HotbarController hotbarController;

    private bool inputWasEnabled;
    private bool hotbarWasEnabled;
    private bool wasPausedBeforeNotice;

    private void Awake()
    {
        if (noticePanel == null || beginButton == null)
        {
            enabled = false;
            return;
        }

        noticePanel.SetActive(false);
        beginButton.onClick.AddListener(CloseNotice);
    }

    private void Start()
    {
        if (PlayerPrefs.GetInt(SeenKey, 0) == 0)
            ShowNotice();
    }

    private void ShowNotice()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            playerInput = player.GetComponent<PlayerInput>();
            playerController = player.GetComponent<PlayerController>();
        }

        hotbarController = FindAnyObjectByType<HotbarController>();

        wasPausedBeforeNotice = PauseController.isGamePaused;

        if (playerInput != null)
        {
            inputWasEnabled = playerInput.enabled;
            playerInput.enabled = false;
        }

        if (hotbarController != null)
        {
            hotbarWasEnabled = hotbarController.enabled;
            hotbarController.enabled = false;
        }

        PauseController.SetPause(true);

        noticePanel.SetActive(true);
        noticePanel.transform.SetAsLastSibling();

        beginButton.Select();
    }

    public void CloseNotice()
    {
        PlayerPrefs.SetInt(SeenKey, 1);
        PlayerPrefs.Save();

        noticePanel.SetActive(false);

        if (playerInput != null)
            playerInput.enabled = inputWasEnabled;

        if (hotbarController != null)
            hotbarController.enabled = hotbarWasEnabled;

        PauseController.SetPause(wasPausedBeforeNotice);

        if (!wasPausedBeforeNotice)
            playerController?.ResyncMoveInput();
    }

    [ContextMenu("Reset Notice For Testing")]
    private void ResetNoticeForTesting()
    {
        PlayerPrefs.DeleteKey(SeenKey);
        PlayerPrefs.Save();
    }

    private void OnDestroy()
    {
        if (beginButton != null)
            beginButton.onClick.RemoveListener(CloseNotice);
    }
}