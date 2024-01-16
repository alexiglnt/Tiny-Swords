using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class MapElement : MonoBehaviour
{

    private GridMap _gridMap;

    void Start()
    {
        SetGrid();
        PlaceObjectOnGrid();
    }

    private void SetGrid()
    {
        _gridMap = transform.parent.GetComponent<GridMap>();
    }

    private void PlaceObjectOnGrid()
    {
        Transform t = transform;
        Vector3 pos = t.position;
        int x_pos = (int)pos.x;
        int y_pos = (int)pos.y;
        _gridMap.SetCharacter(this, x_pos, y_pos);
    }
}
