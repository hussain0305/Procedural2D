using System;
using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class Shop : Popup
{
    public PurchasableItems purchasableItems;
    public GameObject shopItemPrefab;
    public Transform itemParent;
    public Button exitButton;
    public Image exitButtonHighlight;

    private List<PurchasableItemInfo> availableItems = new List<PurchasableItemInfo>();
    private List<ShopItem> shopItems = new List<ShopItem>();
    private Dictionary<PurchableItemType, Action> itemActions;
    private Dictionary<int, bool> itemAvailability;
    
    private int currentItemIndex = 0;
    private float inputCooldown = 0.2f;
    private float lastInputTime = 0;

    private bool IsOnExitButton
    {
        get
        {
            return currentItemIndex == shopItems.Count;
        }
    }
    
    private void Start()
    {
        InitShop();
        CreateShopItems();
        exitButton.onClick.AddListener(HideShop);
        HighlightOnEnable();
    }

    private void OnEnable()
    {
        GameManager.Instance.DisablePlayerControls();
        HighlightOnEnable();
    }

    private void InitShop()
    {
        itemActions = new Dictionary<PurchableItemType, Action>
        {
            { PurchableItemType.WallGrab, () => GameManager.Instance.player.abilities.hasWallGrab = true },
            { PurchableItemType.MultiJump, () => GameManager.Instance.player.abilities.additionalJumps++ },
        };

        availableItems = new List<PurchasableItemInfo>(purchasableItems.items);
        itemAvailability = new Dictionary<int, bool>();
        for (int i = 0; i < availableItems.Count; i++)
        {
            itemAvailability.Add(i, true);
        }
    }

    private void CreateShopItems()
    {
        foreach (var item in availableItems)
        {
            GameObject shopItemObj = Instantiate(shopItemPrefab, itemParent);
            ShopItem shopItem = shopItemObj.GetComponent<ShopItem>();

            shopItem.itemName.text = item.itemName;
            shopItem.itemDescription.text = item.description;
            shopItem.itemCost.text = item.price.ToString();
            shopItem.itemIcon.sprite = item.itemIcon;
            shopItem.inventoryCount.text = item.inventoryCount.ToString();

            if (item.inventoryCount > 0)
            {
                shopItem.soldOutSection.SetActive(false);
            }
            else
            {
                shopItem.inventorySection.SetActive(false);
                shopItem.soldOutSection.SetActive(true);
            }

            shopItems.Add(shopItem);
        }
    }

    private void Update()
    {
        if (Time.time - lastInputTime >= inputCooldown)
        {
            if (Input.GetAxisRaw("Vertical") < 0)
            {
                NavigateShop(1);
                lastInputTime = Time.time;
            }
            else if (Input.GetAxisRaw("Vertical") > 0)
            {
                NavigateShop(-1);
                lastInputTime = Time.time;
            }
        }

        if (Input.GetButtonDown("Action"))
        {
            if (IsOnExitButton)
            {
                HideShop();
            }
            else
            {
                PurchaseSelectedItem();
            }
        }
    }

    public void HighlightOnEnable()
    {
        if (itemAvailability != null)
        {
            currentItemIndex = 0;
            for (int i = 0; i < itemAvailability.Count; i++)
            {
                if (itemAvailability[i])
                {
                    currentItemIndex = i;
                    HighlightButton(i);
                    return;
                }
            }

            currentItemIndex = itemAvailability.Count;
            HighlightButton(itemAvailability.Count);//This is the exit button, for eg., if there are 4 items in the list, 0-3 are items; 4th one, last in the list, becomes the exit button
        }
    }
    
    private void NavigateShop(int direction)
    {
        bool foundNext = false;
        int nextIndexInNavigation = (currentItemIndex + direction) % (shopItems.Count + 1);
        while (!foundNext)
        {
            if (nextIndexInNavigation < 0)
            {
                nextIndexInNavigation = shopItems.Count;
            }
            if (nextIndexInNavigation == shopItems.Count)
            {
                foundNext = true;
                HighlightButton(nextIndexInNavigation);
            }
            else if (nextIndexInNavigation < availableItems.Count && itemAvailability[nextIndexInNavigation])
            {
                foundNext = true;
                HighlightButton(nextIndexInNavigation);
            }
            nextIndexInNavigation = (nextIndexInNavigation + direction) % (shopItems.Count + 1);
        }
    }

    private void HighlightButton(int index)
    {
        currentItemIndex = index;
        for (int i = 0; i < shopItems.Count; i++)
        {
            shopItems[i].highlight.gameObject.SetActive(index == i);
        }
        exitButtonHighlight.gameObject.SetActive(index == shopItems.Count);
    }

    private void PurchaseSelectedItem()
    {
        PurchasableItemInfo selectedItem = availableItems[currentItemIndex];

        if (selectedItem.inventoryCount > 0)
        {
            selectedItem.inventoryCount--;

            ShopItem currentShopItem = shopItems[currentItemIndex];
            currentShopItem.inventoryCount.text = selectedItem.inventoryCount.ToString();
            availableItems[currentItemIndex] = selectedItem;
            PurchaseItem(selectedItem);

            if (selectedItem.inventoryCount == 0)
            {
                currentShopItem.inventorySection.SetActive(false);
                currentShopItem.soldOutSection.SetActive(true);
                itemAvailability[currentItemIndex] = false;
                NavigateShop(1);
            }
        }
    }

    private void PurchaseItem(PurchasableItemInfo item)
    {
        if (itemActions.TryGetValue(item.itemType, out Action purchaseAction))
        {
            purchaseAction.Invoke();
        }
    }

    private void HideShop()
    {
        PopupManager.Instance.HideCurrentPopup();
        GameManager.Instance.EnablePlayerControls();
    }
}
