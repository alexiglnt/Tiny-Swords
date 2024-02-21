using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnClickCharacter : MonoBehaviour
{
    public Card card;
    public CardDisplay cardDisplay; // Référence au script CardDisplay
    public GameObject CardCanvas; // Référence au canvas de la carte


    // Appelé lorsque le clic est détecté
    public void OnMouseDown()
    {
        // Afficher ou activer les informations de la carte
        Debug.Log("Click on " + card.name);

        // Passer l'objet Card au script CardDisplay
        cardDisplay.DisplayCardInformation(card);
        //SwitchCanvasVisibility();


        //btnShowCard.SetActive(true);
    }

    void Update()
    {
        // Vérifier si le clic droit de la souris est enfoncé
        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log("Click right ");
            // Afficher ou activer les informations de la carte
            DisplayInformation();
        }
    }

    public void DisplayInformation()
    {
        // Passer l'objet Card au script CardDisplay
        cardDisplay.DisplayCardInformation(card);
    }

    void Start()
    {
        // Désactiver le canvas de la carte
        CardCanvas.SetActive(false);
    }

    void SwitchCanvasVisibility()
    {
        // Si le canvas est actif, le désactiver
        if (CardCanvas.activeSelf)
        {
            CardCanvas.SetActive(false);
        }
        // Sinon, l'activer
        else
        {
            CardCanvas.SetActive(true);
        }
    }
}
