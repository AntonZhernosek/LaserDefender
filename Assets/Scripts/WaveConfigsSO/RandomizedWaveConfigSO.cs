using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Randomized Wave Configuration", fileName = "New Random WaveConfig")]
public class RandomizedWaveConfigSO : WaveConfigSO
{
    [System.Serializable]
    struct WeightedEnemy
    {
        public GameObject enemyPrefab;
        public float weight;
    }

    [System.Serializable]
    class WeightedEnemyGroup: IEnumerable<WeightedEnemy>
    {
        public string name;
        public List<WeightedEnemy> enemies;

        public IEnumerable<WeightedEnemy> Enemies()
        {
            return enemies;
        }

        public IEnumerator<WeightedEnemy> GetEnumerator()
        {
            return Enemies().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    [Header("Enemy amounts")]
    [SerializeField] int minEnemyAmount = 4;
    [SerializeField] int maxEnemyAmount = 8;

    [Header("EnemyMovement")]
    [SerializeField] float moveSpeedDeviation = 1f;
    
    [Header("Enemies")]
    [SerializeField] List<WeightedEnemyGroup> enemyGroups;

    [Header("Paths")]
    [SerializeField] List<Transform> paths;

    public override IEnumerable<GameObject> GetEnemyList()
    {
        List<GameObject> enemiesList = new List<GameObject>();

        int randomEnemyGroupIndex = RandomizeGroupIndex();

        int enemyCountToSpawn = RandomizeEnemyAmount();

        while (enemyCountToSpawn > 0)
        {

            float pickedWeight = CalculateTotalWeight(randomEnemyGroupIndex) * Random.value;

            int chosenIndex = 0;
            float cumulativeWeight = enemyGroups[randomEnemyGroupIndex].enemies[0].weight;


            while (pickedWeight > cumulativeWeight)
            {
                chosenIndex++;
                cumulativeWeight += enemyGroups[randomEnemyGroupIndex].enemies[chosenIndex].weight;
            }

            enemiesList.Add(enemyGroups[randomEnemyGroupIndex].enemies[chosenIndex].enemyPrefab);

            enemyCountToSpawn--;
        }

        return enemiesList;
    }

    public override List<Transform> GetWaypoints()
    {
        int randomIndex = Random.Range(0, paths.Count);

        List<Transform> waypoints = new List<Transform>();

        foreach (Transform child in paths[randomIndex])
        {
            waypoints.Add(child);
        }
        return waypoints;
    }

    public override float GetMoveSpeed()
    {
        float moveSpeed = Random.Range(base.GetMoveSpeed() - moveSpeedDeviation, base.GetMoveSpeed() + moveSpeedDeviation);
        return moveSpeed;
    }

    private int RandomizeGroupIndex()
    {
        return Random.Range(0, enemyGroups.Count);
    }

    private int RandomizeEnemyAmount()
    {
        return Random.Range(minEnemyAmount, maxEnemyAmount + 1);
    }

    private float CalculateTotalWeight(int groupIndex)
    {
        float totalItemWeight = 0f;

        foreach (WeightedEnemy weightedEnemy in enemyGroups[groupIndex])
        {
            totalItemWeight += weightedEnemy.weight;
        }
        return totalItemWeight;
    }
}
