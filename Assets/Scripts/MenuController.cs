using UnityEngine;
using UnityEngine.InputSystem;

public class MenuController : MonoBehaviour
{

    public GameObject menuCanvas;
    public TabsController tabsController;
    
    public int settingsTab = 2;

    private PlayerController playerController;

    void Start()
    {
        menuCanvas.SetActive(false);
        playerController = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerController>();
    }

    void Update()
    {
        if (Keyboard.current == null)
            return;

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            HandleEscape();
        }

        if (Keyboard.current.tabKey.wasPressedThisFrame) 
        {
            HandleTab();
        }
    }

    private void HandleEscape()
    {
        if(ShopController.Instance != null && ShopController.Instance.isShopOpen)
        {
            ShopController.Instance.CloseShop();
            return;
        }

        if (menuCanvas.activeSelf)
        {
            setMenuOpen(false);
            return;
        }

        if (PauseController.isGamePaused)
            return;

        setMenuOpen(true);

        tabsController.ActivateTab(settingsTab);
    }

    private void HandleTab()
    {
        if (ShopController.Instance != null && ShopController.Instance.isShopOpen)
        {
            ShopController.Instance.CloseShop();
            return;
        }

        if (menuCanvas.activeSelf)
        {
            setMenuOpen(false);
            return;
        }

        if (!menuCanvas.activeSelf && PauseController.isGamePaused)
            return;

        if (PauseController.isGamePaused)
            return;

        setMenuOpen(!menuCanvas.activeSelf);
    }

    private void setMenuOpen(bool open)
    {
        menuCanvas.SetActive(open);
        PauseController.SetPause(open);

        if (open)
        {
            InventoryController.Instance?.RefreshAllItems();
        }
        else
            playerController?.ResyncMoveInput();
    }
}