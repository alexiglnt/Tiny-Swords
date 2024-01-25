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
    private Tilemap _selectTilemap; // Tilemap utilisée pour les cellules selectionées

    [SerializeField]
    private TileBase _highlightTile; // Tuile de surbrillance.

    [SerializeField]
    private TileBase _selectTile; // Tuile de de la selection.

    private Character _selectedCharacter; // Personnage actuellement sélectionné.

    private Controls _controls; // System d'input

    private bool _attackMode; // Si le mode attack et activer


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
        _controls.Gameplay.AttackMode.performed += SwitchAttackMode;
    }

    private void OnDisable()
    {
        _controls.Disable();

        _controls.Gameplay.SelectCell.performed -= SelectCell;
        _controls.Gameplay.AttackMode.performed -= SwitchAttackMode;
    }

    private void SelectCell(InputAction.CallbackContext value)
    {
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition); // Conversion de la position de la souris en coordonnées mondiales.
        Vector3Int clickPosition = _targetTilemap.WorldToCell(worldPoint); // Conversion des coordonnées mondiales en coordonnées de la Tilemap.

        _highlightTilemap.ClearAllTiles(); // Efface les tuiles de surbrillance.
        _selectTilemap.ClearAllTiles(); // Efface la selection

        if (_selectedCharacter == null)
        {
            if (!GridManager.Instance.CheckPosition(clickPosition.x, clickPosition.y))
                return;

            _selectedCharacter = GridManager.Instance.GetCharacter(clickPosition.x, clickPosition.y); // Récupère le personnage à la position cliquée.

            if (_selectedCharacter != null)
            {
                // TODO : crée une gridMap pour l'attaque

                int range = 0;

                if (_attackMode)
                {
                    range = _selectedCharacter.MaxRange;
                }
                else 
                {
                    range = _selectedCharacter.CurrentMove;
                }

                _selectTilemap.SetTile(new Vector3Int(clickPosition.x, clickPosition.y, 0), _selectTile);

                List<PathNode> toHighlight = new List<PathNode>();
                GridManager.Instance.Pathfinding.Clear(); // Efface les données de chemin existantes.
                GridManager.Instance.Pathfinding.CalculateWalkableTerrain(
                    clickPosition.x,
                    clickPosition.y,
                    range,
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

            List<PathNode> path = GridManager.Instance.Pathfinding.TrackBackPath(clickPosition.x, clickPosition.y); // Récupère le chemin jusqu'à la position cliquée.

            if (path != null)
            {
                if (path.Count > 0)
                {
                    if (_attackMode)
                    {
                       Character victim = GridManager.Instance.GetCharacter(path[0].xPos, path[0].yPos); // Récupère le personnage à attaquer.

                        if (victim != null)
                        {
                            _selectedCharacter.Attack(victim);
                        }
                    }
                    else
                    {
                        _selectedCharacter.GetComponent<MapElement>().MoveCharacter(path[0].xPos, path[0].yPos, path); // Déplace le personnage à la première case du chemin.
                    }
                    
                }

                Deselect(); // Désélectionne le personnage après le déplacement.
            }
        }

        
    }

    public void SwitchAttackMode(InputAction.CallbackContext value)
    {
        if (_selectedCharacter == null) // On peut selement changer de mode quand aucoun personage n'est selectioné
            _attackMode = !_attackMode;
    }

    // Désélectionne le personnage
    private void Deselect()
    {
        _selectedCharacter = null;
        GridManager.Instance.Pathfinding.Clear(); // Efface les données de chemin après le déplacement.
    }
}
