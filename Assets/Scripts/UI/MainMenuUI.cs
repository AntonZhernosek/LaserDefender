using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI highScoreText;

    private void Start()
    {
        PlayerPrefsHandler prefsHandler = FindObjectOfType<PlayerPrefsHandler>();
        highScoreText.text = prefsHandler.GetHighScore().ToString("0000000");
    }
}
