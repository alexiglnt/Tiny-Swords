using UnityEngine;

// Classe représentant un personnage dans le jeu.
[RequireComponent(typeof(MapElement))]
public class Character : MonoBehaviour
{
    [SerializeField]
    private int _moveDistance = 2; // Distance maximale que le personnage peut parcourir en un un tour.

    public int MoveDistance
    {
        get { return _moveDistance; }
        private set { _moveDistance = value; }
    }

    [SerializeField]
    private string _name; // Nom du personnage.

    public string Name
    {
        get { return _name; }
        private set { _name = value; }
    }
}