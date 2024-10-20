using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu<T> : Popup
{
    public GameObject menuItemPrefab;
    public Transform itemParent;
    public Button exitButton;
    public Image exitButtonHighlight;

    protected List<T> availableItems = new List<T>();
    protected List<MenuItem<T>> menuItems = new List<MenuItem<T>>();
    protected Dictionary<int, bool> itemAvailability;

    protected int currentItemIndex = 0;
    private float inputCooldown = 0.2f;
    private float lastInputTime = 0;

    private bool IsOnExitButton => currentItemIndex == menuItems.Count;

    protected virtual void Start()
    {
        CreateMenuItems();
        exitButton.onClick.AddListener(HideMenu);
        HighlightOnEnable();
    }

    private void OnEnable()
    {
        GameManager.Instance.DisablePlayerControls();
        HighlightOnEnable();
    }

    private void Update()
    {
        if (Time.time - lastInputTime >= inputCooldown)
        {
            float verticalInput = Input.GetAxisRaw("Vertical");
            float horizontalInput = Input.GetAxisRaw("Horizontal");
            
            if (verticalInput < 0 || horizontalInput > 0)
            {
                NavigateMenu(1);
                lastInputTime = Time.time;
            }
            else if (verticalInput > 0 || horizontalInput < 0)
            {
                NavigateMenu(-1);
                lastInputTime = Time.time;
            }
        }

        if (Input.GetButtonDown("Action"))
        {
            if (IsOnExitButton)
            {
                HideMenu();
            }
            else
            {
                PurchaseSelectedItem();
            }
        }
    }

    protected void HighlightOnEnable()
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
            HighlightButton(itemAvailability.Count);
        }
    }

    protected void NavigateMenu(int direction)
    {
        bool foundNext = false;
        int nextIndexInNavigation = (currentItemIndex + direction) % (menuItems.Count + 1);
        while (!foundNext)
        {
            if (nextIndexInNavigation < 0)
            {
                nextIndexInNavigation = menuItems.Count;
            }
            if (nextIndexInNavigation == menuItems.Count)
            {
                foundNext = true;
                HighlightButton(nextIndexInNavigation);
            }
            else if (nextIndexInNavigation < availableItems.Count && itemAvailability[nextIndexInNavigation])
            {
                foundNext = true;
                HighlightButton(nextIndexInNavigation);
            }
            nextIndexInNavigation = (nextIndexInNavigation + direction) % (menuItems.Count + 1);
        }
    }

    private void HighlightButton(int index)
    {
        currentItemIndex = index;
        for (int i = 0; i < menuItems.Count; i++)
        {
            menuItems[i].highlight.gameObject.SetActive(index == i);
        }
        exitButtonHighlight.gameObject.SetActive(index == menuItems.Count);
    }

    protected virtual void PurchaseSelectedItem()
    {
    }

    protected void HideMenu()
    {
        GameManager.Instance.EnablePlayerControls();
        PopupManager.Instance.HideCurrentPopup();
    }

    protected virtual void CreateMenuItems()
    {
        foreach (var item in availableItems)
        {
            GameObject menuItemObj = Instantiate(menuItemPrefab, itemParent);
            MenuItem<T> menuItem = menuItemObj.GetComponent<MenuItem<T>>();

            SetupMenuItem(menuItem, item);

            menuItems.Add(menuItem);
        }
    }

    protected virtual void SetupMenuItem(MenuItem<T> menuItem, T item)
    {
    }
}
