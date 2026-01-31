using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using DG.Tweening;
using System.Collections.Generic;

public class NpcUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public BaseHuman baseHuman;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI contextText;
    public Image npcImage;
    public List<Image> offerImage = new List<Image>();
    public List<Sprite> offerSprites = new List<Sprite>();

    [Header("Hover status tooltip")]
    [Tooltip("GO shown on hover; assign a panel with statusTooltipText inside.")]
    [SerializeField] private GameObject statusTooltipGO;
    [SerializeField] private TextMeshProUGUI statusTooltipText;
    [SerializeField] private float tooltipZoomDuration = 0.2f;

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
        if (statusTooltipGO != null)
        {
            statusTooltipGO.SetActive(false);
            var rt = statusTooltipGO.GetComponent<RectTransform>();
            if (rt != null) rt.localScale = Vector3.zero;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (statusTooltipGO == null || baseHuman == null) return;
        if (statusTooltipText != null)
            statusTooltipText.text = GetNpcStatusText();
        statusTooltipGO.SetActive(true);
        RectTransform rt = statusTooltipGO.GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.DOKill();
            rt.localScale = Vector3.zero;
            rt.DOScale(Vector3.one, tooltipZoomDuration).SetEase(Ease.OutBack);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (statusTooltipGO == null) return;
        RectTransform rt = statusTooltipGO.GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.DOKill();
            rt.DOScale(Vector3.zero, tooltipZoomDuration).SetEase(Ease.InBack)
                .OnComplete(() => { if (statusTooltipGO != null) statusTooltipGO.SetActive(false); });
        }
        else
            statusTooltipGO.SetActive(false);
    }

    /// <summary>
    /// Status text for tooltip: sad/poor, sad/wealthy, happy/poor, happy/wealthy (same logic as GetNpcArtwork).
    /// </summary>
    private string GetNpcStatusText()
    {
        if (baseHuman?.stat == null) return "";
        int mid = (BaseStat.MIN_VALUE + BaseStat.MAX_VALUE) / 2;
        bool lowHappy = baseHuman.stat.Happiness < mid;
        bool lowMoney = baseHuman.stat.Money < mid;
        if (lowHappy && lowMoney) return "looks sad and tired.";
        if (lowHappy && !lowMoney) return "seems sad but has enough.";
        if (!lowHappy && lowMoney) return "looks happy but has little.";
        return "seems happy and is doing well.";
    }

    /// <summary>
    /// Build requirement: "asked for" (&gt; 0), "wanted to give you" (only &lt; 0), or "asked for" + "and in exchange give you" (mixed).
    /// </summary>
    private static string BuildRequirementText(string npcName, int money, int happiness)
    {
        var lines = new List<string>();
        bool hasAsked = money > 0 || happiness > 0;
        bool hasGive = money < 0 || happiness < 0;

        if (hasAsked)
        {
            var asked = new List<string>();
            if (money > 0) asked.Add(money + " money");
            if (happiness > 0) asked.Add(happiness + " happiness");
            if (asked.Count > 0)
                lines.Add(npcName + " asked for " + string.Join(", ", asked));
        }

        if (hasGive)
        {
            var give = new List<string>();
            if (money < 0) give.Add(Mathf.Abs(money) + " money");
            if (happiness < 0) give.Add(Mathf.Abs(happiness) + " happiness");
            if (give.Count > 0)
            {
                if (hasAsked)
                    lines.Add("and in exchange give you " + string.Join(", ", give));
                else
                    lines.Add(npcName + " wanted to give you " + string.Join(", ", give));
            }
        }

        return lines.Count > 0 ? string.Join("\n", lines) : "";
    }

    /// <summary>
    /// Cập nhật nội dung NPC và chạy animation vào từ phải sang trái (không drift).
    /// </summary>
    public void SetNpc(BaseHuman baseHuman, string context, int happiness, int money)
    {
        this.baseHuman = baseHuman;
        nameText.text = baseHuman.name;
        string requirement = BuildRequirementText(baseHuman.name, money, happiness);
        contextText.text = string.IsNullOrEmpty(requirement) ? context : context + "\n" + requirement;
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
        string requirement = BuildRequirementText(baseHuman.name, money, happiness);
        contextText.text = string.IsNullOrEmpty(requirement) ? context : context + "\n" + requirement;
        npcImage.sprite = GetNpcArtwork();
        if (rectTransform == null) rectTransform = GetComponent<RectTransform>();
        if (rectTransform != null)
            rectTransform.anchoredPosition = displayAnchoredPos + Vector2.right * entranceOffsetX;
    }
    public void SetOffer(int happiness, int money)
    {
        offerImage[0].gameObject.SetActive(true);
        offerImage[1].gameObject.SetActive(true);
        Debug.Log("Setting offer: happiness " + happiness + ", money " + money);
        
        if (happiness > 0)
        {
            offerImage[0].sprite = offerSprites[0];
        }
        else if (happiness < 0)
        {
            offerImage[0].sprite = offerSprites[1];
        }
        else
        {
            offerImage[0].gameObject.SetActive(false);
        }
        if (money > 0)
        {
            offerImage[1].sprite = offerSprites[2];
        }
        else if (money < 0)
        {
            offerImage[1].sprite = offerSprites[3];
        }
        else
        {
            offerImage[1].gameObject.SetActive(false);
        }
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
