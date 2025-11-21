using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopSlotUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI priceText;

    private ShopItem item;
    private ShopUI shopUI;

    public void Initialize(ShopItem item, ShopUI shopUI)
    {
        this.item = item;
        this.shopUI = shopUI;

        if (iconImage != null)
            iconImage.sprite = item.icon;

        if (priceText != null)
            priceText.text = $"${item.price}";
    }

    // Hook this to the Button's OnClick
    public void OnClick()
    {
        if (shopUI != null && item != null)
        {
            shopUI.TryBuyItem(item);
        }
    }
}
