using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ManagerOverlord : MonoBehaviour {

    public static ManagerOverlord Instance { get; private set; }

    // constant values that might be necessary
    [SerializeField] private string _resourcesBasePath;
    public string ResourcesBasePath => _resourcesBasePath;

    // managers to instantiate
    public readonly IPlayerDataManager PlayerDataManager;
    public readonly ILootManager LootManager;
    public readonly ISpellCraftManager SpellCraftManager;
    public readonly GameManager GameManager;

    private void Awake() {
        Instance = this;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode) {
        
    }

    // Use this for initialization
    private void Start () {
		
	}

    private void InitializeManagers() {

    }
}
