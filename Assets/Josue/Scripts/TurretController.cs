using UnityEngine;

public class TurretController : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Transform turretHead;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject projectilePrefab;

    [Header("Turret Settings")]
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float fireRate = 0.5f;
    [SerializeField] private float detectionRange = 10f;

    [Header("Projectile Settings")]
    [SerializeField] private float projectileDamage = 10f;
    // IMPORTANT: this should be the *shooter’s* tag, not the enemy’s
    [SerializeField] private string ownerTag = "Player";

    private Transform target;
    private float nextFireTime;

    void Update()
    {
        FindTarget();

        if (target != null)
        {
            AimAtTarget();
            TryFire();
        }
    }

    void FindTarget()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRange);
        Transform closestEnemy = null;
        float closestDistance = Mathf.Infinity;

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy"))               // Turret targets enemies
            {
                float distance = Vector3.Distance(transform.position, hitCollider.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = hitCollider.transform;
                }
            }
        }

        target = closestEnemy;
    }

    void AimAtTarget()
    {
        if (target == null) return;

        Vector3 dir = (target.position - turretHead.position).normalized;
        // If needed: dir.y = 0f;

        Quaternion targetRot = Quaternion.LookRotation(dir);
        turretHead.rotation = Quaternion.Slerp(
            turretHead.rotation,
            targetRot,
            rotationSpeed * Time.deltaTime
        );
    }

    void TryFire()
    {
        if (Time.time >= nextFireTime)
        {
            FireProjectile();
            nextFireTime = Time.time + fireRate;
        }
    }

    void FireProjectile()
    {
        if (target == null) return;

        Vector3 origin = firePoint.position;
        Vector3 dir = (target.position - origin).normalized;

        var go = Instantiate(projectilePrefab, origin, Quaternion.LookRotation(dir));
        var proj = go.GetComponent<Projectile>();
        if (proj != null)
        {
            // EXACTLY like PlayerShooter, but we can keep ownerTag serialized:
            proj.Initialize(dir, projectileDamage, ownerTag);
            // or hardcode: proj.Initialize(dir, projectileDamage, "Player");
        }
        else
        {
            Debug.LogError("Turret projectilePrefab has no Projectile component!");
        }
    }
}
