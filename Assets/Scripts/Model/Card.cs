using UnityEngine;

public class Card : BaseCard 
{
    public BaseCard cardData;

    public void useCard() {
        GameManager.instance.UseCard(this);
    }
}