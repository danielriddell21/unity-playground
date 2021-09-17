public class PlayMenu : IPlayMenu
{
    public void SinglePlayer()
    {
        LevelController.IsMultiplayer = false;
        LevelController.IsHosting = false;
        LevelController.SwitchScene(SceneName);
    }

    public void Multiplayer()
    {
        CurrentPage.SetActive(false);
        Page1.SetActive(true);
    }
}
