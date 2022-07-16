using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    [System.Serializable]
    class Attack
    {
        public string name;

        [Header("Projectile Prefabs")]
        public GameObject projectilePrefab;

        [Header("SpawnLocation")]
        public Transform[] spawnLocations;

        [Header("Projectile Settings")]
        public int projectileDamage;
        public float projectileSpeed;
        public float projectileLifetime;

        [Header("Firing Rate and Time Constraints")]
        public float firingRate;
        public float firingRandomness;
        public float minimumShootingTime;

        public void SetProjectileSpeed(float projectileSpeed)
        {
            this.projectileSpeed = projectileSpeed;
        }
    }

    [Header("Use Automated Shooting")]
    [SerializeField] bool useAI;

    [Header("Attackss")]
    [SerializeField] List<Attack> attacks = new List<Attack>();
    [SerializeField] int bonusDamagePerWeaponSwitch = 2;
    Attack currentAttack;
    int tempCurrentAttackindex = 0;
    int bonusDamageCounter = 0;

    [Header("First Shot Delay")]
    [SerializeField] float minFirstShotDelay = 1f;
    [SerializeField] float maxFirstShotDelay = 2f;

    bool enableShooting = true;

    bool isFiring;

    Coroutine firingCoroutine;
    Coroutine weaponSwitchCoroutine;

    AudioPlayer audioPlayer;

    private void Awake()
    {
        audioPlayer = FindObjectOfType<AudioPlayer>();
    }

    void Start()
    {
        ChooseAttack(0);
        ReverseProjectileSpeed();
    }

    void Update()
    {
        if (!enableShooting) return;
        Fire();
    }

    public void ChooseAttack(int index)
    {
        currentAttack = attacks[index];
    }

    public void ChooseAttack (string name)
    {
        foreach (Attack attack in attacks)
        {
            if (attack.name == name)
            {
                currentAttack = attack;
                return;
            }
        }
        Debug.LogError("Invalid Attack Identifier!");
    }

    public void ChooseRandomAttack()
    {
        int randomIndex = Random.Range(0, attacks.Count);
        
        while (attacks.IndexOf(currentAttack) == randomIndex)
        {
            randomIndex = Random.Range(0, attacks.Count);
        }

        currentAttack = null;

        currentAttack = attacks[randomIndex];
    }

    public void SwitchWeaponTemporarily(float attackTime, int attackIndex)
    {
        weaponSwitchCoroutine = StartCoroutine(SwitchWeaponTemporarilyCoroutine(attackTime, attackIndex));
    }

    public void SwitchWeaponTemporarily(float attackTime, string attackName)
    {
        StartCoroutine(SwitchWeaponTemporarilyCoroutine(attackTime, attackName));
    }

    public void EnableShooting(bool state) 
    { 
        enableShooting = state;

        if (useAI)
        {
            isFiring = state;
        }
        else
        {
            if (!state)
            {
                isFiring = state;
            }
        }

        StopFiringCoroutine();
    }

    public void SetIsFiring(bool state) 
    { 
        isFiring = state; 
    }

    private void ReverseProjectileSpeed()
    {
        if (!useAI) return;

        isFiring = true;

        foreach (Attack attack in attacks)
        {
            attack.SetProjectileSpeed(-attack.projectileSpeed);
        }
    }

    void Fire()
    {
        if (isFiring && firingCoroutine == null)
        {
            firingCoroutine = StartCoroutine(FireContinuously());
        }
        else if (!isFiring)
        {
            StopFiringCoroutine();
        }
    }

    IEnumerator FireContinuously()
    {
        if (useAI)
        {
            yield return new WaitForSeconds(GetFirstShotDelay());
        }

        while (true)
        {
            SpawnProjectiles();
            PlayShootingAudio();
            float shootingTime = AddRandomnessToShootingTime();
            yield return new WaitForSeconds(shootingTime);
        }
    }

    private IEnumerator SwitchWeaponTemporarilyCoroutine(float attackTime, int attackIndex)
    {
        bonusDamageCounter += bonusDamagePerWeaponSwitch;

        StopWeaponSwitchCoroutine();
        StopFiringCoroutine();
        tempCurrentAttackindex = attacks.IndexOf(currentAttack);

        ChooseAttack(attackIndex);

        yield return new WaitForSeconds(attackTime);

        StopFiringCoroutine();
        StopWeaponSwitchCoroutine();
    }

    private IEnumerator SwitchWeaponTemporarilyCoroutine(float attackTime, string attackName)
    {
        bonusDamageCounter += bonusDamagePerWeaponSwitch;

        StopWeaponSwitchCoroutine();
        StopFiringCoroutine();
        tempCurrentAttackindex = attacks.IndexOf(currentAttack);

        ChooseAttack(attackName);

        yield return new WaitForSeconds(attackTime);

        StopFiringCoroutine();
        StopWeaponSwitchCoroutine();
    }

    private void PlayShootingAudio()
    {
        audioPlayer.PlayAudioClip("Shoot");
    }

    private float AddRandomnessToShootingTime()
    {
        float shootingTime = Random.Range(currentAttack.firingRate - currentAttack.firingRandomness, 
            currentAttack.firingRate + currentAttack.firingRandomness);

        shootingTime = Mathf.Clamp(shootingTime, currentAttack.minimumShootingTime, float.MaxValue);
        return shootingTime;
    }

    private void SpawnProjectiles()
    {
        foreach (Transform location in currentAttack.spawnLocations)
        {
            GameObject projectile = Instantiate(currentAttack.projectilePrefab, location.position, Quaternion.identity);
            SetUpProjectile(projectile);
            DestroyProjectile(projectile);
        }
    }

    private void StopFiringCoroutine()
    {
        if (firingCoroutine != null)
        {
            StopCoroutine(firingCoroutine);
            firingCoroutine = null;
        }
    }

    private void StopWeaponSwitchCoroutine()
    {
        if (weaponSwitchCoroutine != null)
        {
            StopCoroutine(weaponSwitchCoroutine);
            weaponSwitchCoroutine = null;

            ChooseAttack(tempCurrentAttackindex);
        }
    }
    private void DestroyProjectile(GameObject projectile)
    {
        Destroy(projectile, currentAttack.projectileLifetime);
    }

    private void SetUpProjectile(GameObject projectile)
    {
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = transform.up * currentAttack.projectileSpeed;
        }

        DamageDealer projectileDamageDealer = projectile.GetComponent<DamageDealer>();
        projectileDamageDealer.SetDamage(currentAttack.projectileDamage + bonusDamageCounter);
    }

    private float GetFirstShotDelay()
    {
        return Random.Range(minFirstShotDelay, maxFirstShotDelay);
    }
}
