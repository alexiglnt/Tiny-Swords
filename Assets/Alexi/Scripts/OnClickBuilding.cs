using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnClickBuilding : MonoBehaviour
{
    public BuildingCard card;
    public BuildingDisplay cardDisplay; // R�f�rence au script CardDisplay
    public GameObject BuildingCardCanvas; // R�f�rence au canvas de la carte

    // Appel� lorsque le clic est d�tect�
    public void OnMouseDown()
    {
        // Afficher ou activer les informations de la carte
        Debug.Log("Click on " + card.name);

        // Passer l'objet Card au script CardDisplay
        cardDisplay.DisplayBuildingCardInformation(card);
    }

    void Start()
    {
        // D�sactiver le canvas de la carte
        BuildingCardCanvas.SetActive(false);
    }
}
