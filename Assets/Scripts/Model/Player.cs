using UnityEngine;

public class Player : HumanBase {
    
    public Player(string name, int happiness, int money) : base(name, happiness, money)
    {
        
    }

    public override void ValidateStat()
    {
        if(Stats.IsFull())
        {
            Debug.Log("na: Player stats is full" + Stats.Happiness + Stats.Money);
        }
        else if (Stats.IsEnd())
        {
            Debug.Log("na: Player out of stat!" + Stats.Happiness + Stats.Money);
        }
    }
}