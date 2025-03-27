using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class SpawnEnemyManager : MonoBehaviour
{
    [Header("Spawn Settings")]
    public Transform spawnOrigin;
    public GameObject enemyPrefab;
    public float spawnInterval = 2f;
    public int maxEnemies = 10;
    public float spawnRadius = 5f;

    [Header("Kill Threshold Settings")]
    public int killThreshold = 5;
    public List<UnityEvent> onStart;
    public List<UnityEvent> onThresholdReached;

    private int killCounter = 0;
    private bool thresholdTriggered = false;

    private float spawnTimer = 0f;
    private List<SurroundingEnemy> activeEnemies = new List<SurroundingEnemy>();
    private Queue<SurroundingEnemy> enemyPool = new Queue<SurroundingEnemy>();

    public static SpawnEnemyManager Instance;

    private void Awake()
    {
        Instance = this;
        killCounter = 0;
        thresholdTriggered = false;
        foreach (var evt in onStart)
        {
            evt?.Invoke();
        }
    }

    private void Update()
    {
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnInterval && activeEnemies.Count < maxEnemies)
        {
            spawnTimer = 0f;
            SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {
        SurroundingEnemy enemy;

        if (enemyPool.Count > 0)
        {
            enemy = enemyPool.Dequeue();
            enemy.gameObject.SetActive(true);
        }
        else
        {
            GameObject newObj = Instantiate(enemyPrefab);
            enemy = newObj.GetComponent<SurroundingEnemy>();
        }

        // Generate a random point inside the upper hemisphere
        Vector3 randomOffset = Random.insideUnitSphere * spawnRadius;
        randomOffset.y = Mathf.Abs(randomOffset.y); // Upper hemisphere only

        Vector3 spawnPos = spawnOrigin.position + randomOffset;

        //Debug.Log($"Spawning enemy at {spawnPos}");

        enemy.transform.position = spawnPos;
        enemy.transform.LookAt(spawnOrigin.position);
        enemy.manager = this; // Let the enemy report back
        enemy.Spawn();
        activeEnemies.Add(enemy);
    }

    public void RecycleEnemy(SurroundingEnemy enemy)
    {
        if (activeEnemies.Contains(enemy))
        {
            activeEnemies.Remove(enemy);
            enemy.gameObject.SetActive(false);
            enemyPool.Enqueue(enemy);
        }
    }

    public List<SurroundingEnemy> GetActiveEnemies()
    {
        return activeEnemies;
    }

    public void KillAllEnemies()
    {
        Debug.Log("Killing all enemies...");

        var enemiesCopy = new List<SurroundingEnemy>(activeEnemies);

        foreach (var enemy in enemiesCopy)
        {
            if (enemy != null)
            {
                enemy.DestroyEnemy();
            }
        }
    }

    public void AddKill(int value)
    {
        killCounter += value;
        Debug.Log($"[SpawnEnemyManager] Kills: {killCounter}");

        if (!thresholdTriggered && killCounter >= killThreshold)
        {
            thresholdTriggered = true;

            Debug.Log("[SpawnEnemyManager] Kill threshold reached!");

            foreach (var evt in onThresholdReached)
            {
                evt?.Invoke();
            }
        }
    }
}
