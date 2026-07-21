using UnityEngine;

public class GameExitController : MonoBehaviour
{
    public void SaveAndExit()
    {
        SaveController saveController = FindAnyObjectByType<SaveController>();
        if (saveController != null)
            saveController.saveGame();

        ExitGame();
    }

    public void ExitDeathScreen()
    {
        ExitGame();
    }

    private void ExitGame()
    {
        PauseController.SetPause(false);

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
