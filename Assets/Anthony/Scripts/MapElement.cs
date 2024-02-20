using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// G�re le positionnement et le d�placement d'un �l�ment dans une grille.
public class MapElement : MonoBehaviour
{
    //////////////////////////////////
    //          Variables           //
    //////////////////////////////////

    // tableau de bool pour specifier quelle zone ocupe l element
    private bool[,] _occupiedArea;

    public bool[,] OccupiedArea
    {
        get { return _occupiedArea; }
        private set { _occupiedArea = value; }
    }

    private GridMap _gridMap; // R�f�rence � la grille.

    private int _xPos = 0; // Position en X dans la grille.
    private int _yPos = 0; // Position en Y dans la grille.

    private float _moveSpeed = 5f; // vitesse de d�placement.
    private Coroutine _moveCoroutine; // La coroutine de d�placement en cours

    private Animator _animator; // Le composant Animator du personnage

    [SerializeField]
    private OccupiedAreaData _occupiedAreaData; // Donnees de l'emplacement de l element

    //////////////////////////////////////////
    //          Fonctions private           //
    //////////////////////////////////////////

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        SetGrid(); // Initialise la r�f�rence � la grille.

        // Initialise les placement de l objet
        OccupiedArea = new bool[3, 3];

        if (_occupiedAreaData != null)
        {
            OccupiedArea[0, 0] = _occupiedAreaData.x_1y_1;
            OccupiedArea[1, 0] = _occupiedAreaData.x0y_1;
            OccupiedArea[2, 0] = _occupiedAreaData.x1y_1;

            OccupiedArea[0, 1] = _occupiedAreaData.x_1y0;
            OccupiedArea[1, 1] = _occupiedAreaData.x0y0;
            OccupiedArea[2, 1] = _occupiedAreaData.x1y0;

            OccupiedArea[0, 2] = _occupiedAreaData.x_1y1;
            OccupiedArea[1, 2] = _occupiedAreaData.x0y1;
            OccupiedArea[2, 2] = _occupiedAreaData.x1y1;

            for (int x = 0; x <= 2; x++)
            {
                for (int y = 0; y <= 2; y++)
                {
                    Debug.Log("x : " + x + ", y : " + y + ", value = " + OccupiedArea[x, y]);
                }
            }
            
        }
        else
        {
            for (int x = 0; x < 2; x++)
            {
                for (int y = 0; y < 2; y++)
                {
                    OccupiedArea[x, y] = false;
                }
            }
            OccupiedArea[1, 1] = true;
        }

        PlaceObjectOnGrid(); // Place l'objet sur la grille lors du d�marrage.
    }

    private void OnDisable()
    {
        RemoveObjectFromGrid();
    }

    private void SetGrid()
    {
        _gridMap = GridManager.Instance.GridMap; // R�cup�re la r�f�rence � la grille depuis le parent de l'objet.
    }

    // Met � jour la position dans la grille et place l'objet � la nouvelle position.
    private void MoveTo(int targetPosX, int targetPosY)
    {
        _gridMap.SetMapElement(this, targetPosX, targetPosY); // Met � jour la position de l'�l�ment dans la grille.
        _xPos = targetPosX; // Met � jour la position en X.
        _yPos = targetPosY; // Met � jour la position en Y.
    }

    // Place l'objet sur la grille en fonction de sa position initiale.
    private void PlaceObjectOnGrid()
    {
        Transform t = transform;
        Vector3 pos = t.position;
        _xPos = (int)pos.x; // Obtient la position en X � partir de la position du transform.
        _yPos = (int)pos.y; // Obtient la position en Y � partir de la position du transform.

        // PLace l objet sur les diferent case qu il occupe
        for (int x = 0; x <= 2; x++)
        {
            for (int y = 0; y <= 2; y++)
            {
                if (OccupiedArea[x, y] == true)
                {
                    _gridMap.SetMapElement(this, _xPos + (x-1), _yPos + (y-1)); // Place l'objet sur la grille.
                }
            }
        }
        
    }

    // Retire l'objet de sa position actuelle dans la grille.
    private void RemoveObjectFromGrid()
    {
        // Retire l objet sur les diferent case qu il occupe
        for (int x = 0; x <= 2; x++)
        {
            for (int y = 0; y <= 2; y++)
            {
                if (OccupiedArea[x, y] == true)
                {
                    _gridMap.ClearMapElement(_xPos + (x - 1), _yPos + (y - 1)); // Retire l'objet de la grille.
                }
            }
        }
        
    }


    /////////////////////////////////////////
    //          Fonctions public           // 
    /////////////////////////////////////////
    
    // D�place le personnage � la position sp�cifi�e dans la grille.
    public void MoveCharacter(int targetPosX, int targetPosY, List<PathNode> path)
    {
        RemoveObjectFromGrid(); // Retire l'objet de l'ancienne position dans la grille.
        MoveTo(targetPosX, targetPosY); // Met � jour la position dans la grille.
        MoveObject(path); // D�place l'objet dans l'espace du monde.
    }

    // D�place l'objet dans l'espace du monde en fonction de sa position dans la grille.
    public void MoveObject(List<PathNode> path)
    {
        //Vector3 worldPosition = new Vector3(_xPos * 1f + 0.5f, _yPos * 1f + 0.5f, -0.5f);
        //transform.position = worldPosition;

        if (_moveCoroutine != null)
            StopCoroutine(_moveCoroutine);

        _moveCoroutine = StartCoroutine(MoveSmooth(path));
    }
    private IEnumerator MoveSmooth(List<PathNode> path)
    {
        for (int i = path.Count - 1; i >= 0; --i)
        {
            Vector3 targetPosition = new Vector3(path[i].xPos * 1f + 0.5f, path[i].yPos * 1f + 0.5f, -0.5f);

            if (_animator != null)
            {
                // Active l'animation de marche
                _animator.SetBool("IsWalking", true);

                // D�termine la direction du d�placement
                Vector3 direction = (targetPosition - transform.position).normalized;

                // Inverse le sprite si le personnage se d�place vers la gauche
                if (direction.x < 0)
                    transform.localScale = new Vector3(-1, 1, 1);
                else if (direction.x > 0)
                    transform.localScale = new Vector3(1, 1, 1);
            }

            // D�place le personnage de mani�re fluide
            while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
            {
                // Utilisez Mathf.MoveTowards pour d�placer de mani�re fluide entre les positions.
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, _moveSpeed * Time.deltaTime);
                yield return null;
            }

            transform.position = targetPosition; // Assure que la position finale est exacte

            // Optionnel : Ajoutez un petit d�lai entre chaque d�placement.
            yield return new WaitForSeconds(0.01f);
        }

        if (_animator != null)
        {
            // D�sactive l'animation de marche
            _animator.SetBool("IsWalking", false);
        }

        _moveCoroutine = null; // R�initialise la coroutine apr�s le d�placement complet
    }


}
