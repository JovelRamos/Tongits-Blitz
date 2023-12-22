using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Deck
{
    private List<Card> cards = new List<Card>();

    public Deck()
    {
        foreach (Card.Suit suit in Enum.GetValues(typeof(Card.Suit)))
        {
            foreach (Card.Value value in Enum.GetValues(typeof(Card.Value)))
            {
                cards.Add(new Card(suit, value));
            }
        }

        Shuffle();
    }

    public void Shuffle()
    {
        System.Random random = new System.Random();
        int n = cards.Count;
        while (n > 1)
        {
            n--;
            int k = random.Next(n + 1);
            Card value = cards[k];
            cards[k] = cards[n];
            cards[n] = value;
        }
    }

    public Card DrawCard()
    {
        if (cards.Count <= 0)
            return null; // No cards left in the deck

        Card drawnCard = cards[0];
        cards.RemoveAt(0);
        return drawnCard;
    }
}
