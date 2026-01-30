using UnityEngine;

[CreateAssetMenu(fileName = "NewCard", menuName = "Cards/BaseCard")]
public class BaseCard : ScriptableObject {
    public string cardName;
    public BaseStat selfStat;
    public BaseStat opponentStat;
}