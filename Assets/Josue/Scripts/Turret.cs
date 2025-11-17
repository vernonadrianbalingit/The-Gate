using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
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
    [SerializeField] private float maxDistance = 1000f;     // how far to raycast for aiming

    private float fireCooldown;

    private void Update()
    {
        fireCooldown -= Time.deltaTime;

        if (playerCamera == null || rotatingHead == null || muzzle == null)
            return;

        // STEP 1: Find where the camera is looking
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

        // STEP 4: Check if the turret’s muzzle is roughly aimed
        float angle = Vector3.Angle(muzzle.forward, toAim);
        if (angle <= fireToleranceAngle && fireCooldown <= 0f)
        {
            Fire(toAim);
            fireCooldown = 1f / fireRate;
        }
    }

    private void Fire(Vector3 dir)
    {
        var go = Instantiate(projectilePrefab, muzzle.position, Quaternion.LookRotation(dir));
        var proj = go.GetComponent<Projectile>();
        proj.Initialize(dir, projectileDamage, ownerTag: "Turret");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(muzzle.position, muzzle.forward * 3f);
    }
}