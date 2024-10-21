using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Serialization;

public class MenuItem<T> : MonoBehaviour
{
    public Image itemIcon;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemDescriptionText;
    public TextMeshProUGUI itemCostText;
    public Image highlight;
    public GameObject inventorySection;
    public TextMeshProUGUI inventoryCountText;
    public GameObject soldOutSection;
    public GameObject notEnoughCoinsSection;

    protected T itemData;
    
    public virtual void Setup(T item, string name, string description, Sprite icon, int cost, int count)
    {
        itemData = item;
        itemNameText.text = name;
        itemDescriptionText.text = description;
        itemIcon.sprite = icon;
        itemCostText.text = cost.ToString();
        inventoryCountText.text = count.ToString();
        
        UpdateVisibleSections();
    }

    public T GetItemData()
    {
        return itemData;
    }

    public virtual void UpdateVisibleSections()
    {
    }
}