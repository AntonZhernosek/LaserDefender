using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIDisplay : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] Slider healthBar;

    [Header("Score")]
    [SerializeField] TextMeshProUGUI scoreText;

    ScoreKeeper scoreKeeper;
    Health playerHealth;

    private void Awake()
    {
        scoreKeeper = FindObjectOfType<ScoreKeeper>();
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();
    }

    private void Start()
    {
        healthBar.maxValue = playerHealth.GetMaxHealth();
    }

    private void Update()
    {
        scoreText.text = scoreKeeper.GetCurrentScore().ToString("0000000");
        healthBar.value = playerHealth.GetHealth();
    }
}
