using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public static MainMenu Instance { get; private set; }

    public Button startGameButton;
    public Image saveFileIndicator;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        startGameButton.onClick.AddListener(StartGame);
    }

    void OnEnable()
    {
        SaveManager.OnSaveFileLoaded += SaveFileLoaded;
    }

    void OnDisable()
    {
        SaveManager.OnSaveFileLoaded -= SaveFileLoaded;
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void SaveFileLoaded()
    {
        saveFileIndicator.color = Color.green;
    }
}
