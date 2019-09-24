using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenericMessageBox : UIPanel {

    [SerializeField] private bool _inverted;
    
    private float _horizontal;

    [SerializeField] private Text _header;
    [SerializeField] private Text _message;
    
    [SerializeField] private UICustomButton[] _customButtons;
    private int _selectedIndex;
    private int _buttonCount;

    public event UpdateActiveGrid OnUpdateActiveGrid;

    protected override void Start() {
        base.Start();
        for(int i = 0; i < _customButtons.Length; i++) {
            _customButtons[i].Initialize(i, 0);
            _customButtons[i].OnMousePointerHighlight += OnButtonHighlighted;
        }
    }

    public override void Initialize(UIPanelInitData initData) {
        base.Initialize(initData);
        _selectedIndex = 0;
        GenericMessageBoxInitData messageBoxData = initData as GenericMessageBoxInitData;
        if(messageBoxData == null) {
            Debug.LogError($"[{nameof(GenericMessageBox)}] did not receive initData of type [{nameof(GenericMessageBoxInitData)}]");
            return;
        }
        _header.text = messageBoxData.Title;
        _message.text = messageBoxData.Content;
        if(messageBoxData.ButtonDatas.Count == 0) {
            CreateGenericCloseButton();
        } else {
            CreateCustomButtons(messageBoxData.ButtonDatas);
        }
        _customButtons[_selectedIndex].InteractableHighlight();
    }

    private void CreateGenericCloseButton() {
        ClearButtons();
        _customButtons[0].gameObject.SetActive(true);
        _customButtons[0].SetValue(
            new UICustomButtonInitData() {
                X = 0,
                Y = 0,
                Id = GameplayValues.UI.GenericButtonIdClose,
                ButtonText = "Close"
            }
        );
        _customButtons[0].OnMousePointerClick += OnGenericClosedButtonSelected;
        UpdateHighlightedButton();
    }

    private void OnGenericClosedButtonSelected(IUIInteractable interactable) {
        _customButtons[0].OnMousePointerClick -= OnGenericClosedButtonSelected;
        ClosePanel();
    }

    private void ClearButtons() {
        for (int i = 0; i < _customButtons.Length; i++) {
            _customButtons[i].gameObject.SetActive(false);
        }
    }

    private void CreateCustomButtons(IReadOnlyList<ButtonActionData> actions) {
        ClearButtons();
        for(int i = 0; i < actions.Count; i++) {
            if (i >= _customButtons.Length) {
                Debug.LogError($"[{nameof(GenericMessageBox)}] Exceeded button limit in generic message box!");
                break;
            }
            _customButtons[i].SetValue(new UICustomButtonInitData() {
                X = i,
                Y = 0,
                ButtonText = actions[i].ButtonText,
                Id = actions[i].ButtonId
            });
            _customButtons[i].onClick.RemoveAllListeners();
            _customButtons[i].gameObject.SetActive(true);
            _customButtons[i].onClick.AddListener(actions[i].Action);
            _buttonCount = i + 1;
        }
    }

    private void DirectionalInputs(Vector2 input) {
        float _horizontal = _inverted ? -input.y : input.x;

        int selected = _selectedIndex + Mathf.RoundToInt(_horizontal);
        if(selected < 0) { selected = _buttonCount - 1; }
        if(selected >= _buttonCount) { selected = 0; }

        _selectedIndex = selected;
        UpdateHighlightedButton();
    }

    private void SelectButtonInput() {
        _customButtons[_selectedIndex].InteractableSelect();
    }

    private void OnButtonHighlighted(IUIInteractable interactable) {
        _selectedIndex = interactable.XCoord;
        UpdateHighlightedButton();
    }

    private void OnButtonSelected(IUIInteractable interactable) {
        UICustomButton button = interactable as UICustomButton;
        if(button != null) {
            for(int i = 0; i < _customButtons.Length; i++) {
                if(button == _customButtons[i]) {
                    button.InteractableSelect();
                }
            }
        }
    }

    private void UpdateHighlightedButton() {
        for(int i = 0; i < _buttonCount; i++) {
            if(i == _selectedIndex) {
                _customButtons[i].InteractableHighlight();
                continue;
            }
            _customButtons[i].InteractableUnhighlight();
        }
    }

    protected override void SubscribeToGameplayController() {
        base.SubscribeToGameplayController();
        GameplayController.Instance.DirectionalInput += DirectionalInputs;
        GameplayController.Instance.OnSubmitPressed += SelectButtonInput;
    }

    protected override void UnsubscribeToGameplayController() {
        base.UnsubscribeToGameplayController();
        GameplayController.Instance.DirectionalInput -= DirectionalInputs;
        GameplayController.Instance.OnSubmitPressed -= SelectButtonInput;
    }
}

public class GenericMessageBoxInitData : UIPanelInitData {

    public readonly string Title;
    public readonly string Content;
    private readonly List<ButtonActionData> _buttonDatas = new List<ButtonActionData>();
    public IReadOnlyList<ButtonActionData> ButtonDatas => _buttonDatas;

    public GenericMessageBoxInitData(string title, string content, List<ButtonActionData> buttonActions = null) : base() {
        Title = title;
        Content = content;
        if(buttonActions != null) {
            _buttonDatas.AddRange(buttonActions);
        }
    }
}
