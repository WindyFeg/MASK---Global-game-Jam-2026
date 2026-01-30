using UnityEngine;

[CreateAssetMenu(fileName = "NewCard", menuName = "Cards/BaseCard")]
public class BaseCard : ScriptableObject {
    public int id;
    public string cardName;
    public string description;
    public BaseStat selfStat;
    public BaseStat opponentStat;
    public Sprite artwork;
}