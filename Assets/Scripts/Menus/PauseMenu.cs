using MLAPI;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : IMenu
{
    [Space]
    public Text QuitButton;

    void Start()
    {
        QuitButton.text = LevelController.IsMultiplayer ?
            LevelController.IsHosting ? "Stop" : "Disconnect" : "Quit";
    }

    public void Quit()
    {
        LevelController.IsPaused = false;

        if (LevelController.IsMultiplayer)
        {
            if (LevelController.IsHosting)
            {
                NetworkManager.Singleton.StopHost();
            }
            else
            {
                NetworkManager.Singleton.StopClient();
            }
        }
        else
        {
            LevelController.SwitchScene("MainMenu");
        }
    }
}
