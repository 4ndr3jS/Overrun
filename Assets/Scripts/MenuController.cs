using UnityEngine;
using UnityEngine.InputSystem;

public class MenuController : MonoBehaviour
{

    public GameObject menuCanvas;
    private PlayerController playerController;

    void Start()
    {
        menuCanvas.SetActive(false);
        playerController = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerController>();
    }

    void Update()
    {
        if(Keyboard.current.tabKey.wasPressedThisFrame)
        {
            if (!menuCanvas.activeSelf && PauseController.isGamePaused)
            {
                return;
            }

            bool isOpening = !menuCanvas.activeSelf;
            menuCanvas.SetActive(isOpening);
            PauseController.SetPause(isOpening);

            if (!isOpening)
            {
                playerController?.ResyncMoveInput();
            }
        }
    }
}