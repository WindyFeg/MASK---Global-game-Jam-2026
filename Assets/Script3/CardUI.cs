using UnityEngine;
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

    [Header("Animation Settings")]
    [SerializeField] private float hoverScaleAmount = 1.2f; // How big it grows (1.2 = 120%)
    [SerializeField] private float moveUpAmount = 50f;      // How many pixels to move up
    [SerializeField] private float animationDuration = 0.2f;

    private RectTransform rectTransform;
    private Vector3 originalScale;
    private Vector3 originalPosition;
    private bool originalCaptured; // Layout có thể set position sau Awake → capture khi hover lần đầu

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            originalScale = rectTransform.localScale;
            originalPosition = rectTransform.localPosition;
        }
    }

    private void CaptureOriginal()
    {
        if (rectTransform == null || originalCaptured) return;
        originalScale = rectTransform.localScale;
        originalPosition = rectTransform.localPosition;
        originalCaptured = true;
    }

    public void OnSet(Card card)
    {
        UpdateCardUI(card);
    }

    public void UpdateCardUI(Card card)
    {
        // check null
        if (cardNameText == null) return;
        if (cardDescriptionText == null) return;
        if (cardImage == null) return;
        if (cardStatHeartTopLeft == null) return;
        if (cardStatMoneyTopRight == null) return;
        if (cardStatHeartBottomLeft == null) return;
        if (cardStatMoneyBottomRight == null) return;
        
        cardNameText.text = card.cardName;
        cardDescriptionText.text = card.description;
        cardImage.sprite = card.artwork;
        cardStatHeartTopLeft.text = card.stat.Happiness.ToString();
        cardStatMoneyTopRight.text = card.stat.Money.ToString();
        cardStatHeartBottomLeft.text = card.stat.Happiness.ToString();
        cardStatMoneyBottomRight.text = card.stat.Money.ToString();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (rectTransform == null) return;
        CaptureOriginal(); // Lưu vị trí thật (sau khi layout đã set) trước khi scale/move
        rectTransform.DOKill();

        rectTransform.DOScale(originalScale * hoverScaleAmount, animationDuration)
            .SetEase(Ease.OutBack);
        rectTransform.DOLocalMoveY(originalPosition.y + moveUpAmount, animationDuration)
            .SetEase(Ease.OutQuad);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (rectTransform == null) return;
        rectTransform.DOKill();

        rectTransform.DOScale(originalScale, animationDuration)
            .SetEase(Ease.OutQuad);
        rectTransform.DOLocalMove(originalPosition, animationDuration)
            .SetEase(Ease.OutQuad);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        if (rectTransform == null) return;

        rectTransform.DOPunchScale(new Vector3(0.1f, 0.1f, 0.1f), 0.1f);

        // TODO: CardManager.Instance.PlayCard(this);
        Debug.Log($"Card '{gameObject.name}' was clicked/used!");
    }

    private void OnDisable()
    {
        if (rectTransform == null) return;
        rectTransform.DOKill();
        rectTransform.localScale = originalScale;
        rectTransform.localPosition = originalPosition;
    }
}
