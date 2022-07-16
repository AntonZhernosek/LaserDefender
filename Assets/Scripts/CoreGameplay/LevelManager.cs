using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    int previousSceneIndex = 0;

    public static LevelManager Instance { get; private set; }

    private void Awake()
    {
        ManageSingleton();
    }

    void ManageSingleton()
    {
        if (Instance)
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void LoadGameOverScreen(float delay)
    {
        previousSceneIndex = SceneManager.GetActiveScene().buildIndex;
        StartCoroutine(WaitAndLoad("GameOverMenu", delay));
    }

    public void LoadWinScene(float delay)
    {
        previousSceneIndex = SceneManager.GetActiveScene().buildIndex;
        StartCoroutine(WaitAndLoad("WinMenu", delay));
    }
    
    public void LoadMainMenuScene()
    {
        SceneManager.LoadScene(0);
    }

    public void LoadPreviousScene()
    {
        SceneManager.LoadScene(previousSceneIndex);
    }

    public void LoadNormalMode()
    {
        SceneManager.LoadScene(1);
    }

    public void LoadEndlessMode()
    {
        SceneManager.LoadScene(2);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    IEnumerator WaitAndLoad(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
    }
}
