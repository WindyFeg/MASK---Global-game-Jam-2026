using System.Collections.Generic;

public class CardManager {
    public List<Card> allCards = new List<Card>();
    public List<Card> curCards = new List<Card>();
    
    public List<Card> GetAllCards() {
        // Return all card data
        return allCards;
    }

    public List<Card> GetCurrentCards() {
        return curCards;
    }

    public Card GetRandomCard() {
        // Get random card from pool
        return null;
    }
}
