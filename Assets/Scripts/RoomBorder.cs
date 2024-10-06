using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomBorder : MonoBehaviour
{
    public RoomEdge edge;
    public GameObject[] borderBlock;
    
    public void SetupLeftBorder()
    {
        float edgeDistanceFromCenter_x = 0.5f * (Global.CELL_SIZE_INTERIOR_X + 1);
        float edgeDistanceFromCenter_y = 0.5f * (Global.CELL_SIZE_INTERIOR_Y + 1);

        //left border, x is constant at -edgeDistanceFromCenter_x
        //y goes from -edgeDistanceFromCenter_y + 1 to edgeDistanceFromCenter_y - 1
        
        transform.localPosition = new Vector3(-edgeDistanceFromCenter_x, 0, 0);
        float current_y = -edgeDistanceFromCenter_y + 1;
        
        foreach (GameObject block in borderBlock)
        {
            block.transform.localPosition = new Vector3(0, current_y, 0);
            current_y = current_y + 1;
        }
    }
    
    public void SetupRightBorder()
    {
        float edgeDistanceFromCenter_x = 0.5f * (Global.CELL_SIZE_INTERIOR_X + 1);
        float edgeDistanceFromCenter_y = 0.5f * (Global.CELL_SIZE_INTERIOR_Y + 1);

        //right border, x is constant at edgeDistanceFromCenter_x
        //y goes from -edgeDistanceFromCenter_y + 1 to edgeDistanceFromCenter_y - 1
        
        transform.localPosition = new Vector3(edgeDistanceFromCenter_x, 0, 0);
        float current_y = -edgeDistanceFromCenter_y + 1;
        
        foreach (GameObject block in borderBlock)
        {
            block.transform.localPosition = new Vector3(0, current_y, 0);
            current_y = current_y + 1;
        }

    }

    public void SetupCeiling()
    {
        float edgeDistanceFromCenter_x = 0.5f * (Global.CELL_SIZE_INTERIOR_X + 1);
        float edgeDistanceFromCenter_y = 0.5f * (Global.CELL_SIZE_INTERIOR_Y + 1);

        //ceiling, y is constant at edgeDistanceFromCenter_y
        //x goes from -edgeDistanceFromCenter_x to edgeDistanceFromCenter_x
        
        transform.localPosition = new Vector3(0, edgeDistanceFromCenter_y, 0);
        float current_x = -edgeDistanceFromCenter_x + 1;
        
        foreach (GameObject block in borderBlock)
        {
            block.transform.localPosition = new Vector3(current_x, 0, 0);
            current_x = current_x + 1;
        }
    }

    public void SetupFloor()
    {
        float edgeDistanceFromCenter_x = 0.5f * (Global.CELL_SIZE_INTERIOR_X + 1);
        float edgeDistanceFromCenter_y = 0.5f * (Global.CELL_SIZE_INTERIOR_Y + 1);

        //ceiling, y is constant at -edgeDistanceFromCenter_y
        //x goes from -edgeDistanceFromCenter_x to edgeDistanceFromCenter_x
        
        transform.localPosition = new Vector3(0, -edgeDistanceFromCenter_y, 0);
        float current_x = -edgeDistanceFromCenter_x + 1;
        
        foreach (GameObject block in borderBlock)
        {
            block.transform.localPosition = new Vector3(current_x, 0, 0);
            current_x = current_x + 1;
        }
    }
}
