using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    enum GameModeConfiguration
    {
        EndlessOrdered,
        EndlessRandomPreconfigured,
        EndlessRandomNonConfigured,
        Boss
    }

    [Header("Game Mode Configuration")]
    [SerializeField] bool enableSpawning = true;
    [SerializeField] GameModeConfiguration modeConfiguration;

    [Header("Upgrade Over Time")]
    [SerializeField] bool upgradeOverTime = true;

    [Header("Enemy Waves")]
    [SerializeField] List<WaveConfigSO> orderedWaveConfigs = new List<WaveConfigSO>();
    [SerializeField] List<WaveConfigSO> randomizedPreBuiltWaveConfigs = new List<WaveConfigSO>();
    [SerializeField] RandomizedWaveConfigSO randomizedWaveConfig;

    [Header("Time Between Waves")]
    [SerializeField] float timeBetweenWaves = 0.5f;

    [Header("Enemy Upgrades")]
    [SerializeField] int bonusHealth = 10;
    [SerializeField] int bonusDamage = 5;
    [SerializeField] float timeForAnotherEnemyUpgradeInSeconds = 120f;
    int upgradesCounter = 0;
    float currentUpgradeTimer = 0f;

    [Header("Boss")]
    [SerializeField] GameObject boss;
    [SerializeField] Transform spawnLocation;
    [SerializeField] Transform bossStartingArea;
    [SerializeField] float delayToBossSpawn = 3f;

    WaveConfigSO currentWave;
    List<Transform> currentWaypoints;

    void Start()
    {
        if (!enableSpawning) return;
        StartCoroutine(SpawnWavesOfEnemies());
    }

    public void OnDebugKey()
    {
        SpawnBoss();
    }

    private void Update()
    {
        RunUpgradeTimer();
    }

    IEnumerator SpawnWavesOfEnemies()
    {
        switch(modeConfiguration)
        {
            case (GameModeConfiguration.EndlessOrdered):

                while(true)
                {
                    yield return StartCoroutine(SpawnOrderedList());
                }

            case (GameModeConfiguration.EndlessRandomPreconfigured):

                yield return StartCoroutine(SpawnOrderedList());
                while (true)
                {
                    yield return StartCoroutine(SpawnRandomizedConfiguredList());
                }

            case (GameModeConfiguration.EndlessRandomNonConfigured):
                while (true)
                {
                    yield return StartCoroutine(SpawnRandomizedNonConfiguredList());
                }

            case (GameModeConfiguration.Boss):

                yield return StartCoroutine(SpawnOrderedList());
                yield return new WaitForSeconds(delayToBossSpawn);
                SpawnBoss();
                break;
        }
    }

    IEnumerator SpawnOrderedList()
    {
        foreach (WaveConfigSO wave in orderedWaveConfigs)
        {
            currentWave = wave;
            currentWaypoints = currentWave.GetWaypoints();
            yield return StartCoroutine(SpawnEnemies());
            yield return new WaitForSeconds(timeBetweenWaves);
        }
    }

    IEnumerator SpawnRandomizedConfiguredList()
    {
        currentWave = RandomizeWave();
        currentWaypoints = currentWave.GetWaypoints();
        yield return StartCoroutine(SpawnEnemies());
        yield return new WaitForSeconds(timeBetweenWaves);
    }

    IEnumerator SpawnRandomizedNonConfiguredList()
    {
        currentWave = randomizedWaveConfig;
        currentWaypoints = currentWave.GetWaypoints();
        yield return StartCoroutine(SpawnEnemies());
        yield return new WaitForSeconds(timeBetweenWaves);
    }

    IEnumerator SpawnEnemies()
    {
        var enemiesList = currentWave.GetEnemyList();

        float moveSpeed = currentWave.GetMoveSpeed();

        foreach (var enemy in enemiesList)
        {
            GameObject enemyInstance = Instantiate(enemy, currentWaypoints[0].position, Quaternion.identity, transform);

            Pathfinder enemyPathfinder = enemyInstance.GetComponent<Pathfinder>();
            enemyPathfinder.SetWaypoints(currentWaypoints);
            enemyPathfinder.SetMoveSpeed(moveSpeed);

            if (upgradeOverTime && upgradesCounter > 0)
            {
                UpgradeEnemy(enemyInstance);
            }

            yield return new WaitForSeconds(currentWave.GetRandomSpawnTime());
        }
    }

    WaveConfigSO RandomizeWave()
    {
        int randomIndex = Random.Range(0, randomizedPreBuiltWaveConfigs.Count);
        return randomizedPreBuiltWaveConfigs[randomIndex];
    }

    void UpgradeEnemy(GameObject enemy)
    {
        DamageDealer damageDealer = enemy.GetComponent<DamageDealer>();
        Health enemyHealth = enemy.GetComponent<Health>();

        damageDealer.AddDamage(bonusDamage * upgradesCounter);
        enemyHealth.AddMaxHealth(bonusHealth * upgradesCounter);
    }

    void SpawnBoss()
    {
        GameObject bossInstance = Instantiate(boss, spawnLocation.position, Quaternion.identity);
        bossInstance.GetComponent<BossAI>().SetStartPosition(bossStartingArea);
        StartCoroutine(FindObjectOfType<AudioPlayer>().StartNewBGM("Boss"));
    }

    private void RunUpgradeTimer()
    {
        currentUpgradeTimer += Time.deltaTime;

        if (currentUpgradeTimer > timeForAnotherEnemyUpgradeInSeconds)
        {
            currentUpgradeTimer = 0f;
            upgradesCounter++;
        }
    }
}
