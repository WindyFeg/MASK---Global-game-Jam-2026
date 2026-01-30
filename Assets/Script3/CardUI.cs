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

    [Header("Animation Settings")]
    [SerializeField] private float hoverScaleAmount = 1.2f; // How big it grows (1.2 = 120%)
    [SerializeField] private float moveUpAmount = 50f;      // How many pixels to move up
    [SerializeField] private float animationDuration = 0.2f;

    private RectTransform rectTransform;
    private Vector3 originalScale;
    private Vector3 originalPosition;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            originalScale = rectTransform.localScale;
            originalPosition = rectTransform.localPosition;
        }
    }

    public void OnSet()
    {
        UpdateCardUI();
    }

    public void UpdateCardUI()
    {
        // if (cardNameText != null)
        //     cardNameText.text = card.cardName;
        // if (cardDescriptionText != null)
        //     cardDescriptionText.text = card.description;
        // if (cardImage != null && card.artwork != null)
        //     cardImage.sprite = card.artwork;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (rectTransform == null) return;
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
