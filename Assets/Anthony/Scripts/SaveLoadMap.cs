using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveLoadMap : MonoBehaviour
{
    [SerializeField]
    private MapData _mapData;

    [SerializeField]
    private GridMap _gridMap;

    [SerializeField]
    private GridManager _gridManager;

    public void Save()
    {
        int[,] map = _gridManager.ReadTileMap();
        _mapData.Save(map);
    }

    public void Load() 
    {
        _gridManager.Clear();

        int width = _mapData.width;
        int height = _mapData.height;

        int i = 0;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                _gridManager.SetTile(x, y, _mapData.map[i]);
                i += 1;
            }
        }
    }

    internal void Load(GridMap grid)
    {
        _gridManager.Clear();
        _mapData.Load(grid);
    }
}
