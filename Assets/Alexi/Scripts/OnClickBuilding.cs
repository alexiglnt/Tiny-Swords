using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnClickBuilding : MonoBehaviour
{
    public BuildingCard card;
    public BuildingDisplay cardDisplay; // Référence au script CardDisplay
    public GameObject BuildingCardCanvas; // Référence au canvas de la carte

    // Appelé lorsque le clic est détecté
    public void OnMouseDown()
    {
        // Afficher ou activer les informations de la carte
        Debug.Log("Click on " + card.name);

        // Passer l'objet Card au script CardDisplay
        cardDisplay.DisplayBuildingCardInformation(card);
    }

    void Start()
    {
        // Désactiver le canvas de la carte
        BuildingCardCanvas.SetActive(false);
    }
}
