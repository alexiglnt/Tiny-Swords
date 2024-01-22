using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

// Contr�le du personnage, gestion de la s�lection et du d�placement.
public class CharacterControl : MonoBehaviour
{
    //////////////////////////////////
    //          Variables           //
    //////////////////////////////////
    
    [SerializeField]
    private Tilemap _targetTilemap; // Tilemap utilis�e pour le chemin et les actions du personnage.

    [SerializeField]
    private Tilemap _highlightTilemap; // Tilemap utilis�e pour mettre en surbrillance les cases du chemin.

    [SerializeField]
    private TileBase _highlightTile; // Tuile de surbrillance.

    [SerializeField]
    private GridManager _gridManager; // Gestionnaire de la grille.

    private Pathfinding _pathfinding; // Syst�me de recherche de chemin.
    private Character _selectedCharacter; // Personnage actuellement s�lectionn�.

    private Controls _controls; // System d'input


    //////////////////////////////////
    //          Fonctions           // 
    //////////////////////////////////
    
    private void Awake()
    {
        _pathfinding = _gridManager.GetComponent<Pathfinding>(); // R�cup�ration du composant Pathfinding de la Tilemap cible.
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

    //private void Update()
    //{
    //    SelectCell(); // Gestion de l'entr�e de la souris.
    //}



    private void SelectCell(InputAction.CallbackContext value)
    {
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition); // Conversion de la position de la souris en coordonn�es mondiales.
        Vector3Int clickPosition = _targetTilemap.WorldToCell(worldPoint); // Conversion des coordonn�es mondiales en coordonn�es de la Tilemap.

        if (_selectedCharacter == null)
        {
            _highlightTilemap.ClearAllTiles(); // Efface les tuiles de surbrillance.

            if (!_gridManager.CheckPosition(clickPosition.x, clickPosition.y))
                return;

            _selectedCharacter = _gridManager.GetCharacter(clickPosition.x, clickPosition.y); // R�cup�re le personnage � la position cliqu�e.

            if (_selectedCharacter != null)
            {
                List<PathNode> toHighlight = new List<PathNode>();
                _pathfinding.Clear(); // Efface les donn�es de chemin existantes.
                _pathfinding.CalculateWalkableTerrain(
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

            List<PathNode> path = _pathfinding.TrackBackPath(_selectedCharacter, clickPosition.x, clickPosition.y); // R�cup�re le chemin jusqu'� la position cliqu�e.

            if (path != null)
            {
                if (path.Count > 0)
                {
                    for (int i = 0; i < path.Count; i++)
                    {
                        _highlightTilemap.SetTile(new Vector3Int(path[i].xPos, path[i].yPos, 0), _highlightTile); // Met en surbrillance les cases du chemin.
                    }
                    _selectedCharacter.GetComponent<MapElement>().MoveCharacter(path[0].xPos, path[0].yPos); // D�place le personnage � la premi�re case du chemin.
                }

                Deselect(); // D�s�lectionne le personnage apr�s le d�placement.
            }
        }

        
    }

    // D�s�lectionne le personnage
    private void Deselect()
    {
        _selectedCharacter = null;
        _pathfinding.Clear(); // Efface les donn�es de chemin apr�s le d�placement.
    }
}
