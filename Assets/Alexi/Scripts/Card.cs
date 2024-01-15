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
    public int attackPoint;
    public int healthPoint;
    public int movevementPoint;

    public void Print()
    {
        Debug.Log(name + ": " + description + " - " + movevementPoint + " movevementPoint, " + attackPoint + "/" + healthPoint);
    }
}
