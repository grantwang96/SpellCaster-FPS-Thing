using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISpellOptionsMenu : UISubPanel {

    [SerializeField] private UICustomButton _buttonPrefab;
    [SerializeField] private RectTransform _menuListContainer;
    [SerializeField] private RectTransform _menuListContent;
    [SerializeField] private List<UICustomButton> _options = new List<UICustomButton>();
    [SerializeField] private int _buttonCount;

    [SerializeField] private UISubPanel _previousSubPanel;
    [SerializeField] private bool _inverted;

    private bool _isBuilt = false;

    private bool _inLoadout = false;
    private int _selectedIndex = 0;

    public override void Initialize(UIPanelInitData initData) {
        base.Initialize(initData);
        if (_isBuilt) { return; }
        InitializeButtons();
        _isBuilt = true;
    }

    private void InitializeButtons() {
        for(int i = 0; i < _buttonCount; i++) {
            GenerateButton(i);
        }
    }

    private void GenerateButton(int xCoord) {
        UICustomButton newButton = Instantiate(_buttonPrefab, _menuListContent);
        newButton.OnMousePointerHighlight += OnButtonHighlighted;
        _options.Add(newButton);
    }

    public void SetValue(SpellOptionsInitData init) {
        PositionMenu(init.Position);
        _buttonCount = init.ButtonDatas.Count;
        int count = init.ButtonDatas.Count > _options.Count ? init.ButtonDatas.Count : _options.Count;
        for(int i = 0; i < count; i++) {
            if (i >= _options.Count) {
                GenerateButton(i);
            } else if(i >= init.ButtonDatas.Count) {
                _options[i].gameObject.SetActive(false);
                continue;
            }
            _options[i].gameObject.SetActive(true);
            _options[i].onClick.RemoveAllListeners();
            _options[i].SetValue(new UICustomButtonInitData() {
                Id = init.ButtonDatas[i].ButtonId,
                X = i,
                ButtonText = init.ButtonDatas[i].ButtonText
            });
            // Use onClick for actions that don't use data (i.e. 'Change Spell' doesn't need any UIInteractable stuff)
            _options[i].onClick.AddListener(init.ButtonDatas[i].Action);
        }
    }

    private void PositionMenu(Vector2 position) {
        _menuListContainer.anchoredPosition = position;
    }

    public override void SetFocus(bool isFocused, bool hardLocked, IntVector3 dir) {
        base.SetFocus(isFocused, hardLocked, dir);
        UnsubscribeToController();
        if (isFocused) {
            SubscribeToController();
            _selectedIndex = 0;
            UpdateHighlightedButton();
        }
        SetVisible(isFocused);
    }

    private void DirectionalInputs(Vector2 input) {
        float horizontal = _inverted ? -input.y : input.x;

        int selected = _selectedIndex + Mathf.RoundToInt(horizontal);
        if (selected < 0) { selected = _buttonCount - 1; }
        if (selected >= _buttonCount) { selected = 0; }

        _selectedIndex = selected;
        UpdateHighlightedButton();
    }

    private void UpdateHighlightedButton() {
        for (int i = 0; i < _options.Count; i++) {
            if (i == _selectedIndex) {
                _options[i].InteractableHighlight();
                continue;
            }
            _options[i].InteractableUnhighlight();
        }
    }

    // if interacted via select button and not mouse click
    private void SelectButtonInput() {
        _options[_selectedIndex].InteractableSelect();
    }

    private void OnButtonHighlighted(IUIInteractable interactable) {
        _selectedIndex = interactable.XCoord;
        UpdateHighlightedButton();
    }

    private void SubscribeToController() {
        PlayerController.Instance.DirectionalInput += DirectionalInputs;
        PlayerController.Instance.OnSubmitPressed += SelectButtonInput;
    }

    private void UnsubscribeToController() {
        PlayerController.Instance.DirectionalInput -= DirectionalInputs;
        PlayerController.Instance.OnSubmitPressed -= SelectButtonInput;
    }

    protected override void OnActivePanelUpdated(bool isCurrentPanel) {
        base.OnActivePanelUpdated(isCurrentPanel);
        UnsubscribeToController();
        if (isCurrentPanel && IsFocused) {
            SubscribeToController();
        }
    }
}

public class SpellOptionsInitData : UIPanelInitData{
    public Vector2 Position;
    public List<ButtonActionData> ButtonDatas;
}