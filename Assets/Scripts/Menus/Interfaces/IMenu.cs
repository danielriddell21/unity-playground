
using System;
using UnityEngine;

public class IMenu : MonoBehaviour
{
    public GameObject CurrentPage;
    public GameObject BackPage;

    [Space]
    public GameObject Page1;
    public GameObject Page2;

    void Start()
    {
        try
        {
            CurrentPage.SetActive(true);
            BackPage.SetActive(false);
            Page1.SetActive(false);
            Page2.SetActive(false);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Page not found, ex: {ex}");
        }
    }

    public void Back()
    {
        CurrentPage.SetActive(false);
        BackPage.SetActive(true);
    }
}
