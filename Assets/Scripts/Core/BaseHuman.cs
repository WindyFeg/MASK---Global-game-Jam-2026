using UnityEngine;

[CreateAssetMenu(fileName = "NewHuman", menuName = "Cards/BaseHuman")]
public class BaseHuman : ScriptableObject {
    public int id;
    public string name;
    public BaseStat stat;
    public Sprite artwork;
}