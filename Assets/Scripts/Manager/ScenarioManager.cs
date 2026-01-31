using System;
using System.Collections.Generic;
using UnityEngine;

public class ScenarioManager : MonoBehaviour {
    public static ScenarioManager Instance;
    
    private Dictionary<(NPCType, Emotion), List<ScenarioEntry>> _scenarioDict;

    private void Awake() {
        Instance = this;
        LoadScenarios();
    }

    private void LoadScenarios() {
        _scenarioDict = new Dictionary<(NPCType, Emotion), List<ScenarioEntry>>();
        TextAsset jsonFile = Resources.Load<TextAsset>("npc_scenarios_v2"); // Tên file json mới

        if (jsonFile == null) return;

        ScenarioList rawData = JsonUtility.FromJson<ScenarioList>(jsonFile.text);

        foreach (var entry in rawData.data) {
            try {
                NPCType typeEnum = (NPCType)Enum.Parse(typeof(NPCType), entry.npcType);
                Emotion stateEnum = (Emotion)Enum.Parse(typeof(Emotion), entry.state);

                var key = (typeEnum, stateEnum);

                if (!_scenarioDict.ContainsKey(key)) {
                    _scenarioDict[key] = new List<ScenarioEntry>();
                }
                
                // Lưu cả object ScenarioEntry thay vì chỉ string dialogue
                // Để sau này truy cập được context và requirement
                _scenarioDict[key].Add(entry);
            }
            catch (Exception e) {
                Debug.LogWarning($"Lỗi parse: {e.Message}");
            }
        }
    }

    // Hàm lấy Scenario ngẫu nhiên
    public ScenarioEntry GetRandomScenario(NPCType type, Emotion state) {
        if (_scenarioDict.TryGetValue((type, state), out var list)) {
            if (list.Count > 0) {
                return list[UnityEngine.Random.Range(0, list.Count)];
            }
        }
        return null; 
    }
}