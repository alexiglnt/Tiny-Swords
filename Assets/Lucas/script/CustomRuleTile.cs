using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

[CreateAssetMenu(menuName = "Custom Tiles/Advanced Rule Tile")]
public class CustomRuleTile : RuleTile<CustomRuleTile.Neighbor>
{
    [Header("Custom Tile Connections")]
    public TileBase[] tilesToConnect3; // Tiles to connect for condition "2"
    // ... Ajoutez d'autres listes de tuiles pour d'autres conditions si nécessaire

    public class Neighbor : RuleTile.TilingRule.Neighbor
    {
        public const int Specified3 = 3; // Custom constant for tiles to connect to for condition "2"
        // ... Définissez d'autres constantes pour d'autres conditions si nécessaire
    }

    public override bool RuleMatch(int neighbor, TileBase tile)
    {
        switch (neighbor)
        {
            case Neighbor.This: return Check_This(tile);
            case Neighbor.NotThis: return Check_NotThis(tile);
            case Neighbor.Specified3: return Check_Specified(tile, tilesToConnect3);
            // ... Ajoutez d'autres cas pour d'autres constantes si nécessaire
            default: return base.RuleMatch(neighbor, tile);
        }
    }

    private bool Check_This(TileBase tile)
    {
        // Votre logique pour "This"
        return tile == this;
    }

    private bool Check_NotThis(TileBase tile)
    {
        // Logique pour "NotThis"
        return tile != this;
    }


    private bool Check_Specified(TileBase tile, TileBase[] specifiedTiles)
    {
        // Vérifier si le tableau specifiedTiles est initialisé
        if (specifiedTiles == null)
        {
            return false;
        }

        // Vérifier si la tuile est non-null avant d'accéder à ses membres
        if (tile != null)
        {
            // Utiliser Linq pour vérifier si la tuile est dans specifiedTiles
            return specifiedTiles.Contains(tile);
        }
        else
        {
            // Si tile est null, alors il ne peut pas être dans specifiedTiles
            return false;
        }
    }




    // ... Ajoutez d'autres méthodes de vérification pour d'autres listes de tuiles si nécessaire
}