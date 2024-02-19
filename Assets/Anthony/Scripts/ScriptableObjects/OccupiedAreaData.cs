using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

[CreateAssetMenu(fileName = "NewOccupiedAreaData")]
public class OccupiedAreaData : ScriptableObject
{
    [Header("y = 1")]
    public bool x_1y1;
    public bool x0y1;
    public bool x1y1;

    [Header("y = 0")]
    public bool x_1y0;
    public bool x0y0;
    public bool x1y0;

    [Header("y = -1")]
    public bool x_1y_1;
    public bool x0y_1;
    public bool x1y_1;
}
