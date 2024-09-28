using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    void Start()
    {
        Camera.main.gameObject.transform.SetPositionAndRotation(new Vector3(Global.GRID_SIZE / 2, Global.GRID_SIZE / 2, Camera.main.gameObject.transform.position.z), quaternion.identity);
    }

}
