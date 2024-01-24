using UnityEngine;
using UnityEngine.Tilemaps;

// Ce script gère les tuils sur la tilemap
[RequireComponent(typeof(SaveLoadMap))]
public class GridManager : Singleton<GridManager>
{
    //////////////////////////////////
    //          Variables           // 
    //////////////////////////////////
    
    private GridMap _groundGridMap;      // Référence à la classe GridMap qui gère la logique de la grille.

    public GridMap GroundGridMap 
    {  
        get { return _groundGridMap; } 
        private set { _groundGridMap = value; }
    }


    private Pathfinding _pathfinding; // Système de recherche de chemin.

    public Pathfinding Pathfinding
    {
        get { return _pathfinding; }
        private set { _pathfinding = value; }
    }

    private SaveLoadMap _saveLoadMap;  // Référence à la classe qui gère la sauvegarde et le chargement de la carte.

    [SerializeField]
    private Tilemap _groundTilemap;  // Référence au Tilemap Unity pour les graphiques de la grille.

    [SerializeField]
    private TileSet _tileSet;   // Ensemble de tuiles utilisé pour mapper les indices de tuiles aux tuiles réelles.

    [SerializeField]
    private bool _useMapDataOfSaveLoadMap;   // Si la carte ce génre avec les données de la sauvegarde.


    //////////////////////////////////////////
    //          Fonctions private           // 
    //////////////////////////////////////////

    protected void Awake()
    {
        GroundGridMap = new GridMap();
        Pathfinding = new Pathfinding();
        _saveLoadMap = GetComponent<SaveLoadMap>();

        if (_useMapDataOfSaveLoadMap)
        {
            // Efface la grille avant de charger.
            Clear(); 

            // Charge la carte à partir d'une sauvegarde.
            _saveLoadMap.Load();

            // Met à jour le Tilemap pour refléter l'état actuel de la grille.
            UpdateTileMap();
        }
        
    }

    // Met à jour la tuile à la position spécifiée dans le Tilemap en fonction de la grille.
    private void UpdateTile(int x, int y)
    {
        int tileId = GroundGridMap.GetTile(x, y);

        if (tileId == -1)
            return;

        _groundTilemap.SetTile(new Vector3Int(x, y, 0), _tileSet.tiles[tileId]);
    }


    //////////////////////////////////////////
    //          Fonctions public            //
    //////////////////////////////////////////

    // Efface le Tilemap.
    public void Clear()
    {
        _groundTilemap.ClearAllTiles();
    }

    // Met à jour le Tilemap en parcourant toute la grille.
    public void UpdateTileMap()
    {
        for (int x = 0; x < GroundGridMap.Width; x++)
        {
            for (int y = 0; y < GroundGridMap.Height; y++)
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
        return GroundGridMap.CheckPosition(x, y);
    }


    // Récupère le personnage à la position spécifiée dans la grille.
    public Character GetCharacter(int x, int y)
    {
        return GroundGridMap.GetCharacter(x, y);
    }

    // Définit la tuile dans la grille et met à jour le Tilemap.
    public void Set(int x, int y, int to)
    {
        GroundGridMap.SetTile(x, y, to);
        UpdateTile(x, y);
    }

    // Lit les données du Tilemap et retourne une matrice d'indices de tuiles.
    public int[,] ReadTileMap()
    {
        int[,] tilemapData = new int[0, 0];

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

        return tilemapData;
    }


}
