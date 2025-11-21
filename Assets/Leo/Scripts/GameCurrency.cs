using UnityEngine;
using TMPro;

public class GameCurrency : MonoBehaviour
{
    public static GameCurrency Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private float startingMoney = 100f;

    public float Money { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        Money = startingMoney;
        UpdateMoneyUI();
    }

    public void AddMoney(float amount)
    {
        Money += amount;
        UpdateMoneyUI();
    }

    public bool SpendMoney(float amount)
    {
        if (Money < amount)
            return false;

        Money -= amount;
        UpdateMoneyUI();
        return true;
    }

    private void UpdateMoneyUI()
    {
        if (moneyText != null)
        {
            moneyText.text = $"Money: ${Mathf.FloorToInt(Money)}";
        }
    }
}

