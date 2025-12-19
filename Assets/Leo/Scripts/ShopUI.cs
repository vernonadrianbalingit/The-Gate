using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private RectTransform panelShop;     // Panel_Shop (this object)
    [SerializeField] private Button toggleButton;         // Button_ToggleShop
    [SerializeField] private Transform itemsParent;       // Content of ScrollView
    [SerializeField] private ShopSlotUI shopSlotPrefab;   // ShopSlot prefab

    [Header("Items")]
    [SerializeField] private ShopItem[] shopItems;

    [Header("Options")]
    [SerializeField] private bool startOpen = true;       // whether the shop starts visible
    [SerializeField] private GameObject grabberScript;       // Reference to the Grabber script

    private bool isOpen;

    private void Awake()
    {
        if (panelShop == null)
            panelShop = GetComponent<RectTransform>();
    }

    private void Start()
    {
        // Populate shop slots
        foreach (ShopItem item in shopItems)
        {
            ShopSlotUI slot = Instantiate(shopSlotPrefab, itemsParent);
            slot.Initialize(item, this);
        }

        // Hook up button
        if (toggleButton != null)
        {
            toggleButton.onClick.AddListener(ToggleShop);
        }

        // Set initial state
        isOpen = startOpen;
        UpdatePanelVisibility();
    }

    private void Update()
    {
        // Tab to toggle
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleShop();
        }
    }

    public void ToggleShop()
    {
        isOpen = !isOpen;
        UpdatePanelVisibility();
    }

    private void UpdatePanelVisibility()
    {
        if (panelShop == null) return;

        // Hide by scaling X to 0 instead of disabling the GameObject
        if (isOpen)
        {
            panelShop.localScale = Vector3.one;                // (1,1,1)
        }
        else
        {
            panelShop.localScale = new Vector3(0f, 1f, 1f);    // squish it flat on X
        }
    }

    public void TryBuyItem(ShopItem item)
    {
        if (GameCurrency.Instance == null)
        {
            Debug.LogWarning("No GameCurrency instance found.");
            return;
        }

        if (GameCurrency.Instance.SpendMoney(item.price))
        {
            Debug.Log($"Bought {item.itemName} for {item.price}");
            // later: spawn or place your tower here
            grabberScript.GetComponent<Grabber>().SpawnPrefabAtCursor(item.turretPrefab, item.price);
        }
        else
        {
            Debug.Log("Not enough money!");
        }
    }
}
