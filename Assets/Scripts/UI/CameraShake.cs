using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField] float shakeDuration = 0.5f;
    [SerializeField] float shakeMagnitude = 0.5f;

    Vector3 initialPosition;

    Coroutine cameraShakingCoroutine;

    private void OnEnable()
    {
        FindObjectOfType<PauseMenuUI>().OnGamePaused += StopOnPause;
    }

    private void OnDisable()
    {
        var pauseMenu = FindObjectOfType<PauseMenuUI>();
        if (!pauseMenu) return;
        pauseMenu.OnGamePaused += StopOnPause;
    }

    void Start()
    {
        initialPosition = transform.position;
    }

    public void Play()
    {
        StopCameraShake();
        cameraShakingCoroutine = StartCoroutine(ShakeCamera());
    }

    private void StopOnPause(bool isGamePaused)
    {
        if (isGamePaused)
        {
            StopCameraShake();
        }
    }

    private void StopCameraShake()
    {
        if (cameraShakingCoroutine != null)
        {
            StopCoroutine(cameraShakingCoroutine);
            cameraShakingCoroutine = null;
        }
        transform.position = initialPosition;
    }

    IEnumerator ShakeCamera()
    {
        float currentShakeTime = 0f;
        while (currentShakeTime < shakeDuration)
        {
            currentShakeTime += Time.deltaTime;
            transform.position = initialPosition + (Vector3)Random.insideUnitCircle * shakeMagnitude;
            yield return new WaitForEndOfFrame();
        }
        transform.position = initialPosition;
    }
}
