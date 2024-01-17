using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class CharacterControl : MonoBehaviour
{
    [SerializeField]
    private Tilemap _targetTilemap;

    [SerializeField]
    private Tilemap _highlightTilemap;

    [SerializeField]
    private TileBase _highlightTile;

    [SerializeField]
    private GridManager _gridManager;

    private Pathfinding _pathfinding;
    private Character _selectedCharacter;

    private void Awake()
    {
        _pathfinding = _targetTilemap.GetComponent<Pathfinding>();
    }

    private void Update()
    {
        MouseInput();
    }

    private void MouseInput()
    {
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int clickPosition = _targetTilemap.WorldToCell(worldPoint);

        if (Input.GetMouseButtonDown(1))
        {
            _highlightTilemap.ClearAllTiles();

            if (!_gridManager.CheckPosition(clickPosition.x, clickPosition.y))
                return;

            _selectedCharacter = _gridManager.GetCharacter(clickPosition.x, clickPosition.y);
            if(_selectedCharacter != null)
            {
                List<PathNode> toHighlight = new List<PathNode>();
                _pathfinding.Clear();
                _pathfinding.CalculateWalkableTerrain(
                    clickPosition.x, 
                    clickPosition.y, 
                    _selectedCharacter.MoveDistance, 
                    ref toHighlight
                );

                for (int i = 0; i < toHighlight.Count; i++)
                {
                    _highlightTilemap.SetTile(new Vector3Int(toHighlight[i].xPos, toHighlight[i].yPos, 0), _highlightTile);
                }
            }
        }

        if (Input.GetMouseButtonDown(0)) 
        {
            if (_selectedCharacter == null)
                return;

            _highlightTilemap.ClearAllTiles();

            List<PathNode> path = _pathfinding.TrackBackPath(_selectedCharacter, clickPosition.x, clickPosition.y);

            if (path!= null)
            {
                if (path.Count > 0)
                {
                    for (int i = 0; i < path.Count; i++)
                    {
                        _highlightTilemap.SetTile(new Vector3Int(path[i].xPos, path[i].yPos, 0), _highlightTile);
                    }
                    _selectedCharacter.GetComponent<MapElement>().MoveCharacter(path[0].xPos, path[0].yPos);
                }
                
                Deselect();
            }
        }
    }

    private void Deselect()
    {
        _selectedCharacter = null;
        _pathfinding.Clear();
    }
}
