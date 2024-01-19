using UnityEngine;
using UnityEngine.Tilemaps;

// Ce script g�re les tuils sur la tilemap
[RequireComponent(typeof(GridMap))]
[RequireComponent(typeof(Tilemap))]
public class GridManager : MonoBehaviour
{

    //////////////////////////////////
    //          Variables           // 
    //////////////////////////////////

    private Tilemap _tilemap;  // R�f�rence au Tilemap Unity pour les graphiques de la grille.
    private GridMap _grid;      // R�f�rence � la classe GridMap qui g�re la logique de la grille.
    private SaveLoadMap _saveLoadMap;  // R�f�rence � la classe qui g�re la sauvegarde et le chargement de la carte.

    [SerializeField]
    private TileSet _tileSet;   // Ensemble de tuiles utilis� pour mapper les indices de tuiles aux tuiles r�elles.


    //////////////////////////////////////////
    //          Fonctions private           // 
    //////////////////////////////////////////

    private void Awake()
    {
        _tilemap = GetComponent<Tilemap>();
        _grid = GetComponent<GridMap>();
        _saveLoadMap = GetComponent<SaveLoadMap>();

        // Charge la carte � partir d'une sauvegarde.
        _saveLoadMap.Load(_grid);

        // Met � jour le Tilemap pour refl�ter l'�tat actuel de la grille.
        UpdateTileMap();
    }

    // Met � jour le Tilemap en parcourant toute la grille.
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

    // Met � jour la tuile � la position sp�cifi�e dans le Tilemap en fonction de la grille.
    private void UpdateTile(int x, int y)
    {
        int tileId = _grid.GetTile(x, y);

        if (tileId == -1)
            return;

        _tilemap.SetTile(new Vector3Int(x, y, 0), _tileSet.tiles[tileId]);
    }


    //////////////////////////////////////////
    //          Fonctions public           // 
    //////////////////////////////////////////

    // Efface le Tilemap.
    public void Clear()
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

    // D�finit la tuile � la position sp�cifi�e dans la grille.
    public void SetTile(int x, int y, int tileId)
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

    // V�rifie si la position sp�cifi�e dans la grille est valide.
    public bool CheckPosition(int x, int y)
    {
        return _grid.CheckPosition(x, y);
    }


    // R�cup�re le personnage � la position sp�cifi�e dans la grille.
    public Character GetCharacter(int x, int y)
    {
        return _grid.GetCharacter(x, y);
    }

    // D�finit la tuile dans la grille et met � jour le Tilemap.
    public void Set(int x, int y, int to)
    {
        _grid.SetTile(x, y, to);
        UpdateTile(x, y);
    }

    // Lit les donn�es du Tilemap et retourne une matrice d'indices de tuiles.
    public int[,] ReadTileMap()
    {
        if (_tilemap == null)
            _tilemap = GetComponent<Tilemap>();

        int size_x = _tilemap.size.x;
        int size_y = _tilemap.size.y;
        int[,] tilemapData = new int[size_x, size_y];

        for (int x = 0; x < size_x; x++)
        {
            for (int y = 0; y < size_y; y++)
            {
                TileBase tileBase = _tilemap.GetTile(new Vector3Int(x, y, 0));
                int indexTile = _tileSet.tiles.FindIndex(t => t == tileBase);
                tilemapData[x, y] = indexTile;
            }
        }

        _tilemap = null;
        return tilemapData;
    }
}
