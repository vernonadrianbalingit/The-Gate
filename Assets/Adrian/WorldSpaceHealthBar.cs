using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Runtime-generated world-space health bar that follows a TowerHealth component.
/// Designed to require no prefab setup.
/// </summary>
[ExecuteAlways]
[DisallowMultipleComponent]
public class WorldSpaceHealthBar : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private TowerHealth target;

    [Header("Layout")]
    [SerializeField] private Vector3 worldOffset = new Vector3(0f, 2.5f, 0f);
    [SerializeField] private Vector2 size = new Vector2(1.6f, 0.22f);
    [SerializeField] private float worldScale = 0.01f;
    [SerializeField] private bool faceCamera = true;
    [SerializeField] private bool debugLogs = false;
    [SerializeField] private bool autoOffsetFromRendererBounds = true;
    [SerializeField] private float extraHeightAboveRenderer = 0.35f;
    [SerializeField] private bool anchorToRendererBounds = true;

    [Header("Colors")]
    [SerializeField] private Color backgroundColor = new Color(0f, 0f, 0f, 0.6f);
    [SerializeField] private Color fillColor = new Color(0.1f, 0.9f, 0.2f, 1f);
    [SerializeField] private Color lowHealthColor = new Color(0.95f, 0.2f, 0.1f, 1f);
    [SerializeField, Range(0f, 1f)] private float lowHealthThreshold = 0.25f;

    private Canvas canvas;
    private RectTransform rootRect;
    private Image fillImage;
    private Camera cachedCam;
    private static Sprite runtimeWhiteSprite;
    private Renderer cachedRenderer;

    private void Reset()
    {
        target = GetComponent<TowerHealth>();
    }

    private void OnEnable()
    {
        if (target == null)
            target = GetComponent<TowerHealth>();

        if (cachedRenderer == null)
            cachedRenderer = GetComponentInChildren<Renderer>();

        BuildUIIfNeeded();

        // Assign a camera immediately (helps with debugging in inspector and UI behavior).
        TryAssignCanvasCamera();

        if (autoOffsetFromRendererBounds)
        {
            Renderer r = cachedRenderer != null ? cachedRenderer : GetComponentInChildren<Renderer>();
            if (r != null)
            {
                float y = r.bounds.extents.y + extraHeightAboveRenderer;
                worldOffset = new Vector3(worldOffset.x, Mathf.Max(worldOffset.y, y), worldOffset.z);
            }
        }

        if (target != null)
        {
            target.OnHealthChanged += HandleHealthChanged;
            HandleHealthChanged(target.CurrentHealth, target.MaxHealth);
        }
        else if (debugLogs)
        {
            Debug.LogWarning($"WorldSpaceHealthBar on '{gameObject.name}': No TowerHealth found.");
        }
    }

    private void OnDisable()
    {
        if (target != null)
            target.OnHealthChanged -= HandleHealthChanged;
    }

    private void LateUpdate()
    {
        if (rootRect != null)
        {
            // If the tower pivot is under the map (common with imported models),
            // anchor the health bar to the visible renderer bounds instead.
            if (anchorToRendererBounds)
            {
                if (cachedRenderer == null)
                    cachedRenderer = GetComponentInChildren<Renderer>();

                if (cachedRenderer != null)
                {
                    Bounds b = cachedRenderer.bounds;
                    rootRect.position = b.center + Vector3.up * (b.extents.y + extraHeightAboveRenderer);
                }
                else
                {
                    rootRect.position = transform.position + worldOffset;
                }
            }
            else
            {
                rootRect.position = transform.position + worldOffset;
            }
        }

        Camera cam = GetActiveCamera();
        if (cam != null && canvas != null)
        {
            // Keep Canvas "Event Camera" set (not required for rendering, but avoids confusion and supports UI interaction if needed).
            if (canvas.worldCamera != cam)
                canvas.worldCamera = cam;
        }

        if (!faceCamera)
            return;

        if (cam != null && canvas != null)
        {
            // Billboard toward camera
            // NOTE: UI is typically single-sided; we want the canvas "front" to face the camera.
            canvas.transform.rotation = Quaternion.LookRotation(cam.transform.position - canvas.transform.position, Vector3.up);
        }
    }

    private void HandleHealthChanged(float current, float max)
    {
        if (fillImage == null)
            return;

        float pct = (max <= 0f) ? 0f : Mathf.Clamp01(current / max);
        fillImage.fillAmount = pct;
        fillImage.color = (pct <= lowHealthThreshold) ? lowHealthColor : fillColor;

        if (canvas != null)
        {
            // Hide bar when dead (optional). Keeps it clean.
            canvas.gameObject.SetActive(current > 0f);
        }
    }

    private void BuildUIIfNeeded()
    {
        if (canvas != null)
            return;

        EnsureRuntimeSprite();

        // Reuse if it already exists (important in Edit Mode + domain reloads).
        Transform existing = transform.Find("TowerHealthBar");
        GameObject canvasObj = existing != null ? existing.gameObject : new GameObject("TowerHealthBar");
        canvasObj.transform.SetParent(transform, worldPositionStays: false);

        canvas = canvasObj.GetComponent<Canvas>();
        if (canvas == null)
            canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.overrideSorting = true;
        canvas.sortingOrder = 5000;

        if (canvasObj.GetComponent<CanvasScaler>() == null)
            canvasObj.AddComponent<CanvasScaler>();

        rootRect = canvas.GetComponent<RectTransform>();
        rootRect.sizeDelta = size * 100f; // rect units (pixels-ish); we scale down in world
        // Make the bar visually consistent even if the tower is scaled in the hierarchy.
        rootRect.localScale = Vector3.one * (worldScale * GetInverseUniformLossyScale(transform));
        rootRect.position = transform.position + worldOffset;

        // Background
        Transform bgT = canvasObj.transform.Find("BG");
        GameObject bgObj = bgT != null ? bgT.gameObject : new GameObject("BG");
        bgObj.transform.SetParent(canvasObj.transform, worldPositionStays: false);
        Image bg = bgObj.GetComponent<Image>();
        if (bg == null)
            bg = bgObj.AddComponent<Image>();
        bg.sprite = runtimeWhiteSprite;
        bg.color = backgroundColor;
        bg.raycastTarget = false;
        RectTransform bgRect = bg.GetComponent<RectTransform>();
        bgRect.anchorMin = new Vector2(0f, 0f);
        bgRect.anchorMax = new Vector2(1f, 1f);
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;

        // Fill
        Transform fillT = bgObj.transform.Find("Fill");
        GameObject fillObj = fillT != null ? fillT.gameObject : new GameObject("Fill");
        fillObj.transform.SetParent(bgObj.transform, worldPositionStays: false);
        fillImage = fillObj.GetComponent<Image>();
        if (fillImage == null)
            fillImage = fillObj.AddComponent<Image>();
        fillImage.sprite = runtimeWhiteSprite;
        fillImage.type = Image.Type.Filled;
        fillImage.fillMethod = Image.FillMethod.Horizontal;
        fillImage.fillOrigin = (int)Image.OriginHorizontal.Left;
        fillImage.color = fillColor;
        fillImage.raycastTarget = false;
        RectTransform fillRect = fillImage.GetComponent<RectTransform>();
        fillRect.anchorMin = new Vector2(0f, 0f);
        fillRect.anchorMax = new Vector2(1f, 1f);
        fillRect.offsetMin = new Vector2(3f, 3f);
        fillRect.offsetMax = new Vector2(-3f, -3f);

        // Ensure the healthbar uses the same layer as the tower (avoids culling mask issues on turret cameras).
        SetLayerRecursively(canvasObj, gameObject.layer);

        if (debugLogs)
        {
            Debug.Log($"WorldSpaceHealthBar: Built health bar for '{gameObject.name}' on layer {gameObject.layer}.");
        }
    }

    private static void EnsureRuntimeSprite()
    {
        if (runtimeWhiteSprite != null)
            return;

        Texture2D tex = Texture2D.whiteTexture;
        Rect rect = new Rect(0, 0, tex.width, tex.height);
        Vector2 pivot = new Vector2(0.5f, 0.5f);
        runtimeWhiteSprite = Sprite.Create(tex, rect, pivot, pixelsPerUnit: 100f);
        runtimeWhiteSprite.name = "RuntimeWhiteSprite";
    }

    private void TryAssignCanvasCamera()
    {
        if (canvas == null)
            return;
        Camera cam = GetActiveCamera();
        if (cam != null)
            canvas.worldCamera = cam;
    }

    private Camera GetActiveCamera()
    {
        Camera cam = Camera.main;
        if (cam != null)
            return cam;

        if (cachedCam == null)
            cachedCam = FindFirstObjectByType<Camera>();
        return cachedCam;
    }

    private static void SetLayerRecursively(GameObject obj, int layer)
    {
        if (obj == null) return;
        obj.layer = layer;
        foreach (Transform child in obj.transform)
        {
            if (child != null)
                SetLayerRecursively(child.gameObject, layer);
        }
    }

    private static float GetInverseUniformLossyScale(Transform t)
    {
        if (t == null)
            return 1f;

        Vector3 s = t.lossyScale;
        float maxAxis = Mathf.Max(0.0001f, Mathf.Max(Mathf.Abs(s.x), Mathf.Abs(s.y), Mathf.Abs(s.z)));
        return 1f / maxAxis;
    }
}


