using System;
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
    [Header("Transition (NPC exit / enter)")]
    [SerializeField] private float npcExitDuration = 0.4f;
    [SerializeField] private float npcEnterDuration = 0.8f;

    private Requirement requirement;
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
            var cardUIComponent = obj.GetComponent<CardUI>();
            cardUIComponent.OnSet(newCard);
            cardUIComponent.PlayEntranceAnimation();
            onHandCards.Add(cardUIComponent);
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

        // GDD: Kiểm tra "pretending" theo mood TRƯỚC khi áp dụng lá (current Happy vs hiệu ứng lá)
        int playerHappyBefore = playerUI.player.GetHappiness();
        int cardPlayerHappyDelta = cardUI.card.cardData.selfStat.Happiness;

        playerUI.player.AddHappiness(cardPlayerHappyDelta);
        playerUI.player.AddMoney(cardUI.card.cardData.selfStat.Money);

        // Condition 1 (Fake Happy): Player Unhappy (Happy < 3) mà lá cộng Happy (>0) -> -1 Sanity
        bool fakeHappy = (playerHappyBefore < 3 && cardPlayerHappyDelta > 0);
        // Condition 2 (Fake Sad): Player Happy (Happy >= 3) mà lá trừ Happy (<0) -> -1 Sanity
        bool fakeSad = (playerHappyBefore >= 3 && cardPlayerHappyDelta < 0);
        if (fakeHappy || fakeSad)
            playerUI.player.LoseSanity(1);

        playerUI.SetPlayer(playerUI.player); // Cập nhật UI (Sanity, Money, Happy)
        // if (cardUI.card.cardData.selfStat.Money > reqMoney)
        // {
        //     money = cardUI.card.cardData.selfStat.Money;
        // }
        // if (cardUI.card.cardData.selfStat.Happiness > reqHappiness)
        // {
        //     happiness = cardUI.card.cardData.selfStat.Happiness;
        // }

        // Discard 1, draw 1 vào đúng slot vừa bỏ
        DrawCard(slotIndex);
        RunTransitionSequence();
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
        // Ép layout cập nhật ngay để lá có vị trí đúng trước khi entrance + capture
        var layoutRect = cardLayout.GetComponent<RectTransform>();
        if (layoutRect != null)
            LayoutRebuilder.ForceRebuildLayoutImmediate(layoutRect);
        var cardUI = obj.GetComponent<CardUI>();
        cardUI.OnSet(newCard);
        cardUI.PlayEntranceAnimation();
        if (insertIndex >= 0)
            onHandCards.Insert(insertIndex, cardUI);
        else
            onHandCards.Add(cardUI);
    }

    /// <summary>
    /// Load lần đầu (Start): chọn NPC, set nền, set NPC và entrance từ phải.
    /// </summary>
    public void LoadNextLevel()
    {
        Npc human = LevelManager.instance.LoadLevel();
        ScenarioEntry scenario = ScenarioManager.Instance.GetRandomScenario((NPCType)human.id, human.GetCurrentEmotion());
        bgImage.sprite = human.bg2D[0];
        requirement = scenario.requirement;
        npcUI.SetNpc(human, scenario.context + "\n" + scenario.dialogue, requirement.Happiness, requirement.Money);
        LoadPlayer();
    }

    /// <summary>
    /// Luồng chuyển cảnh: NPC thoát trái → đổi nền + NPC mới → NPC mới vào từ phải.
    /// </summary>
    private void RunTransitionSequence()
    {
        npcUI.ExitLeft(npcExitDuration, () =>
        {
            Npc human = LevelManager.instance.LoadLevel();
            ScenarioEntry scenario = ScenarioManager.Instance.GetRandomScenario((NPCType)human.id, human.GetCurrentEmotion());
            bgImage.sprite = human.bg2D[0];
            requirement = scenario.requirement;
            npcUI.SetNpcContentOnly(human, scenario.context + "\n" + scenario.dialogue, requirement.Happiness, requirement.Money);

            npcUI.EnterFromRight(npcEnterDuration);
            LoadPlayer();
        });
    }

    public void LoadPlayer()
    {
        playerUI.SetPlayer(LevelManager.instance.player);
    }
}
