using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UITextBoxEditor : UIPanel, IUIViewGridParent {
    
    private const string ConfirmButtonId = "SPELL_NAME_CONFIRM_BTN";
    private const string CancelButtonId = "CANCEL_BTN";

    [SerializeField] private UICustomInputField _inputField;
    [SerializeField] private IntVector3 _inputFieldDir; // this is some hacky shit
    [SerializeField] private UIViewGrid _buttonsView;
    [SerializeField] private UICustomButton _buttonPrefab;

    public event Action OnNameUpdated;
    public event Action OnNameChangeCancel;

    public string TextValue { get; private set; }

    private int[] _rowLengths = new int[] {
        1, 1
    };

    public event UpdateActiveGrid OnUpdateActiveGrid;

    public override void Initialize(UIPanelInitData initData) {
        base.Initialize(initData);
        SpellNameEditorInitData spellNameEditorData = initData as SpellNameEditorInitData;
        if(spellNameEditorData == null) {
            return;
        }
        _inputField.OnMousePointerClick += OnInputFieldSelected;
        _inputField.onEndEdit.AddListener(OnTextChanged);
        _inputField.StartActive = true;
        InitializeButtonView();
        _buttonsView.SetActive(false);
    }

    private void InitializeButtonView() {
        UIViewGridInitData initData = new UIViewGridInitData() {
            RowLengths = _rowLengths
        };
        _buttonsView.Initialize(initData);
        _buttonsView.SetInteractableItem(0, 0, new UICustomButtonInitData() {
            X = 0,
            Y = 0,
            Id = ConfirmButtonId,
            ButtonText = "Confirm"
        });
        _buttonsView.SetInteractableItem(1, 0, new UICustomButtonInitData() {
            X = 1,
            Y = 0,
            Id = CancelButtonId,
            ButtonText = "Cancel"
        });
        _buttonsView.OnSelectPressed += OnButtonsViewSubmit;
    }

    private void OnInputFieldSelected(IUIInteractable inputField) {
        _buttonsView.SetActive(false);
    }

    private void OnTextChanged(string name) {
        TextValue = name;
        _inputField.InteractableUnhighlight();
        CoroutineGod.Instance.ExecuteAfterOneFrame(()=> {
            _buttonsView.SetActive(true);
        });
    }

    private void OnButtonsViewSubmit(IUIInteractable interactable) {
        string id = interactable.Id;
        if (id.Equals(ConfirmButtonId)) {
            UIPanelManager.Instance.PassStringData(TextValue);
        }
        ClosePanel();
    }

    private void OnButtonsViewHover(IUIInteractable interactable) {
        _inputField.InteractableUnhighlight();
        _buttonsView.SetActive(true);
    }

    public override void ClosePanel() {
        base.ClosePanel();
        _inputField.onEndEdit.RemoveListener(OnTextChanged);
        _inputField.OnMousePointerClick -= OnInputFieldSelected;
        _buttonsView.OnSelectPressed -= OnButtonsViewSubmit;
        _buttonsView.OnHighlighted -= OnButtonsViewHover;
    }

    protected override void MenuBtnAction() {
        // do something only if we're highlighting the inputfield
        if(GetCurrentInteractable() == _inputField) {
            _inputField.InteractableUnhighlight();
            CoroutineGod.Instance.ExecuteAfterOneFrame(() =>
            {
                _buttonsView.SetActive(true);
            });
        }
    }

    public IUIInteractable GetCurrentInteractable() {
        return _buttonsView.Active ?
            _buttonsView.GetInteractableAt(_buttonsView.CurrentItemX, _buttonsView.CurrentItemY) : _inputField;
    }

    public void UpdateActiveGrid(UIViewGrid newGrid) {
        
    }

    public void OutOfBounds(IntVector3 dir) {
        if(dir == _inputFieldDir) {
            _buttonsView.SetActive(false);
            _inputField.InteractableHighlight();
        }
    }
}

public class SpellNameEditorInitData : UIPanelInitData {
    public bool ShowCancelButton;
    public string InitialName;
}
