using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SniperTurretController : MonoBehaviour
{

    [Header("Refs")]
    [SerializeField] private Transform turretHead;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject projectilePrefab;

    [Header("Turret Settings")]
    [SerializeField] private float rotationSpeed = 8f;
    [SerializeField] private float detectionRange = 30f;

    [Header("Sniper Fire Settings")]
    [SerializeField] private int shots = 2;
    [SerializeField] private float timeBetweenShots = .2f;
    [SerializeField] private float cooldownBetweenBursts = 2.4f;

    [Header("Projectile Settings")]
    [SerializeField] private float projectileDamage = 5f;
    [SerializeField] private string ownerTag = "Turret";

    private Transform target;
    private float nextBurstTime;
    private bool isFiringBurst;

    private void Update()
    {
        FindTarget();

        if (target != null)
        {
            AimAtTarget();

            if (!isFiringBurst && Time.time >= nextBurstTime)
            {
                StartCoroutine(Fire());
            }
        }
    }

    private void FindTarget()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRange);
        Transform closestEnemy = null;
        float closestDistance = Mathf.Infinity;

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy"))
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

    private void AimAtTarget()
    {
        if (target == null || turretHead == null) return;

        Vector3 dir = (target.position - turretHead.position).normalized;

        Quaternion targetRot = Quaternion.LookRotation(dir);
        turretHead.rotation = Quaternion.Slerp(
            turretHead.rotation,
            targetRot,
            rotationSpeed * Time.deltaTime
        );
    }

    private IEnumerator Fire()
    {
        if (target == null || projectilePrefab == null || firePoint == null)
            yield break;

        isFiringBurst = true;

        for (int i = 0; i < shots; i++)
        {
            if (target == null) break; // in case the target dies mid-burst

            FireSingleShot();

            if (i < shots - 1)
                yield return new WaitForSeconds(timeBetweenShots);
        }

        nextBurstTime = Time.time + cooldownBetweenBursts;
        isFiringBurst = false;
    }

    private void FireSingleShot()
    {
        Vector3 origin = firePoint.position;
        Vector3 dir = (target.position - origin).normalized;

        GameObject go = Instantiate(projectilePrefab, origin, Quaternion.LookRotation(dir));
        Projectile proj = go.GetComponent<Projectile>();

        if (proj != null)
        {
            proj.Initialize(dir, projectileDamage, ownerTag);
        }
        else
        {
            Debug.LogError("SniperTurret projectilePrefab has no Projectile component!");
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }

}

