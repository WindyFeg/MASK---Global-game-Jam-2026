using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Requirement
{
    // Tên field phải khớp JSON: "requirement": { "money": 2, "happiness": 0 }
    public int money;
    public int happiness;

    public int Money => money;
    public int Happiness => happiness;
}

[System.Serializable]
public class ScenarioEntry
{
    public string npcType;
    public string state;
    public string context;
    public string dialogue;
    public Requirement requirement; // Khớp JSON, parse đúng thay vì BaseStat
}

[System.Serializable]
public class ScenarioList {
    public List<ScenarioEntry> data;
}