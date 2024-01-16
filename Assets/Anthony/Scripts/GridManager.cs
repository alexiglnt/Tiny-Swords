using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.WSA;

[RequireComponent(typeof(GridMap))]
[RequireComponent(typeof(Tilemap))]
public class GridManager : MonoBehaviour
{
    private Tilemap _tilemap;
    private GridMap _grid;
    private SaveLoadMap _saveLoadMap;

    [SerializeField]
    private TileSet _tileSet;

    private void Awake()
    {
        _tilemap = GetComponent<Tilemap>();
        _grid = GetComponent<GridMap>();
        _saveLoadMap = GetComponent<SaveLoadMap>();

        _saveLoadMap.Load(_grid);

        UpdateTileMap();
    }

    void UpdateTileMap()
    {
        for(int x = 0; x < _grid.Width; x++)
        {
            for (int y = 0; y < _grid.Height; y++)
            {
                UpdateTile(x, y);

            }
        }
    }

    internal void Clear()
    {
        if (_tilemap == null)
        {
            _tilemap = GetComponent<Tilemap>();
            _tilemap.ClearAllTiles();
            _tilemap = null;
        }
        else
        {
            _tilemap.ClearAllTiles();
        }

    }

    internal void SetTile(int x, int y, int tileId)
    {
        if (tileId == -1)
            return;

        if (_tilemap == null)
        {
            _tilemap = GetComponent<Tilemap>();

            _tilemap.SetTile(new Vector3Int(x, y, 0), _tileSet.tiles[tileId]);

            _tilemap = null;
        }
        else
        {
            _tilemap.SetTile(new Vector3Int(x, y, 0), _tileSet.tiles[tileId]);
        }


    }

    private void UpdateTile(int x, int y)
    {
        int tileId = _grid.GetTile(x, y);

        if (tileId == -1)
            return;

        _tilemap.SetTile(new Vector3Int(x, y, 0), _tileSet.tiles[tileId]);
    }

    internal Character GetCharacter(int x, int y)
    {
        return _grid.GetCharacter(x, y);
    }

    public void Set(int x, int y, int to)
    {
        _grid.SetTile(x, y, to);
        UpdateTile(x, y);
    }

    public int[,] ReadTileMap() 
    { 
        if( _tilemap == null )
            _tilemap = GetComponent<Tilemap>();

        int size_x = _tilemap.size.x;
        int size_y = _tilemap.size.y;
        int[,] tilemapData = new int[size_x, size_y];

        for (int x = 0; x < size_x; x++)
        {
            for (int y = 0; y < size_y; y++)
            {

                TileBase tileBase = _tilemap.GetTile(new Vector3Int(x, y, 0));
                int indexTile = _tileSet.tiles.FindIndex(x => x == tileBase);
                tilemapData[x, y] = indexTile;

            }
        }

        _tilemap = null;
        return tilemapData;
    }

    
}
