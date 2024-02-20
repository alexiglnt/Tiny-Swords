using System.Collections.Generic;
using UnityEngine;

// Classe responsable de la sauvegarde et du chargement de la carte.
public class SaveLoadMap : MonoBehaviour
{
    //////////////////////////////////
    //          Variables           //
    //////////////////////////////////

    [SerializeField]
    private MapData _groundMapData;   // Les données de la carte du sol à sauvegarder ou charger.

    //[SerializeField]
    //private MapData _obstacleMapData;   // Les données de la carte des obstacles à sauvegarder ou charger.

    //////////////////////////////////
    //          Fonctions           // 
    //////////////////////////////////

    // Sauvegarde la carte en lisant les données de la grille via le gestionnaire de grille.
    public void Save()
    {
        int[,] groundMap = GridManager.Instance.ReadGroundTileMap();  // Lit les données de la grille du sol.
        int[,] obstacleMap = GridManager.Instance.ReadObstacleTileMap();  // Lit les données de la grille des obstacle.

        _groundMapData.width = groundMap.GetLength(0);
        _groundMapData.height = groundMap.GetLength(1);
        _groundMapData.map = new List<int>();

        //_obstacleMapData.width = obstacleMap.GetLength(0);
        //_obstacleMapData.height = obstacleMap.GetLength(1);
        //_obstacleMapData.map = new List<int>();

        for (int x = 0; x < _groundMapData.width; x++)
        {
            for (int y = 0; y < _groundMapData.height; y++)
            {
                _groundMapData.map.Add(groundMap[x, y]); // Ajoute les données du tableau à la liste du sol.
                //_obstacleMapData.map.Add(obstacleMap[x, y]); // Ajoute les données du tableau à la liste des obstacles.
            }
        }
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(_groundMapData); // Marque l'objet scriptable comme modifié pour l'éditeur Unity.
#endif
    }

    // Sauvegarde les données de la grille dans l'objet scriptable.
    public void Save(GridMap gridMap)
    {
        _groundMapData.height = gridMap.Height;
        _groundMapData.width = gridMap.Width;

        _groundMapData.map = new List<int>();
        for (int x = 0; x < _groundMapData.width; x++)
        {
            for (int y = 0; y < _groundMapData.height; y++)
            {
                _groundMapData.map.Add(gridMap.GetTile(x, y)); // Ajoute les données de la grille à la liste.
            }
        }
    }

    // Charge la carte en lisant les données de la carte et en les appliquant à la grille via le gestionnaire de grille.
    public void LoadTilemap()
    {
        GridManager.Instance.Clear();  // Efface la grille avant de charger.

        int width = _groundMapData.width;  // Largeur de la carte à charger.
        int height = _groundMapData.height;  // Hauteur de la carte à charger.

        int i = 0;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GridManager.Instance.SetTile(x, y, _groundMapData.map[i]);  // Applique les données de la carte à la grille.
                i += 1;
            }
        }
    }

    // Charge la carte en lisant les données de la grille et en les appliquant à la grille.
    public void Load()
    {
        GridManager.Instance.GridMap.Init(_groundMapData.width, _groundMapData.height); // Initialise la grille avec les dimensions spécifiées.

        for (int x = 0; x < _groundMapData.width; x++)
        {
            for (int y = 0; y < _groundMapData.height; y++)
            {
                GridManager.Instance.GridMap.SetTile(x, y, _groundMapData.Get(x, y)); // Remplit la grille avec les données de carte.
            }
        }


        GridManager.Instance.Pathfinding.UpdateGrid();
    }
}