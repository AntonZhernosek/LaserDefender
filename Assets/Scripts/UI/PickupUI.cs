using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickupUI : MonoBehaviour
{
    [SerializeField] Image spriteUIImage;
    [SerializeField] Sprite defaultUISprite;

    private void Awake()
    {
        ResetPickupIcon();
    }

    public void ChangePickupIcon(Sprite newIcon)
    {
        spriteUIImage.sprite = newIcon;
    }

    public void ResetPickupIcon()
    {
        spriteUIImage.sprite = defaultUISprite;
    }
}
