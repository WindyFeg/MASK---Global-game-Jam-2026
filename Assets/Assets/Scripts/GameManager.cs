using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private List<CardUI> onHandCards = new List<CardUI>();
    [SerializeField] private GameObject cardLayout;
    [SerializeField] private Image bgImage;
    [SerializeField] private NpcUI npcUI;
    [SerializeField] private Player player;
    // [SerializeField] private List<NPc> humanPool;

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
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        OnStart();
        LoadNextLevel();
    }

    public void OnStart()
    {
        for (int i = 0; i < 3; i++)
        {

            BaseCard card = CardManager.Instance.GetRandomCard();
            Card newCard = new Card();
            newCard.cardData = card;
            var obj = Instantiate(cardPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            obj.transform.SetParent(cardLayout.transform, false);
            obj.GetComponent<CardUI>().OnSet(newCard);
            onHandCards.Add(obj.GetComponent<CardUI>());

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
        BaseHuman human = LevelManager.instance.LoadLevel();

        npcUI.SetNpc(human);

        bgImage.sprite = human.bg2D[0];

    }
}
