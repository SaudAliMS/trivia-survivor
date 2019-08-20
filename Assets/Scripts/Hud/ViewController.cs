using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewController : SingletonMono<ViewController>
{
    public MainMenuViewController       mainMenuViewController;
    public GameplayViewController       gameplayViewController;
    public LevelCompletedViewController levelCompletedViewController;
    public GameCompleteViewController       gameCompleteViewController;

    public Views currentView;
    public void OpenView(Views view)
    {
        if (currentView != view)
        {
            CloseView(currentView);
            currentView = view;
            switch (view)
            {
                case Views.MainMenu:
                    mainMenuViewController.Open();
                    break;
                case Views.GamePlay:
                    gameplayViewController.Open();
                    break;
                case Views.GameComplete:
                    gameCompleteViewController.Open();
                    break;
                case Views.LevelComplete:
                    levelCompletedViewController.Open();
                    break;
            }
        }
    }

    public void CloseView(Views view)
    {
        switch (view)
        {
            case Views.MainMenu:
                mainMenuViewController.Close();
                break;
            case Views.GamePlay:
                gameplayViewController.Close();
                break;
            case Views.GameComplete:
                gameCompleteViewController.Close();
                break;
            case Views.LevelComplete:
                levelCompletedViewController.Close();
                break;
        }
    }  
}
