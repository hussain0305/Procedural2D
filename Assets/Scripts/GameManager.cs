using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public MazeGenerator mazeGenerator;
    public PlayerController player;
    
    private Coroutine cameraLerp;
    
    private void OnEnable()
    {
        MazeGenerator.OnMazeGenerationComplete += OnMazeGenerationComplete;
        EventManager.OnRoomEntered += HandleRoomEntered;
    }

    private void OnDisable()
    {
        MazeGenerator.OnMazeGenerationComplete -= OnMazeGenerationComplete;
        EventManager.OnRoomEntered -= HandleRoomEntered;
    }
    private void OnMazeGenerationComplete()
    {
        Vector3 roomPosition = mazeGenerator.allRooms[mazeGenerator.startNode].transform.position;
        player.transform.position = roomPosition;
        SetCameraOnRoom(mazeGenerator.startNode);
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

}
