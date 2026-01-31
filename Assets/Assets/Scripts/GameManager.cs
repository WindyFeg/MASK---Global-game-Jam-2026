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
    [SerializeField] private playerUI playerUI;
    private BaseStat requirement;
    // [SerializeField] private Player player;
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

    public void UseCard(CardUI cardUI)
    {
        int slotIndex = cardUI.transform.GetSiblingIndex();
        onHandCards.Remove(cardUI);
        Destroy(cardUI.gameObject);

        int reqMoney = requirement.Money;
        int reqHappiness = requirement.Happiness;
        if (cardUI.card.cardData.opponentStat.Money - reqMoney >= 0)
        {

        }
        else
        {
            npcUI.baseHuman.stat.Money -= reqMoney;
        }
        if (cardUI.card.cardData.opponentStat.Happiness - reqHappiness >= 0)
        {

        }
        else
        {
            npcUI.baseHuman.stat.Happiness -= reqHappiness;

        }
        npcUI.baseHuman.stat.Happiness += cardUI.card.cardData.opponentStat.Happiness;
        npcUI.baseHuman.stat.Money += cardUI.card.cardData.opponentStat.Money;

        playerUI.player.AddHappiness(cardUI.card.cardData.selfStat.Happiness);
        playerUI.player.AddMoney(cardUI.card.cardData.selfStat.Money);
        bool isHappy = playerUI.player.stat.Happiness > (BaseStat.MIN_VALUE + BaseStat.MAX_VALUE) / 2;
        if (isHappy && cardUI.card.cardData.selfStat.Happiness > 0)
        {

        }
        else if (!isHappy && cardUI.card.cardData.selfStat.Happiness < 0)
        {

        }
        else
        {
            playerUI.player.stat.Money -= 1;
        }
        // if (cardUI.card.cardData.selfStat.Money > reqMoney)
        // {
        //     money = cardUI.card.cardData.selfStat.Money;
        // }
        // if (cardUI.card.cardData.selfStat.Happiness > reqHappiness)
        // {
        //     happiness = cardUI.card.cardData.selfStat.Happiness;
        // }

        // Discard 1, draw 1 vào đúng slot vừa bỏ → thứ tự: card 1, card 4, card 3
        DrawCard(slotIndex);
        LoadNextLevel();
    }

    // public List<Card> GetOnHandCards()
    // {
    //     return onHandCards;
    // }
    /// <param name="insertIndex">Slot (sibling index) để chèn lá mới. &lt; 0 = thêm vào cuối.</param>
    public void DrawCard(int insertIndex = -1)
    {
        BaseCard card = CardManager.Instance.GetRandomCard();
        Card newCard = new Card();
        newCard.cardData = card;
        var obj = Instantiate(cardPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        obj.transform.SetParent(cardLayout.transform, false);
        if (insertIndex >= 0)
            obj.transform.SetSiblingIndex(insertIndex);
        var cardUI = obj.GetComponent<CardUI>();
        cardUI.OnSet(newCard);
        if (insertIndex >= 0)
            onHandCards.Insert(insertIndex, cardUI);
        else
            onHandCards.Add(cardUI);
    }

    public void LoadNextLevel()
    {
        Npc human = LevelManager.instance.LoadLevel();
        ScenarioEntry scenario = ScenarioManager.Instance.GetRandomScenario((NPCType)human.id, human.GetCurrentEmotion());
        bgImage.sprite = human.bg2D[0];
        requirement = scenario.requirement;
        npcUI.SetNpc(human, scenario.context + "\n" + scenario.dialogue, requirement.Happiness, requirement.Money);
        LoadPlayer();
    }
    public void LoadPlayer()
    {
        playerUI.SetPlayer(LevelManager.instance.player);
    }
}
