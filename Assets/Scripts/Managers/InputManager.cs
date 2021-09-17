using System;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private class UnknownKeybindException : Exception
    {
        public UnknownKeybindException(string message) : base(message)
        {
        }
    }

    public static bool GetButtonDown(string buttonName)
    {
        var key = GetBinding(buttonName);
        return Input.GetKeyDown(key);
    }

    public static bool GetButtonUp(string buttonName)
    {
        var key = GetBinding(buttonName);
        return Input.GetKeyUp(key);
    }

    public static bool GetButton(string buttonName)
    {
        var key = GetBinding(buttonName);
        return Input.GetKey(key);
    }

    private static KeyCode GetBinding(string buttonName)
    {
        try
        {
            return (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(buttonName));
        }
        catch (Exception ex)
        {
            throw new UnknownKeybindException($"Cannot find binding {buttonName}. Exception: {ex}");
        }
    }
}
