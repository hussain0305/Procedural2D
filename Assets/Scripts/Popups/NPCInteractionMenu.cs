using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NPCInteractionMenu : Popup
{
    public TextMeshProUGUI npcName;
    public static NPCInteractionMenu Instance;
    public GameObject buttonPrefab;
    public Transform buttonParent;

    private List<NPCInteractionButton> interactionButtons = new List<NPCInteractionButton>();
    private Queue<NPCInteractionButton> buttonPool = new Queue<NPCInteractionButton>();

    private int currentButtonIndex = 0; 
    private float inputCooldown = 0.2f;
    private float lastInputTime = 0;
    private bool isMenuOpen = false;

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

    private void Update()
    {
        if (!isMenuOpen) return;

        float verticalInput = Input.GetAxisRaw("Vertical");
        float horizontalInput = Input.GetAxisRaw("Horizontal");

        if (Time.time - lastInputTime >= inputCooldown)
        {
            if (verticalInput > 0 || horizontalInput < 0)
            {
                NavigateMenu(-1);
                lastInputTime = Time.time;
            }
            else if (verticalInput < 0 || horizontalInput > 0)
            {
                NavigateMenu(1);
                lastInputTime = Time.time;
            }
        }

        if (Input.GetButtonDown("Submit") || Input.GetButtonDown("Action"))
        {
            interactionButtons[currentButtonIndex].button.onClick.Invoke();
        }
    }
    
    public void ShowMenu(string _npcName, List<NPCInteraction.NPCInteractionOption> options)
    {
        ClearMenu();

        npcName.text = _npcName;

        foreach (var option in options)
        {
            NPCInteractionButton button = GetButtonFromPool();
            button.InitButton(option.actionName, option.actionEvent);
        }

        AddExitOption();

        gameObject.SetActive(true);
        DisablePlayerController();

        currentButtonIndex = 0;
        HighlightButton(currentButtonIndex);

        isMenuOpen = true;
    }

    public void HideMenu()
    {
        ClearMenu();
        EnablePlayerController();
        isMenuOpen = false;
        PopupManager.Instance.GetCurrentNPCBeingInteractedWith().EndInteraction();
        PopupManager.Instance.HideCurrentPopup();
    }

    private NPCInteractionButton GetButtonFromPool()
    {
        NPCInteractionButton button;
        if (buttonPool.Count > 0)
        {
            button = buttonPool.Dequeue();
            button.gameObject.SetActive(true);
        }
        else
        {
            GameObject newButtonObject = Instantiate(buttonPrefab, buttonParent);
            button = newButtonObject.GetComponent<NPCInteractionButton>();
        }

        interactionButtons.Add(button);
        return button;
    }

    private void ReturnButtonToPool(NPCInteractionButton button)
    {
        buttonPool.Enqueue(button);
        button.gameObject.SetActive(false);
    }

    private void ClearMenu()
    {
        foreach (var button in interactionButtons)
        {
            ReturnButtonToPool(button);
        }
        interactionButtons.Clear();
    }

    private void NavigateMenu(int direction)
    {
        interactionButtons[currentButtonIndex].highlight.SetActive(false);
        currentButtonIndex = (currentButtonIndex + direction + interactionButtons.Count) % interactionButtons.Count;
        HighlightButton(currentButtonIndex);
    }

    private void HighlightButton(int index)
    {
        for (int i = 0; i < interactionButtons.Count; i++)
        {
            interactionButtons[i].highlight.SetActive(i == index);
        }
    }

    private void AddExitOption()
    {
        NPCInteractionButton exitButton = GetButtonFromPool();
        exitButton.InitButton("Exit", new UnityEngine.Events.UnityEvent());
        exitButton.button.onClick.AddListener(() => HideMenu());
    }

    private void DisablePlayerController()
    {
        GameManager.Instance.DisablePlayerControls();
    }

    private void EnablePlayerController()
    {
        GameManager.Instance.EnablePlayerControls();
    }
}
