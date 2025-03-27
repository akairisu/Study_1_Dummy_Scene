using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class SurroundingEnemy : MonoBehaviour
{
    [Header("Noise Movement Settings")]
    public float speed = 1f;
    public float scale = 1f;
    public float strength = 1f;

    [Header("Auto Destroy Settings")]
    public float idleLifeTime = 10f;

    [Header("Spawn/Destroy Events")]
    public UnityEvent onSpawn;
    public UnityEvent onDestroy;

    [Header("Effect Settings")]
    public AudioSource shootAudioSource;
    public AudioClip destroyAudioClip;
    public GameObject destroyParticlePrefab;

    public List<string> destroyOnHitTags = new List<string> { "player_bullet", "sword" };

    [HideInInspector]
    public MonoBehaviour manager;

    private Vector3 initialPosition;
    private float seedX, seedY, seedZ;
    private float lifeTimer = 0f;
    private bool hasInteracted = false;

    private void Awake()
    {
        seedX = Random.Range(0f, 100f);
        seedY = Random.Range(0f, 100f);
        seedZ = Random.Range(0f, 100f);
    }

    private void Update()
    {
        float time = Time.time * speed;

        float offsetX = Mathf.PerlinNoise(time + seedX, 0f) * 2f - 1f;
        float offsetY = Mathf.PerlinNoise(time + seedY, 1f) * 2f - 1f;
        float offsetZ = Mathf.PerlinNoise(time + seedZ, 2f) * 2f - 1f;

        Vector3 noiseOffset = new Vector3(offsetX, offsetY, offsetZ) * strength;
        transform.position = initialPosition + noiseOffset * scale;

        // If you want idle timeout destruction again, uncomment this:
        /*
        if (!hasInteracted)
        {
            lifeTimer += Time.deltaTime;
            if (lifeTimer >= idleLifeTime)
            {
                DestroyEnemy();
            }
        }
        */
    }

    public void Spawn()
    {
        initialPosition = transform.position;
        lifeTimer = 0f;
        hasInteracted = false;
        onSpawn?.Invoke();
    }

    public void DestroyEnemy()
    {
        onDestroy?.Invoke();

        PlayDestroyEffect(); // 🔊💥 safe effect call before recycling
        Debug.Log("play destroy");

        if (manager is ShootingTaskManager shooting)
        {
            shooting.RecycleEnemy(this);
        }
        else if (SpawnEnemyManager.Instance != null)
        {
            SpawnEnemyManager.Instance.RecycleEnemy(this);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject other = collision.gameObject;

        foreach (var tag in destroyOnHitTags)
        {
            if (other.CompareTag(tag))
            {
                hasInteracted = true;

                if (manager is ShootingTaskManager shooting)
                {
                    shooting.AddKill(1);
                }
                else if (SpawnEnemyManager.Instance != null)
                {
                    SpawnEnemyManager.Instance.AddKill(1);
                }

                DestroyEnemy();
                break;
            }
        }
    }

    public void PlayShootSound()
    {
        if (shootAudioSource != null)
        {
            shootAudioSource.Play();
        }
    }

    private void PlayDestroyEffect()
    {
        if (destroyAudioClip != null)
        {
            GameObject tempAudio = new GameObject("TempDestroySound");
            tempAudio.transform.position = transform.position;

            AudioSource a = tempAudio.AddComponent<AudioSource>();
            a.clip = destroyAudioClip;
            a.Play();

            Destroy(tempAudio, destroyAudioClip.length);
        }

        if (destroyParticlePrefab != null)
        {
            GameObject vfx = Instantiate(destroyParticlePrefab, transform.position, Quaternion.identity);
            Destroy(vfx, 2f); // particle system lifetime
        }
    }
}
