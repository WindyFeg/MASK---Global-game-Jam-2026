using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private List<CardUI> onHandCards = new List<CardUI>();
    [SerializeField] private GameObject cardLayout;
    [SerializeField] private Image bgImage;
    [SerializeField] private NpcUI npcUI;
    [SerializeField] private playerUI playerUI;
    [Header("Result (Win / Game Over)")]
    [SerializeField] private ResultPanelUI resultPanel;
    [Header("Screen Shake (lose sanity)")]
    [Tooltip("Assign a Panel (child under Canvas) that contains your game UI. If you assign the Canvas, we shake its first child. Overlay Canvas root does not move.")]
    [SerializeField] private RectTransform shakeTarget;
    [SerializeField] private float sanityShakeDuration = 0.4f;
    [SerializeField] private float sanityShakeStrength = 25f;
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
        // Reset ScriptableObject data so each Play session starts fresh (assets persist in Editor otherwise)
        ResetScriptableObjectState();
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
        {
            playerUI.player.LoseSanity(1);
            ShakeScreen();
        }

        playerUI.SetPlayer(playerUI.player); // Cập nhật UI (Sanity, Money, Happy)

        // GDD: Kiểm tra thua / thắng sau khi áp dụng hiệu ứng
        if (CheckGameOverOrWin(out string endReason))
        {
            // Kết thúc turn, không draw/transition (có thể mở UI kết quả / load scene)
            return;
        }

        // Discard 1, draw 1 vào đúng slot vừa bỏ
        DrawCard(slotIndex);
        RunTransitionSequence();
    }

    /// <summary>
    /// GDD: Thua = Player (Money/Happy/Sanity ≤ 0) hoặc bất kỳ NPC (Money/Happy ≤ 0).
    /// Thắng = Player 5/5/5 và tất cả NPC còn sống.
    /// </summary>
    private bool CheckGameOverOrWin(out string reason)
    {
        reason = null;
        Player p = playerUI.player;
        if (p == null) return false;

        // Lose: Player
        if (p.GetMoney() <= 0 || p.GetHappiness() <= 0 || p.IsOutOfSanity())
        {
            reason = p.IsOutOfSanity()
                ? GetRandom(PlayerSanityLossLines)
                : GetRandom(PlayerMoneyHappyLossLines);
            OnGameOver(reason);
            return true;
        }

        // Lose: any NPC is gone
        if (LevelManager.instance != null && LevelManager.instance.humanPool != null)
        {
            foreach (Npc npc in LevelManager.instance.humanPool)
            {
                if (npc != null && !npc.IsAlive())
                {
                    reason = GetNpcGoneReason(npc);
                    OnGameOver(reason);
                    return true;
                }
            }
        }

        // Win: Player 5/5/5 and all NPCs alive
        bool playerFull = p.GetMoney() == BaseStat.MAX_VALUE && p.GetHappiness() == BaseStat.MAX_VALUE && p.GetSanity() >= p.maxSanity - 2;
        if (!playerFull) return false;

        if (LevelManager.instance != null && LevelManager.instance.humanPool != null)
        {
            foreach (Npc npc in LevelManager.instance.humanPool)
            {
                if (npc == null || !npc.IsAlive())
                {
                    return false;
                }
            }
        }
        else
        {
            return false;
        }

        reason = "You reached 5/5/5 and everyone close to you is still okay.";
        OnWin(reason);
        return true;
    }

    private void OnGameOver(string reason)
    {
        Debug.Log("[GAME OVER] " + reason);
        resultPanel?.ShowGameOver(reason);
    }

    private void OnWin(string reason)
    {
        Debug.Log("[WIN] " + reason);
        resultPanel?.ShowWin(reason);
    }

    // Small pool of funny loss lines (picked at random)
    private static readonly string[] PlayerSanityLossLines =
    {
        "You lost your mind from pretending too much.",
        "Too many fake smiles. Your brain checked out.",
        "The masks finally broke you. Sanity: 0.",
        "You forgot who you really are. Oops.",
    };
    private static readonly string[] PlayerMoneyHappyLossLines =
    {
        "You ran out of money or happiness. Game over.",
        "Wallet empty, mood in the basement. Bye.",
        "No cash, no joy. That's a wrap.",
    };
    private static readonly string[] NpcUnhappinessLossSuffixes =
    {
        " couldn't fake it anymore and left.",
        " went to find happiness elsewhere. Good luck with that.",
        " hit zero happiness and quit the game.",
        " forgot how to smile. They're gone.",
    };
    private static readonly string[] NpcMoneyLossSuffixes =
    {
        " ran out of money and left.",
        "'s wallet said no. They're gone.",
        " went broke. See you never.",
        " couldn't afford life anymore. Ouch.",
    };

    private static string GetRandom(string[] pool)
    {
        if (pool == null || pool.Length == 0) return "You lost.";
        return pool[UnityEngine.Random.Range(0, pool.Length)];
    }

    /// <summary>
    /// Returns a short narrative reason: random funny line for NPC gone from unhappiness or from no money.
    /// </summary>
    private static string GetNpcGoneReason(Npc npc)
    {
        if (npc == null) return "Someone close to you is gone.";
        string who = string.IsNullOrEmpty(npc.name) ? "Someone close to you" : "Your " + npc.name;
        if (npc.stat.Happiness <= 0)
            return who + GetRandom(NpcUnhappinessLossSuffixes);
        if (npc.stat.Money <= 0)
            return who + GetRandom(NpcMoneyLossSuffixes);
        return who + " is gone.";
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
        npcUI.SetNpc(human, "<size=80><b>" + scenario.context + "</b></size>\n" + scenario.dialogue, requirement.Happiness, requirement.Money);
        npcUI.SetOffer(requirement.Happiness, requirement.Money);
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
            npcUI.SetOffer(requirement.Happiness, requirement.Money);

            npcUI.EnterFromRight(npcEnterDuration);
            LoadPlayer();
        });
    }

    public void LoadPlayer()
    {
        playerUI.SetPlayer(LevelManager.instance.player);
    }

    private void ShakeScreen()
    {
        if (shakeTarget == null)
        {
            Camera cam = Camera.main;
            if (cam != null && cam.transform != null)
                cam.transform.DOShakePosition(sanityShakeDuration, sanityShakeStrength * 0.01f, 20, 90f, false, true);
            return;
        }

        // With Overlay Canvas, the Canvas root often doesn't move; shake a child Panel instead.
        RectTransform target = shakeTarget;
        Canvas c = shakeTarget.GetComponent<Canvas>();
        if (c != null && shakeTarget.childCount > 0)
        {
            Transform firstChild = shakeTarget.GetChild(0);
            RectTransform rt = firstChild as RectTransform;
            if (rt != null) target = rt;
        }

        target.DOKill(false);
        target.DOShakeAnchorPos(sanityShakeDuration, sanityShakeStrength, 25, 90f, false, true);
    }

    /// <summary>
    /// Current player Happiness (for CardUI sanity-damage check). Returns 3 if player not set.
    /// </summary>
    public int GetPlayerHappiness()
    {
        if (playerUI?.player == null) return 3;
        return playerUI.player.GetHappiness();
    }

    /// <summary>
    /// Reset game trong cùng scene: Player 3/3/5, NPC random [1,4], xóa bài rồi rút 3 lá, load NPC mới.
    /// Gọi từ ResultPanelUI khi bấm Replay (không load scene).
    /// </summary>
    /// <summary>
    /// Reset Player and NPC ScriptableObject data to initial values. Call on game start and on Replay
    /// so data always starts fresh (ScriptableObjects persist in Editor and would keep last run values).
    /// </summary>
    private void ResetScriptableObjectState()
    {
        if (LevelManager.instance == null) return;

        // Player: Money 3, Happy 3, Sanity 5 (GDD)
        Player p = LevelManager.instance.player;
        if (p != null)
        {
            p.stat.Money = 3;
            p.stat.Happiness = 3;
            p.curSanity = 5;
            p.maxSanity = 5;
        }

        // NPC: random [1, 4] per stat (GDD)
        if (LevelManager.instance.humanPool != null)
        {
            foreach (Npc npc in LevelManager.instance.humanPool)
            {
                if (npc != null)
                {
                    npc.stat.Money = UnityEngine.Random.Range(1, 5);
                    npc.stat.Happiness = UnityEngine.Random.Range(1, 5);
                }
            }
        }
    }

    public void ResetGame()
    {
        resultPanel?.Hide();
        ResetScriptableObjectState();

        // Xóa bài trên tay, rút lại 3 lá
        foreach (CardUI c in onHandCards)
        {
            if (c != null && c.gameObject != null)
                Destroy(c.gameObject);
        }
        onHandCards.Clear();
        OnStart();

        // Hiển thị NPC mới và cập nhật UI player
        LoadNextLevel();
    }
}
