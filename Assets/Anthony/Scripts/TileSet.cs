using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// TileSet utilis� pour d�finir les id des tuiles
[CreateAssetMenu]
public class TileSet : ScriptableObject
{
    public List<TileBase> tiles; // Liste de tuiles du TileSet.
}