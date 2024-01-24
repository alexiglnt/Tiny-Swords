using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

// Contrôle du personnage, gestion de la sélection et du déplacement.
public class CharacterControl : MonoBehaviour
{
    //////////////////////////////////
    //          Variables           //
    //////////////////////////////////
    
    [SerializeField]
    private Tilemap _targetTilemap; // Tilemap utilisée pour le chemin et les actions du personnage.

    [SerializeField]
    private Tilemap _highlightTilemap; // Tilemap utilisée pour mettre en surbrillance les cases du chemin.

    [SerializeField]
    private TileBase _highlightTile; // Tuile de surbrillance.

    private Character _selectedCharacter; // Personnage actuellement sélectionné.

    private Controls _controls; // System d'input


    //////////////////////////////////
    //          Fonctions           //
    //////////////////////////////////
    
    private void Awake()
    {
        _controls = new Controls();
    }

    private void OnEnable()
    {
        _controls.Enable();

        _controls.Gameplay.SelectCell.performed += SelectCell;
    }

    private void OnDisable()
    {
        _controls.Disable();

        _controls.Gameplay.SelectCell.performed -= SelectCell;
    }

    private void SelectCell(InputAction.CallbackContext value)
    {
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition); // Conversion de la position de la souris en coordonnées mondiales.
        Vector3Int clickPosition = _targetTilemap.WorldToCell(worldPoint); // Conversion des coordonnées mondiales en coordonnées de la Tilemap.

        if (_selectedCharacter == null)
        {
            _highlightTilemap.ClearAllTiles(); // Efface les tuiles de surbrillance.

            if (!GridManager.Instance.CheckPosition(clickPosition.x, clickPosition.y))
                return;

            _selectedCharacter = GridManager.Instance.GetCharacter(clickPosition.x, clickPosition.y); // Récupère le personnage à la position cliquée.

            if (_selectedCharacter != null)
            {
                List<PathNode> toHighlight = new List<PathNode>();
                GridManager.Instance.Pathfinding.Clear(); // Efface les données de chemin existantes.
                GridManager.Instance.Pathfinding.CalculateWalkableTerrain(
                    clickPosition.x,
                    clickPosition.y,
                    _selectedCharacter.MoveDistance,
                    ref toHighlight
                );

                for (int i = 0; i < toHighlight.Count; i++)
                {
                    _highlightTilemap.SetTile(new Vector3Int(toHighlight[i].xPos, toHighlight[i].yPos, 0), _highlightTile); // Met en surbrillance les cases du chemin.
                }
            }
        }
        else
        {
            _highlightTilemap.ClearAllTiles(); // Efface les tuiles de surbrillance.

            List<PathNode> path = GridManager.Instance.Pathfinding.TrackBackPath(_selectedCharacter, clickPosition.x, clickPosition.y); // Récupère le chemin jusqu'à la position cliquée.

            if (path != null)
            {
                if (path.Count > 0)
                {
                    for (int i = 0; i < path.Count; i++)
                    {
                        _highlightTilemap.SetTile(new Vector3Int(path[i].xPos, path[i].yPos, 0), _highlightTile); // Met en surbrillance les cases du chemin.
                    }
                    _selectedCharacter.GetComponent<MapElement>().MoveCharacter(path[0].xPos, path[0].yPos); // Déplace le personnage à la première case du chemin.
                }

                Deselect(); // Désélectionne le personnage après le déplacement.
            }
        }

        
    }

    // Désélectionne le personnage
    private void Deselect()
    {
        _selectedCharacter = null;
        GridManager.Instance.Pathfinding.Clear(); // Efface les données de chemin après le déplacement.
    }
}
