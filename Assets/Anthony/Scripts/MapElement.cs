using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Element { Character, Tree}

// G�re le positionnement et le d�placement d'un �l�ment dans une grille.
public class MapElement : MonoBehaviour
{
    //////////////////////////////////
    //          Variables           //
    //////////////////////////////////

    private GridMap _gridMap; // R�f�rence � la grille.

    private int _xPos = 0; // Position en X dans la grille.
    private int _yPos = 0; // Position en Y dans la grille.

    private float _moveSpeed = 5f; // vitesse de d�placement.
    private Coroutine _moveCoroutine; // La coroutine de d�placement en cours

    private Animator _animator; // Le composant Animator du personnage

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
        _gridMap.SetCharacter(this, targetPosX, targetPosY); // Met � jour la position du personnage dans la grille.
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
        _gridMap.SetCharacter(this, _xPos, _yPos); // Place l'objet sur la grille.
    }

    // Retire l'objet de sa position actuelle dans la grille.
    private void RemoveObjectFromGrid()
    {
        _gridMap.ClearCharacter(_xPos, _yPos); // Retire l'objet de la grille.
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
