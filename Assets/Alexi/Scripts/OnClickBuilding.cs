using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnClickBuilding : MonoBehaviour
{
    public BuildingCard card;
    private BuildingDisplay cardDisplay; // Reference to the CardDisplay script
    public GameObject BuildingCardCanvas; // Reference to the card canvas

    // Static variable to keep track of the currently visible BuildingCardCanvas
    private static GameObject currentlyActiveCanvas = null;

    void Start()
    {
        // Disable the card canvas initially
        BuildingCardCanvas.SetActive(false);
        BuildingCardCanvas = Instantiate(BuildingCardCanvas, new Vector3(BuildingCardCanvas.transform.position.x, BuildingCardCanvas.transform.position.y, 0), Quaternion.identity);
        cardDisplay = BuildingCardCanvas.GetComponent<BuildingDisplay>();
    }

    public void OnMouseDown()
    {
        // If there is already an active canvas and it's not this one, disable it
        if (currentlyActiveCanvas != null && currentlyActiveCanvas != BuildingCardCanvas)
        {
            currentlyActiveCanvas.SetActive(false);
        }

        Debug.Log("Click on " + card.name);

        // Display the clicked building's information
        cardDisplay.DisplayBuildingCardInformation(card);

        // Show the canvas for the clicked building
        BuildingCardCanvas.SetActive(true);

        // Update the currently active canvas
        currentlyActiveCanvas = BuildingCardCanvas;
    }
}
