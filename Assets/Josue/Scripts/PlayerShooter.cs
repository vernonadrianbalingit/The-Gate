using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera playerCam;
    [SerializeField] private Transform muzzle; 
    [SerializeField] private GameObject projectilePrefab;

    [Header("Weapon")]
    [SerializeField] private float fireRate = 8f; 
    [SerializeField] private float projectileDamage = 10f;
    [SerializeField] private float maxRange = 1000f;

    private float cooldown;

    private void Update()
    {
        cooldown -= Time.deltaTime;

        if (Input.GetButton("Fire1") && cooldown <= 0f)
        {
            Shoot();
            cooldown = 1f / fireRate;
        }
    }

    private void Shoot()
    {
        Vector3 origin = muzzle ? muzzle.position : playerCam.transform.position;
        Vector3 dir = playerCam.transform.forward;

        // Optional: aim-assist via raycast to center
        //if (Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out var hit, maxRange))
        //{
        //    dir = (hit.point - origin).normalized;
        //}

        var go = Instantiate(projectilePrefab, origin, Quaternion.LookRotation(dir));
        var proj = go.GetComponent<Projectile>();
        proj.Initialize(dir, projectileDamage, ownerTag: "Player");
    }
}
