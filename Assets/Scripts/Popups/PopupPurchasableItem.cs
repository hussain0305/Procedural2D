using TMPro;
using UnityEngine.UI;

public class PopupPurchasableItem : Popup
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI priceText;
    public Image itemImage;
    
    public void UpdateInfo(PurchasableItemInfo itemInfo)
    {
        nameText.text = itemInfo.itemName;
        descriptionText.text = itemInfo.description;
        priceText.text = itemInfo.price.ToString();
        itemImage.sprite = itemInfo.itemIcon;
    }
}