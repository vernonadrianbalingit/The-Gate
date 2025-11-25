using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
private float cooldown;
[Header("Refs")]
    [SerializeField] private Transform rotatingHead;    // rotates to face camera direction
    [SerializeField] private Transform muzzle;          // projectile spawn
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Camera playerCamera;       // <--- NEW: reference to player camera

    [Header("Gun Settings")]
    [SerializeField] private float headTurnSpeed = 360f;  // deg/sec
    [SerializeField] private float fireRate = 3f;
    [SerializeField] private float projectileDamage = 10f;
    [SerializeField] private float fireToleranceAngle = 3f; // degrees
    [SerializeField] private float maxDistance = 1000f;

    private void Update()
    {
        cooldown -= Time.deltaTime;


        Vector3 aimPoint;
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance))
            aimPoint = hit.point;
        else
            aimPoint = playerCamera.transform.position + playerCamera.transform.forward * maxDistance;

        // STEP 2: Compute aiming direction from muzzle to aimPoint
        Vector3 toAim = (aimPoint - muzzle.position).normalized;

        // STEP 3: Rotate turret head smoothly toward that direction
        Quaternion targetRot = Quaternion.LookRotation(new Vector3(toAim.x, 0f, toAim.z)); // yaw only
        rotatingHead.rotation = Quaternion.RotateTowards(rotatingHead.rotation, targetRot, headTurnSpeed * Time.deltaTime);


        if (Input.GetButton("Fire1") && cooldown <= 0f)
        {
            Shoot();
            cooldown = 1f / fireRate;
        }
    }

    private void Shoot()
    {
        // Determine origin and direction
        Vector3 origin = muzzle ? muzzle.position : playerCamera.transform.position;
        Vector3 dir = playerCamera.transform.forward;

        // Raycast to find exact hit point/
        var go = Instantiate(projectilePrefab, origin, Quaternion.LookRotation(dir));
        var proj = go.GetComponent<Projectile>();
        proj.Initialize(dir, projectileDamage, ownerTag: "Player");
    }
}
