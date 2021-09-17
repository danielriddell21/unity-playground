using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class KeybindsMenu : IMenu
{
    [Space]
    public Button Keybind1;
    public Button Keybind2;
    public Button Keybind3;
    public Button Keybind4;

    private Text _keybind1text;
    private Text _keybind2text;
    private Text _keybind3text;
    private Text _keybind4text;

    private Event keyEvent;
    private KeyCode newKey;
    bool waitingForKey;

    void Start()
    {
        _keybind1text = Keybind1.GetComponentInChildren<Text>();
        _keybind2text = Keybind2.GetComponentInChildren<Text>();
        _keybind3text = Keybind3.GetComponentInChildren<Text>();
        _keybind4text = Keybind4.GetComponentInChildren<Text>();

        waitingForKey = false;

        LoadKeybinds();
    }

    void OnGUI()
    {
        keyEvent = Event.current;
        if (keyEvent.isKey && waitingForKey)
        {
            newKey = keyEvent.keyCode;
            waitingForKey = false;
        }
    }

    public void StartAssignment(string keyName)
    {
        if (!waitingForKey)
            StartCoroutine(SetButtonText(keyName));
    }

    void SaveKeybinds()
    {
        SettingsManager.SetKeybind("SprintKeybind", _keybind1text.text);
        SettingsManager.SetKeybind("JumpKeybind", _keybind2text.text);
        SettingsManager.SetKeybind("PullKeybind", _keybind3text.text);
        SettingsManager.SetKeybind("ChangeCharacterKeybind", _keybind4text.text);
    }

    void LoadKeybinds()
    {
        _keybind1text.text = SettingsManager.GetKeybind("SprintKeybind");
        _keybind2text.text = SettingsManager.GetKeybind("JumpKeybind");
        _keybind3text.text = SettingsManager.GetKeybind("PullKeybind");
        _keybind4text.text = SettingsManager.GetKeybind("ChangeCharacterKeybind");
    }

    private IEnumerator WaitForKeypress()
    {
        while (!keyEvent.isKey)
        {
            yield return null;
        }
    }

    private IEnumerator SetButtonText(string keyName)
    {
        waitingForKey = true;

        yield return WaitForKeypress();

        switch (keyName)
        {
            case "Sprint":
                _keybind1text.text = newKey.ToString();
                break;
            case "Jump":
                _keybind2text.text = newKey.ToString();
                break;
            case "Pull":
                _keybind3text.text = newKey.ToString();
                break;
            case "ChangeChar":
                _keybind4text.text = newKey.ToString();
                break;
        }

        yield return null;
    }

    public void Back()
    {
        SaveKeybinds();

        CurrentPage.SetActive(false);
        BackPage.SetActive(true);
    }
}
