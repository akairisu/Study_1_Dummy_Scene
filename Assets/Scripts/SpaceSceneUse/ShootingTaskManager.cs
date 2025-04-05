using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class ShootingTaskManager : MonoBehaviour
{
    public static ShootingTaskManager Instance;

    [Header("Enemy Spawn Settings")]
    public GameObject enemyPrefab;
    public Transform spawnOrigin;
    public float spawnRadius = 10f;
    public float spawnInterval = 2f;

    [Header("Bullet Settings")]
    public GameObject bulletPrefab;
    public int bulletPoolSize = 20;

    [Header("Enemy Limit")]
    public int maxEnemies = 10;


    [Header("Kill Threshold Settings")]
    public int killThreshold = 5;
    public List<UnityEvent> onThresholdReached;
    public List<UnityEvent> onStart;

    private int killCounter = 0;
    private bool thresholdTriggered = false;

    private List<SurroundingEnemy> activeEnemies = new List<SurroundingEnemy>();
    private Queue<SurroundingEnemy> enemyPool = new Queue<SurroundingEnemy>();

    private List<Bullet> activeBullets = new List<Bullet>();
    private Queue<Bullet> bulletPool = new Queue<Bullet>();

    private float spawnTimer = 0f;

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

    private void Start()
    {
        for (int i = 0; i < bulletPoolSize; i++)
        {
            GameObject bulletObj = Instantiate(bulletPrefab, transform);
            Bullet bullet = bulletObj.GetComponent<Bullet>();
            bullet.Initialize(this);
            bulletObj.SetActive(false);
            bulletPool.Enqueue(bullet);
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
            GameObject enemyGO = Instantiate(enemyPrefab);
            enemy = enemyGO.GetComponent<SurroundingEnemy>();
        }

        Vector3 offset = Random.insideUnitSphere * spawnRadius;
        Vector3 spawnPos = spawnOrigin.position + offset;

        enemy.transform.position = spawnPos;
        enemy.transform.LookAt(spawnOrigin.position);
        enemy.manager = this;
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

    public GameObject GetBulletFromPool()
    {
        if (bulletPool.Count > 0)
        {
            Bullet bullet = bulletPool.Dequeue();
            bullet.gameObject.SetActive(true);
            activeBullets.Add(bullet);
            return bullet.gameObject;
        }
        return null;
    }

    public void RecycleBullet(Bullet bullet)
    {
        if (activeBullets.Contains(bullet))
        {
            activeBullets.Remove(bullet);
            bullet.gameObject.SetActive(false);
            bulletPool.Enqueue(bullet);
        }
    }

    public void KillAll()
    {
        foreach (var enemy in activeEnemies)
        {
            if (enemy != null)
                enemy.DestroyEnemy();
        }
        activeEnemies.Clear();

        foreach (var bullet in activeBullets)
        {
            if (bullet != null)
            {
                bullet.Stop();
                bullet.gameObject.SetActive(false);
                bulletPool.Enqueue(bullet);
            }
        }
        activeBullets.Clear();
    }

    public void AddKill(int value)
    {
        killCounter += value;
        Debug.Log($"[ShootingTaskManager] Kills: {killCounter}");

        if (!thresholdTriggered && killCounter >= killThreshold)
        {
            thresholdTriggered = true;
            Debug.Log("[ShootingTaskManager] Kill threshold reached!");

            foreach (var evt in onThresholdReached)
            {
                evt?.Invoke();
            }
        }
    }
}
