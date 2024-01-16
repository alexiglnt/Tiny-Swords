using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PlayerSortingOrder : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public int mapHeight = 100;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Adjust as needed; using mapHeight ensures the player's sorting order is positive and inversely related to the Y position.
        spriteRenderer.sortingOrder = Mathf.RoundToInt((mapHeight - transform.position.y) * 100f);
    }
}

