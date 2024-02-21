using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnClickCharacter : MonoBehaviour
{
    public Card card;
    public CardDisplay cardDisplay; // R�f�rence au script CardDisplay
    public GameObject CardCanvas; // R�f�rence au canvas de la carte


    // Appel� lorsque le clic est d�tect�
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
        // V�rifier si le clic droit de la souris est enfonc�
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
        // D�sactiver le canvas de la carte
        CardCanvas.SetActive(false);
    }

    void SwitchCanvasVisibility()
    {
        // Si le canvas est actif, le d�sactiver
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
