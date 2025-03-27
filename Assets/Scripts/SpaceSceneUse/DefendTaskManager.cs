using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class DefendTaskManager : MonoBehaviour
{
    public static DefendTaskManager Instance;

    [Header("Enemy Spawn Settings")]
    public GameObject enemyPrefab;
    public Transform spawnOrigin;
    public int numberOfEnemies = 5;
    public float spawnRadius = 10f;

    [Header("Bullet Settings")]
    public GameObject bulletPrefab;
    public Transform playerTarget;
    public float bulletSpeed = 10f;
    public float bulletSpawnInterval = 1f;
    public int bulletPoolSize = 20;

    [Header("Kill Threshold Settings")]
    public int killThreshold = 5;
    public List<UnityEvent> onStart;
    public List<UnityEvent> onThresholdReached;
    [SerializeField] private float bulletSpawnOffset = 1f; // distance in front of the shooter
    private int killCounter = 0;
    private bool thresholdTriggered = false;

    private List<SurroundingEnemy> activeEnemies = new List<SurroundingEnemy>();
    private List<Bullet> activeBullets = new List<Bullet>();
    private Queue<Bullet> bulletPool = new Queue<Bullet>();

    private float bulletTimer = 0f;

    private void Awake()
    {
        Instance = this;
        killCounter = 0;
        thresholdTriggered = false;
        StartTask();
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
        bulletTimer += Time.deltaTime;
        if (bulletTimer >= bulletSpawnInterval)
        {
            bulletTimer = 0f;
            SpawnBullet();
        }
    }

    public void StartTask()
    {
        SpawnEnemies();
        bulletTimer = 0f;
        foreach (var evt in onStart)
        {
            evt?.Invoke();
        }
    }

    private void SpawnEnemies()
    {
        for (int i = 0; i < numberOfEnemies; i++)
        {
            Vector3 randomOffset = Random.insideUnitSphere * spawnRadius;
            Vector3 spawnPos = spawnOrigin.position + randomOffset;

            GameObject enemyGO = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
            SurroundingEnemy enemy = enemyGO.GetComponent<SurroundingEnemy>();

            if (playerTarget != null)
            {
                enemy.transform.LookAt(playerTarget.position);
            }

            enemy.manager = this; // 🔁 Tell the enemy who to report to
            enemy.Spawn();
            activeEnemies.Add(enemy);
        }
    }

    private void SpawnBullet()
    {
        if (playerTarget == null || bulletPool.Count == 0 || activeEnemies.Count == 0) return;

        int index = Random.Range(0, activeEnemies.Count);
        SurroundingEnemy shooter = activeEnemies[index];

        // Play shoot sound
        shooter.PlayShootSound();

        Bullet bullet = bulletPool.Dequeue();
        GameObject bulletGO = bullet.gameObject;

        bulletGO.SetActive(true);

        Vector3 direction = (playerTarget.position - shooter.transform.position).normalized;
        Vector3 spawnPos = shooter.transform.position + direction * bulletSpawnOffset;

        bulletGO.transform.position = spawnPos;
        bulletGO.transform.forward = direction;

        bullet.Shoot(direction * bulletSpeed);
        activeBullets.Add(bullet);
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
            {
                enemy.DestroyEnemy();
            }
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

    public void AddKill(int value)
    {
        killCounter += value;
        Debug.Log($"[DefendTaskManager] Kills: {killCounter}");

        if (!thresholdTriggered && killCounter >= killThreshold)
        {
            thresholdTriggered = true;
            Debug.Log("[DefendTaskManager] Kill threshold reached!");

            foreach (var evt in onThresholdReached)
            {
                evt?.Invoke();
            }
        }
    }
}
