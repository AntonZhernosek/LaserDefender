using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrefsHandler : MonoBehaviour
{
    private const string SFXVolume = "SFXVolume";
    private const string MusicVolume = "MusicVolume";
    private const string HighScore = "HighScore";

    public void SetSFXVolume(float value)
    {
        PlayerPrefs.SetFloat(SFXVolume, value);
    }

    public void SetMusicVolume(float value)
    {
        PlayerPrefs.SetFloat(MusicVolume, value);
    }

    public void SetHighScore(int value)
    {
        if (value > GetHighScore())
        {
            PlayerPrefs.SetInt(HighScore, value);
        }
    }

    public float GetSFXVolume()
    {
        return PlayerPrefs.GetFloat(SFXVolume, 10);
    }

    public float GetMusicVolume()
    {
        return PlayerPrefs.GetFloat(MusicVolume, 10);
    }    

    public int GetHighScore()
    {
        return PlayerPrefs.GetInt(HighScore, 0);
    }
}
