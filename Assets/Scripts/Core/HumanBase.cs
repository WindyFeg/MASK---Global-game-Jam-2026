using UnityEngine;

public abstract class HumanBase : BaseHuman {
    public bool IsAlive() {
        return !stat.IsEnd();
    }

    public bool IsFullStats() {
        return stat.IsFull();
    }

    public void AddHappiness(int value) {
        stat.Happiness += value;
        ValidateStat();
    }

    public void AddMoney(int value) {
        stat.Money += value;
        ValidateStat();
    }

    public int GetMoney() {
        return stat.Money;
    }

    public int GetHappiness() {
        return stat.Happiness;
    }

    public abstract void ValidateStat();
}