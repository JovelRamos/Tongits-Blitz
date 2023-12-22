using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine.UI; // Needed for UI components like Image
using UnityEngine;

public class CardBehaviour : MonoBehaviour
{
    public Card CardData { get; set; }
}

public class DrawCards : MonoBehaviour
{
    public GameObject cardPrefab;
    public RectTransform handArea;
    public RectTransform discardPile; // Reference to your discard pile
    private Deck deck = new Deck();
    private Dictionary<Card, Sprite> cardToSpriteMap;
    private List<GameObject> handCards = new List<GameObject>(); // List to keep track of cards in hand


    // Start is called before the first frame update
    void Start()
    {
        InitializeCardToSpriteMap();
        foreach (var card in cardToSpriteMap.Keys)
        {
            Debug.Log($"Dictionary contains: {card}"); // Add this line
        }
    }

    private void InitializeCardToSpriteMap()
    {
        cardToSpriteMap = new Dictionary<Card, Sprite>();

        // Load sprites from resources (assuming they are stored in a Resources folder)
        Sprite[] allCardSprites = Resources.LoadAll<Sprite>("cards");

        foreach (Card.Suit suit in Enum.GetValues(typeof(Card.Suit)))
        {
            foreach (Card.Value value in Enum.GetValues(typeof(Card.Value)))
            {
            // Construct the file name
            string fileName = GetSpriteFileNameForCard(suit, value);
            Debug.Log($"Looking for sprite: {fileName}"); // Log the filename we're looking for

            // Find the corresponding sprite
            Sprite foundSprite = Array.Find(allCardSprites, sprite => sprite.name == fileName);
            if (foundSprite != null)
            {
                cardToSpriteMap.Add(new Card(suit, value), foundSprite);
                Debug.Log($"Mapped {fileName} to card: {suit} {value}"); // Log successful mapping
            }
            else
            {
                Debug.LogWarning($"Sprite not found for card: {fileName}");
            }
            }
        }
    }

    private string GetSpriteFileNameForCard(Card.Suit suit, Card.Value value)
    {
        // Map the suit to its corresponding number


        string suitString = suit switch
        {
            Card.Suit.Hearts => "2",
            Card.Suit.Diamonds => "4",
            Card.Suit.Spades => "5",
            Card.Suit.Clubs => "7",
            _ => throw new ArgumentOutOfRangeException(nameof(suit), $"Invalid suit: {suit}")
        };

        // Map the value to its corresponding number
        // Assuming that the enum's underlying values for Ace through King are 1 through 13 respectively
        int valueNumber = (int)value;

        // Construct the file name
        return $"{valueNumber}.{suitString}";
    }


    public void OnClick()
    {
        Card drawnCard = deck.DrawCard();
        Debug.Log($"Drawn card: {drawnCard.CardValue} of {drawnCard.CardSuit}");

        if (drawnCard == null)
        {
            Debug.Log("No more cards in the deck.");
            return;
        }

        // Instantiate the card prefab
        GameObject newCard = Instantiate(cardPrefab, handArea, false);

        // Attach the Card object to the newCard GameObject
        CardBehaviour cardBehaviour = newCard.AddComponent<CardBehaviour>();
        cardBehaviour.CardData = drawnCard;

        // Update the card's visual to represent the drawn card
        UpdateCardVisual(newCard, drawnCard);

        // Add the drag-and-drop script only once
        DragDrop dragDropScript = newCard.AddComponent<DragDrop>();
        dragDropScript.discardPile = discardPile;

        // Subscribe to the OnCardDiscarded action
        dragDropScript.OnCardDiscarded += CardDiscarded; // Make sure this method is defined in DrawCards

        // Add the card to the hand list and update layout
        handCards.Add(newCard);
        SortHandCards();
        UpdateHandLayout();
    }

    private void CardDiscarded(GameObject card)
    {
        handCards.Remove(card); // Remove the card from handCards
                                // Optionally, update the hand layout immediately
        UpdateHandLayout();
    }

    void OnDestroy()
    {
        foreach (var card in handCards)
        {
            var dragDropScript = card.GetComponent<DragDrop>();
            if (dragDropScript != null)
            {
                dragDropScript.OnCardDiscarded -= CardDiscarded;
            }
        }
    }



    private void SortHandCards()
    {
        handCards = handCards
            .Where(cardGameObject => !cardGameObject.GetComponent<DragDrop>().IsInDiscardPile) // Ignore cards in discard pile
            .OrderBy(cardGameObject => cardGameObject.GetComponent<CardBehaviour>().CardData.CardValue)
            .ThenByDescending(cardGameObject => cardGameObject.GetComponent<CardBehaviour>().CardData.CardSuit)
            .ToList();
    }


    private void UpdateHandLayout()
    {
        float cardSpacing = CalculateCardSpacing(handArea.rect.width, handCards.Count, cardPrefab.GetComponent<RectTransform>().rect.width);
        for (int i = 0; i < handCards.Count; i++)
        {
            if (!handCards[i].GetComponent<DragDrop>().IsInDiscardPile)
            {
                RectTransform cardRect = handCards[i].GetComponent<RectTransform>();
                // Overlap cards by setting their horizontal position with a negative spacing
                float cardPositionX = i * cardSpacing;
                cardRect.anchoredPosition = new Vector2(cardPositionX, cardRect.anchoredPosition.y);
                // Update the z-index to ensure cards are layered properly
                cardRect.SetSiblingIndex(i);
            }
        }
    }

    private float CalculateCardSpacing(float handWidth, int cardCount, float cardWidth)
    {
        // If there's only one card, no need to overlap
        if (cardCount <= 1) return 0;

        // If there are multiple cards, calculate the overlap so that all cards fit into the hand area
        float overlap = (handWidth - cardWidth) / (cardCount - 1);
        return overlap;
    }

    private void UpdateCardVisual(GameObject cardGameObject, Card card)
    {
        // Get the Image component from the card GameObject
        Image cardImage = cardGameObject.GetComponent<Image>();
        if (cardImage == null)
        {
            Debug.LogError("Card prefab does not have an Image component attached.");
            return;
        }

        // Retrieve the sprite for the specific card
        if (cardToSpriteMap.TryGetValue(card, out Sprite cardSprite))
        {
            // Apply the sprite to the card's Image component
            cardImage.sprite = cardSprite;
            Debug.Log($"Successfully updated card visual for {card}.");
        }
        else
        {
            Debug.LogError($"Sprite not found for card: {card}. Make sure the card's Equals and GetHashCode methods are correctly implemented.");
        }
    }

}
