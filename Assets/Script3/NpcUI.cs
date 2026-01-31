using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using DG.Tweening;

public class NpcUI : MonoBehaviour
{
    public BaseHuman baseHuman;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI contextText;
    public Image npcImage;

    [Header("Entrance (right to left)")]
    [SerializeField] private float entranceOffsetX = 800f;
    [SerializeField] private float entranceDuration = 0.8f;
    [SerializeField] private float breathingScale = 0.96f;
    [SerializeField] private float breathingDuration = 1.2f;

    private RectTransform rectTransform;
    private Vector2 displayAnchoredPos;
    private bool displayPosCaptured;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            displayAnchoredPos = rectTransform.anchoredPosition;
            displayPosCaptured = true;
        }
    }

    /// <summary>
    /// Cập nhật nội dung NPC và chạy animation vào từ phải sang trái (không drift).
    /// </summary>
    public void SetNpc(BaseHuman baseHuman, string context, int happiness, int money)
    {
        this.baseHuman = baseHuman;
        nameText.text = baseHuman.name;
        contextText.text = context + "\n" + "Asked for: " + money + " money, " + happiness + " happiness";
        npcImage.sprite = GetNpcArtwork();

        if (rectTransform == null) rectTransform = GetComponent<RectTransform>();
        if (rectTransform == null) return;

        transform.DOKill();
        if (!displayPosCaptured)
        {
            displayAnchoredPos = rectTransform.anchoredPosition;
            displayPosCaptured = true;
        }

        // Bắt đầu từ bên phải (off-screen), bay vào vị trí hiển thị (phải → trái)
        Vector2 startPos = displayAnchoredPos + Vector2.right * entranceOffsetX;
        rectTransform.anchoredPosition = startPos;
        rectTransform.DOAnchorPos(displayAnchoredPos, entranceDuration).SetEase(Ease.OutCubic);

        // Breathing: scale nhẹ, một vòng Yoyo (kill cái cũ tránh chồng loop)
        transform.DOScale(breathingScale, breathingDuration).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
    }

    /// <summary>
    /// Thoát sang trái (dùng trong transition trước khi đổi NPC).
    /// </summary>
    public void ExitLeft(float duration, Action onComplete = null)
    {
        if (rectTransform == null) rectTransform = GetComponent<RectTransform>();
        if (rectTransform == null) { onComplete?.Invoke(); return; }
        transform.DOKill();
        Vector2 exitPos = displayAnchoredPos + Vector2.left * (entranceOffsetX + 200f);
        rectTransform.DOAnchorPos(exitPos, duration).SetEase(Ease.InCubic).OnComplete(() => onComplete?.Invoke());
    }

    /// <summary>
    /// Chỉ set nội dung và đặt vị trí off-screen phải (để EnterFromRight gọi sau).
    /// </summary>
    public void SetNpcContentOnly(BaseHuman baseHuman, string context, int happiness, int money)
    {
        this.baseHuman = baseHuman;
        nameText.text = baseHuman.name;
        contextText.text = context + "\n" + "Asked for: " + money + " money, " + happiness + " happiness";
        npcImage.sprite = GetNpcArtwork();
        if (rectTransform == null) rectTransform = GetComponent<RectTransform>();
        if (rectTransform != null)
            rectTransform.anchoredPosition = displayAnchoredPos + Vector2.right * entranceOffsetX;
    }

    /// <summary>
    /// Bay vào từ phải sang trái (sau khi màn đen/đổi nền xong).
    /// </summary>
    public void EnterFromRight(float duration, Action onComplete = null)
    {
        if (rectTransform == null) rectTransform = GetComponent<RectTransform>();
        if (rectTransform == null) { onComplete?.Invoke(); return; }
        transform.DOKill();
        rectTransform.anchoredPosition = displayAnchoredPos + Vector2.right * entranceOffsetX;
        rectTransform.DOAnchorPos(displayAnchoredPos, duration).SetEase(Ease.OutCubic).OnComplete(() =>
        {
            transform.DOScale(breathingScale, breathingDuration).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
            onComplete?.Invoke();
        });
    }

    private Sprite GetNpcArtwork()
    {
        if (baseHuman.stat.Happiness < (BaseStat.MIN_VALUE + BaseStat.MAX_VALUE) / 2 && baseHuman.stat.Money < (BaseStat.MIN_VALUE + BaseStat.MAX_VALUE) / 2) {
            return baseHuman.artwork[0];
        } else if (baseHuman.stat.Happiness < (BaseStat.MIN_VALUE + BaseStat.MAX_VALUE) / 2 && baseHuman.stat.Money > (BaseStat.MIN_VALUE + BaseStat.MAX_VALUE) / 2) {
            return baseHuman.artwork[1];
        } else if (baseHuman.stat.Happiness > (BaseStat.MIN_VALUE + BaseStat.MAX_VALUE) / 2 && baseHuman.stat.Money < (BaseStat.MIN_VALUE + BaseStat.MAX_VALUE) / 2) {
            return baseHuman.artwork[2];
        } else {
            return baseHuman.artwork[3];
        }
    }
}
