using UnityEngine;
using UnityEngine.Tilemaps;

// Ce script gère les tuils sur la tilemap
[RequireComponent(typeof(GridMap))]
//[RequireComponent(typeof(Tilemap))]
public class GridManager : Singleton<GridManager>
{
    //////////////////////////////////
    //          Variables           // 
    //////////////////////////////////
    
    private GridMap _grid;      // Référence à la classe GridMap qui gère la logique de la grille.
    private SaveLoadMap _saveLoadMap;  // Référence à la classe qui gère la sauvegarde et le chargement de la carte.

    [SerializeField]
    private Tilemap _groundTilemap;  // Référence au Tilemap Unity pour les graphiques de la grille.

    [SerializeField]
    private TileSet _tileSet;   // Ensemble de tuiles utilisé pour mapper les indices de tuiles aux tuiles réelles.


    //////////////////////////////////////////
    //          Fonctions private           // 
    //////////////////////////////////////////

    protected void Awake()
    {
        _grid = GetComponent<GridMap>();
        _saveLoadMap = GetComponent<SaveLoadMap>();

        // Charge la carte à partir d'une sauvegarde.
        _saveLoadMap.Load(_grid);

        // Met à jour le Tilemap pour refléter l'état actuel de la grille.
        UpdateTileMap();
    }

    // Met à jour la tuile à la position spécifiée dans le Tilemap en fonction de la grille.
    private void UpdateTile(int x, int y)
    {
        int tileId = _grid.GetTile(x, y);

        if (tileId == -1)
            return;

        _groundTilemap.SetTile(new Vector3Int(x, y, 0), _tileSet.tiles[tileId]);
    }


    //////////////////////////////////////////
    //          Fonctions public           // 
    //////////////////////////////////////////

    // Efface le Tilemap.
    public void Clear()
    {
        if (_groundTilemap == null)
        {
            _groundTilemap = GetComponent<Tilemap>();
            _groundTilemap.ClearAllTiles();
            _groundTilemap = null;
        }
        else
        {
            _groundTilemap.ClearAllTiles();
        }
    }

    // Met à jour le Tilemap en parcourant toute la grille.
    private void UpdateTileMap()
    {
        for (int x = 0; x < _grid.Width; x++)
        {
            for (int y = 0; y < _grid.Height; y++)
            {
                UpdateTile(x, y);
            }
        }
    }

    // Définit la tuile à la position spécifiée dans la grille.
    public void SetTile(int x, int y, int tileId)
    {
        if (tileId == -1)
            return;

        if (_groundTilemap == null)
        {
            _groundTilemap = GetComponent<Tilemap>();

            _groundTilemap.SetTile(new Vector3Int(x, y, 0), _tileSet.tiles[tileId]);

            _groundTilemap = null;
        }
        else
        {
            _groundTilemap.SetTile(new Vector3Int(x, y, 0), _tileSet.tiles[tileId]);
        }
    }

    // Vérifie si la position spécifiée dans la grille est valide.
    public bool CheckPosition(int x, int y)
    {
        return _grid.CheckPosition(x, y);
    }


    // Récupère le personnage à la position spécifiée dans la grille.
    public Character GetCharacter(int x, int y)
    {
        return _grid.GetCharacter(x, y);
    }

    // Définit la tuile dans la grille et met à jour le Tilemap.
    public void Set(int x, int y, int to)
    {
        _grid.SetTile(x, y, to);
        UpdateTile(x, y);
    }

    // Lit les données du Tilemap et retourne une matrice d'indices de tuiles.
    public int[,] ReadTileMap()
    {
        int[,] tilemapData = new int[0, 0];

        if (_groundTilemap == null)
        {

            _groundTilemap = GetComponent<Tilemap>();

            int size_x = _groundTilemap.size.x;
            int size_y = _groundTilemap.size.y;
            tilemapData = new int[size_x, size_y];

            for (int x = 0; x < size_x; x++)
            {
                for (int y = 0; y < size_y; y++)
                {
                    TileBase tileBase = _groundTilemap.GetTile(new Vector3Int(x, y, 0));
                    int indexTile = _tileSet.tiles.FindIndex(t => t == tileBase);
                    tilemapData[x, y] = indexTile;
                }
            }

            _groundTilemap = null;
        }
        else
        {
            int size_x = _groundTilemap.size.x;
            int size_y = _groundTilemap.size.y;
            tilemapData = new int[size_x, size_y];

            for (int x = 0; x < size_x; x++)
            {
                for (int y = 0; y < size_y; y++)
                {
                    TileBase tileBase = _groundTilemap.GetTile(new Vector3Int(x, y, 0));
                    int indexTile = _tileSet.tiles.FindIndex(t => t == tileBase);
                    tilemapData[x, y] = indexTile;
                }
            }
        }

        return tilemapData;
    }
}
