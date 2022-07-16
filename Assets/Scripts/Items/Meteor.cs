using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteor : MonoBehaviour
{
    [Header("Meteor Size")]
    [SerializeField] float maxMeteorScale = 2f;
    [SerializeField] float minMeteorScale = 0.8f;

    [Header("Sprites")]
    [SerializeField] Sprite[] sprites;

    private void Start()
    {
        int randomSpriteIndex = Random.Range(0, sprites.Length);
        GetComponent<SpriteRenderer>().sprite = sprites[randomSpriteIndex];

        float randomObjectScale = Random.Range(minMeteorScale, maxMeteorScale);
        transform.localScale = new Vector3 (randomObjectScale, randomObjectScale, randomObjectScale);
    }
}
