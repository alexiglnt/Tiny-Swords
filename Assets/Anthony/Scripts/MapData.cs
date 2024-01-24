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


    ///////////////////////////////////
    //          Fonctions            // 
    ///////////////////////////////////

    // Obtient la valeur de la carte � la position sp�cifi�e.
    public int Get(int x, int y)
    {
        int index = x * height + y;

        if (index >= map.Count)
        {
            Debug.LogError("Out of range on the map data!");
            return -1;
        }

        return map[index];
    }
}
