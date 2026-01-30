using UnityEngine;

public abstract class HumanBase {
    public string Name;
    public BaseStat Stats; // Dùng chung cho cả Player và NPC

    // Constructor cơ bản
    public HumanBase(string name, int happiness, int money) {
        this.Name = name;
        this.Stats = new BaseStat(happiness, money);
    }

    public bool IsAlive() {
        return !Stats.IsEnd();
    }

    public bool IsFullStats() {
        return Stats.IsFull();
    }

    public void ChangeStat(BaseStat change) {
        Stats.Happiness += change.Happiness;
        Stats.Money += change.Money;
        // Debug cho tiện theo dõi
        Debug.Log($"{Name} stats changed: HP={Stats.Happiness}, Money={Stats.Money}");
    }
}