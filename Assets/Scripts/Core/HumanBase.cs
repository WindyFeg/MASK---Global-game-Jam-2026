using UnityEngine;

public abstract class HumanBase {
    BaseHuman data;

    public bool IsAlive() {
        return !data.stat.IsEnd();
    }

    public bool IsFullStats() {
        return data.stat.IsFull();
    }

    public void AddHappiness(int value) {
        data.stat.Happiness += value;
        ValidateStat();
    }

    public void AddMoney(int value) {
        data.stat.Money += value;
        ValidateStat();
    }

    public int GetMoney() {
        return data.stat.Money;
    }

    public int GetHappiness() {
        return data.stat.Happiness;
    }

    public abstract void ValidateStat();
}