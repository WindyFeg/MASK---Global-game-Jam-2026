using UnityEngine;
using System;

[System.Serializable]
public class BaseStat {
    [SerializeField] private int _happiness;
    [SerializeField] private int _money;
    
    // Giới hạn chỉ số (ví dụ: 0 đến 5)
    public const int MAX_VALUE = 5;
    public const int MIN_VALUE = 0;

    public BaseStat(int happiness, int money) {
        _happiness = happiness;
        _money = money;
    }

    // Getter / Setter với logic kẹp giá trị (Clamp)
    public int Happiness {
        get => _happiness;
        set => _happiness = Mathf.Clamp(value, MIN_VALUE, MAX_VALUE);
    }

    public int Money {
        get => _money;
        set => _money = Mathf.Clamp(value, MIN_VALUE, MAX_VALUE);
    }

    public bool IsEnd() {
        return _happiness <= 0 || _money <= 0;
    }

    public bool IsFull() {
        return _happiness >= MAX_VALUE && _money >= MAX_VALUE;
    }
}