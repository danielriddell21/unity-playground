using UnityEngine;

public class MainMenu :  IMenu
{
    public void Play()
    {
        CurrentPage.SetActive(false);
        Page1.SetActive(true);
    }

    public void Settings()
    {
        CurrentPage.SetActive(false);
        Page2.SetActive(true);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
