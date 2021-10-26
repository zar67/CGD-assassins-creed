using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneControl
{
    static string menuSceneName = "MainScene";
    static string gameOverSceneName = "GameOverScene";

    public static void LoadMainMenu()
    {
        SceneManager.LoadScene(menuSceneName);
	}
    public static void LoadGameOver()
    {
        SceneManager.LoadScene(gameOverSceneName);
	}
    public static void LoadPlay()
    {

	}
}
