using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private RectTransform panelShop;     // Panel_Shop
    [SerializeField] private Button toggleButton;         // Button_ToggleShop
    [SerializeField] private Transform itemsParent;       // Content (inside ScrollView)
    [SerializeField] private ShopSlotUI shopSlotPrefab;   // ShopSlot prefab

    [Header("Items")]
    [SerializeField] private ShopItem[] shopItems;

    private bool isOpen = true;

    private void Start()
    {
        // Populate shop slots
        foreach (ShopItem item in shopItems)
        {
            ShopSlotUI slot = Instantiate(shopSlotPrefab, itemsParent);
            slot.Initialize(item, this);
        }

        if (toggleButton != null)
        {
            toggleButton.onClick.AddListener(ToggleShop);
        }
    }

    private void Update()
    {
        // Optional: toggle shop with Tab key
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleShop();
        }
    }

    public void ToggleShop()
    {
        isOpen = !isOpen;
        panelShop.gameObject.SetActive(isOpen);
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
            // TODO: call your placement logic here later
        }
        else
        {
            Debug.Log("Not enough money!");
        }
    }
    
}
