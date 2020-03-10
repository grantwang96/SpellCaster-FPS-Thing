using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfigManager : MonoBehaviour
{
    public static ConfigManager Instance { get; private set; }

    [SerializeField] private UIConfigData _uiConfigData;
    public UIConfigData UIConfigData => _uiConfigData;
    [SerializeField] private ChestConfig _chestConfig;
    public ChestConfig ChestConfigData => _chestConfig;

    private void Awake() {
        Instance = this;
    }
}
