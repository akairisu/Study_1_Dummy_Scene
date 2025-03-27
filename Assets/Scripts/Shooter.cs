using UnityEngine;

public class Shooter : MonoBehaviour
{
    public float bulletSpeed = 10f;
    public float fireRate = 1f;
    public bool autoFire = false;

    public enum BulletSource { Defend, Shooting }
    public BulletSource source = BulletSource.Shooting;

    private float fireTimer = 0f;

    private void Update()
    {
        if (!autoFire) return;

        fireTimer += Time.deltaTime;
        if (fireTimer >= 1f / fireRate)
        {
            fireTimer = 0f;
            Shoot();
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
