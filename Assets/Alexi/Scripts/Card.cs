using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Card")]
public class Card : ScriptableObject
{
    public new string name;
    public string description;

    public Sprite artwork;

    // Point d'attaque, de vie et de mouvement
    public int attackMax;
    public int scopeMax;

    public int healthCurrent;
    public int movementCurrent;
    public int healthMax;
    public int movevementMax;
    public void Print()
    {
        Debug.Log(name + ": " + description + " - " + movevementMax + " movevementPoint, " + attackMax + "/" + healthMax);
    }
}
