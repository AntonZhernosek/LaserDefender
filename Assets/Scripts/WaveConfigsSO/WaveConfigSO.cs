using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WaveConfigSO : ScriptableObject
{
    [SerializeField] protected float moveSpeed = 5f;
    [SerializeField] protected float timeBetweenEnemySpawns = 2f;
    [SerializeField] protected float spawnTimeVariance = 0.5f;
    [SerializeField] protected float minimumSpawnTime = 0.2f;

    public abstract List<Transform> GetWaypoints();

    public abstract IEnumerable<GameObject> GetEnemyList();

    public virtual float GetMoveSpeed()
    {
        return moveSpeed;
    }

    public float GetRandomSpawnTime()
    {
        float spawnTime = Random.Range(timeBetweenEnemySpawns - spawnTimeVariance, timeBetweenEnemySpawns + spawnTimeVariance);
        return Mathf.Clamp(spawnTime, minimumSpawnTime, float.MaxValue);
    }
}
