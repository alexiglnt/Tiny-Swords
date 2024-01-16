using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridControl : MonoBehaviour
{
    [SerializeField]
    private Tilemap _targetTilemap;

    [SerializeField]
    private GridManager _gridManager;

    [SerializeField] 
    private TileBase _highlightTile;

    Pathfinding _pathfinding;

    private int _currentX = 0;
    private int _currentY = 0;
    private int _targetPosX = 0;
    private int _targetPosY = 0;

    private void Start()
    {
        _pathfinding = _gridManager.GetComponent<Pathfinding>();
    }

    private void Update()
    {
        MousInput();
    }

    private void MousInput()
    {
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int clickPosition = _targetTilemap.WorldToCell(worldPoint);

        

        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("test");

            _targetTilemap.ClearAllTiles();
            //_gridManager.Set(clickPosition.x, clickPosition.y, 1);

            _targetPosX = clickPosition.x;
            _targetPosY = clickPosition.y;

            List<PathNode> path = _pathfinding.FindPath(_currentX, _currentY, _targetPosX, _targetPosY);

            if (path != null)
            {
                for (int i = 0; i < path.Count; i++)
                {
                    _targetTilemap.SetTile(new Vector3Int(path[i].xPos, path[i].yPos, 0), _highlightTile);
                }

                _currentX = _targetPosX;
                _currentY = _targetPosY;
            }
        }

        if (Input.GetMouseButtonDown(1)) 
        {
            Character character = _gridManager.GetCharacter(clickPosition.x, clickPosition.y);
            if (character != null)
            {
                Debug.Log("Character in the cell " + character.Name);
            }
        }

    }
}
