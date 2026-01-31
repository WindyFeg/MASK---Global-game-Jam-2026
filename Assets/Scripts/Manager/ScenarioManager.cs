using System;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class RequirementData
{ // Class này để hứng { "money": 2, "happiness": 0 }
    public int money;
    public int happiness;
}

public class ScenarioManager : MonoBehaviour
{
    public static ScenarioManager Instance;

    private Dictionary<(NPCType, Emotion), List<ScenarioEntry>> _scenarioDict;

    private void Awake()
    {
        Instance = this;
        LoadScenarios();
    }

    private void LoadScenarios()
    {
        _scenarioDict = new Dictionary<(NPCType, Emotion), List<ScenarioEntry>>();

        // Đảm bảo tên file đúng với file trong thư mục Resources
        TextAsset jsonFile = Resources.Load<TextAsset>("npc_scenarios");

        if (jsonFile == null)
        {
            Debug.LogError("Không tìm thấy file JSON!");
            return;
        }

        // Dòng này sẽ TỰ ĐỘNG parse cả requirement nếu Class đã khai báo đúng
        ScenarioList rawData = JsonUtility.FromJson<ScenarioList>(jsonFile.text);

        if (rawData == null || rawData.data == null) return;

        foreach (var entry in rawData.data)
        {
            try
            {
                // 1. Parse NPC Type
                NPCType typeEnum = (NPCType)Enum.Parse(typeof(NPCType), entry.npcType);

                Emotion stateEnum = (Emotion)Enum.Parse(typeof(Emotion), entry.state);

                Debug.Log($"Parsed Context: > {entry.context} < NPCType: {typeEnum}, Emotion: {stateEnum}, Requirement: {entry.requirement.Happiness}, {entry.requirement.Money}");

                if (entry.requirement == null)
                {
                    Debug.LogWarning($"Yêu cầu trống tại bối cảnh '{entry.context}'");
                    continue;
                }

                var key = (typeEnum, stateEnum);

                if (!_scenarioDict.ContainsKey(key))
                {
                    _scenarioDict[key] = new List<ScenarioEntry>();
                }

                _scenarioDict[key].Add(entry);
            }
            catch (Exception e)
            {
                // Log warning để biết dòng nào trong JSON bị sai tên Enum
                Debug.LogWarning($"Lỗi parse tại bối cảnh '{entry.context}': {e.Message}");
            }
        }
    }

    // Hàm lấy Scenario ngẫu nhiên
    public ScenarioEntry GetRandomScenario(NPCType type, Emotion state)
    {
        if (_scenarioDict.TryGetValue((type, state), out var list))
        {
            if (list.Count > 0)
            {
                return list[UnityEngine.Random.Range(0, list.Count)];
            }
        }
        return null;
    }
}