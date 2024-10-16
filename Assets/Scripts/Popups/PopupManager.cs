using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Serialization;

public class PopupManager : MonoBehaviour
{
    public static PopupManager Instance;

    public PopupNPC npcPopup;
    public PopupPurchasableItem purchasableItemPopup;
    public PopupItemInfo itemInfoPopup;
    public Canvas worldSpaceCanvas;

    private Popup currentPopup;
    private RectTransform popupTransform;
    private Coroutine currentAnimation;

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
    }

    public void ShowPopup(PopupType type, Vector3 position, string name, string description = "", int price = 0)
    {
        if (currentPopup != null)
        {
            HideCurrentPopup();
        }

        switch (type)
        {
            case PopupType.NPC:
                currentPopup = npcPopup;
                UpdateNpcPopup(name, description);
                break;
            case PopupType.ShopItem:
                currentPopup = purchasableItemPopup;
                UpdateShopItemPopup(name, price);
                break;
            case PopupType.ItemInfo:
                currentPopup = itemInfoPopup;
                UpdateItemInfoPopup(name, description);
                break;
        }

        popupTransform = currentPopup.GetComponent<RectTransform>();

        if (currentAnimation != null)
        {
            StopCoroutine(currentAnimation);
        }
        currentAnimation = StartCoroutine(AnimatePopup(true));

        PositionPopupInWorldSpace(position);
    }

    private void PositionPopupInWorldSpace(Vector3 position)
    {
        Vector3 canvasPosition = worldSpaceCanvas.WorldToCanvasPosition(position);
        popupTransform.localPosition = position;
    }

    public void HideCurrentPopup()
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

    private void UpdateNpcPopup(string name, string description)
    {
        npcPopup.UpdateInfo(name, description);
    }

    private void UpdateShopItemPopup(string name, int price)
    {
        purchasableItemPopup.UpdateInfo(name, price.ToString());
    }

    private void UpdateItemInfoPopup(string name, string description)
    {
        itemInfoPopup.UpdateInfo(name, description);
    }

    private IEnumerator AnimatePopup(bool opening)
    {
        float duration = 0.1f;
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
}
