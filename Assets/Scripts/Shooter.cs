using UnityEngine;

public class Shooter : MonoBehaviour
{
    public float bulletSpeed = 10f;
    public float fireRate = 1f;
    public bool autoFire = false;

    [Header("Non-auto Fire Movement Sensitivity")]
    public float variationThreshold = 0.05f; // smaller = more strict

    public enum BulletSource { Defend, Shooting }
    public BulletSource source = BulletSource.Shooting;

    private float fireTimer = 0f;
    private Vector3 lastPosition;
    private Quaternion lastRotation;

    private void Start()
    {
        lastPosition = transform.position;
        lastRotation = transform.rotation;
    }

    private void Update()
    {
        fireTimer += Time.deltaTime;

        if (autoFire)
        {
            if (fireTimer >= 1f / fireRate)
            {
                fireTimer = 0f;
                Shoot();
            }
        }
        else
        {
            float posDelta = Vector3.Distance(transform.position, lastPosition);
            float rotDelta = Quaternion.Angle(transform.rotation, lastRotation);

            bool isSteady = posDelta < variationThreshold && rotDelta < variationThreshold;

            if (isSteady && fireTimer >= 1f / fireRate)
            {
                fireTimer = 0f;
                Shoot();
            }

            lastPosition = transform.position;
            lastRotation = transform.rotation;
        }
    }

    public void Shoot()
    {
        GameObject bulletGO = null;

        switch (source)
        {
            case BulletSource.Defend:
                bulletGO = DefendTaskManager.Instance?.GetBulletFromPool();
                break;
            case BulletSource.Shooting:
                bulletGO = ShootingTaskManager.Instance?.GetBulletFromPool();
                break;
        }

        if (bulletGO == null) return;

        bulletGO.transform.position = transform.position;
        bulletGO.transform.rotation = transform.rotation;

        Bullet bullet = bulletGO.GetComponent<Bullet>();
        if (bullet != null)
        {
            bullet.Shoot(transform.forward * bulletSpeed);
        }
    }
}
