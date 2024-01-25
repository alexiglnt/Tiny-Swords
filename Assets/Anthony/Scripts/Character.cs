using UnityEngine;

// Classe représentant un personnage dans le jeu.
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(MapElement))]
public class Character : MonoBehaviour
{
    //////////////////////////////////
    //          Variables           //
    //////////////////////////////////

    [SerializeField] 
    private CharacterStats _stats;

    //////////////////
    //      GET     //
    //////////////////

    // ID
    public string Name
    {
        get { return _stats.characterName; }
    }

    public string Description
    {
        get { return _stats.description; }
    }


    // Stats
    public int MaxHealth
    {
        get { return _stats.maxHealth; }
    }

    public int MaxRange
    {
        get { return _stats.maxRange; }
    }

    public int MaxMove
    {
        get { return _stats.maxMove; }
    }

    public int Damage
    {
        get { return _stats.damage; }
    }

    //////////////////////
    //      GET SET     //
    //////////////////////

    private int _health = 0; // Vie actuelle du personage.

    public int Health
    {
        get { return _health; }
        private set { _health = value; }
    }

    private int _currentMove = 0; // Distance restant au personage à parcourir dans ce tour.

    public int CurrentMove
    {
        get { return _currentMove; }
        private set { _currentMove = value; }
    }

    //////////////////////////////////////////
    //          Fonctions private           //
    //////////////////////////////////////////

    private void Start()
    {
        Health = MaxHealth;
        CurrentMove = MaxMove;
    }

    /////////////////////////////////////////
    //          Fonctions public           // 
    /////////////////////////////////////////

    public void TakeDamage(int damage)
    {
        Health -= damage;
        Debug.Log(transform.position.x + " " + transform.position.y + " " + Health);
    }

    public void Attack(Character victim)
    {
        victim.TakeDamage(Damage);
    }
}