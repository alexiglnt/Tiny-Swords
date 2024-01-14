using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(GridMap))]
[RequireComponent(typeof(Tilemap))]
public class GridManager : MonoBehaviour
{
    private Tilemap _tilemap;
    private GridMap _grid;

    [SerializeField]
    private TileSet _tileSet;

    private void Awake()
    {
        _tilemap = GetComponent<Tilemap>();
        _grid = GetComponent<GridMap>();
        _grid.Init(10, 8);
        
        for (int x = 1; x < 5; x++)
        {
            for(int y = 1; y < 5; y++) 
            {
                Set(x, y, 2);
            }
        }

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

    private void UpdateTile(int x, int y)
    {
        int tileId = _grid.Get(x, y);

        if (tileId == -1)
            return;

        _tilemap.SetTile(new Vector3Int(x, y, 0), _tileSet.tiles[tileId]);
    }

    public void Set(int x, int y, int to)
    {
        _grid.Set(x, y, to);
        UpdateTile(x, y);
    }
}
