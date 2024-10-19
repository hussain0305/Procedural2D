using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    public MazeGenerator mazeGenerator;
    public PlayerController player;
    
    [Header("Scriptable Objects")]
    public NPCList npcList;  
    
    private Coroutine cameraLerp;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void OnEnable()
    {
        MazeGenerator.OnMazeGenerationComplete += OnMazeGenerationComplete;
        MazeGenerator.OnAllRoomsAnalyzed += OnAllRoomsAnalyzed;
        EventManager.OnRoomEntered += HandleRoomEntered;
    }

    private void OnDisable()
    {
        MazeGenerator.OnMazeGenerationComplete -= OnMazeGenerationComplete;
        MazeGenerator.OnAllRoomsAnalyzed -= OnAllRoomsAnalyzed;
        EventManager.OnRoomEntered -= HandleRoomEntered;
    }
    private void OnMazeGenerationComplete()
    {
        SetCameraOnRoom(mazeGenerator.startNode);
    }

    private void OnAllRoomsAnalyzed()
    {
        PlacePlayerInStartingRoom();
        PlaceShopkeeperInStartingRoom();
    }

    private void PlacePlayerInStartingRoom()
    {
        Room room = mazeGenerator.allRooms[mazeGenerator.startNode];
        StartingRoom startingRoom = room.GetComponentInChildren<StartingRoom>();
        player.transform.position = room.GetCellLocation(startingRoom.playerStartPosition);
    }

    private void PlaceShopkeeperInStartingRoom()
    {
        Room room = mazeGenerator.allRooms[mazeGenerator.startNode];
        StartingRoom startingRoom = room.GetComponentInChildren<StartingRoom>();

        NPC shopkeeperPrefab = npcList.GetNPC(NPCCharacter.CyrusTheMadScribe);
        if (shopkeeperPrefab != null)
        {
            Instantiate(shopkeeperPrefab, room.GetCellLocation(startingRoom.shopkeeperPosition), Quaternion.identity);
        }
    }

    private void HandleRoomEntered(Vector2Int gridIndex)
    {
        MoveCameraToRoom(gridIndex);
    }
    public void SetCameraOnRoom(Vector2Int roomIndex)
    {
        Vector3 roomPosition = mazeGenerator.allRooms[roomIndex].transform.position;
        Camera.main.gameObject.transform.SetPositionAndRotation(new Vector3(roomPosition.x, roomPosition.y, Camera.main.gameObject.transform.position.z), quaternion.identity);
    }
    
    public void MoveCameraToRoom(Vector2Int roomIndex)
    {
        IEnumerator LerpCameraToRoom()
        {
            Vector3 roomPosition = mazeGenerator.allRooms[roomIndex].transform.position;
            Vector3 cameraTarget = new Vector3(roomPosition.x, roomPosition.y, Camera.main.gameObject.transform.position.z);
            
            float timeToLerp = 0.5f;
            float timePassed = 0;

            while (timePassed <= timeToLerp)
            {
                Camera.main.gameObject.transform.SetPositionAndRotation(Vector3.Lerp(Camera.main.gameObject.transform.position, cameraTarget, timePassed / timeToLerp) , quaternion.identity);

                timePassed += Time.deltaTime;
                yield return null;
            }
            
            SetCameraOnRoom(roomIndex);
            cameraLerp = null;
        }

        if (cameraLerp != null)
        {
            StopCoroutine(cameraLerp);
        }

        cameraLerp = StartCoroutine(LerpCameraToRoom());
    }

    public void DisablePlayerControls()
    {
        player.ForceStopPlayer();
        player.enabled = false;
    }

    public void EnablePlayerControls()
    {
        player.enabled = true;
    }
}
