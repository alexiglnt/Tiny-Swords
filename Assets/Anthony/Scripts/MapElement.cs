using UnityEngine;

// Gère le positionnement et le déplacement d'un élément dans une grille.
public class MapElement : MonoBehaviour
{
    //////////////////////////////////
    //          Variables           //
    //////////////////////////////////

    private GridMap _gridMap; // Référence à la grille.

    private int _xPos = 0; // Position en X dans la grille.
    private int _yPos = 0; // Position en Y dans la grille.


    //////////////////////////////////////////
    //          Fonctions private           //
    //////////////////////////////////////////

    private void OnEnable()
    {
        SetGrid(); // Initialise la référence à la grille.
        PlaceObjectOnGrid(); // Place l'objet sur la grille lors du démarrage.
    }

    private void OnDisable()
    {
        RemoveObjectFromGrid();
    }

    private void SetGrid()
    {
        _gridMap = GridManager.Instance.GroundGridMap; // Récupère la référence à la grille depuis le parent de l'objet.
    }

    // Met à jour la position dans la grille et place l'objet à la nouvelle position.
    private void MoveTo(int targetPosX, int targetPosY)
    {
        _gridMap.SetCharacter(this, targetPosX, targetPosY); // Met à jour la position du personnage dans la grille.
        _xPos = targetPosX; // Met à jour la position en X.
        _yPos = targetPosY; // Met à jour la position en Y.
    }

    // Place l'objet sur la grille en fonction de sa position initiale.
    private void PlaceObjectOnGrid()
    {
        Transform t = transform;
        Vector3 pos = t.position;
        _xPos = (int)pos.x; // Obtient la position en X à partir de la position du transform.
        _yPos = (int)pos.y; // Obtient la position en Y à partir de la position du transform.
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
    
    // Déplace le personnage à la position spécifiée dans la grille.
    public void MoveCharacter(int targetPosX, int targetPosY)
    {
        RemoveObjectFromGrid(); // Retire l'objet de l'ancienne position dans la grille.
        MoveTo(targetPosX, targetPosY); // Met à jour la position dans la grille.
        MoveObject(); // Déplace l'objet dans l'espace du monde.
    }

    // Déplace l'objet dans l'espace du monde en fonction de sa position dans la grille.
    public void MoveObject()
    {
        Vector3 worldPosition = new Vector3(_xPos * 1f + 0.5f, _yPos * 1f + 0.5f, -0.5f);
        transform.position = worldPosition;
    }

    
}
