using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomItemSpawner : MonoBehaviour
{
    [System.Serializable]
    class SpawnedItem
    {
        public string name;
        public bool shouldSpawn;
        public GameObject item;
        public int itemSpawnWeight;

        public void SetShouldSpawn(bool state)
        {
            this.shouldSpawn = state;
        }
    }

    [Header("Spawning Enabled")]
    [SerializeField] bool enableSpawning = true;
    
    [Header("Items List")]
    [SerializeField] List<SpawnedItem> randomItems = new List<SpawnedItem>();

    [Header("Spawn Area")]
    [SerializeField] bool enableGizmos = true;
    [SerializeField] Vector2 spawnArea;

    [Header("Timers")]
    [SerializeField] float minSpawnTimer = 4f;
    [SerializeField] float maxSpawnTimer = 10f;

    Coroutine itemSpawning;

    private void Start()
    {
        if (!enableSpawning || randomItems.Count == 0) return;
        itemSpawning = StartCoroutine(SpawnNewItem());
    }

    public void SwitchItemSpawn (string name, bool state)
    {
        foreach (SpawnedItem item in randomItems)
        {
            if (string.Equals(name, item.name))
            {
                item.SetShouldSpawn(state);
            }
        }

        ResetItemSpawning();
    }

    public void SetSpawnTimers(float minTime, float maxTime)
    {
        minSpawnTimer = minTime;
        maxSpawnTimer = maxTime;
    }

    IEnumerator SpawnNewItem()
    {
        yield return SpawnItemCooldown();

        float pickedWeight = CalculateTotalWeight() * Random.value;

        int chosenIndex = GetFirstSpawnableIndex();

        if (chosenIndex < 0)
        {
            ResetItemSpawning();
            yield return null;
        }

        int cumulativeWeight = randomItems[chosenIndex].itemSpawnWeight;

        while (pickedWeight > cumulativeWeight)
        {
            if (chosenIndex < randomItems.Count)
            {
                chosenIndex++;
                if (!randomItems[chosenIndex].shouldSpawn) continue;
                cumulativeWeight += randomItems[chosenIndex].itemSpawnWeight;
            }
        }
        if (randomItems[chosenIndex].item)
        {
            GameObject pickupInstance = Instantiate(randomItems[chosenIndex].item, transform);
            pickupInstance.transform.localPosition = RandomizeSpawnPosition();
        }

        ResetItemSpawning();
    }

    IEnumerator SpawnItemCooldown()
    {
        float randomTime = Random.Range(minSpawnTimer, maxSpawnTimer);
        yield return new WaitForSeconds(randomTime);
    }

    private int GetFirstSpawnableIndex()
    {
        foreach (SpawnedItem item in randomItems)
        {
            if (!item.shouldSpawn) continue;
            return randomItems.IndexOf(item);
        }
        return -1;
    }

    Vector2 RandomizeSpawnPosition()
    {
        Vector2 newPos = new Vector2();
        newPos.x = Random.Range(-spawnArea.x / 2, spawnArea.x / 2);
        newPos.y = 0;

        return newPos;
    }

    void ResetItemSpawning()
    {
        if (!enableSpawning) return;

        if (itemSpawning != null)
        {
            StopCoroutine(itemSpawning);
            itemSpawning = null;
        }

        itemSpawning = StartCoroutine(SpawnNewItem());
    }
    private float CalculateTotalWeight()
    {
        float totalItemWeight = 0f;
        foreach (SpawnedItem item in randomItems)
        {
            if (!item.shouldSpawn) continue;
            totalItemWeight += item.itemSpawnWeight;
        }
        return totalItemWeight;
    }

    private void OnDrawGizmos()
    {
        if (!enableGizmos) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, spawnArea);
    }
}
