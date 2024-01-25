using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCharaterStats")]
public class CharacterStats : ScriptableObject
{
    [Header("ID")]
    public string characterName;
    public string description; 

    [Header("Stats")]
    public int maxHealth;
    public int maxRange;
    public int maxMove;
    public int damage;
}
