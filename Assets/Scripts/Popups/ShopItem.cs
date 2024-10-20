using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    public Image itemIcon;
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI itemDescription;
    public TextMeshProUGUI itemCost;
    public Image highlight;
    public GameObject inventorySection;
    public TextMeshProUGUI inventoryCount;
    public GameObject soldOutSection;

}
