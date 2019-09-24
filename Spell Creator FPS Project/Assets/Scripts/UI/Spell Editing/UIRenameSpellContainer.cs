using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIRenameSpellContainer : UISubPanel, IUIViewGridParent {

    private const string _inputFieldId = "SPELL_NAME_INPUT_FIELD";
    private const string _confirmButtonId = "SPELL_NAME_CONFIRM_BTN";
    private const string _cancelButtonId = "CANCEL_BTN";

    [SerializeField] private UIViewGrid _inputFieldGrid;
    [SerializeField] private UIViewGrid _buttonsView;
    [SerializeField] private UICustomButton _buttonPrefab;

    private IUIInteractable _currentInteractable;

    public string SpellName { get; private set; }

    public event UpdateActiveGrid OnUpdateActiveGrid;

    public override void Initialize(UIPanelInitData initData) {
        base.Initialize(initData);
        SpellNameEditorInitData spellNameEditorData = initData as SpellNameEditorInitData;
        if(spellNameEditorData == null) {
            return;
        }
        InitializeInputField(spellNameEditorData.InitialName);
        InitializeButtons(spellNameEditorData.ShowCancelButton);
    }

    public IUIInteractable GetCurrentInteractable() {
        return _currentInteractable;
    }

    private void InitializeInputField(string spellName) {
        int[] rowLengths = new int[] { 1 };
        UIViewGridInitData inputFieldViewInitData = new UIViewGridInitData() {
            RowLengths = rowLengths
        };
        _inputFieldGrid.Initialize(inputFieldViewInitData);
        UICustomInputFieldData initData = new UICustomInputFieldData() {
            X = 0,
            Y = 0,
            Id = "SPELL_NAME_INPUT_FIELD",
            InputFieldText = spellName,
        };
        _inputFieldGrid.SetInteractableItem(0, 0, initData);
    }

    private void InitializeButtons(bool showCancelButton) {
        int buttonsCount = showCancelButton ? 2 : 1; // super hacks
        int[] rowLengths = new int[] { buttonsCount };
        UIViewGridInitData buttonsViewInitData = new UIViewGridInitData() {
            RowLengths = rowLengths
        };
        _buttonsView.Initialize(buttonsViewInitData);
    }

    public void OutOfBounds(IntVector3 dir) {
        UISubPanel neighbor;
        if (dir == IntVector3.Up) {
            neighbor = _upNeighbor;
        } else if (dir == IntVector3.Right) {
            neighbor = _rightNeighbor;
        } else if (dir == IntVector3.Down) {
            neighbor = _downNeighbor;
        } else {
            neighbor = _leftNeighbor;
        }
        if (neighbor == null) {
            return;
        }
        _parentPanel.ChangePanel(neighbor, dir);
    }

    public override void SetFocus(bool isFocused, bool hardLocked, IntVector3 dir) {
        base.SetFocus(isFocused, hardLocked, dir);
        UpdateActiveGrid(_inputFieldGrid);
    }

    public void UpdateActiveGrid(UIViewGrid newGrid) {
        _inputFieldGrid.SetActive(newGrid == _inputFieldGrid);
        _buttonsView.SetActive(newGrid == _buttonsView);
        _currentInteractable = newGrid.GetInteractableAt(newGrid.CurrentItemX, newGrid.CurrentItemY);
    }

    private void OnButtonsViewSubmit(IUIInteractable interactable) {
        
    }
}

public class SpellNameEditorInitData : UIPanelInitData {
    public bool ShowCancelButton;
    public string InitialName;
}
