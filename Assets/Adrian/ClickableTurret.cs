using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Makes a turret clickable and provides visual feedback
/// </summary>
public class ClickableTurret : MonoBehaviour
{
    [Header("Visual Feedback")]
    [SerializeField] private Material highlightMaterial;
    [SerializeField] private Renderer[] renderersToHighlight;
    [SerializeField] private Color highlightColor = Color.yellow;
    [SerializeField] private float highlightIntensity = 1.5f;
    [SerializeField] private bool autoAddCollider = true; // Automatically add a BoxCollider if none exists

    private Material[] originalMaterials;
    private FirstPersonTurretController turretController;
    private bool isHighlighted = false;

    void Start()
    {
        turretController = GetComponent<FirstPersonTurretController>();
        
        // Check for collider and add one if needed
        Collider col = GetComponent<Collider>();
        if (col == null)
        {
            if (autoAddCollider)
            {
                // Try to add a BoxCollider that fits the mesh
                BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
                
                // Try to get bounds from renderer
                Renderer renderer = GetComponent<Renderer>();
                if (renderer != null)
                {
                    // Calculate center and size in local space
                    Bounds bounds = renderer.bounds;
                    boxCollider.center = transform.InverseTransformPoint(bounds.center);
                    boxCollider.size = bounds.size;
                }
                
                Debug.Log($"ClickableTurret: Automatically added BoxCollider to {gameObject.name}");
            }
            else
            {
                Debug.LogWarning($"ClickableTurret: No Collider found on {gameObject.name}. The turret won't be clickable! Add a Collider component or enable 'Auto Add Collider'.");
            }
        }
        
        // Store original materials if renderers are specified
        if (renderersToHighlight != null && renderersToHighlight.Length > 0)
        {
            originalMaterials = new Material[renderersToHighlight.Length];
            for (int i = 0; i < renderersToHighlight.Length; i++)
            {
                if (renderersToHighlight[i] != null)
                {
                    originalMaterials[i] = renderersToHighlight[i].material;
                }
            }
        }
    }

    /// <summary>
    /// Highlights the turret to show it's clickable
    /// </summary>
    public void Highlight()
    {
        if (isHighlighted)
            return;

        isHighlighted = true;

        // If no renderers specified, try to find them automatically
        if (renderersToHighlight == null || renderersToHighlight.Length == 0)
        {
            renderersToHighlight = GetComponentsInChildren<Renderer>();
        }

        if (renderersToHighlight != null && renderersToHighlight.Length > 0)
        {
            // Store original materials if not already stored
            if (originalMaterials == null || originalMaterials.Length != renderersToHighlight.Length)
            {
                originalMaterials = new Material[renderersToHighlight.Length];
            }

            for (int i = 0; i < renderersToHighlight.Length; i++)
            {
                if (renderersToHighlight[i] != null)
                {
                    // Store original material if not already stored
                    if (originalMaterials[i] == null)
                    {
                        originalMaterials[i] = renderersToHighlight[i].material;
                    }

                    // Create a highlight material
                    Material highlightMat = new Material(renderersToHighlight[i].material);
                    highlightMat.color = highlightColor * highlightIntensity;
                    highlightMat.EnableKeyword("_EMISSION");
                    highlightMat.SetColor("_EmissionColor", highlightColor * highlightIntensity);
                    renderersToHighlight[i].material = highlightMat;
                }
            }
        }
        else
        {
            // Fallback: Use a simple outline effect or just log
            Debug.Log($"ClickableTurret on {gameObject.name}: No renderers found to highlight. Add renderers to 'Renderers To Highlight' array.");
        }
    }

    /// <summary>
    /// Removes highlight from the turret
    /// </summary>
    public void RemoveHighlight()
    {
        if (!isHighlighted)
            return;

        isHighlighted = false;

        if (renderersToHighlight != null && originalMaterials != null)
        {
            for (int i = 0; i < renderersToHighlight.Length && i < originalMaterials.Length; i++)
            {
                if (renderersToHighlight[i] != null && originalMaterials[i] != null)
                {
                    renderersToHighlight[i].material = originalMaterials[i];
                }
            }
        }
    }

    /// <summary>
    /// Gets the FirstPersonTurretController component
    /// </summary>
    public FirstPersonTurretController GetTurretController()
    {
        return turretController;
    }
}

