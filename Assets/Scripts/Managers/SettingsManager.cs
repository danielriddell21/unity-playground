using System.Collections.Generic;
using UnityEngine;

public class SettingsManager
{
    public static Dictionary<string, float> defaultSettings => new Dictionary<string, float>()
    {
        { "CurrentProgress", 0 },
        { "GraphicsPreference", 4 },
        { "FieldOfViewPreference", 45 },
        { "SensitivityPreference", 1 },
    };

    public static Dictionary<string, string> defaultKeybinds => new Dictionary<string, string>()
    {
        { "SprintKeybind", "LeftShift" },
        { "JumpKeybind", "Space" },
        { "PullKeybind", "Mouse0" },
        { "ChangeCharacterKeybind", "T" }
    };

    public static void SetSetting(string settingKey, float value)
    {
        PlayerPrefs.SetFloat(settingKey, value);
    }

    public static void SetKeybind(string keybindKey, string value)
    {
        PlayerPrefs.SetString(keybindKey, value);
    }

    public static float GetSetting(string settingKey)
    {
        return PlayerPrefs.GetFloat(settingKey);
    }

    public static string GetKeybind(string keybindKey)
    {
        return PlayerPrefs.GetString(keybindKey);
    }

    [RuntimeInitializeOnLoadMethod]
    static void SetDefaultSettings()
    {
        foreach (var key in defaultSettings.Keys)
        {
            if (!PlayerPrefs.HasKey(key))
            {
                PlayerPrefs.SetFloat(key, defaultSettings[key]);
            }
        }
    }
    [RuntimeInitializeOnLoadMethod]
    static void SetDefaultKeybinds()
    {
        foreach (var key in defaultKeybinds.Keys)
        {
            if (!PlayerPrefs.HasKey(key))
            {
                PlayerPrefs.SetString(key, defaultKeybinds[key]);
            }
        }
    }
}