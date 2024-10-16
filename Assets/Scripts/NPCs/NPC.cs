using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public string npcName;
    public string description;

    private void OnTriggerEnter2D(Collider2D other)
    {
        PopupManager.Instance.ShowPopup(PopupType.NPC, transform.position + new Vector3(0, 1, 0), npcName,
            description: description);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        PopupManager.Instance.HideCurrentPopup();
    }
}
