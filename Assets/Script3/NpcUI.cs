using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using DG.Tweening;
using UnityEditor;

public class NpcUI : MonoBehaviour
{
    public BaseHuman baseHuman;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI contextText;
    public Image npcImage;

    public void SetNpc(BaseHuman baseHuman, string context, int happiness, int money)
    {
        this.baseHuman = baseHuman;
        nameText.text = baseHuman.name;
        contextText.text = context + "\n" + "Asked for: " + money + " money, " + happiness + " happiness";
        npcImage.sprite = GetNpcArtwork();

        // DOtween it appear from right to left
        transform.DOMoveX(transform.position.x + 100, 1f).SetEase(Ease.InOutSine);
        // Dotwwen for it to small and breathing
        transform.DOScale(0.9f, 1f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
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
