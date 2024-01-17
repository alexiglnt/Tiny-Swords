using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public int tileId;
    public Character character;
}

public class GridMap : MonoBehaviour
{
    private int _height;
    public int Height
    {
        get { return _height; }
        private set { _height = value; }
    }

    private int _width;
    public int Width
    {
        get { return _width; }
        private set { _width = value; }
    }

    private Node[,] _grid;

    public void Init(int width, int height)
    {
        _grid = new Node[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                _grid[x, y] = new Node();
            }
        }

        _height = height;
        _width = width;
    }

    internal void ClearCharacter(int xPos, int yPos)
    {
        _grid[xPos, yPos].character = null;
    }

    internal void SetCharacter(MapElement mapElement, int x_pos, int y_pos)
    {
        _grid[x_pos, y_pos].character = mapElement.GetComponent<Character>();
    }

    public void SetTile(int x, int y, int to)
    {
        if (!CheckPosition(x, y))
        {
            Debug.LogWarning("Trying to Set an cell outside the Grid boudries x = "
                + x.ToString() + ", y = " + y.ToString());
            return;
        }
            

        _grid[x, y].tileId = to;
    }

    public int GetTile(int x, int y) 
    {
        if (!CheckPosition(x, y))
        {
            Debug.LogWarning("Trying to Get an cell outside the Grid boudries x = " 
                +  x.ToString() + ", y = " + y.ToString());
            return -1;
        }
            

        return _grid[x, y].tileId;
    }

    public bool CheckPosition(int x, int y)
    {
        if(x < 0 || x >= _width)
            return false;

        if(y < 0 || y >= _height) 
            return false;

        return true;
    }

    public Character GetCharacter(int x, int y)
    {
        return _grid[x, y].character;
    }

    internal bool CheckWalkable(int xPos, int yPos)
    {
        return _grid[xPos, yPos].tileId == 0;
    }

}
