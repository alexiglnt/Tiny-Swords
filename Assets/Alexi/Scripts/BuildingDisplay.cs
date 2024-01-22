using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BuildingDisplay : MonoBehaviour
{
    public BuildingCard card;

    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;
    public Image artworkImage;

    public TextMeshProUGUI attackText;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI healthTextCurrent;
    // public TextMeshProUGUI movementText;


    void Start()
    {
        UpdateUI(card);
    }


    //void Update()
    //{
    //    if (Input.GetMouseButtonDown(0))
    //    {
    //        Vector3 clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //        clickPosition.z = 0f;

    //        RaycastHit2D hit = Physics2D.Raycast(clickPosition, Vector2.zero);

    //        // Debug.Log pour vérifier ce qui est touché par le raycast
    //        Debug.Log(hit.collider);

    //        if (hit.collider == null || hit.collider.gameObject != gameObject)
    //        {
    //            CloseCardUI();
    //        }
    //    }
    //}


    // Fonction pour afficher les informations de la carte
    public void DisplayBuildingCardInformation(BuildingCard newcard)
    {
        card = newcard;
        UpdateUI(card);
        gameObject.SetActive(true);
    }

    public void CloseCardUI()
    {
        // Désactiver l'UI de la carte
        gameObject.SetActive(false);
    }

    private void UpdateUI(BuildingCard card)
    {
        nameText.text = card.name;
        descriptionText.text = card.description;

        artworkImage.sprite = card.artwork;

        attackText.text = card.attackPoint.ToString();
        healthText.text = card.healthPoint.ToString();
        healthTextCurrent.text = card.healthPointCurrent.ToString();
    }
}
