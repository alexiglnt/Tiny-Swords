using UnityEngine;
using UnityEngine.Tilemaps;

// Ce script g�re les tuils sur la tilemap
[RequireComponent(typeof(SaveLoadMap))]
public class GridManager : Singleton<GridManager>
{
    //////////////////////////////////
    //          Variables           // 
    //////////////////////////////////
    
    private GridMap _groundGridMap;      // R�f�rence � la classe GridMap qui g�re la logique de la grille.

    public GridMap GroundGridMap 
    {  
        get { return _groundGridMap; } 
        private set { _groundGridMap = value; }
    }


    private Pathfinding _pathfinding; // Syst�me de recherche de chemin.

    public Pathfinding Pathfinding
    {
        get { return _pathfinding; }
        private set { _pathfinding = value; }
    }

    private SaveLoadMap _saveLoadMap;  // R�f�rence � la classe qui g�re la sauvegarde et le chargement de la carte.

    [SerializeField]
    private Tilemap _groundTilemap;  // R�f�rence au Tilemap Unity pour les graphiques de la grille.

    [SerializeField]
    private TileSet _tileSet;   // Ensemble de tuiles utilis� pour mapper les indices de tuiles aux tuiles r�elles.

    [SerializeField]
    private bool _useMapDataOfSaveLoadMap;   // Si la carte ce g�nre avec les donn�es de la sauvegarde.


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

            // Charge la carte � partir d'une sauvegarde.
            _saveLoadMap.Load();

            // Met � jour le Tilemap pour refl�ter l'�tat actuel de la grille.
            UpdateTileMap();
        }
        
    }

    // Met � jour la tuile � la position sp�cifi�e dans le Tilemap en fonction de la grille.
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

    // Met � jour le Tilemap en parcourant toute la grille.
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

    // D�finit la tuile � la position sp�cifi�e dans la grille.
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

    // V�rifie si la position sp�cifi�e dans la grille est valide.
    public bool CheckPosition(int x, int y)
    {
        return GroundGridMap.CheckPosition(x, y);
    }


    // R�cup�re le personnage � la position sp�cifi�e dans la grille.
    public Character GetCharacter(int x, int y)
    {
        return GroundGridMap.GetCharacter(x, y);
    }

    // D�finit la tuile dans la grille et met � jour le Tilemap.
    public void Set(int x, int y, int to)
    {
        GroundGridMap.SetTile(x, y, to);
        UpdateTile(x, y);
    }

    // Lit les donn�es du Tilemap et retourne une matrice d'indices de tuiles.
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
