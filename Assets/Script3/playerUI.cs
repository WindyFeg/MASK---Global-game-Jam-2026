using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class playerUI : MonoBehaviour
{
    // 3 textmeshpro for money, happiness, sanity
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI happinessText;
    public TextMeshProUGUI sanityText;
    public Player player;

    [Header("Sanity stages (5 sprites)")]
    [Tooltip("Index 0 = sanity 1 (worst), Index 4 = sanity 5 (full). Assign 5 sprites in order.")]
    [SerializeField] private Sprite[] sanityStageSprites = new Sprite[5];
    [SerializeField] private Image sanityStageImage;

    public void SetPlayer(Player player)
    {
        moneyText.text = "💰 " + player.GetMoney().ToString() + " / " + BaseStat.MAX_VALUE;
        happinessText.text = "😊 " + player.GetHappiness().ToString() + " / " + BaseStat.MAX_VALUE;
        sanityText.text = "🧠 " + player.GetSanity() + "/" + player.maxSanity;
        this.player = player;

        UpdateSanityStageImage(player.GetSanity());
    }

    private void UpdateSanityStageImage(int sanity)
    {
        if (sanityStageImage == null || sanityStageSprites == null || sanityStageSprites.Length == 0)
            return;
        int index = Mathf.Clamp(sanity - 1, 0, sanityStageSprites.Length - 1);
        if (sanityStageSprites[index] != null)
            sanityStageImage.sprite = sanityStageSprites[index];
    }
}
