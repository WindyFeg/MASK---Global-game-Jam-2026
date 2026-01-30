using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [SerializeField] private GameObject cardPrefab;

    // private readonly List<CardUI> onHandCards = new List<Card>();

    private void Awake()
    {
        // Chống tạo trùng GameManager khi load scene khác
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        // Giữ object này khi chuyển scene
        OnStart();
        DontDestroyOnLoad(gameObject);
    }

    public void OnStart()
    {
        for (int i = 0; i < 3; i++)
        {
            Debug.Log("Start");
            // Card = CardManager.GetRandomCard();
            var obj = Instantiate(cardPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            // obj.GetComponent<CardUI>().OnSet(Card);
  
        }
    }

    // public void UseCard(Card card)
    // {
    //     // card.Use();
    //     // onHandCards.Remove(card);
    //     // onHandCards.Add(CardManager.GetRandomCard());
    // }

    // public List<Card> GetOnHandCards()
    // {
    //     return onHandCards;
    // }

    public void LoadNextLevel()
    {
        
    }
}
