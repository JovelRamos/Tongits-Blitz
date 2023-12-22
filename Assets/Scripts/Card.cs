using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card
{
    public enum Suit { Hearts, Diamonds, Clubs, Spades }
    public enum Value { Ace = 1, Two, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Jack, Queen, King }

    public Suit CardSuit { get; private set; }
    public Value CardValue { get; private set; }

    public Card(Suit suit, Value value)
    {
        CardSuit = suit;
        CardValue = value;
    }

    public override string ToString()
    {
        return $"{CardValue} of {CardSuit}";
    }

    public override bool Equals(object obj)
    {
        if (obj is Card card)
        {
            return card.CardSuit == this.CardSuit && card.CardValue == this.CardValue;
        }
        return false;
    }

    public override int GetHashCode()
    {
        unchecked // Overflow is fine, just wrap
        {
            int hash = 17;
            // Suitable nullity checks etc, of course :)
            hash = hash * 23 + CardSuit.GetHashCode();
            hash = hash * 23 + CardValue.GetHashCode();
            return hash;
        }
    }



}


