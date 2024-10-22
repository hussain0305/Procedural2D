using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PopupManager : MonoBehaviour
{
    public static PopupManager Instance;

    public PopupNPC npcPopup;
    public PopupPurchasableItem purchasableItemPopup;
    public PopupItemInfo itemInfoPopup;
    public InteractionPrompt interactionPrompt;
    public NPCInteractionMenu npcInteractionMenu;
    public Shop shop;
    public Canvas worldSpaceCanvas;

    private Popup currentPopup;
    private RectTransform popupTransform;
    private Coroutine currentAnimation;
    private NPCInteraction currentNPC;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
        NPCInteraction.OnNPCInteraction += ShowInteractionMenu;
    }

    private void ShowPopup(Popup newPopup, Vector3 position, System.Action setupAction)
    {
        HideCurrentPopup();
        currentPopup = newPopup;
        popupTransform = currentPopup.GetComponent<RectTransform>();

        setupAction.Invoke();

        if (currentAnimation != null)
        {
            StopCoroutine(currentAnimation);
        }
        ShowPopupWithoutAnimation();
        PositionPopupInWorldSpace(position);
    }

    private void ShowPopup(Popup newPopup, System.Action setupAction)
    {
        HideCurrentPopup();
        currentPopup = newPopup;

        setupAction?.Invoke();

        if (currentAnimation != null)
        {
            StopCoroutine(currentAnimation);
        }
        ShowPopupWithoutAnimation();
    }

    public void HideCurrentPopup(bool closeAnimation = false)
    {
        if (closeAnimation)
        {
            if (currentPopup != null)
            {
                if (currentAnimation != null)
                {
                    StopCoroutine(currentAnimation);
                }
                currentAnimation = StartCoroutine(AnimatePopup(false));
            }
        }
        else
        {
            HidePopupWithoutAnimation();
        }
    }

    private void PositionPopupInWorldSpace(Vector3 position)
    {
        Vector3 canvasPosition = worldSpaceCanvas.WorldToCanvasPosition(position);
        popupTransform.localPosition = position;
    }

    private IEnumerator AnimatePopup(bool opening, float _duration = 0f)
    {
        float duration = _duration;
        float elapsedTime = 0;
        Vector3 startScale = opening ? new Vector3(1, 0, 1) : new Vector3(1, 1, 1);
        Vector3 endScale = opening ? new Vector3(1, 1, 1) : new Vector3(1, 0, 1);

        if (opening)
        {
            currentPopup.gameObject.SetActive(true);
        }

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            t = Mathf.Sin(t * Mathf.PI * 0.5f);
            popupTransform.localScale = Vector3.Lerp(startScale, endScale, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        popupTransform.localScale = endScale;

        if (!opening)
        {
            currentPopup.gameObject.SetActive(false);
            currentPopup = null;
        }

        currentAnimation = null;
    }
    
    private void ShowPopupWithoutAnimation()
    {
        currentPopup.gameObject.SetActive(true);
    }

    private void HidePopupWithoutAnimation()
    {
        if (currentPopup)
        {
            currentPopup.gameObject.SetActive(false);
        }
        currentPopup = null;
    }
    
    private void ShowInteractionMenu(NPCInteraction npc)
    {
        currentNPC = npc;
        HideCurrentPopup();

        List<NPCInteraction.NPCInteractionOption> options = npc.GetInteractionOptions();
        
        currentPopup = npcInteractionMenu;

        if (npcInteractionMenu != null)
        {
            npcInteractionMenu.ShowMenu(npc.npcName, options);
            // currentAnimation = StartCoroutine(AnimatePopup(true));
            ShowPopupWithoutAnimation();
            popupTransform = npcInteractionMenu.GetComponent<RectTransform>();
            PositionPopupInWorldSpace(npc.transform.position + Global.GetPromptOffset());
        }    
    }

    public void ShowNPCPopup(Vector3 position, string name, string description)
    {
        ShowPopup(npcPopup, position, () => UpdateNPCPopup(name, description));
    }

    public void ShowPurchasableItemPopup(Vector3 position, PurchasableItemInfo itemInfo)
    {
        ShowPopup(purchasableItemPopup, position, () => UpdatePurchasableItemPopup(itemInfo));
    }

    public void ShowItemInfoPopup(Vector3 position, string name, string description)
    {
        ShowPopup(itemInfoPopup, position, () => UpdateItemInfoPopup(name, description));
    }
    
    public void ShowActionPrompt(Vector3 position, string name, string description)
    {
        ShowPopup(interactionPrompt, position, () => UpdateInteractionInfoPopup(name, description));
    }

    public void ShowShop()
    {
        ShowPopup(shop, () => UpdateInteractionInfoPopup(name, null));
    }

    private void UpdateNPCPopup(string prompt, string description)
    {
        npcPopup.UpdateInfo(name, description);
    }

    private void UpdatePurchasableItemPopup(PurchasableItemInfo itemInfo)
    {
        purchasableItemPopup.UpdateInfo(itemInfo);
    }

    private void UpdateItemInfoPopup(string name, string description)
    {
        itemInfoPopup.UpdateInfo(name, description);
    }
    
    private void UpdateInteractionInfoPopup(string actionKey, string action)
    {
        interactionPrompt.UpdateInfo(actionKey, action);
    }
    
    private void OnDestroy()
    {
        NPCInteraction.OnNPCInteraction -= ShowInteractionMenu;
    }

    public NPCInteraction GetCurrentNPCBeingInteractedWith()
    {
        return currentNPC;
    }
}
