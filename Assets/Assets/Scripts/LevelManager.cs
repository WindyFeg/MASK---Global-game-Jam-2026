using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public List<Npc> humanPool = new List<Npc>();
    public Player player;
    public static LevelManager instance;
    private void Awake()
    {
        // Chống tạo trùng GameManager khi load scene khác
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        // Giữ object này khi chuyển scene
        DontDestroyOnLoad(gameObject);
    }
    public Npc LoadLevel()
    {
        return humanPool[Random.Range(0, 3)];
        
    }
}
