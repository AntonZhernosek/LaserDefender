using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class Health : MonoBehaviour
{
    enum EntityStatus
    {
        Player,
        Enemy,
        Boss,
        Meteor
    }

    [Header("Player Check")]
    [SerializeField] EntityStatus status;

    [Header("Health")]
    [SerializeField] int maxHealth = 100;
    int currentHealth;

    [Header("Score")]
    [SerializeField] int scoreWorth = 100;

    [Header("VFX")]
    [SerializeField] ParticleSystem hitEffect;
    [SerializeField] bool applyCameraShake = false;

    [Header("Invincibility")]
    [SerializeField] int invincibilityDuration = 3;
    bool isInvincible = false;

    [Header("Death")]
    [SerializeField] GameObject destroyedParticles;
    [SerializeField] float deathDelay = 3f;

    Shield shield;
    CameraShake cameraShake;
    AudioPlayer audioPlayer;
    Animator anim;
    Lives lives;

    private void Awake()
    {
        if (status == EntityStatus.Player)
        {
            cameraShake = Camera.main.GetComponent<CameraShake>();
            anim = GetComponent<Animator>();
            lives = GetComponent<Lives>();
            shield = GetComponentInChildren<Shield>();
        }
        else if (status == EntityStatus.Boss)
        {
            anim = GetComponent<Animator>();
        }

        audioPlayer = FindObjectOfType<AudioPlayer>();
    }

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void AddMaxHealth(int amount)
    {
        maxHealth += amount;
        currentHealth = maxHealth;
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }    

    public int GetHealth()
    {
        return currentHealth;
    }

    public IEnumerator InvincibilityState(float duration)
    {
        SwitchInvincibility(true);
        yield return new WaitForSeconds(duration);
        SwitchInvincibility(false);
    }

    private void ShakeCamera()
    {
        if (cameraShake != null && applyCameraShake)
        {
            cameraShake.Play();
        }
    }

    private void PlayHitEffects()
    {
        if (hitEffect != null)
        {
            ParticleSystem particles = Instantiate(hitEffect, transform);
            Destroy(particles.gameObject, particles.main.duration + particles.main.startLifetime.constantMax);
        }
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible) return;

        currentHealth = Mathf.Max(currentHealth-damage, 0);

        audioPlayer.PlayAudioClip("TakeDamage");
        PlayHitEffects();
        ShakeCamera();

        if (currentHealth == 0)
        {
            Die();
        }
        
        if (status == EntityStatus.Player)
        {
            StartCoroutine(InvincibilityState(invincibilityDuration));
        }
    }

    private void SwitchInvincibility(bool state)
    {
        anim.SetBool("Invincible", state);
        isInvincible = state;
    }

    private void Die()
    {
        

        switch (status)
        {
            case EntityStatus.Player:
                HandlePlayerDeath();
                break;

            case EntityStatus.Boss:
                HandleBossDeath();
                break;

            case EntityStatus.Enemy:
                HandleEnemyDeath();
                break;

            case EntityStatus.Meteor:
                SpawnDestroyParticles();
                Destroy(gameObject);
                break;
        }
    }

    private void HandlePlayerDeath()
    {
        if (!lives) return;

        lives.DecreaseLives();

        if (lives.GetCurrentLives() == 0)
        {
            anim.SetTrigger("Die");
            audioPlayer.PlayAudioClip("GameOver");
            audioPlayer.FadeOutMusic();
            FindObjectOfType<LevelManager>().LoadGameOverScreen(deathDelay);
            GetComponent<PlayerInput>().actions.Disable();

            SpawnDestroyParticles();
        }
        else
        {
            StartCoroutine(InvincibilityState(invincibilityDuration));
            currentHealth = maxHealth;
        }
    }

    private void HandleBossDeath()
    {
        GetComponent<BossAI>().enabled = false;

        GetComponent<Collider2D>().enabled = false;

        GetComponent<Shooter>().EnableShooting(false);

        anim.SetTrigger("Die");

        audioPlayer.FadeOutMusic();

        FindObjectOfType<ScoreKeeper>().ModifyScore(scoreWorth);

        FindObjectOfType<LevelManager>().LoadWinScene(deathDelay);

        SpawnDestroyParticles();
    }

    private void HandleEnemyDeath()
    {
        SpawnDestroyParticles();

        FindObjectOfType<ScoreKeeper>().ModifyScore(scoreWorth);

        Destroy(gameObject);
    }

    private void SpawnDestroyParticles()
    {
        if (destroyedParticles)
        {
            Instantiate(destroyedParticles, transform.position, Quaternion.identity);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (status == EntityStatus.Player)
        {
            if (shield.IsShielding() && collision.transform.tag == "Projectile") return;
        }

        DamageDealer damageDealer = collision.GetComponent<DamageDealer>();

        if (!damageDealer) return;

        TakeDamage(damageDealer.GetDamage());
        damageDealer.Hit();
    }
}
