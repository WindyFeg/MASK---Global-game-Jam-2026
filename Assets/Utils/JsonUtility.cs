using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ScenarioEntry
{
    // Lưu dưới dạng string để tránh lỗi parse JSON ban đầu
    public string npcType; 
    public string emotion;
    public List<string> dialogues;
}

[System.Serializable]
public class ScenarioList
{
    public List<ScenarioEntry> data;
}