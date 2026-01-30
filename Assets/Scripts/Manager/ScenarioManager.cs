using UnityEngine;
using System.Collections.Generic;
using System;

public class ScenarioManager : MonoBehaviour
{
    public static ScenarioManager Instance;

    #region Private Fields

    private Dictionary<(NPCType, Emotion), List<string>> _scenarioDict;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        LoadScenarios();
    }

    private void LoadScenarios()
    {
        _scenarioDict = new Dictionary<(NPCType, Emotion), List<string>>();

        // 1. Load file từ Resources (không cần đuôi .json)
        TextAsset jsonFile = Resources.Load<TextAsset>("npc_scenarios");

        if (jsonFile == null)
        {
            Debug.LogError("Không tìm thấy file 'npc_scenarios' trong thư mục Resources!");
            return;
        }

        // 2. Parse JSON sang Object Wrapper
        ScenarioList rawData = JsonUtility.FromJson<ScenarioList>(jsonFile.text);

        // 3. Chuyển đổi sang Dictionary và Parse Enum
        foreach (var entry in rawData.data)
        {
            try
            {
                // Convert String "Mom" -> Enum NPCType.Mom
                NPCType typeEnum = (NPCType)Enum.Parse(typeof(NPCType), entry.npcType);
                
                // Convert String "Happy" -> Enum Emotion.Happy
                Emotion emoEnum = (Emotion)Enum.Parse(typeof(Emotion), entry.emotion);

                // Lưu vào Dictionary
                _scenarioDict[(typeEnum, emoEnum)] = entry.dialogues;
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Lỗi parse dữ liệu tại dòng: {entry.npcType} - {entry.emotion}. Lỗi: {e.Message}");
            }
        }

        Debug.Log("Đã load thành công kịch bản NPC!");
    }
    
    #endregion
    public string GetRandomDialogue(NPCType type, Emotion emotion)
    {
        // Kiểm tra xem có key này trong từ điển không
        if (_scenarioDict.ContainsKey((type, emotion)))
        {
            List<string> dialogues = _scenarioDict[(type, emotion)];
            if (dialogues.Count > 0)
            {
                return dialogues[UnityEngine.Random.Range(0, dialogues.Count)];
            }
        }

        return "..."; // Trả về mặc định nếu không tìm thấy
    }
}