using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LivesUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI livesText;
    Lives playerLives;

    private void OnEnable()
    {
        playerLives = GameObject.FindGameObjectWithTag("Player").GetComponent<Lives>();
        playerLives.onLivesUpdated += UpdateHealthUI;
    }

    private void OnDisable()
    {
        if (!playerLives) return;
        playerLives.onLivesUpdated -= UpdateHealthUI;
    }

    void UpdateHealthUI()
    {
        livesText.text = string.Format("x {0}", playerLives.GetCurrentLives());
    }
}
