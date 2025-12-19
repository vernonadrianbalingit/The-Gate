using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class WorldSpaceHealthBar : MonoBehaviour
{
    public Vector3 offset = new Vector3(0f, 5.5f, 0f);
    public Vector2 size = new Vector2(1.6f, 0.22f);
    public Color fillColor = new Color(0.1f, 0.9f, 0.2f);
    public Color lowHealthColor = new Color(0.95f, 0.2f, 0.1f);
    public float lowHealthThreshold = 0.25f;
    public float damagePerSecond = 5f;
    public float tickInterval = 0.25f;

    private TowerHealth towerHealth;
    private Canvas canvas;
    private Image fillImage;
    private HashSet<GameObject> enemiesInRange = new HashSet<GameObject>();
    private float nextTickTime;
    private TurretSwitchManager turretSwitchManager;
    private Grabber grabber;
    
    void Start()
    {
        if (!gameObject.CompareTag("Tower"))
        {
            enabled = false;
            return;
        }

        towerHealth = GetComponent<TowerHealth>();
        if (towerHealth == null)
            return;

        CreateHealthBar();
        towerHealth.OnHealthChanged += UpdateHealthBar;
        UpdateHealthBar(towerHealth.CurrentHealth, towerHealth.MaxHealth);
        turretSwitchManager = FindObjectOfType<TurretSwitchManager>();
        grabber = FindObjectOfType<Grabber>();
    }

    void OnDestroy()
    {
        if (towerHealth != null)
            towerHealth.OnHealthChanged -= UpdateHealthBar;
    }

    void Update()
    {
        // Remove destroyed enemies from the set
        enemiesInRange.RemoveWhere(enemy => enemy == null);
        
        int enemyCount = enemiesInRange.Count;
        
        if (enemyCount > 0 && towerHealth != null && Time.time >= nextTickTime)
        {
            float damage = damagePerSecond * tickInterval * enemyCount;
            towerHealth.TakeDamage(damage);
            Debug.Log($"Tower {gameObject.name}: Taking {damage} damage. Health: {towerHealth.CurrentHealth}/{towerHealth.MaxHealth}. Enemies: {enemyCount}");
            nextTickTime = Time.time + tickInterval;
        }

        if (towerHealth != null && towerHealth.CurrentHealth <= 0f)
        {
            GameObject tower = this.gameObject;
            Camera towerCamera = tower.GetComponentInChildren<Camera>();
            if (towerCamera.transform.position == turretSwitchManager.mainCamera.transform.position)
            {
                turretSwitchManager.ExitTurret();
            }
            
            Destroy(canvas.gameObject);
            Destroy(this);
            Destroy(tower);
        }
    }

    void LateUpdate()
    {
        if (canvas == null)
            return;

        canvas.transform.position = transform.position + offset;
        
        Camera cam = Camera.main;
        if (cam != null)
            canvas.transform.rotation = Quaternion.LookRotation(cam.transform.position - canvas.transform.position);
    }

    void CreateHealthBar()
    {
        GameObject canvasObj = new GameObject("HealthBar");
        canvasObj.transform.SetParent(transform);
        
        canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        
        RectTransform rect = canvas.GetComponent<RectTransform>();
        rect.sizeDelta = size * 100f;
        rect.localScale = Vector3.one * 0.01f;

        GameObject bgObj = new GameObject("Background");
        bgObj.transform.SetParent(canvasObj.transform, false);
        Image bg = bgObj.AddComponent<Image>();
        bg.sprite = CreateWhiteSprite();
        bg.color = new Color(0f, 0f, 0f, 0.6f);
        RectTransform bgRect = bg.rectTransform;
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;

        GameObject fillObj = new GameObject("Fill");
        fillObj.transform.SetParent(bgObj.transform, false);
        fillImage = fillObj.AddComponent<Image>();
        fillImage.sprite = CreateWhiteSprite();
        fillImage.type = Image.Type.Filled;
        fillImage.fillMethod = Image.FillMethod.Horizontal;
        fillImage.fillOrigin = (int)Image.OriginHorizontal.Left;
        fillImage.fillAmount = 1f;
        fillImage.color = fillColor;
        RectTransform fillRect = fillImage.rectTransform;
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = new Vector2(3f, 3f);
        fillRect.offsetMax = new Vector2(-3f, -3f);
    }

    Sprite CreateWhiteSprite()
    {
        Texture2D tex = new Texture2D(1, 1);
        tex.SetPixel(0, 0, Color.white);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f));
    }

    void UpdateHealthBar(float current, float max)
    {
        if (fillImage == null)
        {
            Debug.LogWarning($"Tower {gameObject.name}: fillImage is null in UpdateHealthBar!");
            return;
        }

        float pct = max > 0 ? current / max : 0;
        fillImage.fillAmount = pct;
        fillImage.color = pct <= lowHealthThreshold ? lowHealthColor : fillColor;
        
        Debug.Log($"Tower {gameObject.name}: UpdateHealthBar called. Current: {current}, Max: {max}, Percent: {pct}, FillAmount: {fillImage.fillAmount}");
        
        if (canvas != null)
            canvas.gameObject.SetActive(current > 0);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            enemiesInRange.Add(other.gameObject);
            Debug.Log($"Tower {gameObject.name}: Enemy entered. Count: {enemiesInRange.Count}");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            enemiesInRange.Remove(other.gameObject);
            Debug.Log($"Tower {gameObject.name}: Enemy exited. Count: {enemiesInRange.Count}");
        }
    }
}
