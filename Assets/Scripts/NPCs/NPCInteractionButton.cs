using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class NPCInteractionButton : MonoBehaviour
{
    public Button button;
    public TextMeshProUGUI buttonText;
    public GameObject highlight;

    public void InitButton(string buttonName, UnityEvent actionEvent)
    {
        buttonText.text = buttonName;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(actionEvent.Invoke);
    }
}