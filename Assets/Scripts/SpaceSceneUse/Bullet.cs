using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    private Rigidbody rb;

    [SerializeField] private float lifeTime = 5f;

    [Header("Valid Hit Tags")]
    public List<string> validHitTags = new List<string>() { "shield" };

    [Header("Hit Effects")]
    public AudioClip hitSound;
    public GameObject hitParticlePrefab;

    private MonoBehaviour manager;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Initialize(MonoBehaviour assignedManager)
    {
        manager = assignedManager;
    }

    public void Shoot(Vector3 velocity)
    {
        rb.velocity = velocity;
        CancelInvoke(nameof(ReturnToPool));
        Invoke(nameof(ReturnToPool), lifeTime);
    }

    public void Stop()
    {
        rb.velocity = Vector3.zero;
        CancelInvoke(nameof(ReturnToPool));
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject other = collision.gameObject;

        foreach (var tag in validHitTags)
        {
            if (other.CompareTag(tag))
            {
                Debug.Log("bullet Collide  " + tag);

                PlayHitEffect();

                if (manager is DefendTaskManager defend)
                {
                    defend.AddKill(1);
                    defend.RecycleBullet(this);
                }
                else if (manager is ShootingTaskManager shooting)
                {
                    shooting.RecycleBullet(this);
                }

                return;
            }
        }

        // No valid hit tag: just return to pool, no kill
        ReturnToPool();
    }

    private void ReturnToPool()
    {
        Stop();
        PlayHitEffect();

        if (manager is DefendTaskManager defend)
        {
            defend.RecycleBullet(this);
        }
        else if (manager is ShootingTaskManager shooting)
        {
            shooting.RecycleBullet(this);
        }
    }

    private void PlayHitEffect()
    {
        if (hitSound != null)
        {
            GameObject soundObj = new GameObject("TempBulletSound");
            soundObj.transform.position = transform.position;

            AudioSource audio = soundObj.AddComponent<AudioSource>();
            audio.clip = hitSound;
            audio.Play();

            Destroy(soundObj, hitSound.length);
        }

        if (hitParticlePrefab != null)
        {
            GameObject vfx = Instantiate(hitParticlePrefab, transform.position, Quaternion.identity);
            Destroy(vfx, 2f); // Adjust lifetime as needed
        }
    }
}
