using TMPro;

public class PopupPurchasableItem : Popup
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI priceText;

    public void UpdateInfo(string name, string price)
    {
        nameText.text = name;
        priceText.text = price;
    }
}
