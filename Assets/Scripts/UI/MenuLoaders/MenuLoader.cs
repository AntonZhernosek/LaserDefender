using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuLoader : MonoBehaviour
{
    enum SceneToLoad
    {
        MainMenu,
        PreviousScene,
        NormalMode,
        EndlessMode,
        Quit
    }

    [SerializeField] SceneToLoad sceneToLoad;

    void Start()
    {
        Button button = GetComponent<Button>();

        switch (sceneToLoad)
        {
            case SceneToLoad.MainMenu:
                button.onClick.AddListener(LevelManager.Instance.LoadMainMenuScene);
                break;

            case SceneToLoad.PreviousScene:
                button.onClick.AddListener(LevelManager.Instance.LoadPreviousScene);
                break;

            case SceneToLoad.NormalMode:
                button.onClick.AddListener(LevelManager.Instance.LoadNormalMode);
                break;

            case SceneToLoad.EndlessMode:
                button.onClick.AddListener(LevelManager.Instance.LoadEndlessMode);
                break;

            case SceneToLoad.Quit:
                button.onClick.AddListener(LevelManager.Instance.QuitGame);
                break;
        }
    }
}
