using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class MultiplayerMenu : IPlayMenu
{
    [Space]
    public Text IpTextBox;

    public void Host()
    {
        LevelController.IsHosting = true;
        LevelController.IsMultiplayer = true;

        LevelController.SwitchScene(SceneName);
    }

    public void JoinScreen()
    {
        CurrentPage.SetActive(false);
        Page1.SetActive(true);
    }

    public void Join()
    {
        var regex = new Regex(@"\b(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\b");
        var isLocalHost = string.IsNullOrEmpty(IpTextBox.text);

        if (regex.IsMatch(IpTextBox.text) || isLocalHost)
        {
            LevelController.IsHosting = false;
            LevelController.IsMultiplayer = true;
            LevelController.MultiplayerIpAddress = IpTextBox.text;

            LevelController.SwitchScene(SceneName);
        }
    }
}
