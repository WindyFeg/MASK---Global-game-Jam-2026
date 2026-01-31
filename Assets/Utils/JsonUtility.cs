using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Requirement {
    public int money;      // Số tiền Player cần chi (Cost)
    public int happiness;  // Số happiness Player cần bỏ ra (Cost)
}

[System.Serializable]
public class ScenarioEntry {
    public string npcType;
    public string state;        // Tên Enum mới (Crisis, Fulfilled...)
    public string context;      // Bối cảnh (để hiển thị tooltip hoặc debug)
    public string dialogue;     // Câu thoại
    public BaseStat requirement; // Yêu cầu
}

[System.Serializable]
public class ScenarioList {
    public List<ScenarioEntry> data;
}