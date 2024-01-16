using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardDisplay : MonoBehaviour
{
    public Card card;

    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;
    public Image artworkImage;

    public TextMeshProUGUI attackText;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI movementText;


    void Start()
    {
        nameText.text = card.name;
        descriptionText.text = card.description;

        artworkImage.sprite = card.artwork;

        attackText.text = card.attackPoint.ToString();
        healthText.text = card.healthPoint.ToString();
        movementText.text = card.movevementPoint.ToString();
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
    public void DisplayCardInformation(Card newcard)
    {
        card = newcard;
        gameObject.SetActive(true);
    }

    public void CloseCardUI()
    {
        // Désactiver l'UI de la carte
        gameObject.SetActive(false);
    }
}
