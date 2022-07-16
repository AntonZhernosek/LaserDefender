using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAI : MonoBehaviour
{
    [Header("Start AI")]
    [SerializeField] bool startAIBehaviour = false;

    [Header("Attacking")]
    [SerializeField] float attackingPeriod = 6f;
    [SerializeField] float attackingPeriodRandomness = 2f;
    [SerializeField] float timeBetweenAttackCycles = 2f;
    bool shouldFollowPlayer = false;
    Coroutine currentlyAttacking;

    [Header("Movement")]
    [SerializeField] float roamMoveSpeed = 5f;
    [SerializeField] float chaseMoveSpeed = 5f;
    [SerializeField] float minMoveDistance = 2f;
    [SerializeField] Vector2 moveArea;
    bool shouldMove = true;
    Transform startPosition;
    Vector2 movement;
    Coroutine currentlyMoving;

    [Header("Item Spawner Data")]
    [SerializeField] string[] itemsToDisable;
    [SerializeField] float minPowerUpSpawnTime = 10f;
    [SerializeField] float maxPowerUpSpawnTime = 20f;

    [Header("Gizmos")]
    [SerializeField] bool drawGizmos = true;

    Rigidbody2D rb2d;
    GameObject player;
    Shooter shooter;
    BoxCollider2D col;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        rb2d = GetComponent<Rigidbody2D>();
        shooter = GetComponent<Shooter>();
        col = GetComponent<BoxCollider2D>();
    }

    private IEnumerator Start()
    {
        HandleItemSpawns();

        startAIBehaviour = false;
        shooter.EnableShooting(false);
        col.enabled = false;

        currentlyMoving = StartCoroutine(MoveToNewPosition(startPosition.position, timeBetweenAttackCycles));
        yield return currentlyMoving;

        startAIBehaviour = true;
        col.enabled = true;
    }

    private void Update()
    {
        if (!startAIBehaviour) return;

        HandleAttacking();
        HandleMovement();
    }

    public void SetStartPosition (Transform pos)
    {
        startPosition = pos;
    }

    private void HandleAttacking()
    {
        if (currentlyAttacking != null) return;
        currentlyAttacking = StartCoroutine(AttackBehaviour());
    }

    private IEnumerator AttackBehaviour()
    {
        float tempStayStore = timeBetweenAttackCycles;
        timeBetweenAttackCycles = 0f;

        shooter.EnableShooting(false);

        shooter.ChooseRandomAttack();

        shouldFollowPlayer = RandomizeShouldFollowPlayer();

        shouldMove = true;

        shooter.EnableShooting(true);

        float randomTime = RandomizeAttackTime();

        yield return new WaitForSeconds(randomTime);

        shooter.EnableShooting(false);

        timeBetweenAttackCycles = tempStayStore;

        StopMovingCoroutine();

        shouldMove = false;

        yield return new WaitForSeconds(timeBetweenAttackCycles);

        currentlyAttacking = null;
    }

    private void HandleMovement()
    {
        if (!shouldMove) return;

        if (shouldFollowPlayer)
        {
            StopMovingCoroutine();

            MoveAfterPlayer();
        }
        else if (currentlyMoving == null)
        {
            currentlyMoving = StartCoroutine(MoveToNewPosition(RandomizePosition(), timeBetweenAttackCycles));
        }
    }

    private void StopMovingCoroutine()
    {
        if (currentlyMoving != null)
        {
            StopCoroutine(currentlyMoving);
            currentlyMoving = null;
        }
    }

    private bool RandomizeShouldFollowPlayer()
    {
        int random = Random.Range(0, 2);
        if (random == 1)
        {
            return true;
        }
        return false;
    }

    private float RandomizeAttackTime()
    {
        return Random.Range(attackingPeriod - attackingPeriodRandomness, attackingPeriod + attackingPeriodRandomness);
    }

    void MoveAfterPlayer()
    {
        Vector2 targetPos = new Vector2(player.transform.position.x, transform.position.y);

        transform.position = Vector2.MoveTowards(transform.position, targetPos, chaseMoveSpeed * Time.deltaTime);
    }

    IEnumerator MoveToNewPosition(Vector2 targetPos, float waitTime)
    {
        while (Vector2.Distance(transform.position, targetPos) > 0.1f)
        {
            movement = new Vector2(0, 0);

            Vector2 relativePos = targetPos - (Vector2)transform.position;
            relativePos.Normalize();

            movement.x = relativePos.x * roamMoveSpeed * Time.deltaTime;
            movement.y = relativePos.y * roamMoveSpeed * Time.deltaTime;

            rb2d.MovePosition((Vector2)transform.position + movement);

            yield return null;
        }

        yield return new WaitForSeconds(waitTime);

        currentlyMoving = null;
    }

    Vector2 RandomizePosition()
    {
        Vector2 newPos = new Vector2();

        newPos.x = Random.Range(startPosition.position.x - moveArea.x, startPosition.position.x + moveArea.x);
        newPos.y = Random.Range(startPosition.position.y - moveArea.y, startPosition.position.y + moveArea.y);

        while (Vector2.Distance(transform.position, newPos) < minMoveDistance)
        {
            newPos.x = Random.Range(startPosition.position.x - moveArea.x, startPosition.position.x + moveArea.x);
            newPos.y = Random.Range(startPosition.position.y - moveArea.y, startPosition.position.y + moveArea.y);
        }

        return newPos;
    }

    private void HandleItemSpawns()
    {
        RandomItemSpawner spawner = FindObjectOfType<RandomItemSpawner>();
        DisableItemSpawns(spawner);
        SetPickUpSpawnTime(spawner);
    }

    private void DisableItemSpawns(RandomItemSpawner spawner)
    {
        foreach (string item in itemsToDisable)
        {
            spawner.SwitchItemSpawn(item, false);
        }
    }

    private void SetPickUpSpawnTime(RandomItemSpawner spawner)
    {
        spawner.SetSpawnTimers(minPowerUpSpawnTime, maxPowerUpSpawnTime);
    }

    private void OnDrawGizmos()
    {
        if (!drawGizmos) return;
        if (!startPosition)
        {
            Debug.Log("No starting position!");
            return;
        }

        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(startPosition.position, moveArea * 2);
    }
}
