using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public struct LoadingProgress
{
    public bool mazeGenerated;
    public bool roomsAnalyzed;
    public bool trapsPlaced;

    public bool LoadingComplete()
    {
        return mazeGenerated && roomsAnalyzed && trapsPlaced;
    }
}

public class LoadingScreen : MonoBehaviour
{
    public Text loadingText;
    public GameObject loadingScreen;

    private List<List<string>> loadingScreenTexts;

    private LoadingProgress loadingProgress;
    
    private void InitTexts()
    {
        loadingScreenTexts = new List<List<string>>();
        loadingScreenTexts.Add(new List<string> { "Descending into The Abyss.", "Descending into The Abyss..", "Descending into The Abyss..." });
        loadingScreenTexts.Add(new List<string> { "The Abyss beckons.", "The Abyss beckons..", "The Abyss beckons..." });
        loadingScreenTexts.Add(new List<string> { "Into the depths of madness.", "Into the depths of madness..", "Into the depths of madness..." });
        loadingScreenTexts.Add(new List<string> { "The shadows stir beneath.", "The shadows stir beneath..", "The shadows stir beneath..." });
        loadingScreenTexts.Add(new List<string> { "Falling further from the light.", "Falling further from the light..", "Falling further from the light..." });
        loadingScreenTexts.Add(new List<string> { "Embracing the void.", "Embracing the void..", "Embracing the void..." });
        loadingScreenTexts.Add(new List<string> { "The labyrinth shifts once more.", "The labyrinth shifts once more..", "The labyrinth shifts once more..." });
        loadingScreenTexts.Add(new List<string> { "Whispers from the dark await.", "Whispers from the dark await..", "Whispers from the dark await..." });
        loadingScreenTexts.Add(new List<string> { "The echoes of eternity call.", "The echoes of eternity call..", "The echoes of eternity call..." });
        loadingScreenTexts.Add(new List<string> { "Venturing into the unknown.", "Venturing into the unknown..", "Venturing into the unknown..." });
    }

    private void Start()
    {
        StartCoroutine(DisableLoadingScreen());
    }

    private void OnEnable()
    {
        loadingScreen.SetActive(true);
        MazeGenerator.OnMazeGenerationComplete += OnMazeGenerationComplete;
        MazeGenerator.OnAllRoomsAnalyzed += OnAllRoomsAnalyzed;
        MazeGenerator.OnAllRoomsPlacedTraps += OnTrapPlacementCompleted;
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
        loadingProgress.roomsAnalyzed = true;
    }

    IEnumerator DisableLoadingScreen()
    {
        yield return new WaitForSeconds(0.5f);
        yield return new WaitUntil(() => loadingProgress.LoadingComplete());
        loadingScreen.SetActive(false);
        StopAllCoroutines();
    }

    IEnumerator LoadingScreenText()
    {
        int randomMessage = Random.Range(0, loadingScreenTexts.Count);
        int totalMessages = loadingScreenTexts[randomMessage].Count;
        int index = 0;
        while (true)
        {
            loadingText.text = loadingScreenTexts[randomMessage][index];
            index = (index + 1) % totalMessages;
            yield return new WaitForSeconds(1f);
        }
    }

    private void OnMazeGenerationComplete()
    {
        loadingProgress.mazeGenerated = true;
    }

    private void OnTrapPlacementCompleted()
    {
        loadingProgress.trapsPlaced = true;
    }

}
