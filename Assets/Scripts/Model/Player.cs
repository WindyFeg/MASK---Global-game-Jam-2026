using UnityEngine;

public class Player : BaseHuman
{
    public int maxSanity;
    public int curSanity;

    public override void ValidateStat()
    {
        if (IsOutOfSanity())
        {
            Debug.Log("na: Player is out of sanity!");
        }
    }

    public bool IsOutOfSanity()
    {
        return curSanity <= 0;
    }
    public int GetSanity()
    {
        if (IsOutOfSanity())
        {
            Debug.Log("na: Player is out of sanity!");
        }
        return curSanity;
    }

}