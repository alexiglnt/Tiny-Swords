using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField]
    private int _moveDistance = 2;

    [SerializeField]
    private string _name;

    public string Name
    { 
        get { return _name; } 
        private set {  _name = value; } 
    }
}
