using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UISpellStagingArea : UISubPanel, IUIViewGridParent {

    [SerializeField] private string _loadoutPrefabId;

    [SerializeField] private RectTransform[] _componentsLists;
    [SerializeField] private RectTransform _imageTarget;
    [SerializeField] private UICustomButton _craftSpellButton;

    [SerializeField] private string _cachedSpellName;
    public string SpellName => _cachedSpellName;
    private bool _playerModifiedName;

    [SerializeField] private int[] _spellNameEditorViewRowLengths;
    [SerializeField] private int[] _spellComponentsViewRowLengths;
    [SerializeField] private int[] _craftButtonViewRowLengths;

    [SerializeField] private UIViewGrid _spellNameEditorView;
    [SerializeField] private UIViewGrid _spellComponentsView;
    [SerializeField] private UIViewGrid _craftButtonView;

    [SerializeField] private string _renameSpellPanelId;

    private UIViewGrid _cachedUIViewGrid;
    private IUIInteractable _currentInteractable;

    private SpellCraftManager _spellCraftManager;

    public delegate void SpellSlotsUpdatedEvent(string itemId);
    public event SpellSlotsUpdatedEvent OnSpellSlotsUpdated;

    public delegate void SpellSlotEvent(string itemId);
    public event SpellSlotEvent OnSpellSlotHighlighted;
    public event SpellSlotEvent OnSpellSlotSelected;

    public delegate void CraftSpellEvent();
    public event CraftSpellEvent OnCraftSpellPressed;
    public event UpdateActiveGrid OnUpdateActiveGrid;

    public override void Initialize(UIPanelInitData initData) {
        base.Initialize(initData);

        _playerModifiedName = false;
        _spellCraftManager = new SpellCraftManager();

        UIViewGridInitData spellNameEditorInitData = new UIViewGridInitData();
        spellNameEditorInitData.RowLengths = _spellNameEditorViewRowLengths;
        _spellNameEditorView.Initialize(spellNameEditorInitData);
        _spellNameEditorView.SetInteractableItem(0, 0, new UICustomButtonInitData() {
            ButtonText = "Insert Spell Name Here..."
        });

        UIViewGridInitData spellComponentsInit = new UIViewGridInitData();
        spellComponentsInit.RowLengths = _spellComponentsViewRowLengths;
        _spellComponentsView.Initialize(spellComponentsInit);
        _spellComponentsView.SetCurrentAtBound(IntVector3.Right);

        UIViewGridInitData craftButtonInit = new UIViewGridInitData();
        craftButtonInit.RowLengths = _craftButtonViewRowLengths;
        _craftButtonView.Initialize(craftButtonInit);

        SubscribeToViewGrids();
    }

    #region SUBCRIBE TO EVENTS
    private void SubscribeToViewGrids() {
        _spellNameEditorView.OnSelectPressed += OnSpellNameEditorSelected;
        _spellNameEditorView.OnHighlighted += OnSpellNameEditorHighlighted;
        _spellComponentsView.OnSelectPressed += OnSpellComponentSelected;
        _spellComponentsView.OnHighlighted += OnSpellComponentHighlighted;
        _craftButtonView.OnSelectPressed += OnCraftSpellButtonSelected;
        _craftButtonView.OnHighlighted += OnCraftSpellButtonHighlighted;
    }

    private void UnsubscribeToViewGrids() {
        _spellNameEditorView.OnSelectPressed -= OnSpellNameEditorSelected;
        _spellNameEditorView.OnHighlighted -= OnSpellNameEditorHighlighted;
        _spellComponentsView.OnSelectPressed -= OnSpellComponentSelected;
        _spellComponentsView.OnHighlighted -= OnSpellComponentHighlighted;
        _craftButtonView.OnSelectPressed -= OnCraftSpellButtonSelected;
        _craftButtonView.OnHighlighted -= OnCraftSpellButtonHighlighted;
    }
    #endregion

    #region UI FUNCTIONS AND OVERRIDES
    protected override void OnParentPanelClosed() {
        base.OnParentPanelClosed();
        UnsubscribeToViewGrids();
    }

    public void UpdateActiveGrid(UIViewGrid newGrid) {
        _craftButtonView.SetActive(newGrid == _craftButtonView);
        _spellComponentsView.SetActive(newGrid == _spellComponentsView);
        _spellNameEditorView.SetActive(newGrid == _spellNameEditorView);
        newGrid.UpdateHighlightedViewCell(newGrid.CurrentItemX, newGrid.CurrentItemY);
        _currentInteractable = newGrid.GetInteractableAt(newGrid.CurrentItemX, newGrid.CurrentItemY);
        _cachedUIViewGrid = newGrid;
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
        _craftButtonView.SetActive(false);
        _craftButtonView.UnhighlightCell(_craftButtonView.CurrentItemX, _craftButtonView.CurrentItemY);
        _spellComponentsView.SetActive(false);
        _spellComponentsView.UnhighlightCell(_spellComponentsView.CurrentItemX, _spellComponentsView.CurrentItemY);
        _spellNameEditorView.SetActive(false);
        _spellNameEditorView.UnhighlightCell(_spellNameEditorView.CurrentItemX, _spellNameEditorView.CurrentItemY);
        _parentPanel.ChangePanel(neighbor, dir);
    }

    public override void OnPointerEnter(PointerEventData eventData) {
        _parentPanel.ChangePanel(this, IntVector3.Zero);
    }

    public override void SetFocus(bool active, bool hardLocked, IntVector3 dir) {
        base.SetFocus(active, hardLocked, dir);
        if (active) {
            _spellComponentsView.SetCurrentAtBound(dir);
        }
        _spellComponentsView.SetActive(active, hardLocked);
        _craftButtonView.SetActive(false, hardLocked);
        _spellNameEditorView.SetActive(false, hardLocked);
    }

    public IUIInteractable GetCurrentInteractable() {
        return _currentInteractable;
    }

    protected override void OnActivePanelUpdated(bool isCurrentPanel) {
        base.OnActivePanelUpdated(isCurrentPanel);
        if (isCurrentPanel) {
            _spellNameEditorView.SetActive(_spellNameEditorView == _cachedUIViewGrid && IsFocused);
            _spellComponentsView.SetActive(_spellComponentsView == _cachedUIViewGrid && IsFocused);
            _craftButtonView.SetActive(_craftButtonView == _cachedUIViewGrid && IsFocused);
        } else {
            _spellNameEditorView.SetActive(false);
            _spellComponentsView.SetActive(false);
            _craftButtonView.SetActive(false);
        }
    }
    #endregion

    #region SPELL EDITS
    public void SetCastingMethod(Spell_CastingMethod castingMethod) {

        _spellCraftManager.SetCastingMethod(castingMethod);

        SpellComponentData componentData = new SpellComponentData(0, 0);
        // temp
        componentData.Id = castingMethod.Id;
        componentData.Name = castingMethod.Name;
        componentData.Text = castingMethod.Name;
        
        _spellComponentsView.SetInteractableItem(0, 0, componentData);
        OnSpellSlotsUpdated?.Invoke(_spellComponentsView.GetInteractableAt(_spellComponentsView.CurrentItemX, _spellComponentsView.CurrentItemY).Id);
    }

    public bool AddSpellEffect(Effect spellEffect) {
        if (_spellCraftManager.LoadedSpellEffects.Contains(spellEffect)) {
            // do failed effect here
            Debug.Log($"Already contains component {spellEffect.Id}");
            return false;
        }
        _spellCraftManager.AddSpellEffect(spellEffect);

        SpellComponentData componentData = new SpellComponentData(1, 0);
        componentData.Id = spellEffect.Id;
        componentData.Name = spellEffect.Name;
        componentData.Text = spellEffect.Name;

        _spellComponentsView.AddInteractableItemToRow(1, componentData);
        OnSpellSlotsUpdated?.Invoke(_spellComponentsView.GetInteractableAt(_spellComponentsView.CurrentItemX, _spellComponentsView.CurrentItemY).Id);
        return true;
    }

    public bool AddSpellModifier(SpellModifier spellModifier) {

        if (_spellCraftManager.LoadedSpellModifiers.Contains(spellModifier)) {
            // do failed effect here
            Debug.Log($"Already contains component {spellModifier.Id}");
            return false;
        }

        SpellComponentData componentData = new SpellComponentData(2, 0);
        componentData.Id = spellModifier.Id;
        componentData.Name = spellModifier.Name;
        componentData.Text = spellModifier.Name;

        _spellComponentsView.AddInteractableItemToRow(2, componentData);
        OnSpellSlotsUpdated?.Invoke(_spellComponentsView.GetInteractableAt(_spellComponentsView.CurrentItemX, _spellComponentsView.CurrentItemY).Id);
        return true;
    }

    public void RemoveHighlightedSpellComponent() {
        int x = _spellComponentsView.CurrentItemX;
        int y = _spellComponentsView.CurrentItemY;
        _spellComponentsView.RemoveInteractableFromRow(x, y);
        OnSpellSlotsUpdated?.Invoke(_spellComponentsView.GetInteractableAt(x, y).Id);
    }

    public void ClearSpellComponentSlots() {
        for (int i = 0; i < _spellComponentsViewRowLengths.Length; i++) {
            for(int j = 0; j < _spellComponentsViewRowLengths[i]; j++) {
                _spellComponentsView.RemoveInteractableFromRow(i, 0);
            }
        }
        OnSpellSlotsUpdated?.Invoke(_spellComponentsView.GetInteractableAt(_spellComponentsView.CurrentItemX, _spellComponentsView.CurrentItemY).Id);
    }
    #endregion

    #region BUTTON PRESSES
    private void OnStageComponentsUpdated() {
        if (_playerModifiedName) {
            return;
        }
        string newName = string.Empty;
    }

    private void OnSpellNameEditorHighlighted(IUIInteractable interactable) {
        _craftButtonView.SetActive(false);
        _spellComponentsView.SetActive(false);
        _spellNameEditorView.SetActive(true);
    }

    private void OnSpellNameEditorSelected(IUIInteractable interactable) {
        UIPanelManager.Instance.OnStringDataPassed -= OnSpellNameUpdated;
        SpellNameEditorInitData initData = new SpellNameEditorInitData() {
            InitialName = _cachedSpellName
        };
        // open rename spell dialog
        UIPanelManager.Instance.OpenUIPanel(_renameSpellPanelId, initData);
        // listen to rename spell dialog
        UIPanelManager.Instance.OnStringDataPassed += OnSpellNameUpdated;
    }

    private void OnSpellNameUpdated(string spellName) {
        _cachedSpellName = spellName;
        _spellNameEditorView.SetInteractableItem(0, 0, new UICustomButtonInitData() {
            ButtonText = string.IsNullOrEmpty(_cachedSpellName) ? "Insert spell name here..." : _cachedSpellName
        });
        UIPanelManager.Instance.OnStringDataPassed -= OnSpellNameUpdated;
    }

    private void OnSpellComponentHighlighted(IUIInteractable interactable) {
        _craftButtonView.SetActive(false);
        _spellComponentsView.SetActive(true);
        _spellNameEditorView.SetActive(false);
        string id = interactable.Id;
        OnSpellSlotHighlighted?.Invoke(id);
    }

    private void OnSpellComponentSelected(IUIInteractable interactable) {
        string id = interactable.Id;
        if (id.Equals(GameplayValues.UI.EmptyInventoryItemId)) {
            return;
        }
        _spellCraftManager.RemoveComponentFromSpell(id);
        OnSpellSlotSelected?.Invoke(id);
    }

    private void OnCraftSpellButtonHighlighted(IUIInteractable interactable) {
        _spellComponentsView.UnhighlightCell(_spellComponentsView.CurrentItemX, _spellComponentsView.CurrentItemY);
        _spellNameEditorView.UnhighlightCell(_spellNameEditorView.CurrentItemX, _spellNameEditorView.CurrentItemY);
        _craftButtonView.SetActive(true);
        _spellComponentsView.SetActive(false);
        _spellNameEditorView.SetActive(false);
    }

    private void OnCraftSpellButtonSelected(IUIInteractable interactable) {
        if (_spellCraftManager.LoadedCastingMethod == null) {
            return;
        }
        if (_spellCraftManager.LoadedSpellEffects.Count == 0) {
            return;
        }
        StorableSpell storableSpell = _spellCraftManager.GenerateSpell();
        storableSpell.SetName(_cachedSpellName);
        GameManager.GameManagerInstance.CurrentSpellInventory.AddSpell(storableSpell);
        _spellCraftManager.ClearSpellComponents();
        ClearSpellComponentSlots();

        // add this to loadout if loadout is not full
        for (int i = 0; i < GameplayValues.Magic.PlayerLoadoutMaxSize; i++) {
            if (GameManager.GameManagerInstance.CurrentSpellInventory.CurrentLoadout[i] == null) {
                GameManager.GameManagerInstance.CurrentSpellInventory.SetSpellInLoadout(storableSpell.InstanceId, i);
                return;
            }
        }

        ShowAskToEnterLoadoutMenuPopup();
        OnCraftSpellPressed?.Invoke();
    }
    #endregion

    private void ShowAskToEnterLoadoutMenuPopup() {
        // ask the player if they wish to use this spell now
        List<ButtonActionData> buttonActionDatas = new List<ButtonActionData>();
        buttonActionDatas.Add(
            new ButtonActionData() {
                ButtonId = GameplayValues.UI.GenericButtonIdYes,
                ButtonText = "Yes",
                Action = OpenLoadoutEditor,
            }
        );
        buttonActionDatas.Add(
            new ButtonActionData() {
                ButtonId = GameplayValues.UI.GenericButtonIdNo,
                ButtonText = "No",
                Action = CloseUseSpellNowDialog
            }
        );
        GenericMessageBoxInitData messageBoxData =
            new GenericMessageBoxInitData("Use Spell Now?", "Would you like to use this spell in your current loadout?", buttonActionDatas);
        UIPanelManager.Instance.OpenUIPanel(UIPanelManager.GenericMessageBoxPrefabId, messageBoxData);
    }

    private void OpenLoadoutEditor() {
        CloseUseSpellNowDialog();
        CoroutineGod.Instance.ExecuteAfterOneFrame(OpenLoadoutPrefab);
    }

    private void OpenLoadoutPrefab() {
        UIPanelManager.Instance.OpenUIPanel(_loadoutPrefabId);
    }

    private void CloseUseSpellNowDialog() {
        UIPanelManager.Instance.CloseUIPanel();
    }
}
