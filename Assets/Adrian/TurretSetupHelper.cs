using UnityEngine;

/// <summary>
/// Helper script to debug and verify turret setup
/// </summary>
public class TurretSetupHelper : MonoBehaviour
{
    [ContextMenu("Check Turret Setup")]
    public void CheckTurretSetup()
    {
        FirstPersonTurretController controller = GetComponent<FirstPersonTurretController>();
        ClickableTurret clickable = GetComponent<ClickableTurret>();
        Collider col = GetComponent<Collider>();

        Debug.Log("=== Turret Setup Check for: " + gameObject.name + " ===");
        
        if (controller == null)
        {
            Debug.LogError("❌ FirstPersonTurretController component is missing!");
        }
        else
        {
            Debug.Log("✓ FirstPersonTurretController found");
            
            var camera = controller.GetCamera();
            if (camera == null)
            {
                Debug.LogError("❌ Turret Camera is not assigned!");
            }
            else
            {
                Debug.Log("✓ Turret Camera assigned: " + camera.name);
            }

            // Use reflection to check private fields
            var rotatingHeadField = typeof(FirstPersonTurretController).GetField("rotatingHead", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (rotatingHeadField != null)
            {
                Transform rotatingHead = rotatingHeadField.GetValue(controller) as Transform;
                if (rotatingHead == null)
                {
                    Debug.LogError("❌ Rotating Head is not assigned!");
                }
                else
                {
                    Debug.Log("✓ Rotating Head assigned: " + rotatingHead.name);
                }
            }
        }

        if (clickable == null)
        {
            Debug.LogError("❌ ClickableTurret component is missing!");
        }
        else
        {
            Debug.Log("✓ ClickableTurret found");
        }

        if (col == null)
        {
            Debug.LogError("❌ No Collider found! Turret won't be clickable.");
        }
        else
        {
            Debug.Log("✓ Collider found: " + col.GetType().Name);
        }

        // Check for TurretSwitchManager in scene
        TurretSwitchManager manager = FindObjectOfType<TurretSwitchManager>();
        if (manager == null)
        {
            Debug.LogError("❌ TurretSwitchManager not found in scene! Create an empty GameObject and add TurretSwitchManager component.");
        }
        else
        {
            Debug.Log("✓ TurretSwitchManager found in scene");
        }
    }
}

