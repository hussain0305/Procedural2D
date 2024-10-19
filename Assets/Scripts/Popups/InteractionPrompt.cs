using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InteractionPrompt : Popup
{
    public TextMeshProUGUI keyText;
    public TextMeshProUGUI actionText;
    
    public void UpdateInfo(string actionKey, string actionescription)
    {
        keyText.text = actionKey;
        actionText.text = actionescription;
    }
}
