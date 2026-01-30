using System.Data.Common;
using UnityEngine;

public class Npc : HumanBase
{
    public Emotion curEmotion;

    public override void ValidateStat()
    {
        
    }

    public void RandomEmotion()
    {
        var rnd = Random.Range(0, 5);
        curEmotion = (Emotion)rnd;
        Debug.Log("na: NPC Emotion changed to " + curEmotion.ToString());
    }

    public Emotion GetCurrentEmotion()
    {
        return curEmotion;
    }
}