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

    private T itemData;

    public void Setup(T item, string name, string description, Sprite icon, int cost, int count)
    {
        itemData = item;
        itemName.text = name;
        itemDescription.text = description;
        itemIcon.sprite = icon;
        itemCost.text = cost.ToString();
        inventoryCount.text = count.ToString();

        if (count > 0)
        {
            soldOutSection.SetActive(false);
            inventorySection.SetActive(true);
        }
        else
        {
            soldOutSection.SetActive(true);
            inventorySection.SetActive(false);
        }
    }

    public T GetItemData()
    {
        return itemData;
    }
}