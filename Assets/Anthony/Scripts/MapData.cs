using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class MapData : ScriptableObject
{
    public int width, height;

    public List<int> map;

    public void Load(GridMap gridMap)
    {
        for (int x = 0; x <  width; x++) 
        { 
            for(int y = 0; y < height; y++)
            {
                gridMap.Set(x, y, Get(x, y));
            }
        }
    }

    private int Get(int x, int y)
    {
        int index = y * width + x;

        if(index >= map.Count)
        {
            Debug.LogError("Out of range on the map data!");
            return -1;
        }

        return map[index];
    }

    public void Save(GridMap gridMap)
    {
        height = gridMap.Height;
        width = gridMap.Width;

        map = new List<int>();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                map.Add(gridMap.Get(x, y));
            }
        }
    }
}
