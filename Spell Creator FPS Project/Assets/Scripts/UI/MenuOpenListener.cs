using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuOpenListener : MonoBehaviour {

    [SerializeField] private string _menuPrefabId;

    private void Start() {
        PlayerController.Instance.OnControllerStateUpdated += OnControllerStateUpdated;
        OnControllerStateUpdated();
    }

    private void OnControllerStateUpdated() {
        bool active = PlayerController.Instance.ControllerState == ControllerState.Gameplay;
        PlayerController.Instance.OnCancelPressed -= OnCancelPressed;
        if (active) {
            PlayerController.Instance.OnCancelPressed += OnCancelPressed;
        }
    }

    private void OnCancelPressed() {
        UIPanelManager.Instance.OpenUIPanel(_menuPrefabId);
    }
}
