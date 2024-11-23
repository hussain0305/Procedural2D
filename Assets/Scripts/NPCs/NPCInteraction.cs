using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;

public class NPCInteraction : MonoBehaviour
{
    [Serializable]
    public struct NPCInteractionOption
    {
        public string actionName;
        public UnityEvent actionEvent;
    }

    public string npcName;
    public List<NPCInteractionOption> interactionOptions = new List<NPCInteractionOption>();

    public delegate void OnInteractionDelegate(NPCInteraction npc);
    public static event OnInteractionDelegate OnNPCInteraction;
    
    private bool isInteracting = false;
    private bool isInRange = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        isInRange = true;
        PopupManager.Instance.ShowActionPrompt(transform.position + Global.GetPromptOffset(), "E", "Interact");
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        isInRange = false;
        PopupManager.Instance.HideCurrentPopup();
        GameManager.Instance.EnablePlayerControls();
        EndInteraction();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Action") && !isInteracting && isInRange)
        {
            StartInteraction();
        }
    }
    
    public List<NPCInteractionOption> GetInteractionOptions()
    {
        return interactionOptions;
    }

    void StartInteraction()
    {
        isInteracting = true;
        PopupManager.Instance.HideCurrentPopup();
        OnNPCInteraction?.Invoke(this);
    }

    public void EndInteraction()
    {
        isInteracting = false;
    }
}