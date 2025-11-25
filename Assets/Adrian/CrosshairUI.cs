using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Displays a crosshair in the center of the screen when controlling a turret
/// </summary>
public class CrosshairUI : MonoBehaviour
{
    [Header("Crosshair Settings")]
    [SerializeField] private bool showCrosshair = true;
    [SerializeField] private Color crosshairColor = Color.white;
    [SerializeField] private float crosshairSize = 20f;
    [SerializeField] private float crosshairThickness = 2f;
    [SerializeField] private float crosshairGap = 5f; // Gap from center

    [Header("References")]
    [SerializeField] private Canvas crosshairCanvas;
    [SerializeField] private Image crosshairImage;

    private RectTransform crosshairRect;
    private FirstPersonTurretController currentTurret;

    void Start()
    {
        // Create canvas if not assigned
        if (crosshairCanvas == null)
        {
            GameObject canvasObj = new GameObject("CrosshairCanvas");
            canvasObj.transform.SetParent(transform);
            crosshairCanvas = canvasObj.AddComponent<Canvas>();
            crosshairCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
        }

        // Create simple crosshair using 4 lines
        CreateCrosshair();
        
        // Initially hide crosshair
        SetCrosshairVisible(false);
    }

    void Update()
    {
        // Check if we're controlling a turret
        TurretSwitchManager manager = FindObjectOfType<TurretSwitchManager>();
        if (manager != null)
        {
            FirstPersonTurretController turret = manager.GetCurrentTurret();
            if (turret != null && turret != currentTurret)
            {
                currentTurret = turret;
                SetCrosshairVisible(showCrosshair);
            }
            else if (turret == null && currentTurret != null)
            {
                currentTurret = null;
                SetCrosshairVisible(false);
            }
        }
    }

    /// <summary>
    /// Shows or hides the crosshair
    /// </summary>
    public void SetCrosshairVisible(bool visible)
    {
        if (crosshairCanvas != null)
        {
            crosshairCanvas.gameObject.SetActive(visible && showCrosshair);
        }
    }

    /// <summary>
    /// Creates a simple crosshair using UI elements
    /// </summary>
    void CreateCrosshair()
    {
        if (crosshairCanvas == null)
            return;

        // Create a simple crosshair using 4 lines (top, bottom, left, right)
        // Top line
        CreateCrosshairLine("TopLine", new Vector2(0, crosshairGap + crosshairSize / 2), new Vector2(crosshairThickness, crosshairSize));
        // Bottom line
        CreateCrosshairLine("BottomLine", new Vector2(0, -crosshairGap - crosshairSize / 2), new Vector2(crosshairThickness, crosshairSize));
        // Left line
        CreateCrosshairLine("LeftLine", new Vector2(-crosshairGap - crosshairSize / 2, 0), new Vector2(crosshairSize, crosshairThickness));
        // Right line
        CreateCrosshairLine("RightLine", new Vector2(crosshairGap + crosshairSize / 2, 0), new Vector2(crosshairSize, crosshairThickness));
    }

    void CreateCrosshairLine(string name, Vector2 position, Vector2 size)
    {
        GameObject lineObj = new GameObject(name);
        lineObj.transform.SetParent(crosshairCanvas.transform, false);
        Image lineImage = lineObj.AddComponent<Image>();
        lineImage.color = crosshairColor;
        RectTransform rect = lineObj.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
    }

    /// <summary>
    /// Updates crosshair appearance
    /// </summary>
    public void UpdateCrosshairAppearance()
    {
        if (crosshairImage != null)
        {
            crosshairImage.color = crosshairColor;
        }
    }
}

