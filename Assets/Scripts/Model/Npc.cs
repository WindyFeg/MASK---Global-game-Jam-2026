using System.Data.Common;
using UnityEngine;
[CreateAssetMenu(fileName = "NewHuman", menuName = "Cards/Npc")]
public class Npc : HumanBase
{
    public Emotion curEmotion;

    public override void ValidateStat()
    {
        
    }

    public void RandomEmotion()
    {
        
    }

    public Emotion GetCurrentEmotion()
    {
        return curEmotion;
    }
}