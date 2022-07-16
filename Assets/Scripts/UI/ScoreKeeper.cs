using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreKeeper : MonoBehaviour
{
    [SerializeField] int scoreForAdditionalLife = 1000;
    int bonusLivesCounter = 0;

    int currentScore = 0;

    PlayerPrefsHandler prefsHandler;
    Lives playerLives;

    private void Awake()
    {
        ManageSingleton();

        prefsHandler = FindObjectOfType<PlayerPrefsHandler>();
        playerLives = GameObject.FindGameObjectWithTag("Player").GetComponent<Lives>();
    }

    void ManageSingleton()
    {
        int instanceCount = FindObjectsOfType(GetType()).Length;

        if (instanceCount > 1)
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    public void SetHighScore()
    {
        if (currentScore > prefsHandler.GetHighScore())
        {
            prefsHandler.SetHighScore(currentScore);
        }
    }

    public int GetCurrentScore()
    {
        return currentScore;
    }

    public void ModifyScore(int scoreChange)
    {
        currentScore += scoreChange;

        if (Mathf.FloorToInt(currentScore / scoreForAdditionalLife) > bonusLivesCounter)
        {
            bonusLivesCounter++;
            playerLives.AddALife();
        }
    }

    public void ResetScore()
    {
        currentScore = 0;
    }
}
