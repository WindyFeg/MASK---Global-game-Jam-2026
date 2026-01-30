using UnityEngine;
using TMPro;

public class playerUI : MonoBehaviour
{
    // 3 textmeshpro for money, happiness, sanity
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI happinessText;
    public TextMeshProUGUI sanityText;

    public void SetPlayer(Player player)
    {
        moneyText.text = "💰 " + player.GetMoney().ToString() + " / " + BaseStat.MAX_VALUE;
        happinessText.text = "😊 " + player.GetHappiness().ToString() + " / " + BaseStat.MAX_VALUE;
        sanityText.text = "🧠 " + player.GetSanity() + "/" + player.maxSanity;
    }
}
