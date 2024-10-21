using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuItem<T> : MonoBehaviour
{
    public Image itemIcon;
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI itemDescription;
    public TextMeshProUGUI itemCost;
    public Image highlight;
    public GameObject inventorySection;
    public TextMeshProUGUI inventoryCount;
    public GameObject soldOutSection;
    public GameObject notEnoughCoinsSection;

    private T itemData;
    private int itemPrice;
    
    public void Setup(T item, string name, string description, Sprite icon, int cost, int count)
    {
        itemData = item;
        itemName.text = name;
        itemDescription.text = description;
        itemIcon.sprite = icon;
        itemCost.text = cost.ToString();
        inventoryCount.text = count.ToString();
        itemPrice = cost;
        
        UpdateVisibleSections();
    }

    public T GetItemData()
    {
        return itemData;
    }

    public void UpdateVisibleSections()
    {
        int playerCoins = GameManager.Instance.PlayerInventory.GetCoinsBalance();
        //Inventory section will be active if it's both available and player has enough coins to buy it
        inventorySection.SetActive(itemPrice > 0 && playerCoins >= itemPrice);
        //sold out section will be active if it's out of stock
        soldOutSection.SetActive(itemPrice <= 0 );
        //not enough coins section will be active if sold out section is not active and player doesn't have enough coins
        notEnoughCoinsSection.SetActive(!soldOutSection.activeSelf && playerCoins < itemPrice);
    }
}