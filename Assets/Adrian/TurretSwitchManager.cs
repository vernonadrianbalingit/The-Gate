using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages switching between turrets by clicking on them
/// </summary>
public class TurretSwitchManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private LayerMask turretLayerMask = -1; // All layers by default
    [SerializeField] private float maxClickDistance = 1000f;
    [SerializeField] private KeyCode exitTurretKey = KeyCode.Escape;
    [SerializeField] private bool allowSwitchWhileControlling = true; // Allow switching turrets while controlling one
    [SerializeField] private bool useRightClickToSwitch = true; // Use right-click to switch when controlling a turret (left-click is for shooting)
    [SerializeField] private bool showDebugLogs = true; // Show debug messages in console

    private FirstPersonTurretController currentControlledTurret = null;
    private Camera mainCamera; // Camera used for raycasting when not in a turret
    private ClickableTurret hoveredTurret = null;
    private bool temporarilyUnlocked = false; // Track if cursor was temporarily unlocked
    private bool turretFunctionsEnabled = false; // Track if turret functions are currently enabled

    public GameObject TurretManager; // Turret manager for turret placements reference

    void Start()
    {
        // Find the main camera if not assigned
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            if (mainCamera == null)
            {
                mainCamera = FindObjectOfType<Camera>();
            }
        }

        if (mainCamera == null)
        {
            Debug.LogError("TurretSwitchManager: No camera found in scene! Make sure there's a Main Camera.");
        }
        else
        {
            // Make sure the main camera is active and enabled
            if (!mainCamera.gameObject.activeInHierarchy)
            {
                mainCamera.gameObject.SetActive(true);
            }
            mainCamera.enabled = true;
            
            // Make sure main camera has Audio Listener enabled
            AudioListener audioListener = mainCamera.GetComponent<AudioListener>();
            if (audioListener == null)
            {
                audioListener = mainCamera.gameObject.AddComponent<AudioListener>();
            }
            audioListener.enabled = true;
            
            // Disable Audio Listeners on all other cameras
            Camera[] allCameras = FindObjectsOfType<Camera>(true);
            foreach (Camera cam in allCameras)
            {
                if (cam != mainCamera)
                {
                    AudioListener otherListener = cam.GetComponent<AudioListener>();
                    if (otherListener != null)
                    {
                        otherListener.enabled = false;
                    }
                }
            }
            
            if (showDebugLogs)
            {
                Debug.Log($"TurretSwitchManager: Using camera '{mainCamera.name}' for raycasting.");
            }
        }

        // Ensure cursor is unlocked at start
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Auto-install health + healthbar + enemy contact damage on all controllable turrets
        InstallTowerSurvivabilityComponents();
    }

    void Update()
    {
        HandleTurretSwitching();
        HandleHoverHighlight();
        HandleExitTurret();
    }

    /// <summary>
    /// Handles clicking on turrets to switch control
    /// </summary>
    private void HandleTurretSwitching()
    {
        // Don't allow switching when turret functions are disabled
        if (currentControlledTurret != null && !turretFunctionsEnabled)
        {
            return;
        }
        
        // Determine which mouse button to use
        bool shouldCheckClick = false;

        if (currentControlledTurret == null)
        {
            // Not controlling a turret - use left click
            shouldCheckClick = Input.GetMouseButtonDown(0);
        }
        else if (allowSwitchWhileControlling)
        {
            // Controlling a turret - use right click to switch (left click is for shooting)
            if (useRightClickToSwitch)
            {
                shouldCheckClick = Input.GetMouseButtonDown(1); // Right click
            }
            else
            {
                // Alternative: Use a key to unlock cursor, then left-click
                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    // Temporarily unlock cursor to allow clicking
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    temporarilyUnlocked = true;
                }
                shouldCheckClick = temporarilyUnlocked && Input.GetMouseButtonDown(0);
            }
        }

        if (shouldCheckClick)
        {
            // Use the current camera (either main camera or turret camera)
            Camera cameraToUse = currentControlledTurret != null ? 
                currentControlledTurret.GetCamera() : mainCamera;

            if (cameraToUse == null)
            {
                if (showDebugLogs)
                    Debug.LogWarning("TurretSwitchManager: No camera available for raycasting!");
                return;
            }

            // When controlling a turret with locked cursor, use center of screen
            Vector3 screenPoint = Input.mousePosition;
            if (currentControlledTurret != null && Cursor.lockState == CursorLockMode.Locked)
            {
                // Use center of screen when cursor is locked
                screenPoint = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);
            }

            Ray ray = cameraToUse.ScreenPointToRay(screenPoint);
            RaycastHit hit;

            // Try raycast without layer mask first (more permissive)
            bool hitSomething = Physics.Raycast(ray, out hit, maxClickDistance);
            
            // If no hit with default, try with layer mask
            if (!hitSomething)
            {
                hitSomething = Physics.Raycast(ray, out hit, maxClickDistance, turretLayerMask);
            }

            if (hitSomething)
            {
                if (showDebugLogs)
                    Debug.Log($"TurretSwitchManager: Raycast hit '{hit.collider.name}'");

                ClickableTurret clickableTurret = hit.collider.GetComponent<ClickableTurret>();
                
                // Also check parent objects in case the collider is on a child
                if (clickableTurret == null)
                {
                    clickableTurret = hit.collider.GetComponentInParent<ClickableTurret>();
                }

                // Also check children in case the collider is on a parent
                if (clickableTurret == null)
                {
                    clickableTurret = hit.collider.GetComponentInChildren<ClickableTurret>();
                }

                if (clickableTurret != null)
                {
                    FirstPersonTurretController turretController = clickableTurret.GetTurretController();
                    if (turretController != null && turretController != currentControlledTurret)
                    {
                        if (showDebugLogs)
                            Debug.Log($"TurretSwitchManager: Switching to turret '{turretController.gameObject.name}'");
                        SwitchToTurret(turretController);
                        
                        // Cursor will be re-locked by the coroutine
                    }
                    else if (turretController == null)
                    {
                        if (showDebugLogs)
                            Debug.LogWarning($"TurretSwitchManager: ClickableTurret found on '{clickableTurret.gameObject.name}' but no FirstPersonTurretController component!");
                    }
                    else if (turretController == currentControlledTurret)
                    {
                        if (showDebugLogs)
                            Debug.Log($"TurretSwitchManager: Clicked on the same turret you're already controlling.");
                    }
                }
                else
                {
                    if (showDebugLogs)
                        Debug.Log($"TurretSwitchManager: Clicked on '{hit.collider.name}' but no ClickableTurret component found. Make sure the turret has ClickableTurret component.");
                }
            }
            else
            {
                if (showDebugLogs && shouldCheckClick)
                    Debug.Log($"TurretSwitchManager: Right-click detected but raycast didn't hit anything.");
            }
        }
    }

    /// <summary>
    /// Handles highlighting turrets when hovering over them
    /// </summary>
    private void HandleHoverHighlight()
    {
        // Don't allow highlighting when turret functions are disabled
        if (currentControlledTurret != null && !turretFunctionsEnabled)
        {
            if (hoveredTurret != null)
            {
                hoveredTurret.RemoveHighlight();
                hoveredTurret = null;
            }
            return;
        }
        
        // Always show highlights - even when controlling a turret (so you can see which turret to switch to)
        // Only hide highlights if we're using left-click switching with cursor unlock (Tab method)
        if (currentControlledTurret != null && !temporarilyUnlocked && !useRightClickToSwitch)
        {
            // Only hide highlights if we're using left-click switching (which requires cursor unlock)
            if (hoveredTurret != null)
            {
                hoveredTurret.RemoveHighlight();
                hoveredTurret = null;
            }
            return;
        }

        // Use the appropriate camera for raycasting
        Camera cameraToUse = currentControlledTurret != null ? 
            currentControlledTurret.GetCamera() : mainCamera;

        if (cameraToUse == null)
        {
            return;
        }

        // When controlling a turret, we need to use the center of the screen for raycasting
        // since the cursor is locked
        Vector3 screenPoint = Input.mousePosition;
        if (currentControlledTurret != null && Cursor.lockState == CursorLockMode.Locked)
        {
            // Use center of screen when cursor is locked
            screenPoint = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);
        }

        Ray ray = cameraToUse.ScreenPointToRay(screenPoint);
        RaycastHit hit;

        // Try raycast without layer mask first, then with layer mask
        bool hitSomething = Physics.Raycast(ray, out hit, maxClickDistance);
        if (!hitSomething)
        {
            hitSomething = Physics.Raycast(ray, out hit, maxClickDistance, turretLayerMask);
        }

        if (hitSomething)
        {
            ClickableTurret clickableTurret = hit.collider.GetComponent<ClickableTurret>();
            
            if (clickableTurret == null)
            {
                clickableTurret = hit.collider.GetComponentInParent<ClickableTurret>();
            }

            if (clickableTurret == null)
            {
                clickableTurret = hit.collider.GetComponentInChildren<ClickableTurret>();
            }

            // Don't highlight the turret we're currently controlling
            if (clickableTurret != null)
            {
                FirstPersonTurretController turretController = clickableTurret.GetTurretController();
                if (turretController == currentControlledTurret)
                {
                    // Remove highlight if hovering over current turret
                    if (hoveredTurret != null)
                    {
                        hoveredTurret.RemoveHighlight();
                        hoveredTurret = null;
                    }
                    return;
                }
            }

            if (clickableTurret != null && clickableTurret != hoveredTurret)
            {
                // Remove previous highlight
                if (hoveredTurret != null)
                {
                    hoveredTurret.RemoveHighlight();
                }

                // Highlight new turret
                hoveredTurret = clickableTurret;
                hoveredTurret.Highlight();
            }
        }
        else
        {
            // Remove highlight if not hovering over anything
            if (hoveredTurret != null)
            {
                hoveredTurret.RemoveHighlight();
                hoveredTurret = null;
            }
        }
    }

    /// <summary>
    /// Handles exiting turret control
    /// </summary>
    private void HandleExitTurret()
    {
        if (currentControlledTurret != null && Input.GetKeyDown(exitTurretKey))
        {
            ToggleTurretFunctions();
        }

        // Re-lock cursor after a short delay if temporarily unlocked (to allow clicking)
        if (temporarilyUnlocked)
        {
            // Re-lock after a frame to allow the click to register
            StartCoroutine(ReLockCursorAfterDelay(0.1f));
        }
    }

    /// <summary>
    /// Switches control to a specific turret
    /// </summary>
    public void SwitchToTurret(FirstPersonTurretController turretController)
    {
        // Exit current turret if controlling one
        if (currentControlledTurret != null)
        {
            currentControlledTurret.DeactivateControl();
        }

        // Switch to new turret
        currentControlledTurret = turretController;
        if (currentControlledTurret != null)
        {
            // Ensure this turret can take damage + show health, even if it was spawned after Start().
            EnsureTowerSurvivabilityOn(currentControlledTurret.gameObject);

            currentControlledTurret.ActivateControl();
            turretFunctionsEnabled = true;
            
            // Update main camera reference to the turret's camera
            Camera turretCam = currentControlledTurret.GetCamera();
            if (turretCam != null)
            {
                mainCamera = turretCam;
            }
        }

        // Remove hover highlight
        if (hoveredTurret != null)
        {
            hoveredTurret.RemoveHighlight();
            hoveredTurret = null;
        }
    }

    /// <summary>
    /// Toggles turret functions on/off
    /// </summary>
    private void ToggleTurretFunctions()
    {
        if (currentControlledTurret != null)
        {
            turretFunctionsEnabled = !turretFunctionsEnabled;
            
            if (turretFunctionsEnabled)
            {
                // Disable Grabber script when turret functions are enabled
                if (TurretManager != null)
                {
                    Grabber grabber = TurretManager.GetComponent<Grabber>();
                    if (grabber != null)
                    {
                        grabber.enabled = false;
                        if (showDebugLogs)
                            Debug.Log("TurretSwitchManager: Grabber script disabled");
                    }
                }
                
                // Re-enable turret functions
                currentControlledTurret.ActivateControl();
                if (showDebugLogs)
                    Debug.Log("TurretSwitchManager: Turret functions enabled");
            }
            else
            {
                // Disable turret functions but keep camera enabled
                currentControlledTurret.DeactivateControl();
                
                // Re-enable the camera so it stays active
                Camera turretCam = currentControlledTurret.GetCamera();
                if (turretCam != null)
                {
                    turretCam.enabled = true;
                }
                
                // Enable Grabber script when turret functions are disabled
                if (TurretManager != null)
                {
                    Grabber grabber = TurretManager.GetComponent<Grabber>();
                    if (grabber != null)
                    {
                        grabber.enabled = true;
                        if (showDebugLogs)
                            Debug.Log("TurretSwitchManager: Grabber script enabled");
                    }
                }
                
                // Unlock cursor
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                if (showDebugLogs)
                    Debug.Log("TurretSwitchManager: Turret functions disabled (camera still active)");
            }
        }
    }

    /// <summary>
    /// Exits the current turret control
    /// </summary>
    public void ExitTurret()
    {
        if (currentControlledTurret != null)
        {
            currentControlledTurret.DeactivateControl();
            currentControlledTurret = null;
            turretFunctionsEnabled = false;
        }

        // Unlock cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    /// <summary>
    /// Gets the currently controlled turret
    /// </summary>
    public FirstPersonTurretController GetCurrentTurret()
    {
        return currentControlledTurret;
    }

    /// <summary>
    /// Checks if the player is currently controlling a turret
    /// </summary>
    public bool IsControllingTurret()
    {
        return currentControlledTurret != null;
    }

    /// <summary>
    /// Coroutine to re-lock cursor after a delay
    /// </summary>
    private IEnumerator ReLockCursorAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        if (currentControlledTurret != null && temporarilyUnlocked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            temporarilyUnlocked = false;
        }
    }

    /// <summary>
    /// Ensures all controllable turrets can take damage (bees) and display a health bar.
    /// This avoids requiring prefab/scene edits in a shared project.
    /// </summary>
    private void InstallTowerSurvivabilityComponents()
    {
        int installedCount = 0;

        // Primary: controllable turrets
        FirstPersonTurretController[] turrets = FindObjectsOfType<FirstPersonTurretController>(true);
        if (turrets != null)
        {
            foreach (FirstPersonTurretController turret in turrets)
            {
                if (turret == null)
                    continue;
                installedCount += EnsureTowerSurvivabilityOn(turret.gameObject);
            }
        }

        // Secondary: any clickable turret roots (covers edge cases where controller is on a child/parent, or turrets are swapped)
        ClickableTurret[] clickables = FindObjectsOfType<ClickableTurret>(true);
        if (clickables != null)
        {
            foreach (ClickableTurret clickable in clickables)
            {
                if (clickable == null)
                    continue;
                installedCount += EnsureTowerSurvivabilityOn(clickable.gameObject);
            }
        }

        if (showDebugLogs)
        {
            int turretCount = turrets != null ? turrets.Length : 0;
            int clickableCount = clickables != null ? clickables.Length : 0;
            Debug.Log($"TurretSwitchManager: Survivability scan complete. Found controllers={turretCount}, clickables={clickableCount}. Components added this run: {installedCount}");
        }
    }

    private int EnsureTowerSurvivabilityOn(GameObject go)
    {
        if (go == null)
            return 0;

        int added = 0;

        // Add in order: health first, then dependents (their Awake will auto-hook TowerHealth).
        if (go.GetComponent<TowerHealth>() == null)
        {
            go.AddComponent<TowerHealth>();
            added++;
        }

        if (go.GetComponent<WorldSpaceHealthBar>() == null)
        {
            go.AddComponent<WorldSpaceHealthBar>();
            added++;
        }

        TowerEnemyContactDamage dmg = go.GetComponent<TowerEnemyContactDamage>();
        if (dmg == null)
        {
            dmg = go.AddComponent<TowerEnemyContactDamage>();
            added++;
        }

        // Ensure it points at the root TowerHealth (so it still works if colliders are on child objects).
        TowerHealth health = go.GetComponent<TowerHealth>();
        if (health != null && dmg != null)
            dmg.SetTowerHealth(health);

        return added;
    }
}

