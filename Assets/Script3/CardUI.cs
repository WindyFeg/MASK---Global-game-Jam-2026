using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // Required for mouse events
using DG.Tweening;              // Required for DOTween

public class CardUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Card Display")]
    // public Card card; // Reference to the Card scriptable object
    public TMPro.TextMeshProUGUI cardNameText; // UI Text for card name
    public TMPro.TextMeshProUGUI cardDescriptionText; // UI Text for card description
    public UnityEngine.UI.Image cardImage; // UI Image for card artwork
    public TMPro.TextMeshProUGUI cardStatHeartTopLeft; // UI Text for card stat
    public TMPro.TextMeshProUGUI cardStatMoneyTopRight; // UI Text for card stat
    public TMPro.TextMeshProUGUI cardStatHeartBottomLeft; // UI Text for card stat
    public TMPro.TextMeshProUGUI cardStatMoneyBottomRight; // UI Text for card stat
    public Sprite hearthart;
    public Sprite moneyart;

    [Header("Card Frame (sanity warning)")]
    [SerializeField] private Sprite defaultCardSprite;
    [SerializeField] private Sprite sanityDamageCardSprite;

    [Header("Animation Settings")]
    [SerializeField] private float hoverScaleAmount = 1.2f; // How big it grows (1.2 = 120%)
    [SerializeField] private float moveUpAmount = 50f;      // How many pixels to move up
    [SerializeField] private float animationDuration = 0.2f;
    [Header("Throw (Use Card)")]
    [SerializeField] private float throwMoveUp = 400f;
    [SerializeField] private float throwScaleEnd = 0.2f;
    [SerializeField] private float throwRotationZ = 35f;
    [SerializeField] private float throwDuration = 0.4f;
    [Header("Entrance (New Card)")]
    [SerializeField] private float entranceDuration = 0.35f;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Image cardFrameImage;
    private Vector3 originalScale;
    private Vector3 originalPosition;
    public Card card;
    private bool isThrowing;
    private bool isPlayingEntrance;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        if (rectTransform != null)
            CaptureOriginal();
        if (GetComponent<CanvasGroup>() == null)
            gameObject.AddComponent<CanvasGroup>();
        canvasGroup = GetComponent<CanvasGroup>();
        cardFrameImage = GetComponent<Image>();
    }

    /// <summary>
    /// Capture scale and position. Use when at rest (e.g. after exit animation) so we never save a hover-inflated scale.
    /// </summary>
    private void CaptureOriginal()
    {
        if (rectTransform == null) return;
        originalScale = rectTransform.localScale;
        originalPosition = rectTransform.localPosition;
    }

    /// <summary>
    /// True if card is at rest (scale matches original), so it's safe to capture without drift.
    /// </summary>
    private bool IsAtRest()
    {
        if (rectTransform == null) return false;
        return (rectTransform.localScale - originalScale).sqrMagnitude < 0.0001f;
    }

    public void OnSet(Card card)
    {
        this.card = card;
        UpdateCardUI(card);
    }

    public void UpdateCardUI(Card card)
    {
        // check null
        if (cardNameText == null)
        {
            Debug.LogError("cardNameText is null");
            return;
        }
        if (cardDescriptionText == null)
        {
            Debug.LogError("cardDescriptionText is null");
            return;
        }
        if (cardImage == null)
        {
            Debug.LogError("cardImage is null");
            return;
        }
        if (cardStatHeartTopLeft == null)
        {
            Debug.LogError("cardStatHeartTopLeft is null");
            return;
        }
        if (cardStatMoneyTopRight == null)
        {
            Debug.LogError("cardStatMoneyTopRight is null");
            return;
        }
        if (cardStatHeartBottomLeft == null)
        {
            Debug.LogError("cardStatHeartBottomLeft is null");
            return;
        }
        if (cardStatMoneyBottomRight == null)
        {
            Debug.LogError("cardStatMoneyBottomRight is null");
            return;
        }

        cardNameText.text = card.cardData.cardName;
        cardDescriptionText.text = card.cardData.description;
        cardImage.sprite = card.cardData.artwork;
        cardStatHeartTopLeft.text = card.cardData.opponentStat.Happiness.ToString();
        cardStatMoneyTopRight.text = card.cardData.opponentStat.Money.ToString();
        cardStatHeartBottomLeft.text = card.cardData.selfStat.Happiness.ToString();
        cardStatMoneyBottomRight.text = card.cardData.selfStat.Money.ToString();

        // Card frame: use sanity-damage sprite if playing this card would cost -1 Sanity (pretending)
        if (cardFrameImage != null && (defaultCardSprite != null || sanityDamageCardSprite != null))
        {
            bool causesSanityDamage = WouldCauseSanityDamage(card);
            if (causesSanityDamage && sanityDamageCardSprite != null)
                cardFrameImage.sprite = sanityDamageCardSprite;
            else if (defaultCardSprite != null)
                cardFrameImage.sprite = defaultCardSprite;
        }
    }

    /// <summary>
    /// Same logic as GameManager: Fake Happy (player &lt; 3 and card +Happy) or Fake Sad (player >= 3 and card -Happy) = -1 Sanity.
    /// </summary>
    private static bool WouldCauseSanityDamage(Card card)
    {
        if (card?.cardData == null) return false;
        int playerHappy = GameManager.instance != null ? GameManager.instance.GetPlayerHappiness() : 3;
        int cardSelfHappy = card.cardData.selfStat.Happiness;
        bool fakeHappy = playerHappy < 3 && cardSelfHappy > 0;
        bool fakeSad = playerHappy >= 3 && cardSelfHappy < 0;
        return fakeHappy || fakeSad;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (rectTransform == null || isThrowing || isPlayingEntrance) return;
        // Only capture when at rest (scale back to normal), else we drift scale up or position up
        if (IsAtRest())
            CaptureOriginal();
        rectTransform.DOKill();

        rectTransform.DOScale(originalScale * hoverScaleAmount, animationDuration)
            .SetEase(Ease.OutBack);
        rectTransform.DOLocalMoveY(originalPosition.y + moveUpAmount, animationDuration)
            .SetEase(Ease.OutQuad);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (rectTransform == null || isThrowing || isPlayingEntrance) return;
        rectTransform.DOKill();

        rectTransform.DOScale(originalScale, animationDuration)
            .SetEase(Ease.OutQuad);
        rectTransform.DOLocalMove(originalPosition, animationDuration)
            .SetEase(Ease.OutQuad)
            .OnComplete(CaptureOriginal); // Save rest state only when fully returned, so scale never drifts
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        if (rectTransform == null || isThrowing || isPlayingEntrance) return;

        PlayThrowAnimation(() =>
        {
            if (this != null && gameObject != null)
                GameManager.instance.UseCard(this);
        });
    }

    /// <summary>
    /// Animation ném lá lên (move up, scale down, fade out, xoay) rồi gọi onComplete.
    /// </summary>
    public void PlayThrowAnimation(Action onComplete)
    {
        if (rectTransform == null || canvasGroup == null) return;
        isThrowing = true;
        rectTransform.DOKill();
        canvasGroup.DOKill();

        CaptureOriginal();
        float targetY = originalPosition.y + throwMoveUp;

        Sequence throwSeq = DOTween.Sequence();
        throwSeq.Join(rectTransform.DOLocalMoveY(targetY, throwDuration).SetEase(Ease.InQuad));
        throwSeq.Join(rectTransform.DOScale(originalScale * throwScaleEnd, throwDuration).SetEase(Ease.InQuad));
        throwSeq.Join(canvasGroup.DOFade(0f, throwDuration));
        throwSeq.Join(rectTransform.DOLocalRotate(new Vector3(0f, 0f, throwRotationZ), throwDuration).SetEase(Ease.InQuad));
        throwSeq.OnComplete(() =>
        {
            isThrowing = false;
            onComplete?.Invoke();
        });
    }

    /// <summary>
    /// Animation lá mới vào tay: scale từ nhỏ + fade in (OutBack).
    /// Bỏ qua hover trong lúc chạy để chuột đè lên không làm sai vị trí.
    /// </summary>
    public void PlayEntranceAnimation()
    {
        if (rectTransform == null || canvasGroup == null) return;
        rectTransform.DOKill();
        canvasGroup.DOKill();

        isPlayingEntrance = true;
        CaptureOriginal();
        Vector3 startScale = originalScale * 0.3f;
        rectTransform.localScale = startScale;
        canvasGroup.alpha = 0f;

        Sequence entranceSeq = DOTween.Sequence();
        entranceSeq.Append(rectTransform.DOScale(originalScale, entranceDuration).SetEase(Ease.OutBack));
        entranceSeq.Join(canvasGroup.DOFade(1f, entranceDuration * 0.7f));
        entranceSeq.OnComplete(() =>
        {
            isPlayingEntrance = false;
            CaptureOriginal();
        });
    }

    private void OnDisable()
    {
        if (rectTransform == null) return;
        rectTransform.DOKill();
        rectTransform.localScale = originalScale;
        rectTransform.localPosition = originalPosition;
    }

}
