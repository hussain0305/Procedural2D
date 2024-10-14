using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    public Text loadingText;
    public GameObject loadingScreen;

    private List<string> loadingScreenTexts;
    
    private void InitTexts()
    {
        loadingScreenTexts = new List<string>();
        loadingScreenTexts.Add("Loading The Abyss.");
        loadingScreenTexts.Add("Loading The Abyss..");
        loadingScreenTexts.Add("Loading The Abyss...");
    }

    private void OnEnable()
    {
        loadingScreen.SetActive(true);
        MazeGenerator.OnMazeGenerationComplete += OnMazeGenerationComplete;
        MazeGenerator.OnAllRoomsAnalyzed += OnAllRoomsAnalyzed;
        InitTexts();
        StartCoroutine(LoadingScreenText());
    }
    
    private void OnDisable()
    {
        MazeGenerator.OnMazeGenerationComplete -= OnMazeGenerationComplete;
        MazeGenerator.OnAllRoomsAnalyzed -= OnAllRoomsAnalyzed;
    }

    private void OnAllRoomsAnalyzed()
    {
        StartCoroutine(DisableLoadingScreen());
    }

    IEnumerator DisableLoadingScreen()
    {
        yield return new WaitForSeconds(0.5f);
        loadingScreen.SetActive(false);
        StopAllCoroutines();
    }

    IEnumerator LoadingScreenText()
    {
        int index = 0;
        while (true)
        {
            loadingText.text = loadingScreenTexts[index];
            index = (index + 1) % loadingScreenTexts.Count;
            yield return new WaitForSeconds(0.25f);
        }
    }

    private void OnMazeGenerationComplete()
    {
        
    }

}
