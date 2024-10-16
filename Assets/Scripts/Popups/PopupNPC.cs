using TMPro;

public class PopupNPC : Popup
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;

    public void UpdateInfo(string name, string description)
    {
        nameText.text = name;
        descriptionText.text = description;
    }
}
