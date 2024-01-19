using UnityEngine;

// Classe responsable de la sauvegarde et du chargement de la carte.
public class SaveLoadMap : MonoBehaviour
{
    //////////////////////////////////
    //          Variables           //
    //////////////////////////////////

    [SerializeField]
    private MapData _mapData;   // Les données de la carte à sauvegarder ou charger.

    [SerializeField]
    private GridMap _gridMap;   // La grille à mettre à jour lors du chargement.

    [SerializeField]
    private GridManager _gridManager;   // Le gestionnaire de grille pour effectuer des opérations liées à la grille.


    //////////////////////////////////
    //          Fonctions           // 
    //////////////////////////////////

    // Sauvegarde la carte en lisant les données de la grille via le gestionnaire de grille.
    public void Save()
    {
        int[,] map = _gridManager.ReadTileMap();  // Lit les données de la grille.
        _mapData.Save(map);  // Sauvegarde les données de la carte.
    }

    // Charge la carte en lisant les données de la carte et en les appliquant à la grille via le gestionnaire de grille.
    public void Load()
    {
        _gridManager.Clear();  // Efface la grille avant de charger.

        int width = _mapData.width;  // Largeur de la carte à charger.
        int height = _mapData.height;  // Hauteur de la carte à charger.

        int i = 0;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                _gridManager.SetTile(x, y, _mapData.map[i]);  // Applique les données de la carte à la grille.
                i += 1;
            }
        }
    }

    // Charge la carte en lisant les données de la grille et en les appliquant à la grille spécifiée.
    public void Load(GridMap grid)
    {
        _gridManager.Clear();  // Efface la grille avant de charger.
        _mapData.Load(grid);  // Charge les données de la carte à partir de la grille spécifiée.
    }
}