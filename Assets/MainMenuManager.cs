using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MainMenuManager : MonoBehaviour
{
    public GameObject MainCanvas;
    public GameObject LocalCanvas;
    public GameObject OptionCanvas;


    public enum MenuScreens
    {
        MainMenu,
        LocalPlay
    }

    public void ToLocalScreen(int previousScreen)
    {
        switch((MenuScreens)previousScreen)
        {
            case MenuScreens.MainMenu:
                MainCanvas.SetActive(false);
                break;
        }

        LocalCanvas.SetActive(true);
    }

    public void ToMainMenu(int previousScreen)
    {
        switch ((MenuScreens)previousScreen)
        {
            case MenuScreens.LocalPlay:
                LocalCanvas.SetActive(false);
                break;
        }

        MainCanvas.SetActive(true);
    }

    public void ToOptionsMenu(int previousScreen)
    {
        switch ((MenuScreens)previousScreen)
        {
            case MenuScreens.MainMenu:
                MainCanvas.SetActive(false);
                break;
        }

        OptionCanvas.SetActive(true);
    }
}
