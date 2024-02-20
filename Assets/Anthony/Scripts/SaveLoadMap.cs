using System.Collections.Generic;
using UnityEngine;

// Classe responsable de la sauvegarde et du chargement de la carte.
public class SaveLoadMap : MonoBehaviour
{
    //////////////////////////////////
    //          Variables           //
    //////////////////////////////////

    [SerializeField]
    private MapData _groundMapData;   // Les donn�es de la carte du sol � sauvegarder ou charger.

    //[SerializeField]
    //private MapData _obstacleMapData;   // Les donn�es de la carte des obstacles � sauvegarder ou charger.

    //////////////////////////////////
    //          Fonctions           // 
    //////////////////////////////////

    // Sauvegarde la carte en lisant les donn�es de la grille via le gestionnaire de grille.
    public void Save()
    {
        int[,] groundMap = GridManager.Instance.ReadGroundTileMap();  // Lit les donn�es de la grille du sol.
        int[,] obstacleMap = GridManager.Instance.ReadObstacleTileMap();  // Lit les donn�es de la grille des obstacle.

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
                _groundMapData.map.Add(groundMap[x, y]); // Ajoute les donn�es du tableau � la liste du sol.
                //_obstacleMapData.map.Add(obstacleMap[x, y]); // Ajoute les donn�es du tableau � la liste des obstacles.
            }
        }
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(_groundMapData); // Marque l'objet scriptable comme modifi� pour l'�diteur Unity.
#endif
    }

    // Sauvegarde les donn�es de la grille dans l'objet scriptable.
    public void Save(GridMap gridMap)
    {
        _groundMapData.height = gridMap.Height;
        _groundMapData.width = gridMap.Width;

        _groundMapData.map = new List<int>();
        for (int x = 0; x < _groundMapData.width; x++)
        {
            for (int y = 0; y < _groundMapData.height; y++)
            {
                _groundMapData.map.Add(gridMap.GetTile(x, y)); // Ajoute les donn�es de la grille � la liste.
            }
        }
    }

    // Charge la carte en lisant les donn�es de la carte et en les appliquant � la grille via le gestionnaire de grille.
    public void LoadTilemap()
    {
        GridManager.Instance.Clear();  // Efface la grille avant de charger.

        int width = _groundMapData.width;  // Largeur de la carte � charger.
        int height = _groundMapData.height;  // Hauteur de la carte � charger.

        int i = 0;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GridManager.Instance.SetTile(x, y, _groundMapData.map[i]);  // Applique les donn�es de la carte � la grille.
                i += 1;
            }
        }
    }

    // Charge la carte en lisant les donn�es de la grille et en les appliquant � la grille.
    public void Load()
    {
        GridManager.Instance.GridMap.Init(_groundMapData.width, _groundMapData.height); // Initialise la grille avec les dimensions sp�cifi�es.

        for (int x = 0; x < _groundMapData.width; x++)
        {
            for (int y = 0; y < _groundMapData.height; y++)
            {
                GridManager.Instance.GridMap.SetTile(x, y, _groundMapData.Get(x, y)); // Remplit la grille avec les donn�es de carte.
            }
        }


        GridManager.Instance.Pathfinding.UpdateGrid();
    }
}