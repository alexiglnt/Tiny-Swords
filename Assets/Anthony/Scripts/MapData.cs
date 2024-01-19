using System.Collections.Generic;
using UnityEngine;

// Class qui sauvgarde les tuiles d'une carte
[CreateAssetMenu]
public class MapData : ScriptableObject
{
    //////////////////////////////////
    //          Variables           // 
    //////////////////////////////////

    public int width, height; // Dimensions de la carte.

    public List<int> map; // Liste de donn�es de carte.


    //////////////////////////////////////////
    //          Fonctions private           // 
    //////////////////////////////////////////

    // Obtient la valeur de la carte � la position sp�cifi�e.
    private int Get(int x, int y)
    {
        int index = x * height + y;

        if (index >= map.Count)
        {
            Debug.LogError("Out of range on the map data!");
            return -1;
        }

        return map[index];
    }


    /////////////////////////////////////////
    //          Fonctions public           // 
    /////////////////////////////////////////

    // Charge les donn�es de la carte dans une grille.
    public void Load(GridMap gridMap)
    {
        gridMap.Init(width, height); // Initialise la grille avec les dimensions sp�cifi�es.

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                gridMap.SetTile(x, y, Get(x, y)); // Remplit la grille avec les donn�es de carte.
            }
        }
    }

    // Sauvegarde les donn�es de la grille dans l'objet scriptable.
    public void Save(GridMap gridMap)
    {
        height = gridMap.Height;
        width = gridMap.Width;

        map = new List<int>();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                map.Add(gridMap.GetTile(x, y)); // Ajoute les donn�es de la grille � la liste.
            }
        }
    }

    // Sauvegarde une carte repr�sent�e par un tableau d'entiers.
    public void Save(int[,] map)
    {
        width = map.GetLength(0);
        height = map.GetLength(1);

        this.map = new List<int>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                this.map.Add(map[x, y]); // Ajoute les donn�es du tableau � la liste.
            }
        }
        UnityEditor.EditorUtility.SetDirty(this); // Marque l'objet scriptable comme modifi� pour l'�diteur Unity.
    }
}
