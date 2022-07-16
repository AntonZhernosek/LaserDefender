using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Shield : MonoBehaviour
{
    enum ShieldState
    {
        ReadyForUse,
        IsInUse,
        Cooldown
    }

    [Header("Shields")]
    [SerializeField] GameObject normalShield = null;
    [SerializeField] GameObject hitShieldVFX;

    [Header("Shield Timers")]
    [SerializeField] float shieldDuration = 5f;
    [SerializeField] float hitShieldDuration = 0.3f;
    [SerializeField] float shieldCooldown = 10f;
    float currentShieldDuration;

    ShieldState shieldState = ShieldState.ReadyForUse;

    Coroutine activeCooldownCoroutine;

    Collider2D col;
    ShieldUI shieldUI;
    AudioPlayer audioPlayer;

    private void Awake()
    {
        col = GetComponent<Collider2D>();
        shieldUI = FindObjectOfType<ShieldUI>();
        audioPlayer = FindObjectOfType<AudioPlayer>();
        SetShieldProperties(false);
    }

    private void Start()
    {
        currentShieldDuration = shieldDuration;
    }

    public bool IsShielding()
    {
        return shieldState == ShieldState.IsInUse;
    }

    public void HandleShieldPickupUse()
    {
        switch (shieldState)
        {
            case ShieldState.ReadyForUse:
                currentShieldDuration *= 2;
                shieldUI.SetPoweredUpColour();
                break;

            case ShieldState.IsInUse:
                currentShieldDuration = Mathf.Clamp(currentShieldDuration + shieldDuration, 0, shieldDuration * 2);
                shieldUI.SetNewDuration(currentShieldDuration / (shieldDuration * 2), shieldDuration * 2);
                break;

            case ShieldState.Cooldown:
                StopCooldownCoroutine();
                shieldUI.ResetUI();
                break;
        }
    }

    public IEnumerator ActivateShield()
    {
        if (shieldState != ShieldState.ReadyForUse) yield break;

        shieldState = ShieldState.IsInUse;

        audioPlayer.PlayAudioClip("ShieldUp");

        SetShieldProperties(true);

        shieldUI.StartDuration(currentShieldDuration);

        while (currentShieldDuration > Mathf.Epsilon)
        {
            currentShieldDuration -= Time.deltaTime;
            yield return null;
        }

        currentShieldDuration = shieldDuration;

        SetShieldProperties(false);

        audioPlayer.PlayAudioClip("ShieldDown");

        activeCooldownCoroutine = StartCoroutine(StartShieldCooldown(shieldCooldown));
    }

    IEnumerator StartShieldCooldown(float cooldown)
    {
        shieldState = ShieldState.Cooldown;
        yield return shieldUI.StartCooldown(cooldown);
        StopCooldownCoroutine();
    }

    private void SetShieldProperties(bool state)
    {
        normalShield.SetActive(state);
        col.enabled = state;
    }

    private void SpawnHitShieldEffect(Collision2D collision)
    {
        GameObject hitShieldVFXInstance = Instantiate(hitShieldVFX, normalShield.transform);
        var vfx = hitShieldVFXInstance.GetComponent<VisualEffect>();

        vfx.SetVector3("SphereCenter", collision.contacts[0].point);
        vfx.SetFloat("Lifetime", hitShieldDuration);

        Destroy(hitShieldVFXInstance, hitShieldDuration);
    }

    private void StopCooldownCoroutine()
    {
        if (activeCooldownCoroutine != null)
        {
            StopCoroutine(activeCooldownCoroutine);
            activeCooldownCoroutine = null;
        }

        shieldState = ShieldState.ReadyForUse;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (shieldState != ShieldState.IsInUse) return;

        if (collision.gameObject.tag == "Projectile")
        {
            SpawnHitShieldEffect(collision);

            Destroy(collision.gameObject);
        }
    }
}
