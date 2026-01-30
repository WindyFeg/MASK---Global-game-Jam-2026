using UnityEngine;

public class Npc : HumanBase
{
    public Npc(string name, int happiness, int money) : base(name, happiness, money)
    {

    }

    public override void ValidateStat()
    {
        if (Stats.IsFull())
        {
            Debug.Log("na: NPC stats is full" + Stats.Happiness + Stats.Money);
        }
        else if (Stats.IsEnd())
        {
            Debug.Log("na: NPC out of stat!" + Stats.Happiness + Stats.Money);
        }
    }
}