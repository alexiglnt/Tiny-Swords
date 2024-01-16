using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

[CreateAssetMenu(menuName = "Custom Tiles/Advanced Rule Tile")]
public class CustomRuleTile : RuleTile<CustomRuleTile.Neighbor>
{
    [Header("Custom Tile Connections")]
    public TileBase[] tilesToConnect3; // Tiles to connect for condition "2"
    // ... Ajoutez d'autres listes de tuiles pour d'autres conditions si n�cessaire

    public class Neighbor : RuleTile.TilingRule.Neighbor
    {
        public const int Specified3 = 3; // Custom constant for tiles to connect to for condition "2"
        // ... D�finissez d'autres constantes pour d'autres conditions si n�cessaire
    }

    public override bool RuleMatch(int neighbor, TileBase tile)
    {
        switch (neighbor)
        {
            case Neighbor.This: return Check_This(tile);
            case Neighbor.NotThis: return Check_NotThis(tile);
            case Neighbor.Specified3: return Check_Specified(tile, tilesToConnect3);
            // ... Ajoutez d'autres cas pour d'autres constantes si n�cessaire
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
        // V�rifier si le tableau specifiedTiles est initialis�
        if (specifiedTiles == null)
        {
            return false;
        }

        // V�rifier si la tuile est non-null avant d'acc�der � ses membres
        if (tile != null)
        {
            // Utiliser Linq pour v�rifier si la tuile est dans specifiedTiles
            return specifiedTiles.Contains(tile);
        }
        else
        {
            // Si tile est null, alors il ne peut pas �tre dans specifiedTiles
            return false;
        }
    }




    // ... Ajoutez d'autres m�thodes de v�rification pour d'autres listes de tuiles si n�cessaire
}