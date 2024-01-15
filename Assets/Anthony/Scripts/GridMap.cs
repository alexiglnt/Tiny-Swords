using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private int[,] _grid;

    public void Init(int width, int height)
    {
        _grid = new int[width, height];
        _height = height;
        _width = width;
    }

    public void Set(int x, int y, int to)
    {
        if (!CheckPosition(x, y))
        {
            Debug.LogWarning("Trying to Set an cell outside the Grid boudries x = "
                + x.ToString() + ", y = " + y.ToString());
            return;
        }
            

        _grid[x, y] = to;
    }

    public int Get(int x, int y) 
    {
        if (!CheckPosition(x, y))
        {
            Debug.LogWarning("Trying to Get an cell outside the Grid boudries x = " 
                +  x.ToString() + ", y = " + y.ToString());
            return -1;
        }
            

        return _grid[x, y];
    }

    public bool CheckPosition(int x, int y)
    {
        if(x < 0 || x >= _width)
            return false;

        if(y < 0 || y >= _height) 
            return false;

        return true;
    }

    internal bool CheckWalkable(int xPos, int yPos)
    {
        return _grid[xPos, yPos] == 0;
    }
}
