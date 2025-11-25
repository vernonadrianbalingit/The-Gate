using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles first-person control of a turret including camera rotation and shooting
/// </summary>
public class FirstPersonTurretController : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] private Camera turretCamera;
    [SerializeField] private Transform cameraPivot; // Where the camera should be positioned
    [SerializeField] private float lookSpeed = 2f;
    [SerializeField] private float lookXLimit = 45f;

    [Header("Turret References")]
    [SerializeField] private Transform rotatingHead; // The turret head that rotates horizontally
    [SerializeField] private Transform muzzle; // Where projectiles spawn
    [SerializeField] private GameObject projectilePrefab;

    [Header("Shooting Settings")]
    [SerializeField] private float fireRate = 8f;
    [SerializeField] private float projectileDamage = 10f;
    [SerializeField] private float maxRange = 1000f;

    [Header("Debug")]
    [SerializeField] private bool debugMode = false;

    private float rotationX = 0;
    private float rotationY = 0;
    private float fireCooldown = 0f;
    private bool isControlled = false;

    void Start()
    {
        // Initialize camera position if not set
        if (turretCamera != null && cameraPivot != null)
        {
            turretCamera.transform.SetParent(cameraPivot);
            turretCamera.transform.localPosition = Vector3.zero;
            turretCamera.transform.localRotation = Quaternion.identity;
        }
        else if (turretCamera != null && cameraPivot == null)
        {
            // If no camera pivot, use the turret's transform
            Debug.LogWarning($"FirstPersonTurretController on {gameObject.name}: No Camera Pivot assigned. Using turret transform.");
            cameraPivot = transform;
            turretCamera.transform.SetParent(cameraPivot);
            turretCamera.transform.localPosition = Vector3.zero;
            turretCamera.transform.localRotation = Quaternion.identity;
        }

        // Store initial rotation of rotating head
        if (rotatingHead != null)
        {
            rotationY = rotatingHead.eulerAngles.y;
        }
        else
        {
            rotationY = transform.eulerAngles.y;
        }

        // Disable camera by default (but keep the Camera component enabled so it can be activated)
        if (turretCamera != null)
        {
            turretCamera.enabled = false; // Disable Camera component, not the GameObject
            
            // Disable Audio Listener to avoid multiple listeners warning
            AudioListener audioListener = turretCamera.GetComponent<AudioListener>();
            if (audioListener != null)
            {
                audioListener.enabled = false;
            }
        }

        if (debugMode)
        {
            Debug.Log($"FirstPersonTurretController initialized on {gameObject.name}. Camera: {(turretCamera != null ? turretCamera.name : "NULL")}, RotatingHead: {(rotatingHead != null ? rotatingHead.name : "NULL")}");
        }
    }

    void Update()
    {
        if (!isControlled)
            return;

        HandleCameraRotation();
        HandleShooting();
    }

    /// <summary>
    /// Handles mouse look for first-person camera control
    /// </summary>
    private void HandleCameraRotation()
    {
        if (turretCamera == null)
        {
            if (debugMode)
                Debug.LogWarning("FirstPersonTurretController: Turret Camera is null!");
            return;
        }

        // Get mouse input
        float mouseX = Input.GetAxis("Mouse X") * lookSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * lookSpeed;

        // Rotate turret head horizontally (yaw) FIRST
        rotationY += mouseX;
        if (rotatingHead != null)
        {
            rotatingHead.rotation = Quaternion.Euler(rotatingHead.eulerAngles.x, rotationY, rotatingHead.eulerAngles.z);
        }
        else
        {
            // If no rotating head, rotate the whole turret
            transform.rotation = Quaternion.Euler(transform.eulerAngles.x, rotationY, transform.eulerAngles.z);
        }

        // Rotate camera vertically (pitch) - this is local to the camera pivot
        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
        
        // The camera should rotate horizontally with the turret head, and vertically independently
        // If camera pivot exists, rotate it horizontally, camera rotates vertically
        if (cameraPivot != null)
        {
            // Rotate the camera pivot horizontally with the turret
            cameraPivot.rotation = Quaternion.Euler(0, rotationY, 0);
            // Rotate the camera vertically (local to the pivot)
            turretCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        }
        else
        {
            // Fallback: rotate camera directly
            turretCamera.transform.rotation = Quaternion.Euler(rotationX, rotationY, 0);
        }
    }

    /// <summary>
    /// Handles shooting from the turret
    /// </summary>
    private void HandleShooting()
    {
        fireCooldown -= Time.deltaTime;

        if (Input.GetButton("Fire1") && fireCooldown <= 0f)
        {
            Shoot();
            fireCooldown = 1f / fireRate;
        }
    }

    /// <summary>
    /// Fires a projectile from the turret muzzle
    /// </summary>
    private void Shoot()
    {
        if (muzzle == null || projectilePrefab == null)
            return;

        Vector3 shootDirection = turretCamera != null ? turretCamera.transform.forward : transform.forward;
        Vector3 spawnPosition = muzzle.position;

        // Optional: Use raycast for more accurate aiming
        if (turretCamera != null)
        {
            Ray ray = new Ray(turretCamera.transform.position, turretCamera.transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, maxRange))
            {
                shootDirection = (hit.point - spawnPosition).normalized;
            }
        }

        GameObject projectile = Instantiate(projectilePrefab, spawnPosition, Quaternion.LookRotation(shootDirection));
        
        // Try to initialize projectile if it has a Projectile component
        var proj = projectile.GetComponent<Projectile>();
        if (proj != null)
        {
            proj.Initialize(shootDirection, projectileDamage, ownerTag: "Player");
        }
    }

    /// <summary>
    /// Activates this turret for player control
    /// </summary>
    public void ActivateControl()
    {
        isControlled = true;
        
        if (turretCamera == null)
        {
            Debug.LogError($"FirstPersonTurretController on {gameObject.name}: Cannot activate - Turret Camera is null! Please assign a Camera to the Turret Camera field in the Inspector.");
            isControlled = false;
            return;
        }

        // Make sure the camera GameObject is active
        if (!turretCamera.gameObject.activeInHierarchy)
        {
            turretCamera.gameObject.SetActive(true);
        }
        
        // Disable all other cameras FIRST (before enabling this one, to ensure at least one is always active)
        Camera[] allCameras = FindObjectsOfType<Camera>(true); // Include inactive cameras
        foreach (Camera cam in allCameras)
        {
            if (cam != turretCamera)
            {
                cam.enabled = false; // Disable Camera component
                
                // Disable Audio Listener on other cameras
                AudioListener otherListener = cam.GetComponent<AudioListener>();
                if (otherListener != null)
                {
                    otherListener.enabled = false;
                }
            }
        }
        
        // NOW enable this camera (ensures at least one camera is always active)
        turretCamera.enabled = true;
        turretCamera.gameObject.SetActive(true);
        
        // Enable Audio Listener on this camera
        AudioListener turretAudioListener = turretCamera.GetComponent<AudioListener>();
        if (turretAudioListener == null)
        {
            turretAudioListener = turretCamera.gameObject.AddComponent<AudioListener>();
        }
        turretAudioListener.enabled = true;
        
        // Set as main camera
        turretCamera.tag = "MainCamera";
        
        // Ensure camera is properly positioned
        if (cameraPivot != null)
        {
            turretCamera.transform.SetParent(cameraPivot);
            turretCamera.transform.localPosition = Vector3.zero;
            turretCamera.transform.localRotation = Quaternion.identity;
        }
        
        Debug.Log($"FirstPersonTurretController: Activated control on {gameObject.name}. Camera '{turretCamera.name}' is now active. Camera enabled: {turretCamera.enabled}, GameObject active: {turretCamera.gameObject.activeInHierarchy}");

        // Lock cursor for first-person control
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        // Reset rotation when taking control
        rotationX = 0;
        if (rotatingHead != null)
        {
            rotationY = rotatingHead.eulerAngles.y;
        }
        else
        {
            rotationY = transform.eulerAngles.y;
        }
    }

    /// <summary>
    /// Deactivates this turret from player control
    /// </summary>
    public void DeactivateControl()
    {
        isControlled = false;
        
        if (turretCamera != null)
        {
            turretCamera.enabled = false; // Disable Camera component, not GameObject
            
            // Disable Audio Listener
            AudioListener audioListener = turretCamera.GetComponent<AudioListener>();
            if (audioListener != null)
            {
                audioListener.enabled = false;
            }
        }

        // Unlock cursor when not controlling
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    /// <summary>
    /// Checks if this turret is currently being controlled
    /// </summary>
    public bool IsControlled()
    {
        return isControlled;
    }

    /// <summary>
    /// Gets the camera component of this turret
    /// </summary>
    public Camera GetCamera()
    {
        return turretCamera;
    }
}

