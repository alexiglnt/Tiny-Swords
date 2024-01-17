using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Building", menuName = "Building")]
public class BuildingCard : ScriptableObject
{
    public new string name;
    public string description;

    public Sprite artwork;

    // Point d'attaque, de vie et de mouvement
    public int attackPoint;
    public int healthPoint;

    public void Print()
    {
        Debug.Log(name + ": " + description + " - " + attackPoint + "/" + healthPoint);
    }
}
