using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VolumeSliderUI : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] TextMeshProUGUI numberText;

    public void UpdateNumberText(float newNumber)
    {
        numberText.text = newNumber.ToString("0");
    }

    public void UpdateSlider(float newNumber)
    {
        slider.value = Mathf.FloorToInt(newNumber);
    }
}
