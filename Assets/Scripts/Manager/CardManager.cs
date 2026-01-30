using UnityEngine;  
using System.Collections.Generic;

public class CardManager : MonoBehaviour {
    public List<BaseCard> allCards = new List<BaseCard>();
    public List<BaseCard> curCards = new List<BaseCard>();

    public static CardManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    
    public List<BaseCard> GetAllCards() {
        // Return all card data
        return allCards;
    }

    public List<BaseCard> GetCurrentCards() {
        return curCards;
    }

    public BaseCard GetRandomCard() {
        // Get random card from pool
        if (allCards.Count == 0) return null;
        var rnd = Random.Range(0, allCards.Count - 1);
        return allCards[rnd];
    }
}
