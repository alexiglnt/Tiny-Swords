using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField]
    private int _moveDistance = 2;

    public int MoveDistance
    { 
        get { return _moveDistance; }
        private set { _moveDistance = value; } 
    }

    [SerializeField]
    private string _name;

    public string Name
    { 
        get { return _name; } 
        private set {  _name = value; } 
    }
}
