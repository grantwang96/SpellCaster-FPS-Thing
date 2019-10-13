using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// This is likely going to be a temporary class
public class UISaveButton : MonoBehaviour {

    [SerializeField] private Button button;

	// Use this for initialization
	void Start () {
        button.onClick.AddListener(SaveGame);
	}

    private void OnDestroy() {
        button.onClick.RemoveAllListeners();
    }

    private void SaveGame() {
        SaveManager.SaveGame();
    }
}
