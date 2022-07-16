using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShieldUI : MonoBehaviour
{
    [Header("Gameobject That Serves As Timer")]
    [SerializeField] GameObject shieldTimer;

    [Header("Colours")]
    [SerializeField][ColorUsage(true, true)] Color defaultColour;
    [SerializeField][ColorUsage(true, true)] Color poweredUpColour;

    Material timerMaterial;

    Coroutine timerUpdating;

    private void Awake()
    {
        timerMaterial = shieldTimer.GetComponent<RawImage>().material;
    }

    private void Start()
    {
        timerMaterial.SetTexture("_MainTexture", shieldTimer.GetComponent<RawImage>().texture);
        timerMaterial.SetFloat("_FillAmount", 1);
        timerMaterial.SetColor("_Color", defaultColour);
    }

    public void SetPoweredUpColour()
    {
        timerMaterial.SetColor("_Color", poweredUpColour);
    }

    public void ResetUI()
    {
        StopUpdatingTimer();
        timerMaterial.SetFloat("_FillAmount", 1);
        timerMaterial.SetColor("_Color", defaultColour);
    }

    public void SetNewDuration(float amount, float newTimer)
    {
        timerMaterial.SetFloat("_FillAmount", amount);
        StartDuration(newTimer);
    }

    public Coroutine StartDuration(float time)
    {
        return UpdateTimer(0f, time);
    }

    public Coroutine StartCooldown(float time)
    {
        return UpdateTimer(1f, time);
    }

    private Coroutine UpdateTimer(float target, float time)
    {
        StopUpdatingTimer();
        timerUpdating = StartCoroutine(UpdateTimerCoroutine(target, time));
        return timerUpdating;
    }

    private IEnumerator UpdateTimerCoroutine(float target, float time)
    {
        timerMaterial.SetColor("_Color", defaultColour);
        while (!Mathf.Approximately(timerMaterial.GetFloat("_FillAmount"), target))
        {
            float setAmount = Mathf.MoveTowards(timerMaterial.GetFloat("_FillAmount"), target, Time.deltaTime / time);
            timerMaterial.SetFloat("_FillAmount", setAmount);
            yield return new WaitForEndOfFrame();
        }
    }

    private void StopUpdatingTimer()
    {
        if (timerUpdating != null)
        {
            StopCoroutine(timerUpdating);
        }
    }

}
