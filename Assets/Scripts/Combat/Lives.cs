using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Lives : MonoBehaviour
{
    [Header("Instantiated Life Up UI")]
    [SerializeField] GameObject lifeUpCanvas;
    [SerializeField] float spawnOffset = 1f;

    [Header("Lives")]
    [SerializeField] int startingLives = 3;
    int currentLives;

    public event Action onLivesUpdated;

    private void Start()
    {
        currentLives = startingLives;
        onLivesUpdated?.Invoke();
    }

    public int GetCurrentLives()
    {
        return currentLives;
    }

    public void DecreaseLives()
    {
        currentLives--;
        onLivesUpdated?.Invoke();
    }

    public void AddALife()
    {
        currentLives++;
        onLivesUpdated?.Invoke();

        FindObjectOfType<AudioPlayer>().PlayAudioClip("AddLife");

        SpawnUIElement();
    }

    void SpawnUIElement()
    {
        var viewportPos = Camera.main.WorldToViewportPoint(transform.position);

        if (viewportPos.x > 0.5)
        {
            Instantiate(lifeUpCanvas, new Vector2(transform.position.x - spawnOffset, transform.position.y), Quaternion.identity, transform);
        }
        else
        {
            Instantiate(lifeUpCanvas, new Vector2(transform.position.x + spawnOffset, transform.position.y), Quaternion.identity, transform);
        }
    }
}
