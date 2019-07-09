using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Class that stores data about the player(like tutorials completed, favorite spell, etc.)
/// </summary>

public delegate void PlayerFlagsUpdatedEvent();
public delegate void PlayerCountersUpdatedEvent();

public interface IPlayerDataManager {
    void SetFlag(string key, bool value);
    void SetCounter(string key, int value);
    bool GetFlag(string key);
    int GetCounter(string key);
    bool HasFlag(string key);
    bool HasCounter(string key);

    event PlayerFlagsUpdatedEvent OnPlayerFlagsUpdated;
    event PlayerCountersUpdatedEvent OnPlayerCountersUpdated;
}

public class PlayerDataManager : MonoBehaviour, IPlayerDataManager {

    public static IPlayerDataManager Instance;

    private Dictionary<string, bool> _flags = new Dictionary<string, bool>();
    private Dictionary<string, int> _counters = new Dictionary<string, int>();

    public event PlayerFlagsUpdatedEvent OnPlayerFlagsUpdated;
    public event PlayerCountersUpdatedEvent OnPlayerCountersUpdated;

    private void Awake() {
        Instance = this;
    }

    public int GetCounter(string key) {
        if (!HasCounter(key)) {
            return -1;
        }
        return _counters[key];
    }

    public bool GetFlag(string key) {
        if (!HasFlag(key)) {
            return false;
        }
        return _flags[key];
    }

    public bool HasCounter(string key) {
        return _counters.ContainsKey(key);
    }

    public bool HasFlag(string key) {
        return _flags.ContainsKey(key);
    }

    public void SetCounter(string key, int value) {
        _counters[key] = value;
    }

    public void SetFlag(string key, bool value) {
        _flags[key] = value;
    }
}