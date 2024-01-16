using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Tilemaps;
using UnityEngine;

public class MapElement : MonoBehaviour
{

    private GridMap _gridMap;
    private int _xPos = 0;
    private int _yPos = 0;

    void Start()
    {
        SetGrid();
        PlaceObjectOnGrid();
    }

    private void SetGrid()
    {
        _gridMap = transform.parent.GetComponent<GridMap>();
    }

    public void MoveCharacter(int targetPosX,  int targetPosY)
    {
        RemoveObjectFromGrid();
        MoveTo(targetPosX, targetPosY);
        MoveObject();
    }

    public void MoveObject()
    {
        Vector3 worldPostion = new Vector3(_xPos * 1f + 0.5f, _yPos * 1f + 0.5f, -0.5f);
        transform.position = worldPostion;
    }

    private void MoveTo(int targetPosX, int targetPosY)
    {
        _gridMap.SetCharacter(this, targetPosX, targetPosY);
        _xPos = targetPosX;
        _yPos = targetPosY;
    }

    private void PlaceObjectOnGrid()
    {
        Transform t = transform;
        Vector3 pos = t.position;
        _xPos = (int)pos.x;
        _yPos = (int)pos.y;
        _gridMap.SetCharacter(this, _xPos, _yPos);
    }

    private void RemoveObjectFromGrid()
    {
        _gridMap.ClearCharacter(_xPos, _yPos);
    }
}
