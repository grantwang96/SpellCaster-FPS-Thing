using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuOpenListener : MonoBehaviour {

    [SerializeField] private string _menuPrefabId;

    private void Start() {
        GameplayController.Instance.OnControllerStateUpdated += OnControllerStateUpdated;
        OnControllerStateUpdated();
    }

    private void OnControllerStateUpdated() {
        bool active = GameplayController.Instance.ControllerState == ControllerState.Gameplay;
        GameplayController.Instance.OnCancelPressed -= OnCancelPressed;
        if (active) {
            GameplayController.Instance.OnCancelPressed += OnCancelPressed;
        }
    }

    private void OnCancelPressed() {
        UIManager.Instance.OpenUIPanel(_menuPrefabId);
    }
}
