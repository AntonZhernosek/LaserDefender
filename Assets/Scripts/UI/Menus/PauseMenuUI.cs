using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.InputSystem;

public class PauseMenuUI : MonoBehaviour
{
    [Header("Pause Menu")]
    [SerializeField] GameObject pauseUI;

    [Header("SliderInfo")]
    [SerializeField] VolumeSliderUI sfxSlider;
    [SerializeField] VolumeSliderUI musicSlider;

    public event Action<bool> OnGamePaused;

    PlayerPrefsHandler prefsHandler;
    AudioPlayer audioPlayer;

    private void Awake()
    {
        prefsHandler = FindObjectOfType<PlayerPrefsHandler>();
        audioPlayer = FindObjectOfType<AudioPlayer>();
        Time.timeScale = 1f;
    }

    private void Start()
    {
        var input = GetComponent<PlayerInput>();

        //input.SwitchCurrentControlScheme("Keyboard&Mouse", new InputDevice[] { Keyboard.current, Mouse.current });

        UpdateOnStart();
        pauseUI.SetActive(false);
    }

    private void OnDisable()
    {
        Time.timeScale = 1f;
    }

    public void ShowHideUI()
    {
        pauseUI.SetActive(!pauseUI.activeSelf);

        if (Mathf.Approximately(Time.timeScale, Mathf.Epsilon))
        {
            Time.timeScale = 1;
            OnGamePaused?.Invoke(false);
        }
        else if (Mathf.Approximately(Time.timeScale, 1))
        {
            OnGamePaused?.Invoke(true);
            Time.timeScale = 0;
        }
    }

    public void UpdateSFXVolume(float volume)
    {
        prefsHandler.SetSFXVolume(volume);
        audioPlayer.SetGlobalSFXVolume(volume);
    }

    public void UpdateMusicVolume(float volume)
    {
        prefsHandler.SetMusicVolume(volume);
        audioPlayer.SetGlobalMusicVolume(volume);
    }

    void UpdateOnStart()
    {
        float sfxVolume = prefsHandler.GetSFXVolume();
        float musicVolume = prefsHandler.GetMusicVolume();

        sfxSlider.UpdateNumberText(sfxVolume);
        sfxSlider.UpdateSlider(sfxVolume);

        musicSlider.UpdateNumberText(musicVolume);
        musicSlider.UpdateSlider(musicVolume);
    }

    private void OnPause()
    {
        ShowHideUI();
    }
}
