using UnityEngine;

// Classe responsable de la sauvegarde et du chargement de la carte.
public class SaveLoadMap : MonoBehaviour
{
    //////////////////////////////////
    //          Variables           //
    //////////////////////////////////

    [SerializeField]
    private MapData _mapData;   // Les donn�es de la carte � sauvegarder ou charger.

    [SerializeField]
    private GridMap _gridMap;   // La grille � mettre � jour lors du chargement.

    [SerializeField]
    private GridManager _gridManager;   // Le gestionnaire de grille pour effectuer des op�rations li�es � la grille.


    //////////////////////////////////
    //          Fonctions           // 
    //////////////////////////////////

    // Sauvegarde la carte en lisant les donn�es de la grille via le gestionnaire de grille.
    public void Save()
    {
        int[,] map = _gridManager.ReadTileMap();  // Lit les donn�es de la grille.
        _mapData.Save(map);  // Sauvegarde les donn�es de la carte.
    }

    // Charge la carte en lisant les donn�es de la carte et en les appliquant � la grille via le gestionnaire de grille.
    public void Load()
    {
        _gridManager.Clear();  // Efface la grille avant de charger.

        int width = _mapData.width;  // Largeur de la carte � charger.
        int height = _mapData.height;  // Hauteur de la carte � charger.

        int i = 0;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                _gridManager.SetTile(x, y, _mapData.map[i]);  // Applique les donn�es de la carte � la grille.
                i += 1;
            }
        }
    }

    // Charge la carte en lisant les donn�es de la grille et en les appliquant � la grille sp�cifi�e.
    public void Load(GridMap grid)
    {
        _gridManager.Clear();  // Efface la grille avant de charger.
        _mapData.Load(grid);  // Charge les donn�es de la carte � partir de la grille sp�cifi�e.
    }
}