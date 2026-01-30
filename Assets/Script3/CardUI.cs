using UnityEngine;

public class CardUI : MonoBehaviour
{
    // public Card card; // Reference to the Card scriptable object
    public TMPro.TextMeshProUGUI cardNameText; // UI Text for card name
    public TMPro.TextMeshProUGUI cardDescriptionText; // UI Text for card description
    public UnityEngine.UI.Image cardImage; // UI Image for card artwork

    public void OnSet(

    )
    {

    }
    public void UpdateCardUI()
    {
        // if (cardNameText != null)
        // {
        //     cardNameText.text = card.cardName;
        // }
        // if (cardDescriptionText != null)
        // {
        //     cardDescriptionText.text = card.description;
        // }
        // if (cardImage != null && card.artwork != null)
        // {
        //     cardImage.sprite = card.artwork;
        // }
    }
}
